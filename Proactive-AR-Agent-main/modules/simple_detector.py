import cv2
import numpy as np
from ultralytics import YOLO
import time

class SimpleDetector:
    def __init__(self, model_path="yolov8n.pt"):
        """
        初始化简单检测器（无追踪）
        """
        print(f"正在加载 YOLOv8n 模型... ({model_path})")
        self.yolo_model = YOLO(model_path)
        print("YOLOv8n 模型加载完成")
        
        # 扩展检测的目标类别
        self.target_classes = [
            'person', 'bicycle', 'car', 'motorcycle', 'airplane', 'bus', 'train', 
            'truck', 'boat', 'traffic light', 'fire hydrant', 'stop sign', 
            'parking meter', 'bench', 'bird', 'cat', 'dog', 'horse', 'sheep', 
            'cow', 'elephant', 'bear', 'zebra', 'giraffe', 'backpack', 'umbrella', 
            'handbag', 'tie', 'suitcase', 'frisbee', 'skis', 'snowboard', 
            'sports ball', 'kite', 'baseball bat', 'baseball glove', 'skateboard', 
            'surfboard', 'tennis racket', 'bottle', 'wine glass', 'cup', 'fork', 
            'knife', 'spoon', 'bowl', 'banana', 'apple', 'sandwich', 'orange', 
            'broccoli', 'carrot', 'hot dog', 'pizza', 'donut', 'cake', 'chair', 
            'couch', 'potted plant', 'bed', 'dining table', 'toilet', 'tv', 
            'laptop', 'mouse', 'remote', 'keyboard', 'cell phone', 'microwave', 
            'oven', 'toaster', 'sink', 'refrigerator', 'book', 'clock', 'vase', 
            'scissors', 'teddy bear', 'hair drier', 'toothbrush'
        ]
        
        self.frame_count = 0
        
    def process_video_frame(self, video_path):
        """
        处理视频帧并返回检测结果
        """
        cap = cv2.VideoCapture(video_path)
        
        # 检查视频文件是否能正确打开
        if not cap.isOpened():
            print(f"错误：无法打开视频文件: {video_path}")
            print(f"请检查文件是否存在且格式正确")
            return
        
        print(f"成功打开视频文件: {video_path}")
        print(f"视频信息：")
        print(f"  宽度: {int(cap.get(cv2.CAP_PROP_FRAME_WIDTH))}")
        print(f"  高度: {int(cap.get(cv2.CAP_PROP_FRAME_HEIGHT))}")
        print(f"  总帧数: {int(cap.get(cv2.CAP_PROP_FRAME_COUNT))}")
        print(f"  FPS: {cap.get(cv2.CAP_PROP_FPS):.2f}")
        
        while True:
            ret, frame = cap.read()
            if not ret:
                print("视频读取结束或失败")
                break
                
            self.frame_count += 1
            
            # YOLO 检测
            results = self.yolo_model(frame, verbose=False)
            
            # 准备检测结果
            frame_objects = []
            frame_detections = []
            
            for result in results:
                boxes = result.boxes
                if boxes is not None:
                    for box in boxes:
                        cls_id = int(box.cls[0])
                        cls_name = result.names[cls_id]
                        conf = float(box.conf[0])
                        bbox = box.xyxy[0].cpu().numpy()  # [x1, y1, x2, y2]
                        
                        # 只检测目标类别
                        if cls_name in self.target_classes:
                            frame_objects.append(cls_name)
                            frame_detections.append({
                                'class': cls_name,
                                'bbox': bbox.tolist(),
                                'conf': conf,
                                'detection_id': f"det_{self.frame_count}_{len(frame_detections)}"
                            })
            
            yield frame_objects, frame_detections, frame
            
        cap.release()
    
    def get_summary(self):
        """
        获取检测摘要（简单版本，无追踪）
        """
        return "使用简单检测模式（无追踪）"
    
    def get_debug_info(self):
        """
        获取调试信息
        """
        return {
            'frame_count': self.frame_count,
            'mode': 'simple_detection'
        } 