import cv2
import numpy as np
from ultralytics import YOLO
from deep_sort_realtime import deepsort_tracker
import time

class DeepSortTracker:
    def __init__(self, model_path="yolov8n.pt"):
        """
        初始化 Deep SORT 追踪器
        """
        print(f"正在加载 YOLOv8n 模型... ({model_path})")
        self.yolo_model = YOLO(model_path)
        print("YOLOv8n 模型加载完成")
        
        # 扩展追踪的目标类别
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
        
        self.tracker = deepsort_tracker.DeepSort(
            max_age=30,  # 最大追踪年龄
            n_init=3,    # 初始化所需帧数
            nms_max_overlap=1.0,  # NMS 最大重叠
            max_cosine_distance=0.2,  # 最大余弦距离
            nn_budget=None,  # 特征预算
            override_track_class=None,  # 覆盖追踪类
            embedder_gpu=False,  # 特征提取器是否使用GPU
            half=True,  # 是否使用半精度
            bgr=True,  # 输入是否为BGR格式
            embedder="mobilenet",  # 特征提取器
            embedder_model_name=None,  # 特征提取器模型名称
            embedder_wts=None,  # 特征提取器权重
            polygon=False,  # 是否使用多边形检测
            today=None  # 日期
        )
        self.tracked_objects = []
        self.frame_count = 0
        
    def process_video_frame(self, video_path):
        """
        处理视频帧并返回追踪结果
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
            detections = []
            frame_objects = []
            
            for result in results:
                boxes = result.boxes
                if boxes is not None:
                    for box in boxes:
                        cls_id = int(box.cls[0])
                        cls_name = result.names[cls_id]
                        conf = float(box.conf[0])
                        bbox = box.xyxy[0].cpu().numpy()  # [x1, y1, x2, y2]
                        
                        # 只追踪人、车辆等主要目标
                        if cls_name in self.target_classes:
                            detections.append((bbox, conf, cls_name))
                            frame_objects.append(cls_name)
            
            # Deep SORT 追踪
            tracks = self.tracker.update_tracks(detections, frame=frame)
            
            # 处理追踪结果
            tracked_detections = []
            for track in tracks:
                if not track.is_confirmed():
                    continue
                    
                track_id = track.track_id
                bbox = track.to_tlbr()  # [top, left, bottom, right]
                class_name = track.det_class
                
                tracked_detections.append({
                    'track_id': track_id,
                    'class': class_name,
                    'bbox': [bbox[1], bbox[0], bbox[3], bbox[2]],  # [x1, y1, x2, y2]
                    'conf': track.get_det_conf(),
                    'age': track.age
                })
            
            # 更新追踪对象列表
            self._update_tracked_objects(tracked_detections)
            
            yield frame_objects, tracked_detections, frame
            
        cap.release()
    
    def _update_tracked_objects(self, new_tracks):
        """
        更新追踪对象列表
        """
        current_time = time.time()
        
        # 更新现有追踪对象
        for track in new_tracks:
            track_id = track['track_id']
            existing = None
            
            for obj in self.tracked_objects:
                if obj.get('track_id') == track_id:
                    existing = obj
                    break
            
            if existing:
                # 更新现有对象
                existing.update({
                    'last_seen': current_time,
                    'bbox': track['bbox'],
                    'conf': track['conf'],
                    'age': track['age']
                })
            else:
                # 添加新对象
                track['first_seen'] = current_time
                track['last_seen'] = current_time
                self.tracked_objects.append(track)
        
        # 清理过期对象（超过30秒未出现）
        self.tracked_objects = [
            obj for obj in self.tracked_objects 
            if current_time - obj['last_seen'] < 30
        ]
    
    def get_summary(self):
        """
        获取追踪摘要
        """
        if not self.tracked_objects:
            return ""
        
        # 按类别统计
        class_counts = {}
        for obj in self.tracked_objects:
            cls = obj['class']
            if cls not in class_counts:
                class_counts[cls] = 0
            class_counts[cls] += 1
        
        summary_parts = []
        for cls, count in class_counts.items():
            summary_parts.append(f"{count} 个 {cls}")
        
        return ', '.join(summary_parts)
    
    def get_debug_info(self):
        """
        获取调试信息
        """
        return {
            'total_tracked': len(self.tracked_objects),
            'frame_count': self.frame_count,
            'objects': [
                {
                    'track_id': obj.get('track_id'),
                    'class': obj['class'],
                    'age': obj.get('age', 0),
                    'conf': obj['conf']
                }
                for obj in self.tracked_objects
            ]
        } 