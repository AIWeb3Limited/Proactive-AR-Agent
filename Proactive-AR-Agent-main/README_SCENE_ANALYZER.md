# 场景分析器 (Scene Analyzer)

## 概述

场景分析器是一个专门用于将YOLOv8检测结果转化为结构化、语义化输入的模块。它能够将原始的视觉检测数据转换为语言模型可以理解的语义化描述，从而提升语言模型对环境和行为的理解能力。

## 核心功能

### 1. 结构化数据处理
- **位置计算**: 自动计算物体在画面中的相对位置（如"画面中央"、"左侧下方"等）
- **置信度过滤**: 过滤低置信度的检测结果，避免干扰
- **场景分类**: 根据检测到的物体自动分类场景（厨房、客厅、办公室等）
- **物体分组**: 按功能对物体进行分组（烹饪工具、食材、家具等）

### 2. 语义化输入生成
- **自然语言描述**: 将检测结果转化为自然语言描述
- **任务特定prompt**: 根据不同的任务类型生成相应的prompt
- **混合输入格式**: 同时提供结构化数据和自然语言描述

### 3. 时序分析
- **变化检测**: 分析物体在时间序列中的变化
- **新增/移除物体**: 跟踪物体的出现和消失
- **行为序列**: 通过时序变化推测用户行为

## 使用方法

### 基本使用

```python
from modules.scene_analyzer import SceneAnalyzer

# 创建场景分析器
analyzer = SceneAnalyzer()

# 模拟YOLOv8检测结果
detections = [
    {
        'class': 'oven',
        'bbox': [100, 150, 200, 250],
        'conf': 0.95
    },
    {
        'class': 'fork',
        'bbox': [50, 300, 80, 320],
        'conf': 0.92
    }
]

# 创建结构化数据
frame_width, frame_height = 640, 480
structured_data = analyzer.create_structured_data(detections, frame_width, frame_height)

# 生成语义化prompt
prompt = analyzer.create_semantic_prompt(structured_data, 'cooking_assistant')
```

### 任务类型

场景分析器支持多种任务类型：

#### 1. 场景分析 (scene_analysis)
- **用途**: 通用场景分析
- **适用场景**: 任何环境
- **输出**: 环境分析和建议

#### 2. 烹饪助手 (cooking_assistant)
- **用途**: 厨房场景分析
- **适用场景**: 厨房环境
- **输出**: 烹饪建议和步骤指导

#### 3. 行为预测 (activity_prediction)
- **用途**: 用户行为预测
- **适用场景**: 各种环境
- **输出**: 行为预测和建议

#### 4. 办公室助手 (office_assistant)
- **用途**: 办公环境分析
- **适用场景**: 办公室
- **输出**: 工作效率建议

### 输出格式

#### 1. 结构化数据
```json
{
  "objects": [
    {
      "class": "oven",
      "confidence": 0.95,
      "bbox": [100, 150, 200, 250],
      "relative_position": "画面中央"
    }
  ],
  "scene": "厨房",
  "groups": {
    "cooking_tools": ["oven", "fork"]
  },
  "timestamp": "2025-01-05 12:30:00",
  "total_objects": 2
}
```

#### 2. 语义化prompt
```
你是烹饪助手。请根据厨房中的物体推测用户的烹饪活动：

[厨房场景]：检测到以下物体：oven（置信度0.95，位于画面中央）、fork（置信度0.92，位于画面左侧下方）
[工具和食材]：烹饪工具：oven, fork

请分析用户可能在准备什么菜品，并给出下一步的烹饪建议。
```

#### 3. 混合输入
```json
{
  "structured_data": {...},
  "semantic_prompt": "...",
  "task_type": "cooking_assistant"
}
```

## 配置选项

### 置信度阈值
```python
# 设置自定义置信度阈值
filtered_detections = analyzer.filter_high_confidence_detections(detections, 0.7)
```

### 场景映射
场景分析器内置了常见场景的物体映射：
- **厨房**: oven, microwave, sink, refrigerator, fork, knife, spoon等
- **客厅**: tv, couch, chair, remote, laptop等
- **办公室**: laptop, keyboard, mouse, chair, desk等
- **卧室**: bed, chair, tv, laptop等
- **浴室**: toilet, sink, toothbrush, hair drier等
- **户外**: car, bicycle, traffic light, stop sign等

### 物体分组
- **烹饪工具**: fork, knife, spoon, bowl, cup, wine glass等
- **食材**: banana, apple, sandwich, orange, broccoli等
- **家具**: chair, couch, bed, dining table, toilet等
- **电子设备**: tv, laptop, mouse, remote, keyboard等
- **家用电器**: microwave, oven, toaster, sink, refrigerator等
- **个人物品**: backpack, umbrella, handbag, toothbrush等

## 时序分析

### 变化检测
```python
# 分析时序变化
temporal_analysis = analyzer.analyze_temporal_changes(current_data, previous_data)

# 输出示例
{
  "changes": "新增物体：knife, apple",
  "new_objects": ["knife", "apple"],
  "removed_objects": []
}
```

### 时序prompt
```python
# 创建包含时序信息的prompt
semantic_prompt = analyzer.create_semantic_prompt(structured_data, 'cooking_assistant')
semantic_prompt += f"\n\n[时序变化]：{temporal_analysis['changes']}"
```

## 集成到主程序

在主程序中使用场景分析器：

```python
# 在main.py中
from modules.scene_analyzer import SceneAnalyzer

# 初始化
scene_analyzer = SceneAnalyzer()

# 在检测循环中
structured_data = scene_analyzer.create_structured_data(frame_detections, frame_width, frame_height)
semantic_prompt = scene_analyzer.create_semantic_prompt(structured_data, task_type)

# 调用LLM
llm_response = query_ollama(semantic_prompt)
```

## 示例运行

运行示例文件查看详细用法：

```bash
python examples/scene_analysis_example.py
```

## 最佳实践

### 1. 信息过滤
- 只传递置信度>0.6的物体
- 避免传递技术细节（如边界框坐标）
- 只传递语义化信息（位置、类别、状态）

### 2. 格式标准化
- 使用固定的prompt模板
- 让语言模型适应固定格式
- 提升回复一致性

### 3. 任务特定优化
- 根据场景类型选择相应的任务类型
- 为不同任务设置不同的置信度阈值
- 使用专门的系统提示

### 4. 时序信息利用
- 跟踪物体变化
- 分析行为序列
- 提供上下文信息

## 扩展功能

### 自定义场景映射
```python
# 添加自定义场景
analyzer.scene_mapping['custom_scene'] = ['object1', 'object2', 'object3']
```

### 自定义物体分组
```python
# 添加自定义分组
analyzer.functional_groups['custom_group'] = ['object1', 'object2']
```

### 自定义位置描述
```python
# 添加自定义位置描述
analyzer.position_descriptions['custom_position'] = '自定义位置描述'
```

## 性能优化

1. **缓存机制**: 对于重复的检测结果使用缓存
2. **批量处理**: 批量处理多个检测结果
3. **异步处理**: 使用异步方式处理时序分析
4. **内存管理**: 及时清理历史数据

## 故障排除

### 常见问题

1. **检测结果为空**
   - 检查置信度阈值设置
   - 确认检测器正常工作

2. **场景分类错误**
   - 检查场景映射配置
   - 确认物体类别名称正确

3. **位置计算异常**
   - 检查帧尺寸参数
   - 确认边界框格式正确

### 调试信息
```python
# 获取调试信息
debug_info = analyzer.get_debug_info()
print(debug_info)
```

## 更新日志

- **v1.0.0**: 初始版本，支持基本场景分析
- **v1.1.0**: 添加时序分析功能
- **v1.2.0**: 支持多种任务类型
- **v1.3.0**: 添加配置文件和示例

## 贡献

欢迎提交Issue和Pull Request来改进场景分析器！

## 许可证

本项目采用MIT许可证。 