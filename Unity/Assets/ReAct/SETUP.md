# Unity ReAct Agent 设置指南

## 🎯 项目概述

这是一个完全基于 Unity 原生 C# 的 ReAct (Reasoning and Acting) 智能体系统，用于 Unity 项目中的智能文件搜索和管理。

## ✅ 问题修复

### 主要修复内容：
1. **移除 Newtonsoft.Json 依赖**：创建了自定义的 `SimpleJsonParser` 类
2. **修复路径兼容性**：替换了 `Path.GetRelativePath` 为兼容性更好的实现
3. **优化 JSON 解析**：增强了对嵌套 JSON 结构的处理能力
4. **移除外部依赖**：项目现在完全自包含，无需额外安装包

## 🚀 快速开始

### 1. 项目导入
```
1. 将整个 ReAct 文件夹复制到 Unity 项目的 Assets 目录下
2. Unity 会自动编译所有脚本
3. 无需安装任何额外的 Package
```

### 2. 基本测试
```csharp
// 在任意 MonoBehaviour 脚本中
using ReAct.Agents;
using ReAct.Models;

public class TestScript : MonoBehaviour
{
    async void Start()
    {
        var agent = new ReactAgent();
        var result = await agent.Execute("统计Assets文件夹下有多少个C#脚本文件");
        Debug.Log(result);
    }
}
```

### 3. 使用测试组件
```
1. 在场景中创建一个空的 GameObject
2. 添加 ReactAgentTest 组件
3. 在 Inspector 中配置测试查询
4. 运行场景，使用 Context Menu 或 OnGUI 按钮测试
```

## 📁 项目结构

```
Assets/ReAct/
├── 📊 Models/                      # 数据模型
│   ├── Message.cs                  # 消息数据结构
│   ├── IGenerativeModel.cs         # AI 模型接口
│   └── MockGenerativeModel.cs      # 模拟 AI 模型
├── 🔧 Tools/                       # 工具系统
│   ├── ToolName.cs                 # 工具名称枚举
│   ├── Tool.cs                     # 工具基类
│   └── UnityFileSearchTools.cs     # Unity 文件搜索工具
├── 📝 Prompts/                     # 提示模板
│   ├── PromptTemplate.cs           # 提示模板基类
│   └── ReactPrompt.cs              # ReAct 专用提示
├── 🤖 Agents/                      # 智能体
│   ├── Agent.cs                    # 智能体基类
│   └── ReactAgent.cs               # ReAct 智能体实现
├── 🛠️ Utils/                       # 工具类
│   └── SimpleJsonParser.cs         # 自定义 JSON 解析器
├── 📋 Examples/                    # 示例代码
│   └── UnityReactDemo.cs           # 完整 UI 示例
├── 🧪 ReactAgentTest.cs            # 简单测试脚本
├── 📦 ReAct.asmdef                 # Assembly Definition
├── 📖 README.md                    # 完整文档
└── 📋 SETUP.md                     # 本设置指南
```

## 🔧 可用工具

### 1. COUNT_FILES
- **功能**：统计指定目录下的文件数量
- **参数**：`path`, `filePattern`, `recursive`
- **示例**：`"统计Assets文件夹下有多少个C#脚本文件"`

### 2. FIND_FILES
- **功能**：查找匹配模式的文件
- **参数**：`path`, `filePattern`, `recursive`
- **示例**：`"查找Assets文件夹中的所有prefab文件"`

### 3. SEARCH_ASSETS
- **功能**：搜索特定类型的Unity资源
- **参数**：`assetType`, `namePattern`
- **示例**：`"搜索所有的脚本文件"`

### 4. FIND_SCRIPTS
- **功能**：查找包含特定内容的脚本文件
- **参数**：`contentPattern`, `namePattern`
- **示例**：`"搜索包含'Test'的脚本文件"`

## 🎮 测试示例

### 使用 ReactAgentTest 组件：
```csharp
// 在 Inspector 中配置测试查询
testQueries = {
    "统计Assets文件夹下有多少个C#脚本文件",
    "查找Assets文件夹中的所有prefab文件",
    "搜索项目中包含'Test'的脚本文件",
    "统计项目中的场景文件数量"
};

// 运行时使用 Context Menu 执行测试
```

### 使用 UnityReactDemo 组件：
```csharp
// 完整的 UI 界面示例
// 支持输入框、按钮、滚动文本显示
// 包含预定义查询按钮
// 支持历史记录管理
```

## 🛠️ 扩展指南

### 添加自定义工具：
```csharp
public class CustomTools
{
    [ToolDescription("自定义工具的描述")]
    public static string MyTool(string param1, bool param2 = false)
    {
        // 实现自定义逻辑
        return "工具执行结果";
    }
}

// 在枚举中添加新工具名称
public enum ToolName
{
    // ... 现有工具
    MY_CUSTOM_TOOL
}
```

### 集成真实 AI 模型：
```csharp
public class OpenAIModel : IGenerativeModel
{
    public string ModelName => "GPT-4";
    
    public async Task<string> GenerateAsync(List<Message> messages)
    {
        // 实现 OpenAI API 调用
        // 返回 JSON 格式的响应
    }
}
```

## ⚠️ 注意事项

1. **兼容性**：支持 Unity 2019.4 及以上版本
2. **依赖**：无需额外安装任何 Package
3. **性能**：所有操作都是异步的，不会阻塞主线程
4. **调试**：详细的日志输出，便于调试和监控
5. **扩展性**：模块化设计，易于添加新功能

## 🚨 常见问题

### Q: 编译错误怎么办？
A: 确保 Unity 版本 >= 2019.4，并检查 Assembly Definition 文件是否正确导入。

### Q: 工具执行没有结果？
A: 检查 Unity Console 中的日志输出，确认工具参数是否正确。

### Q: 如何添加新的文件类型搜索？
A: 在 `UnityFileSearchTools.cs` 中的 `SearchAssets` 方法中添加新的文件扩展名。

### Q: 如何集成真实的 AI 模型？
A: 实现 `IGenerativeModel` 接口，并在创建 `ReactAgent` 时传入您的模型实例。

## 📄 许可证

本项目基于 MIT 许可证开源。 