# Unity ReAct Agent 

这是一个基于 ReAct（Reasoning and Acting）框架的 Unity C# 实现，用于在 Unity 项目中进行智能文件搜索和管理。

## 项目结构

```
Assets/ReAct/
├── Models/                 # 数据模型
│   ├── Message.cs          # 消息模型
│   ├── IGenerativeModel.cs # 生成模型接口
│   └── MockGenerativeModel.cs # 模拟生成模型
├── Tools/                  # 工具系统
│   ├── ToolName.cs         # 工具名称枚举
│   ├── Tool.cs             # 工具基类
│   └── UnityFileSearchTools.cs # Unity 文件搜索工具
├── Prompts/                # 提示模板
│   ├── PromptTemplate.cs   # 提示模板基类
│   └── ReactPrompt.cs      # ReAct 提示模板
├── Agents/                 # 智能体
│   ├── Agent.cs            # 智能体基类
│   └── ReactAgent.cs       # ReAct 智能体实现
├── Utils/                  # 工具类
│   └── SimpleJsonParser.cs # 简单 JSON 解析器
├── Examples/               # 示例代码
│   └── UnityReactDemo.cs   # 完整使用示例
├── ReactAgentTest.cs       # 测试脚本
├── ReAct.asmdef            # Assembly Definition 文件
└── README.md               # 本文档
```

## 功能特性

### 🔧 工具系统
- **FindFiles**: 查找匹配模式的文件
- **CountFiles**: 统计文件数量
- **SearchAssets**: 搜索 Unity 资源
- **FindScripts**: 查找脚本文件

### 🤖 智能体系统
- **Think-Decide-Act 循环**: 实现 ReAct 工作流程
- **工具动态调用**: 根据查询自动选择合适的工具
- **历史记录管理**: 维护对话历史，避免重复思考
- **错误处理**: 智能的异常恢复机制

### 🎯 Unity 集成
- **无缝集成**: 完全适配 Unity 环境
- **可视化测试**: 提供图形界面测试功能
- **异步支持**: 使用 async/await 模式
- **Inspector 配置**: 支持 Unity Inspector 配置

## 快速开始

### 1. 设置项目

1. 将 ReAct 文件夹放入 Assets 目录
2. 在场景中创建一个 GameObject
3. 添加 `ReactAgentTest` 组件

### 2. 基本使用

```csharp
// 创建 ReAct Agent
var agent = new ReactAgent(new MockGenerativeModel());

// 执行查询
var result = await agent.Execute("统计Assets文件夹下有多少个C#脚本文件");
Debug.Log(result);
```

### 3. 在 MonoBehaviour 中使用

```csharp
public class MyScript : MonoBehaviour
{
    private ReactAgent agent;
    
    private void Start()
    {
        agent = new ReactAgent();
    }
    
    public async void SearchFiles()
    {
        var result = await agent.Execute("查找所有的prefab文件");
        Debug.Log(result);
    }
}
```

## 支持的查询类型

### 文件统计
- "统计Assets文件夹下有多少个C#脚本文件"
- "统计项目中的场景文件数量"
- "计算所有材质文件的数量"

### 文件查找
- "查找Assets文件夹中的所有prefab文件"
- "搜索项目中包含'Test'的脚本文件"
- "查找所有的纹理文件"

### 资源搜索
- "搜索所有的脚本文件"
- "查找项目中的音频文件"
- "搜索包含特定内容的脚本"

## 扩展功能

### 添加自定义工具

```csharp
public class CustomTools
{
    [ToolDescription("自定义工具描述")]
    public static string MyCustomTool(string parameter)
    {
        // 实现自定义功能
        return "工具执行结果";
    }
}

// 在 ReactAgent 中注册
agent.SetTool(ToolName.CUSTOM_TOOL, typeof(CustomTools).GetMethod("MyCustomTool"));
```

### 集成真实的 AI 模型

```csharp
public class OpenAIModel : IGenerativeModel
{
    public string ModelName => "GPT-4";
    
    public async Task<string> GenerateAsync(List<Message> messages)
    {
        // 实现与 OpenAI API 的集成
        // 返回生成的响应
    }
}

// 使用真实模型
var agent = new ReactAgent(new OpenAIModel());
```

## 工作流程

1. **Think（思考）**: 分析用户查询，结合历史记录
2. **Decide（决策）**: 解析 AI 响应，判断是否需要工具
3. **Act（行动）**: 执行工具调用，获取结果
4. **重复**: 直到得到最终答案或达到最大迭代次数

## 注意事项

- 项目自带简单 JSON 解析器，无需额外依赖
- 当前使用 MockGenerativeModel 进行演示，实际使用需要集成真实的 AI 模型
- 工具执行结果会在 Unity Console 中显示
- 支持异步操作，避免阻塞主线程
- 兼容 Unity 2019.4 及以上版本

## 贡献与扩展

欢迎贡献代码和建议！您可以：
- 添加新的文件搜索工具
- 优化 Prompt 模板
- 集成更多的 AI 模型
- 改进错误处理机制

## 许可证

本项目基于 MIT 许可证开源。 