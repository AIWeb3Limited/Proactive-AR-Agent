import json
import time
from datetime import datetime
from typing import List, Dict, Any, Tuple

class SceneAnalyzer:
    def __init__(self):
        """
        场景分析器：将YOLOv8检测结果转化为结构化、语义化的输入
        """
        # 场景分类映射
        self.scene_mapping = {
            'kitchen': ['oven', 'microwave', 'toaster', 'sink', 'refrigerator', 'fork', 'knife', 'spoon', 'bowl', 'cup', 'wine glass'],
            'living_room': ['tv', 'couch', 'chair', 'coffee table', 'remote', 'laptop'],
            'bedroom': ['bed', 'chair', 'tv', 'laptop'],
            'office': ['laptop', 'keyboard', 'mouse', 'chair', 'desk'],
            'bathroom': ['toilet', 'sink', 'toothbrush', 'hair drier'],
            'outdoor': ['car', 'bicycle', 'motorcycle', 'bus', 'train', 'truck', 'traffic light', 'fire hydrant', 'stop sign']
        }
        
        # 物体功能分组
        self.functional_groups = {
            'cooking_tools': ['fork', 'knife', 'spoon', 'bowl', 'cup', 'wine glass', 'oven', 'microwave', 'toaster', 'sink'],
            'food_items': ['banana', 'apple', 'sandwich', 'orange', 'broccoli', 'carrot', 'hot dog', 'pizza', 'donut', 'cake'],
            'furniture': ['chair', 'couch', 'bed', 'dining table', 'toilet'],
            'electronics': ['tv', 'laptop', 'mouse', 'remote', 'keyboard', 'cell phone'],
            'appliances': ['microwave', 'oven', 'toaster', 'sink', 'refrigerator'],
            'personal_items': ['backpack', 'umbrella', 'handbag', 'tie', 'suitcase', 'toothbrush', 'hair drier']
        }
        
        # 位置描述映射
        self.position_descriptions = {
            'top_left': '左上角',
            'top_center': '上方中央',
            'top_right': '右上角',
            'center_left': '左侧中央',
            'center': '画面中央',
            'center_right': '右侧中央',
            'bottom_left': '左下角',
            'bottom_center': '下方中央',
            'bottom_right': '右下角'
        }
    
    def calculate_relative_position(self, bbox: List[float], frame_width: int, frame_height: int) -> str:
        """
        计算物体在画面中的相对位置
        """
        center_x = (bbox[0] + bbox[2]) / 2
        center_y = (bbox[1] + bbox[3]) / 2
        
        # 计算相对位置（百分比）
        relative_x = center_x / frame_width
        relative_y = center_y / frame_height
        
        # 确定位置区域
        if relative_x < 0.33:
            x_region = 'left'
        elif relative_x < 0.66:
            x_region = 'center'
        else:
            x_region = 'right'
            
        if relative_y < 0.33:
            y_region = 'top'
        elif relative_y < 0.66:
            y_region = 'center'
        else:
            y_region = 'bottom'
        
        # 生成位置描述
        if x_region == 'center' and y_region == 'center':
            return '画面中央'
        elif x_region == 'center':
            return f'画面{y_region}方'
        elif y_region == 'center':
            return f'画面{x_region}侧'
        else:
            return f'画面{x_region}侧{y_region}方'
    
    def classify_scene(self, detections: List[Dict]) -> str:
        """
        根据检测到的物体分类场景
        """
        detected_classes = [det['class'] for det in detections]
        
        scene_scores = {}
        for scene, objects in self.scene_mapping.items():
            score = sum(1 for obj in detected_classes if obj in objects)
            if score > 0:
                scene_scores[scene] = score
        
        if scene_scores:
            # 返回得分最高的场景
            best_scene = max(scene_scores.items(), key=lambda x: x[1])[0]
            scene_names = {
                'kitchen': '厨房',
                'living_room': '客厅',
                'bedroom': '卧室',
                'office': '办公室',
                'bathroom': '浴室',
                'outdoor': '户外'
            }
            return scene_names.get(best_scene, '未知场景')
        
        return '未知场景'
    
    def group_objects_by_function(self, detections: List[Dict]) -> Dict[str, List[str]]:
        """
        按功能对物体进行分组
        """
        detected_classes = [det['class'] for det in detections]
        
        groups = {}
        for group_name, objects in self.functional_groups.items():
            group_objects = [obj for obj in detected_classes if obj in objects]
            if group_objects:
                groups[group_name] = group_objects
        
        return groups
    
    def filter_high_confidence_detections(self, detections: List[Dict], confidence_threshold: float = 0.6) -> List[Dict]:
        """
        过滤低置信度的检测结果
        """
        return [det for det in detections if det.get('conf', 0.0) >= confidence_threshold]
    
    def create_structured_data(self, detections: List[Dict], frame_width: int, frame_height: int) -> Dict[str, Any]:
        """
        创建结构化的检测数据
        """
        # 过滤低置信度检测
        filtered_detections = self.filter_high_confidence_detections(detections)
        
        # 处理每个检测结果
        objects = []
        for det in filtered_detections:
            relative_position = self.calculate_relative_position(det['bbox'], frame_width, frame_height)
            
            objects.append({
                'class': det['class'],
                'confidence': det.get('conf', 0.0),
                'bbox': det['bbox'],
                'relative_position': relative_position
            })
        
        # 分类场景
        scene = self.classify_scene(filtered_detections)
        
        # 按功能分组
        groups = self.group_objects_by_function(filtered_detections)
        
        return {
            'objects': objects,
            'scene': scene,
            'groups': groups,
            'timestamp': datetime.now().strftime('%Y-%m-%d %H:%M:%S'),
            'total_objects': len(objects)
        }
    
    def create_semantic_prompt(self, structured_data: Dict[str, Any], task_type: str = 'scene_analysis') -> str:
        """
        创建语义化的prompt
        """
        objects = structured_data['objects']
        scene = structured_data['scene']
        groups = structured_data['groups']
        
        # 构建物体描述
        object_descriptions = []
        for obj in objects:
            desc = f"{obj['class']}（置信度{obj['confidence']:.2f}，位于{obj['relative_position']}）"
            object_descriptions.append(desc)
        
        # 构建分组描述
        group_descriptions = []
        for group_name, group_objects in groups.items():
            group_names = {
                'cooking_tools': '烹饪工具',
                'food_items': '食材',
                'furniture': '家具',
                'electronics': '电子设备',
                'appliances': '家用电器',
                'personal_items': '个人物品'
            }
            group_desc = f"{group_names.get(group_name, group_name)}：{', '.join(group_objects)}"
            group_descriptions.append(group_desc)
        
        # 根据任务类型构建不同的prompt
        if task_type == 'scene_analysis':
            prompt = f"""你是智能环境分析助手。请根据以下场景信息进行分析：

[场景分类]：{scene}
[检测到的物体]：{'; '.join(object_descriptions)}
[物体分组]：{'；'.join(group_descriptions)}

请分析当前环境，推测用户可能正在进行的活动，并给出合理的建议或提醒。"""
        
        elif task_type == 'cooking_assistant':
            prompt = f"""你是烹饪助手。请根据厨房中的物体推测用户的烹饪活动：

[厨房场景]：检测到以下物体：{'; '.join(object_descriptions)}
[工具和食材]：{'；'.join(group_descriptions)}

请分析用户可能在准备什么菜品，并给出下一步的烹饪建议。"""
        
        elif task_type == 'activity_prediction':
            prompt = f"""你是行为预测助手。请根据环境中的物体推测用户的活动：

[环境]：{scene}
[物体]：{'; '.join(object_descriptions)}
[分组]：{'；'.join(group_descriptions)}

请预测用户接下来可能进行的活动，并给出相关建议。"""
        
        else:
            prompt = f"""请分析以下场景信息：

[场景]：{scene}
[物体]：{'; '.join(object_descriptions)}
[分组]：{'；'.join(group_descriptions)}

请根据以上信息进行分析和推理。"""
        
        return prompt
    
    def create_hybrid_input(self, structured_data: Dict[str, Any], task_type: str = 'scene_analysis') -> Dict[str, Any]:
        """
        创建混合输入（结构化数据 + 自然语言）
        """
        semantic_prompt = self.create_semantic_prompt(structured_data, task_type)
        
        return {
            'structured_data': structured_data,
            'semantic_prompt': semantic_prompt,
            'task_type': task_type
        }
    
    def analyze_temporal_changes(self, current_data: Dict[str, Any], previous_data: Dict[str, Any] = None) -> Dict[str, Any]:
        """
        分析时序变化（用于视频流）
        """
        if previous_data is None:
            return {
                'changes': '首次检测',
                'new_objects': [obj['class'] for obj in current_data['objects']],
                'removed_objects': []
            }
        
        current_objects = set(obj['class'] for obj in current_data['objects'])
        previous_objects = set(obj['class'] for obj in previous_data['objects'])
        
        new_objects = current_objects - previous_objects
        removed_objects = previous_objects - current_objects
        
        changes = []
        if new_objects:
            changes.append(f"新增物体：{', '.join(new_objects)}")
        if removed_objects:
            changes.append(f"移除物体：{', '.join(removed_objects)}")
        
        return {
            'changes': '；'.join(changes) if changes else '无明显变化',
            'new_objects': list(new_objects),
            'removed_objects': list(removed_objects)
        } 