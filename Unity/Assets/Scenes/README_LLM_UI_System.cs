/*
=================================================================
LLM驱动的Unity UI布局生成系统 - 完整解决方案
=================================================================

## 系统架构和思路

### 1. 数据层 (UIElementData.cs)
定义了LLM可以生成的所有UI参数结构：
- UIElementType: UI元素类型枚举 (Panel, Button, Text, Image, InputField等)
- UIElementData: 单个UI元素的完整参数 (位置、尺寸、样式、内容、事件等)
- UILayoutData: 整个界面布局的参数 (根元素列表、全局设置、主题等)

### 2. 工厂层 (UIElementFactory.cs)
负责根据数据创建实际的Unity UI GameObject：
- 支持多种UI元素类型的创建
- 自动处理父子关系和布局
- 统一的属性设置和事件绑定

### 3. 管理层 (UILayoutManager.cs)
核心控制器，提供给LLM系统的主要接口：
- CreateLayoutFromJson(): 主要接口，接收LLM生成的JSON参数
- UpdateElement(): 动态更新UI元素
- ClearCurrentLayout(): 清理当前布局
- GetElement(): 获取指定UI元素

### 4. 接口层 (LLMUIExample.cs)
演示和封装LLM系统调用的示例：
- CreateUIFromLLM(): 标准LLM调用接口
- 完整的事件处理示例
- JSON参数格式演示

## LLM系统集成指南

### 步骤1: 在场景中设置
1. 在Unity场景中创建一个空GameObject
2. 添加UILayoutManager组件
3. 确保场景中有Canvas（系统会自动创建）

### 步骤2: LLM参数生成
LLM需要生成符合以下格式的JSON参数：

```json
{
    "layoutName": "界面名称",
    "description": "界面描述", 
    "canvasSize": {"x": 1920, "y": 1080},
    "theme": "主题名称",
    "primaryColor": {"r": 0.2, "g": 0.6, "b": 1.0, "a": 1.0},
    "rootElements": [
        {
            "id": "唯一标识",
            "elementType": 0,  // 0=Panel, 1=Button, 2=Text, 3=Image, 4=InputField
            "name": "元素名称",
            "position": {"x": 0, "y": 0},
            "size": {"x": 200, "y": 50},
            "anchorMin": {"x": 0.5, "y": 0.5},
            "anchorMax": {"x": 0.5, "y": 0.5},
            "text": "显示文本",
            "fontSize": 16,
            "backgroundColor": {"r": 1.0, "g": 1.0, "b": 1.0, "a": 1.0},
            "textColor": {"r": 0.0, "g": 0.0, "b": 0.0, "a": 1.0},
            "onClick": "OnButtonClick",
            "children": []  // 子元素数组
        }
    ]
}
```

### 步骤3: 调用接口
```csharp
// 方法1: 直接使用JSON字符串
UILayoutManager layoutManager = FindObjectOfType<UILayoutManager>();
bool success = layoutManager.CreateLayoutFromJson(llmGeneratedJson);

// 方法2: 使用JsonHelper辅助类（推荐）
string jsonData = JsonHelper.GenerateLoginExampleJson();
layoutManager.CreateLayoutFromJson(jsonData);

// 方法3: LLM可以使用JsonHelper快速构建
UILayoutData layout = JsonHelper.CreateLayoutFromSimpleData("我的界面");
layout.rootElements.Add(JsonHelper.CreateButton("btn1", "按钮文字", "OnClick"));
layout.rootElements.Add(JsonHelper.CreateText("text1", "显示文字"));
string json = JsonUtility.ToJson(layout, true);
```

## 支持的UI元素类型

### 基础元素
- Panel: 容器面板，支持布局组件
- Button: 按钮，支持点击事件
- Text: 文本显示
- Image: 图片显示
- InputField: 输入框

### 布局支持
- Vertical: 垂直布局
- Horizontal: 水平布局  
- Grid: 网格布局
- 自动间距和内边距

### 事件系统
- onClick: 按钮点击事件
- onValueChanged: 值改变事件
- 通过委托进行事件回调

## 扩展指南

### 添加新的UI元素类型
1. 在UIElementType枚举中添加新类型
2. 在UIElementFactory中添加创建方法
3. 在UIElementData中添加必要的属性

### 自定义样式和主题
1. 扩展UILayoutData的主题属性
2. 在UILayoutManager的ApplyGlobalSettings中处理主题

### 集成到现有系统
1. 继承UILayoutManager类
2. 重写相关方法以适配现有架构
3. 通过事件系统集成业务逻辑

## 注意事项

### 性能优化
- 大量UI元素时考虑对象池
- 复杂布局时使用分层创建
- 及时清理不需要的UI元素

### JSON格式要求
- elementType使用数字：0=Panel, 1=Button, 2=Text, 3=Image, 4=InputField
- 颜色使用RGBA格式：{"r": 0.0-1.0, "g": 0.0-1.0, "b": 0.0-1.0, "a": 0.0-1.0}
- 位置和尺寸使用像素单位
- 锚点使用0-1的相对值

### 依赖项
- Unity UI系统 (UnityEngine.UI)
- TextMeshPro (TMPro)
- Unity内置JsonUtility (JSON解析)

## 使用示例

### 创建登录界面
```csharp
string loginUI = @"{
    ""layoutName"": ""登录界面"",
    ""rootElements"": [
        {
            ""id"": ""login_panel"",
            ""elementType"": 0,
            ""layoutType"": ""Vertical"",
            ""children"": [
                {""id"": ""title"", ""elementType"": 2, ""text"": ""用户登录""},
                {""id"": ""username"", ""elementType"": 4, ""placeholder"": ""用户名""},
                {""id"": ""password"", ""elementType"": 4, ""placeholder"": ""密码""},
                {""id"": ""login_btn"", ""elementType"": 1, ""text"": ""登录"", ""onClick"": ""DoLogin""}
            ]
        }
    ]
}";

layoutManager.CreateLayoutFromJson(loginUI);
```

这个系统为LLM提供了完整的UI生成能力，只需要LLM生成符合格式的JSON参数即可。

## 故障排除

### 关于JSON解析库
本系统已更新为使用Unity内置的JsonUtility，无需安装额外的Newtonsoft.Json包。
如果您仍然遇到Newtonsoft相关错误，请确保：
1. 删除所有Newtonsoft.Json的using引用
2. 使用JsonUtility.FromJson()替代JsonConvert.DeserializeObject()
3. 使用JsonUtility.ToJson()替代JsonConvert.SerializeObject()

### 如果您希望使用Newtonsoft.Json（可选）
1. 在Unity Package Manager中搜索"com.unity.nuget.newtonsoft-json"
2. 安装Newtonsoft Json包
3. 然后可以将JsonUtility的调用替换回Newtonsoft.Json格式

### JsonHelper辅助类的优势
- 简化LLM的JSON生成过程
- 自动处理Unity特定的数据格式
- 提供验证和错误检查功能
- 支持快速创建常用UI元素
*/ 