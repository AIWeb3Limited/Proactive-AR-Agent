using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// LLM UI系统使用示例
/// 展示如何调用系统和LLM应该生成什么格式的参数
/// </summary>
public class LLMUIExample : MonoBehaviour
{
    [Header("UI管理器")]
    public UILayoutManager layoutManager;

    [Header("测试按钮")]
    public KeyCode testKey = KeyCode.Space;

    void Start()
    {
        // 确保有UILayoutManager
        if (layoutManager == null)
        {
            layoutManager = FindObjectOfType<UILayoutManager>();
            if (layoutManager == null)
            {
                GameObject managerObj = new GameObject("UILayoutManager");
                layoutManager = managerObj.AddComponent<UILayoutManager>();
            }
        }

        // 注册事件监听
        layoutManager.OnLayoutCreated += OnLayoutCreated;
        layoutManager.OnElementClicked += OnElementClicked;
    }

    void Update()
    {
        // 按空格键测试创建UI
        if (Input.GetKeyDown(testKey))
        {
            CreateExampleLayout();
        }
    }

    /// <summary>
    /// 创建示例布局 - 演示LLM应该生成的参数格式
    /// </summary>
    public void CreateExampleLayout()
    {
        // 使用JsonHelper简化布局创建（演示LLM应该如何构建布局）
        string llmGeneratedJson = JsonHelper.GenerateLoginExampleJson();
        
        Debug.Log("[示例] 生成的JSON布局数据:\n" + llmGeneratedJson);

        // 调用系统创建UI
        bool success = layoutManager.CreateLayoutFromJson(llmGeneratedJson);
        
        if (success)
        {
            Debug.Log("[示例] UI布局创建成功！");
        }
        else
        {
            Debug.LogError("[示例] UI布局创建失败！");
        }
    }

    #region LLM系统接口示例

    /// <summary>
    /// LLM系统调用接口 - 通过JSON创建UI
    /// 这是您的LLM系统应该调用的主要方法
    /// </summary>
    /// <param name="jsonLayoutData">LLM生成的JSON布局数据</param>
    /// <returns>是否创建成功</returns>
    public bool CreateUIFromLLM(string jsonLayoutData)
    {
        if (layoutManager != null)
        {
            return layoutManager.CreateLayoutFromJson(jsonLayoutData);
        }
        
        Debug.LogError("[LLM接口] UILayoutManager未初始化");
        return false;
    }

    /// <summary>
    /// LLM系统调用接口 - 更新指定UI元素
    /// </summary>
    /// <param name="elementId">元素ID</param>
    /// <param name="newProperties">新属性的JSON</param>
    public bool UpdateUIElement(string elementId, string newProperties)
    {
        // 这里可以解析新属性并更新元素
        Debug.Log($"[LLM接口] 更新元素 {elementId}: {newProperties}");
        return true;
    }

    /// <summary>
    /// LLM系统调用接口 - 清理当前UI
    /// </summary>
    public void ClearUI()
    {
        if (layoutManager != null)
        {
            layoutManager.ClearCurrentLayout();
        }
    }

    /// <summary>
    /// LLM系统调用接口 - 获取当前布局信息
    /// </summary>
    /// <returns>当前布局的JSON字符串</returns>
    public string GetCurrentLayoutInfo()
    {
        if (layoutManager != null)
        {
            return layoutManager.GetLayoutAsJson();
        }
        return null;
    }

    #endregion

    #region 事件处理

    private void OnLayoutCreated(string layoutName, UILayoutData layoutData)
    {
        Debug.Log($"[事件] 布局创建完成: {layoutName}");
        Debug.Log($"[事件] 根元素数量: {layoutData.rootElements.Count}");
    }

    private void OnElementClicked(string elementId, string clickMethod)
    {
        Debug.Log($"[事件] 元素点击: {elementId} -> {clickMethod}");
        
        // 根据点击方法执行对应逻辑
        switch (clickMethod)
        {
            case "OnLoginClick":
                HandleLogin();
                break;
            default:
                Debug.Log($"[事件] 未处理的点击方法: {clickMethod}");
                break;
        }
    }

    private void HandleLogin()
    {
        // 获取输入框的值
        GameObject usernameInput = layoutManager.GetElement("username_input");
        GameObject passwordInput = layoutManager.GetElement("password_input");
        
        Debug.Log("[登录] 处理登录逻辑");
        // 这里添加实际的登录逻辑
    }

    #endregion

    void OnDestroy()
    {
        if (layoutManager != null)
        {
            layoutManager.OnLayoutCreated -= OnLayoutCreated;
            layoutManager.OnElementClicked -= OnElementClicked;
        }
    }
} 