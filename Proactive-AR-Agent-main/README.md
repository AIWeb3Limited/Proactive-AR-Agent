# 视频内容识别与环境理解系统 (Deep SORT 版本)

本项目结合 YOLOv8 目标检测、Deep SORT 多目标追踪与本地部署 LLM（如 DeepSeek），用于从视频中识别并追踪关键物体，自动推理当前环境或活动内容。

## 📦 项目结构

```
main.py                  # GUI 与多线程逻辑入口
model_selector.py        # YOLO 模型选择器
modules/
├── deep_sort_tracker.py # Deep SORT 多目标追踪器
├── detector.py          # YOLOv8 视频帧识别 (原版)
├── object_tracker.py    # 简单时序追踪器 (原版)
├── summarizer.py        # 多帧目标计数与摘要
├── llm_agent.py         # 调用本地部署的 Ollama LLM
└── voice_input.py       # 占位语音输入模块（当前未启用）
test_deep_sort.py        # Deep SORT 追踪器测试脚本
```

## ✅ 依赖安装

```bash
pip install ultralytics opencv-python pillow requests deep-sort-realtime
```

## 🎯 模型选择

### 可用的 YOLO 模型

| 模型 | 大小 | 速度 | 精度 | 适用场景 |
|------|------|------|------|----------|
| yolov8n | 6.2MB | 快 | 好 | 实时应用 |
| yolov8s | 22MB | 中等 | 更好 | 平衡选择 |
| yolov8m | 52MB | 较慢 | 高 | 复杂场景 |
| yolov8l | 87MB | 慢 | 很高 | 高精度需求 |
| yolov8x | 131MB | 最慢 | 最佳 | 复杂场景 |

### 使用模型选择器

```bash
python model_selector.py
```

然后选择你想要的模型，系统会自动下载。

### 在代码中切换模型

修改 `modules/deep_sort_tracker.py` 中的模型路径：

```python
# 使用 YOLOv8x (最高精度)
deep_sort_tracker = DeepSortTracker(model_path="yolov8x.pt")

# 使用 YOLOv8s (平衡选择)
deep_sort_tracker = DeepSortTracker(model_path="yolov8s.pt")

# 使用 YOLOv8n (最快速度)
deep_sort_tracker = DeepSortTracker(model_path="yolov8n.pt")
```

## ▶️ 运行方法

### 主程序
1. 确保 Ollama 本地部署并运行模型：`ollama run deepseek`
2. 视频文件放在根目录，命名为 `test.mp4`
3. 运行主程序：

```bash
python main.py
```

### 测试 Deep SORT 追踪器
```bash
python test_deep_sort.py
```

### 选择模型
```bash
python model_selector.py
```

## 🚀 新功能亮点

### Deep SORT 多目标追踪
- **精确追踪**: 使用 Deep SORT 算法进行多目标追踪，支持目标 ID 保持
- **抗遮挡**: 即使目标暂时被遮挡，也能保持追踪连续性
- **多类别支持**: 追踪人、车辆、动物、物品等80+类别
- **实时性能**: 优化的追踪算法，保证实时处理性能

### 增强的场景理解
- **时序分析**: 基于追踪历史进行更准确的场景分析
- **目标轨迹**: 记录目标的运动轨迹和停留时间
- **智能摘要**: 结合追踪信息生成更丰富的场景描述

### 灵活的模型选择
- **多种模型**: 支持 YOLOv8n 到 YOLOv8x 的所有版本
- **自动下载**: 模型选择器自动下载所需模型
- **性能优化**: 根据需求选择速度或精度

## 🖼️ 功能特点

- YOLOv8 实时目标识别（支持80+类别）
- Deep SORT 多目标追踪
- 每 5 秒聚合摘要，构建 Prompt 发给 LLM
- GUI 显示缩略图 + 追踪结果 + 场景分析
- 支持目标 ID 显示和轨迹可视化
- 实时终端输出检测结果

## 🔧 技术栈

- **目标检测**: YOLOv8 (n/s/m/l/x)
- **多目标追踪**: Deep SORT
- **大语言模型**: Ollama (DeepSeek)
- **GUI**: Tkinter
- **视频处理**: OpenCV
- **图像处理**: Pillow

## 📊 性能优化

- 追踪器配置优化，平衡精度和速度
- 多线程处理，避免 GUI 卡顿
- 智能目标过滤，支持80+类别追踪
- 模型选择器，根据需求选择合适模型
