import time
import numpy as np
from collections import deque

class ObjectTracker:
    def __init__(self, window_size=30, iou_threshold=0.5):
        """
        初始化目标追踪器
        window_size: 追踪窗口大小（秒）
        iou_threshold: IOU匹配阈值
        """
        self.window_size = window_size
        self.iou_threshold = iou_threshold
        self.tracked_objects = []  # 当前活跃对象池
        self.frame_count = 0
        
    def update(self, new_detections):
        """
        更新追踪状态
        new_detections: 新检测到的目标列表，每个元素包含 {'class': str, 'bbox': [x1,y1,x2,y2], 'conf': float}
        """
        current_time = time.time()
        self.frame_count += 1
        
        # 处理新检测到的目标
        updated_tracked = []
        for det in new_detections:
            cls, box, conf = det['class'], det['bbox'], det['conf']
            matched = False
            
            # 尝试与现有追踪目标匹配
            for obj in self.tracked_objects:
                if obj['class'] == cls and self.iou(obj['bbox'], box) > self.iou_threshold:
                    # 更新现有目标
                    obj['last_seen'] = current_time
                    obj['bbox'] = box
                    obj['conf'] = max(obj['conf'], conf)  # 保留最高置信度
                    obj['frame_count'] = obj.get('frame_count', 0) + 1
                    matched = True
                    break
                    
            if not matched:
                # 新增追踪目标
                updated_tracked.append({
                    'class': cls,
                    'bbox': box,
                    'conf': conf,
                    'first_seen': current_time,
                    'last_seen': current_time,
                    'frame_count': 1
                })
        
        # 合并新匹配进来的目标
        self.tracked_objects.extend(updated_tracked)
        
        # 清除过期目标（未在窗口时间内出现）
        self.tracked_objects = [
            obj for obj in self.tracked_objects 
            if current_time - obj['last_seen'] < self.window_size
        ]
        
    def get_counts(self):
        """
        获取当前活跃目标的统计
        """
        result = {}
        for obj in self.tracked_objects:
            cls = obj['class']
            if cls not in result:
                result[cls] = 0
            result[cls] += 1
        return result
    
    def get_summary(self):
        """
        获取格式化的摘要信息
        """
        counts = self.get_counts()
        if not counts:
            return ""
        
        summary_parts = []
        for cls, count in counts.items():
            summary_parts.append(f"{count} 个 {cls}")
        
        return ', '.join(summary_parts)
    
    def iou(self, box1, box2):
        """
        计算两个边界框的IOU
        box: [x1, y1, x2, y2]
        """
        # 计算交集
        xa = max(box1[0], box2[0])
        ya = max(box1[1], box2[1])
        xb = min(box1[2], box2[2])
        yb = min(box1[3], box2[3])
        
        inter = max(0, xb - xa) * max(0, yb - ya)
        
        # 计算并集
        area1 = (box1[2] - box1[0]) * (box1[3] - box1[1])
        area2 = (box2[2] - box2[0]) * (box2[3] - box2[1])
        union = area1 + area2 - inter
        
        return inter / union if union != 0 else 0
    
    def get_debug_info(self):
        """
        获取调试信息
        """
        return {
            'total_tracked': len(self.tracked_objects),
            'frame_count': self.frame_count,
            'objects': [
                {
                    'class': obj['class'],
                    'frame_count': obj.get('frame_count', 0),
                    'conf': obj['conf']
                }
                for obj in self.tracked_objects
            ]
        } 