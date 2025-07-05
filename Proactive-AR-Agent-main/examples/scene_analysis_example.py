#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
场景分析器使用示例
展示如何将YOLOv8检测结果转化为结构化、语义化的输入
"""

import sys
import os

# 添加项目根目录到Python路径
current_dir = os.path.dirname(os.path.abspath(__file__))
project_root = os.path.dirname(current_dir)
sys.path.insert(0, project_root)

try:
    from modules.scene_analyzer import SceneAnalyzer
    import json
    print("模块导入成功！")
except ImportError as e:
    print(f"导入错误: {e}")
    print(f"当前目录: {os.getcwd()}")
    print(f"Python路径: {sys.path}")
    sys.exit(1)

def example_kitchen_scene():
    """
    厨房场景示例
    """
    print("=== 厨房场景分析示例 ===")
    
    # 模拟YOLOv8检测结果
    kitchen_detections = [
        {
            'class': 'oven',
            'bbox': [100, 150, 200, 250],
            'conf': 0.95
        },
        {
            'class': 'microwave',
            'bbox': [250, 150, 300, 200],
            'conf': 0.88
        },
        {
            'class': 'fork',
            'bbox': [50, 300, 80, 320],
            'conf': 0.92
        },
        {
            'class': 'knife',
            'bbox': [90, 300, 120, 320],
            'conf': 0.85
        },
        {
            'class': 'bowl',
            'bbox': [150, 280, 180, 310],
            'conf': 0.78
        },
        {
            'class': 'apple',
            'bbox': [200, 320, 230, 350],
            'conf': 0.91
        }
    ]
    
    # 创建场景分析器
    analyzer = SceneAnalyzer()
    
    # 模拟帧尺寸
    frame_width, frame_height = 640, 480
    
    # 创建结构化数据
    structured_data = analyzer.create_structured_data(kitchen_detections, frame_width, frame_height)
    
    print("1. 结构化数据:")
    print(json.dumps(structured_data, ensure_ascii=False, indent=2))
    
    print("\n2. 烹饪助手模式prompt:")
    cooking_prompt = analyzer.create_semantic_prompt(structured_data, 'cooking_assistant')
    print(cooking_prompt)
    
    print("\n3. 混合输入格式:")
    hybrid_input = analyzer.create_hybrid_input(structured_data, 'cooking_assistant')
    print(json.dumps(hybrid_input, ensure_ascii=False, indent=2))

def example_office_scene():
    """
    办公室场景示例
    """
    print("\n=== 办公室场景分析示例 ===")
    
    # 模拟YOLOv8检测结果
    office_detections = [
        {
            'class': 'laptop',
            'bbox': [200, 100, 400, 200],
            'conf': 0.96
        },
        {
            'class': 'keyboard',
            'bbox': [150, 250, 350, 280],
            'conf': 0.89
        },
        {
            'class': 'mouse',
            'bbox': [400, 250, 420, 270],
            'conf': 0.92
        },
        {
            'class': 'chair',
            'bbox': [100, 300, 200, 450],
            'conf': 0.94
        },
        {
            'class': 'cup',
            'bbox': [450, 150, 480, 180],
            'conf': 0.76
        }
    ]
    
    analyzer = SceneAnalyzer()
    frame_width, frame_height = 640, 480
    
    structured_data = analyzer.create_structured_data(office_detections, frame_width, frame_height)
    
    print("1. 结构化数据:")
    print(json.dumps(structured_data, ensure_ascii=False, indent=2))
    
    print("\n2. 行为预测模式prompt:")
    activity_prompt = analyzer.create_semantic_prompt(structured_data, 'activity_prediction')
    print(activity_prompt)

def example_temporal_analysis():
    """
    时序分析示例
    """
    print("\n=== 时序分析示例 ===")
    
    analyzer = SceneAnalyzer()
    
    # 模拟前一帧的数据
    previous_data = {
        'objects': [
            {'class': 'oven', 'confidence': 0.95, 'bbox': [100, 150, 200, 250], 'relative_position': '画面中央'},
            {'class': 'fork', 'confidence': 0.92, 'bbox': [50, 300, 80, 320], 'relative_position': '画面左侧下方'},
            {'class': 'bowl', 'confidence': 0.78, 'bbox': [150, 280, 180, 310], 'relative_position': '画面中央下方'}
        ],
        'scene': '厨房',
        'groups': {
            'cooking_tools': ['oven', 'fork', 'bowl']
        },
        'timestamp': '2025-01-05 12:30:00',
        'total_objects': 3
    }
    
    # 模拟当前帧的数据（新增了knife和apple）
    current_data = {
        'objects': [
            {'class': 'oven', 'confidence': 0.95, 'bbox': [100, 150, 200, 250], 'relative_position': '画面中央'},
            {'class': 'fork', 'confidence': 0.92, 'bbox': [50, 300, 80, 320], 'relative_position': '画面左侧下方'},
            {'class': 'knife', 'confidence': 0.85, 'bbox': [90, 300, 120, 320], 'relative_position': '画面左侧下方'},
            {'class': 'bowl', 'confidence': 0.78, 'bbox': [150, 280, 180, 310], 'relative_position': '画面中央下方'},
            {'class': 'apple', 'confidence': 0.91, 'bbox': [200, 320, 230, 350], 'relative_position': '画面右侧下方'}
        ],
        'scene': '厨房',
        'groups': {
            'cooking_tools': ['oven', 'fork', 'knife', 'bowl'],
            'food_items': ['apple']
        },
        'timestamp': '2025-01-05 12:30:05',
        'total_objects': 5
    }
    
    # 分析时序变化
    temporal_analysis = analyzer.analyze_temporal_changes(current_data, previous_data)
    
    print("时序变化分析:")
    print(json.dumps(temporal_analysis, ensure_ascii=False, indent=2))
    
    # 创建包含时序信息的prompt
    semantic_prompt = analyzer.create_semantic_prompt(current_data, 'cooking_assistant')
    semantic_prompt += f"\n\n[时序变化]：{temporal_analysis['changes']}"
    
    print("\n包含时序信息的prompt:")
    print(semantic_prompt)

def example_filtering_and_grouping():
    """
    过滤和分组示例
    """
    print("\n=== 过滤和分组示例 ===")
    
    # 模拟包含低置信度检测的结果
    mixed_detections = [
        {'class': 'oven', 'bbox': [100, 150, 200, 250], 'conf': 0.95},
        {'class': 'fork', 'bbox': [50, 300, 80, 320], 'conf': 0.92},
        {'class': 'unknown_object', 'bbox': [300, 300, 320, 320], 'conf': 0.35},  # 低置信度
        {'class': 'bowl', 'bbox': [150, 280, 180, 310], 'conf': 0.78},
        {'class': 'apple', 'bbox': [200, 320, 230, 350], 'conf': 0.91},
        {'class': 'noise', 'bbox': [400, 400, 410, 410], 'conf': 0.25}  # 低置信度
    ]
    
    analyzer = SceneAnalyzer()
    frame_width, frame_height = 640, 480
    
    # 使用不同的置信度阈值
    print("1. 使用默认置信度阈值 (0.6):")
    structured_data = analyzer.create_structured_data(mixed_detections, frame_width, frame_height)
    print(f"过滤后物体数量: {structured_data['total_objects']}")
    print(f"保留的物体: {[obj['class'] for obj in structured_data['objects']]}")
    
    print("\n2. 使用低置信度阈值 (0.3):")
    # 临时修改置信度阈值
    filtered_detections = analyzer.filter_high_confidence_detections(mixed_detections, 0.3)
    print(f"过滤后物体数量: {len(filtered_detections)}")
    print(f"保留的物体: {[det['class'] for det in filtered_detections]}")
    
    print("\n3. 物体分组:")
    groups = analyzer.group_objects_by_function(mixed_detections)
    for group_name, group_objects in groups.items():
        group_names = {
            'cooking_tools': '烹饪工具',
            'food_items': '食材',
            'furniture': '家具',
            'electronics': '电子设备',
            'appliances': '家用电器',
            'personal_items': '个人物品'
        }
        print(f"  {group_names.get(group_name, group_name)}: {', '.join(group_objects)}")

if __name__ == "__main__":
    print("场景分析器使用示例")
    print("=" * 50)
    
    # 运行各种示例
    example_kitchen_scene()
    example_office_scene()
    example_temporal_analysis()
    example_filtering_and_grouping()
    
    print("\n" + "=" * 50)
    print("示例运行完成！") 