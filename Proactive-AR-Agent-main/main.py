# main.py（集成场景分析器 + 结构化语义化输入）

import threading
import tkinter as tk
from tkinter import scrolledtext
from PIL import Image, ImageTk
import cv2
import time
import json
from modules.simple_detector import SimpleDetector
from modules.scene_analyzer import SceneAnalyzer
from modules.summarizer import update_window, get_attention_summary
from modules.llm_agent import query_ollama

# 修改为基于时间的间隔
LLM_INTERVAL_SECONDS = 5  # 每5秒调用一次LLM
object_window = []
frame_idx = 0
current_summary = ""
llm_output = "尚未生成"
last_llm_time = 0  # 记录上次LLM调用的时间

# 初始化检测器和场景分析器
simple_detector = SimpleDetector()
scene_analyzer = SceneAnalyzer()

# 存储历史数据用于时序分析
previous_scene_data = None

video_path = "Proactive-AR-Agent-main/test.mp4"  # 修正视频路径

def update_gui():
    summary_text.delete(1.0, tk.END)
    summary_text.insert(tk.END, current_summary or "（暂未识别到目标）")
    llm_text.delete(1.0, tk.END)
    llm_text.insert(tk.END, llm_output or "（尚无回复）")
    window.after(1000, update_gui)

def llm_worker(prompt, task_type="scene_analysis"):
    global llm_output
    try:
        # 根据任务类型添加不同的系统提示
        if task_type == "cooking_assistant":
            system_prompt = "你是专业的烹饪助手，擅长分析厨房场景并给出烹饪建议。"
        elif task_type == "activity_prediction":
            system_prompt = "你是智能行为预测助手，能够根据环境物体推测用户活动。"
        else:
            system_prompt = "你是智能环境分析助手，能够分析场景并给出合理建议。"
        
        full_prompt = f"{system_prompt}\n\n{prompt}"
        llm_output = query_ollama(full_prompt)
    except Exception as e:
        llm_output = f"LLM调用失败: {str(e)}"

def run_detection():
    global frame_idx, current_summary, last_llm_time, previous_scene_data
    
    print("run_detection 线程已启动")
    
    # 使用简单检测器处理视频
    print("读取视频帧...")
    for frame_objects, frame_detections, frame in simple_detector.process_video_frame(video_path):
        frame_idx += 1
        
        # 获取帧尺寸
        frame_height, frame_width = frame.shape[:2]
        
        # 使用场景分析器处理检测结果
        structured_data = scene_analyzer.create_structured_data(frame_detections, frame_width, frame_height)
        
        # 分析时序变化
        temporal_analysis = scene_analyzer.analyze_temporal_changes(structured_data, previous_scene_data)
        
        # 实时输出结构化检测结果
        print(f"\n[帧 {frame_idx}] 场景分析结果:")
        print(f"  场景: {structured_data['scene']}")
        print(f"  检测到 {structured_data['total_objects']} 个物体:")
        
        for obj in structured_data['objects']:
            print(f"    - {obj['class']}: 置信度{obj['confidence']:.2f}, 位置{obj['relative_position']}")
        
        if structured_data['groups']:
            print(f"  物体分组:")
            for group_name, group_objects in structured_data['groups'].items():
                group_names = {
                    'cooking_tools': '烹饪工具',
                    'food_items': '食材',
                    'furniture': '家具',
                    'electronics': '电子设备',
                    'appliances': '家用电器',
                    'personal_items': '个人物品'
                }
                print(f"    - {group_names.get(group_name, group_name)}: {', '.join(group_objects)}")
        
        print(f"  时序变化: {temporal_analysis['changes']}")
        
        # 保持原有的窗口更新逻辑（用于兼容）
        update_window(object_window, frame_objects)
        
        # 显示视频帧
        img = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        
        # 在帧上绘制检测框和检测信息
        for detection in frame_detections:
            bbox = detection['bbox']  # [x1, y1, x2, y2]
            class_name = detection['class']
            detection_id = detection.get('detection_id', 'N/A')
            conf = detection.get('conf', 0.0)
            
            # 安全处理conf值
            if conf is None:
                conf = 0.0
            
            # 计算边界框中心点坐标
            center_x = (bbox[0] + bbox[2]) / 2
            center_y = (bbox[1] + bbox[3]) / 2
            
            # 计算相对位置（百分比）
            relative_x = (center_x / frame_width) * 100
            relative_y = (center_y / frame_height) * 100
            
            # 确定位置描述
            if relative_x < 33:
                x_position = "左"
            elif relative_x < 66:
                x_position = "中"
            else:
                x_position = "右"
                
            if relative_y < 33:
                y_position = "上"
            elif relative_y < 66:
                y_position = "中"
            else:
                y_position = "下"
            
            # 绘制边界框
            x1, y1, x2, y2 = int(bbox[0]), int(bbox[1]), int(bbox[2]), int(bbox[3])
            cv2.rectangle(img, (x1, y1), (x2, y2), (0, 255, 0), 2)
            
            # 绘制标签（包含位置信息）
            label = f"{class_name} {x_position}{y_position} ({conf:.2f})"
            label_size = cv2.getTextSize(label, cv2.FONT_HERSHEY_SIMPLEX, 0.5, 2)[0]
            cv2.rectangle(img, (x1, y1 - label_size[1] - 10), (x1 + label_size[0], y1), (0, 255, 0), -1)
            cv2.putText(img, label, (x1, y1 - 5), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 0, 0), 2)
        
        img = cv2.resize(img, (480, 360))  # 放大预览画面
        img_pil = Image.fromarray(img)
        img_tk = ImageTk.PhotoImage(img_pil)
        canvas.create_image(0, 0, anchor=tk.NW, image=img_tk)
        canvas.image = img_tk
        
        # 基于时间间隔调用LLM（每5秒）
        current_time = time.time()
        if current_time - last_llm_time >= LLM_INTERVAL_SECONDS:
            # 使用场景分析器的结构化数据
            current_summary = f"场景: {structured_data['scene']}, 检测到 {structured_data['total_objects']} 个物体"
            
            if structured_data['objects']:  # 如果有检测到目标
                # 根据场景类型选择不同的任务类型
                scene = structured_data['scene']
                if '厨房' in scene:
                    task_type = 'cooking_assistant'
                elif '办公室' in scene:
                    task_type = 'activity_prediction'
                else:
                    task_type = 'scene_analysis'
                
                # 创建语义化prompt
                semantic_prompt = scene_analyzer.create_semantic_prompt(structured_data, task_type)
                
                # 添加时序变化信息
                if temporal_analysis['changes'] != '首次检测':
                    semantic_prompt += f"\n\n[时序变化]：{temporal_analysis['changes']}"
                
                # 启动LLM分析线程
                threading.Thread(target=llm_worker, args=(semantic_prompt, task_type), daemon=True).start()
            
            last_llm_time = current_time  # 更新上次LLM调用时间
            
            # 保存当前数据用于下次时序分析
            previous_scene_data = structured_data
            
        time.sleep(0.01)

    print("视频读取结束或失败")

# 保持原有的窗口更新函数（用于兼容）
def update_window(object_window, frame_objects):
    if len(object_window) >= 60:
        object_window.pop(0)
    object_window.append(frame_objects)

window = tk.Tk()
window.title("智能场景分析系统 - 结构化语义化输入版")
window.geometry("800x600")
canvas = tk.Canvas(window, width=480, height=360, bg="black")
canvas.pack(padx=10, pady=5, anchor=tk.NW)
summary_label = tk.Label(window, text="场景分析摘要:")
summary_label.pack()
summary_text = scrolledtext.ScrolledText(window, height=5)
summary_text.pack(fill=tk.X, padx=10)
llm_label = tk.Label(window, text="LLM 智能分析:")
llm_label.pack()
llm_text = scrolledtext.ScrolledText(window, height=10)
llm_text.pack(fill=tk.BOTH, expand=True, padx=10)
detector_thread = threading.Thread(target=run_detection, daemon=True)
detector_thread.start()
update_gui()
window.mainloop()
