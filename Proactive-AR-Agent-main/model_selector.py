#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
模型选择器 - 支持多种 YOLO 模型
"""

from ultralytics import YOLO
import os

class ModelSelector:
    """模型选择器"""
    
    AVAILABLE_MODELS = {
        'yolov8n': {
            'name': 'YOLOv8 Nano',
            'size': '6.2MB',
            'speed': 'Fast',
            'accuracy': 'Good',
            'description': '轻量级模型，速度快，适合实时应用'
        },
        'yolov8s': {
            'name': 'YOLOv8 Small',
            'size': '22MB',
            'speed': 'Medium',
            'accuracy': 'Better',
            'description': '平衡速度和精度的模型'
        },
        'yolov8m': {
            'name': 'YOLOv8 Medium',
            'size': '52MB',
            'speed': 'Slower',
            'accuracy': 'High',
            'description': '高精度模型，适合复杂场景'
        },
        'yolov8l': {
            'name': 'YOLOv8 Large',
            'size': '87MB',
            'speed': 'Slow',
            'accuracy': 'Very High',
            'description': '大模型，最高精度'
        },
        'yolov8x': {
            'name': 'YOLOv8 XLarge',
            'size': '131MB',
            'speed': 'Slowest',
            'accuracy': 'Best',
            'description': '最大模型，最佳精度，适合复杂场景'
        }
    }
    
    @classmethod
    def list_models(cls):
        """列出所有可用模型"""
        print("可用的 YOLO 模型:")
        print("=" * 80)
        for model_id, info in cls.AVAILABLE_MODELS.items():
            print(f"ID: {model_id}")
            print(f"名称: {info['name']}")
            print(f"大小: {info['size']}")
            print(f"速度: {info['speed']}")
            print(f"精度: {info['accuracy']}")
            print(f"描述: {info['description']}")
            print("-" * 40)
    
    @classmethod
    def download_model(cls, model_id):
        """下载指定模型"""
        if model_id not in cls.AVAILABLE_MODELS:
            print(f"错误：模型 {model_id} 不存在")
            return False
        
        model_path = f"{model_id}.pt"
        if os.path.exists(model_path):
            print(f"模型 {model_id} 已存在")
            return True
        
        print(f"正在下载 {cls.AVAILABLE_MODELS[model_id]['name']}...")
        try:
            model = YOLO(f"{model_id}.pt")
            print(f"模型 {model_id} 下载完成")
            return True
        except Exception as e:
            print(f"下载失败: {e}")
            return False
    
    @classmethod
    def get_model_info(cls, model_id):
        """获取模型信息"""
        return cls.AVAILABLE_MODELS.get(model_id, None)

def main():
    """主函数 - 模型选择器"""
    print("YOLO 模型选择器")
    print("=" * 50)
    
    # 列出所有模型
    ModelSelector.list_models()
    
    # 推荐模型
    print("\n推荐模型:")
    print("1. yolov8n - 实时应用，速度快")
    print("2. yolov8s - 平衡选择，精度和速度都不错")
    print("3. yolov8x - 最高精度，适合复杂场景")
    
    # 用户选择
    while True:
        choice = input("\n请输入模型ID (如: yolov8x) 或 'q' 退出: ").strip()
        
        if choice.lower() == 'q':
            break
        
        if choice in ModelSelector.AVAILABLE_MODELS:
            print(f"\n正在下载 {ModelSelector.AVAILABLE_MODELS[choice]['name']}...")
            if ModelSelector.download_model(choice):
                print(f"模型 {choice} 准备就绪！")
                print(f"在代码中使用: DeepSortTracker(model_path='{choice}.pt')")
            break
        else:
            print(f"无效的模型ID: {choice}")
            print("可用的模型ID:", list(ModelSelector.AVAILABLE_MODELS.keys()))

if __name__ == "__main__":
    main() 