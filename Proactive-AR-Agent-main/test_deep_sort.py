#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Deep SORT 追踪器测试脚本
"""

import cv2
import time
from modules.deep_sort_tracker import DeepSortTracker

def test_deep_sort():
    """测试 Deep SORT 追踪器"""
    print("正在初始化 Deep SORT 追踪器...")
    
    # 初始化追踪器
    tracker = DeepSortTracker()
    
    # 测试视频路径
    video_path = "test.mp4"
    
    print(f"开始处理视频: {video_path}")
    print("按 'q' 键退出")
    
    # 处理视频帧
    frame_count = 0
    start_time = time.time()
    
    for frame_objects, tracked_detections, frame in tracker.process_video_frame(video_path):
        frame_count += 1
        
        # 在帧上绘制追踪结果
        for track in tracked_detections:
            bbox = track['bbox']
            track_id = track['track_id']
            class_name = track['class']
            
            # 绘制边界框
            cv2.rectangle(frame, 
                         (int(bbox[0]), int(bbox[1])), 
                         (int(bbox[2]), int(bbox[3])), 
                         (0, 255, 0), 2)
            
            # 绘制标签
            label = f"{class_name} #{track_id}"
            cv2.putText(frame, label, 
                       (int(bbox[0]), int(bbox[1] - 10)), 
                       cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
        
        # 显示帧
        cv2.imshow('Deep SORT Tracking', frame)
        
        # 显示统计信息
        if frame_count % 30 == 0:  # 每30帧显示一次统计
            elapsed_time = time.time() - start_time
            fps = frame_count / elapsed_time
            summary = tracker.get_summary()
            print(f"帧数: {frame_count}, FPS: {fps:.2f}, 追踪摘要: {summary}")
        
        # 检查按键
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
    
    cv2.destroyAllWindows()
    
    # 显示最终统计
    total_time = time.time() - start_time
    print(f"\n测试完成!")
    print(f"总帧数: {frame_count}")
    print(f"总时间: {total_time:.2f} 秒")
    print(f"平均 FPS: {frame_count / total_time:.2f}")
    print(f"最终追踪摘要: {tracker.get_summary()}")

if __name__ == "__main__":
    test_deep_sort() 