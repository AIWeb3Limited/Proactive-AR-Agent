
# 视频内容识别与环境理解系统

本项目结合 YOLOv8 目标检测与本地部署 LLM（如 DeepSeek），用于从视频中识别关键物体并自动推理当前环境或活动内容。

## 📦 项目结构

```
main.py                  # GUI 与多线程逻辑入口
modules/
├── detector.py          # YOLOv8 视频帧识别
├── summarizer.py        # 多帧目标计数与摘要
├── llm_agent.py         # 调用本地部署的 Ollama LLM
└── voice_input.py       # 占位语音输入模块（当前未启用）
```

## ✅ 依赖安装

```bash
pip install ultralytics opencv-python pillow requests
```

## ▶️ 运行方法

1. 确保 Ollama 本地部署并运行模型：`ollama run deepseek`
2. 视频文件放在根目录，命名为 `test.mp4`
3. 运行主程序：

```bash
python main.py
```

## 🖼️ 功能亮点

- YOLOv8 实时目标识别
- 每 60 帧聚合摘要，构建 Prompt 发给 LLM
- GUI 显示缩略图 + 检测结果 + 场景分析
