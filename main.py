# main.py（多线程 + GUI + 画面缩略图 + 时序目标追踪）

import threading
import tkinter as tk
from tkinter import scrolledtext
from PIL import Image, ImageTk
import cv2
import time
from modules.detector import process_video_frame
from modules.object_tracker import ObjectTracker
from modules.summarizer import update_window, get_attention_summary
from modules.llm_agent import query_ollama

# 修改为基于时间的间隔
LLM_INTERVAL_SECONDS = 5  # 每5秒调用一次LLM
object_window = []
frame_idx = 0
current_summary = ""
llm_output = "尚未生成"
last_llm_time = 0  # 记录上次LLM调用的时间

# 初始化时序目标追踪器
object_tracker = ObjectTracker(window_size=30, iou_threshold=0.5)  # 30秒窗口，IOU阈值0.5

video_path = "test.mp4"
cap = cv2.VideoCapture(video_path)

def update_gui():
    summary_text.delete(1.0, tk.END)
    summary_text.insert(tk.END, current_summary or "（暂未识别到目标）")
    llm_text.delete(1.0, tk.END)
    llm_text.insert(tk.END, llm_output or "（尚无回复）")
    window.after(1000, update_gui)

def llm_worker(prompt):
    global llm_output
    try:
        llm_output = query_ollama(prompt)
    except Exception as e:
        llm_output = f"LLM调用失败: {str(e)}"

def run_detection():
    global frame_idx, current_summary, last_llm_time
    frame_gen = process_video_frame(video_path)
    for frame_objects, frame_detections in frame_gen:
        ret, frame = cap.read()
        if not ret:
            break
        frame_idx += 1
        
        # 更新时序追踪器
        object_tracker.update(frame_detections)
        
        # 保持原有的窗口更新逻辑（用于兼容）
        update_window(object_window, frame_objects)
        
        # 显示视频帧
        img = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        img = cv2.resize(img, (480, 360))  # 放大预览画面
        img_pil = Image.fromarray(img)
        img_tk = ImageTk.PhotoImage(img_pil)
        canvas.create_image(0, 0, anchor=tk.NW, image=img_tk)
        canvas.image = img_tk
        
        # 基于时间间隔调用LLM（每5秒）
        current_time = time.time()
        if current_time - last_llm_time >= LLM_INTERVAL_SECONDS:
            # 使用时序追踪的结果
            summary = object_tracker.get_summary()
            current_summary = summary
            
            if summary:
                prompt = (
                    f"在最近几秒的视频中，检测到以下物体：{summary}。\n"
                    "请你分析当前场景可能的环境或活动内容，并进行合理推测。"
                )
                threading.Thread(target=llm_worker, args=(prompt,), daemon=True).start()
            
            last_llm_time = current_time  # 更新上次LLM调用时间
            
        time.sleep(0.01)

# 保持原有的窗口更新函数（用于兼容）
def update_window(object_window, frame_objects):
    if len(object_window) >= 60:
        object_window.pop(0)
    object_window.append(frame_objects)

window = tk.Tk()
window.title("视频环境理解系统 - 时序追踪版")
window.geometry("800x600")
canvas = tk.Canvas(window, width=480, height=360, bg="black")
canvas.pack(padx=10, pady=5, anchor=tk.NW)
summary_label = tk.Label(window, text="环境物品摘要 (时序追踪):")
summary_label.pack()
summary_text = scrolledtext.ScrolledText(window, height=5)
summary_text.pack(fill=tk.X, padx=10)
llm_label = tk.Label(window, text="LLM 场景分析:")
llm_label.pack()
llm_text = scrolledtext.ScrolledText(window, height=10)
llm_text.pack(fill=tk.BOTH, expand=True, padx=10)
detector_thread = threading.Thread(target=run_detection, daemon=True)
detector_thread.start()
update_gui()
window.mainloop()
