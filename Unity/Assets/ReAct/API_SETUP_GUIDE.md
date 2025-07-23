# 🔑 API Key 设置指南

## 📋 概述

Unity ReAct Agent 支持多种 AI 模型，包括：
- **Mock Model**：本地模拟模型（默认，无需API Key）
- **OpenAI GPT**：支持 GPT-3.5/GPT-4（需要API Key）
- **其他服务**：可扩展支持更多AI服务

## 🚀 设置 API Key 的方法

### 方法一：使用 APIConfig ScriptableObject（推荐）

#### 1. 创建 APIConfig 资源文件
1. 在 Project 窗口中右键点击
2. 选择 `Create` → `ReAct` → `API Config`
3. 将文件命名为 `MyAPIConfig`

#### 2. 配置 API 信息
在 Inspector 中设置以下信息：

**OpenAI 配置：**
- **OpenAI Api Key**: 您的 OpenAI API 密钥
- **OpenAI Base Url**: `https://api.openai.com/v1` (默认)
- **OpenAI Model**: `gpt-3.5-turbo` 或 `gpt-4`

**其他服务配置：**
- **DeepSeek Api Key**: DeepSeek API 密钥
- **Azure Api Key**: Azure OpenAI 服务密钥

#### 3. 在测试脚本中使用
1. 选择场景中的 ReactAgentTest GameObject
2. 在 Inspector 中：
   - 设置 `Model Type` 为 `OpenAI`
   - 将 `Api Config` 字段拖入创建的 APIConfig 资源

### 方法二：直接输入 API Key

1. 选择场景中的 ReactAgentTest GameObject
2. 在 Inspector 中：
   - 设置 `Model Type` 为 `OpenAI`
   - 在 `Manual Api Key` 字段中直接输入您的 API 密钥

## 🔐 获取 API Key

### OpenAI API Key
1. 访问 [OpenAI Platform](https://platform.openai.com/)
2. 注册/登录账户
3. 进入 [API Keys](https://platform.openai.com/api-keys) 页面
4. 点击 `Create new secret key`
5. 复制生成的 API Key

**注意**：
- API Key 格式：`sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx`
- 需要有足够的余额才能使用
- 建议设置使用限额

### DeepSeek API Key
1. 访问 [DeepSeek Platform](https://platform.deepseek.com/)
2. 注册/登录账户
3. 获取 API Key

## ⚙️ 配置示例

### 完整配置示例：
```
APIConfig 设置：
├── OpenAI Configuration
│   ├── OpenAI Api Key: sk-your-openai-key-here
│   ├── OpenAI Base Url: https://api.openai.com/v1
│   └── OpenAI Model: gpt-3.5-turbo
├── Other AI Services
│   ├── DeepSeek Api Key: (可选)
│   └── DeepSeek Base Url: https://api.deepseek.com/v1
└── Azure OpenAI
    ├── Azure Api Key: (可选)
    └── Azure Endpoint: (可选)
```

### ReactAgentTest 配置：
```
Model Configuration:
├── Model Type: OpenAI
├── Api Config: MyAPIConfig (拖入APIConfig资源)
└── Manual Api Key: (留空，使用APIConfig)
```

## 🧪 测试配置

### 1. 基本测试
```csharp
// 在代码中使用
var config = Resources.Load<APIConfig>("MyAPIConfig");
var model = new OpenAIModel(config);
var agent = new ReactAgent(model);
```

### 2. 直接测试
```csharp
// 直接使用API Key
var model = new OpenAIModel("sk-your-api-key-here");
var agent = new ReactAgent(model);
```

### 3. 验证配置
运行以下测试查询来验证配置是否正确：
- `"你好，请介绍一下自己"`
- `"统计Assets文件夹下有多少个C#脚本文件"`

## 🛡️ 安全注意事项

### 保护 API Key
1. **不要提交到版本控制**：
   - 将 APIConfig 文件添加到 `.gitignore`
   - 或使用环境变量/外部配置

2. **限制访问权限**：
   - 设置 API Key 的使用限额
   - 定期轮换 API Key

3. **测试环境分离**：
   - 开发和生产使用不同的 API Key
   - 使用不同的配置文件

### 建议的 .gitignore 设置：
```gitignore
# API 配置文件
**/APIConfig.asset
**/MyAPIConfig.asset

# 或者使用通配符
**/*APIConfig*.asset
```

## 🔧 高级配置

### 自定义模型参数
```csharp
// 在OpenAIModel中可以调整的参数：
- temperature: 0.7f     // 创造性 (0-1)
- max_tokens: 2000      // 最大响应长度
- top_p: 1.0f          // 核心采样
```

### 添加新的AI服务
1. 实现 `IGenerativeModel` 接口
2. 在 `APIConfig` 中添加相应配置
3. 在 `ReactAgentTest` 中添加新的 `ModelType`

## ❓ 常见问题

### Q: API Key 无效怎么办？
A: 检查以下内容：
- API Key 格式是否正确
- 账户余额是否充足
- 网络连接是否正常
- Base URL 是否正确

### Q: 请求失败怎么办？
A: 查看 Unity Console 中的详细错误信息：
- HTTP 错误代码
- API 响应内容
- 网络连接状态

### Q: 如何切换回模拟模型？
A: 将 `Model Type` 设置为 `Mock` 即可。

### Q: 可以同时使用多个API服务吗？
A: 可以，通过创建不同的模型实例来实现。

## 📞 技术支持

如果遇到问题：
1. 检查 Unity Console 中的错误信息
2. 验证 API Key 和配置是否正确
3. 确认网络连接正常
4. 查看本项目的其他文档：
   - `README.md` - 项目概览
   - `SETUP.md` - 项目设置
   - `BUG_FIXES.md` - 问题修复

## 📈 使用建议

1. **开发阶段**：使用 Mock Model 进行功能开发
2. **测试阶段**：使用真实 API 进行集成测试
3. **生产阶段**：配置生产环境的 API 密钥

现在您就可以使用真实的 AI 模型了！🎉 