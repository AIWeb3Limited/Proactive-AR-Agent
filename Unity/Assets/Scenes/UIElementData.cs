using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI元素类型枚举
/// </summary>
[Serializable]
public enum UIElementType
{
    Panel,
    Button,
    Text,
    Image,
    InputField,
    Slider,
    Toggle,
    Dropdown,
    ScrollView
}

/// <summary>
/// UI元素的基础数据结构
/// LLM可以生成这些参数来描述UI元素
/// </summary>
[Serializable]
public class UIElementData
{
    [Header("基础属性")]
    public string id;                    // 元素唯一标识
    public UIElementType elementType;    // 元素类型
    public string name;                  // 元素名称

    [Header("位置和尺寸")]
    public Vector2 position;             // 位置 (0-1相对坐标)
    public Vector2 size;                 // 尺寸 (0-1相对尺寸)
    public Vector2 anchorMin;            // 锚点最小值
    public Vector2 anchorMax;            // 锚点最大值
    public Vector2 pivot;                // 轴心点

    [Header("样式属性")]
    public Color backgroundColor = Color.white;     // 背景颜色
    public Color textColor = Color.black;           // 文字颜色
    public float fontSize = 14f;                    // 字体大小
    public string fontStyle = "Normal";             // 字体样式

    [Header("内容属性")]
    public string text;                  // 文本内容
    public string imagePath;             // 图片路径
    public string placeholder;           // 占位符文本

    [Header("交互属性")]
    public bool interactable = true;     // 是否可交互
    public string onClick;               // 点击事件(方法名)
    public string onValueChanged;        // 值改变事件

    [Header("子元素")]
    public List<UIElementData> children; // 子UI元素

    [Header("布局属性")]
    public string layoutType = "None";   // 布局类型: None, Vertical, Horizontal, Grid
    public float spacing = 5f;           // 元素间距
    public Vector2 padding = Vector2.zero; // 内边距

    public UIElementData()
    {
        children = new List<UIElementData>();
        anchorMin = Vector2.zero;
        anchorMax = Vector2.one;
        pivot = new Vector2(0.5f, 0.5f);
    }
}

/// <summary>
/// 完整的UI布局数据
/// LLM生成的最终参数结构
/// </summary>
[Serializable]
public class UILayoutData
{
    [Header("布局信息")]
    public string layoutName;            // 布局名称
    public string description;           // 布局描述
    public Vector2 canvasSize = new Vector2(1920, 1080); // 画布尺寸

    [Header("根元素")]
    public List<UIElementData> rootElements; // 根级UI元素

    [Header("全局样式")]
    public string theme = "Default";     // 主题
    public Color primaryColor = Color.blue;   // 主色调
    public Color secondaryColor = Color.gray; // 辅色调

    public UILayoutData()
    {
        rootElements = new List<UIElementData>();
    }
} 