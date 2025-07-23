using UnityEngine;

/// <summary>
/// 主控制器 - LLM UI系统的入口点
/// 将此脚本挂载到场景中的GameObject上即可开始使用
/// </summary>
public class NewBehaviourScript : MonoBehaviour
{
    [Header("LLM UI系统")]
    public UILayoutManager uiLayoutManager;
    public LLMUIExample exampleController;

    [Header("快速测试")]
    public bool autoCreateExample = true;
    
    void Start()
    {
        // 自动设置系统组件
        SetupUISystem();
        
        if (autoCreateExample)
        {
            // 延迟0.5秒后创建示例界面
            Invoke("CreateExampleUI", 0.5f);
        }
    }

    /// <summary>
    /// 设置UI系统组件
    /// </summary>
    void SetupUISystem()
    {
        // 确保有UILayoutManager
        if (uiLayoutManager == null)
        {
            uiLayoutManager = FindObjectOfType<UILayoutManager>();
            if (uiLayoutManager == null)
            {
                GameObject managerObj = new GameObject("UILayoutManager");
                uiLayoutManager = managerObj.AddComponent<UILayoutManager>();
                Debug.Log("[主控制器] 自动创建了UILayoutManager");
            }
        }

        // 确保有示例控制器
        if (exampleController == null)
        {
            exampleController = FindObjectOfType<LLMUIExample>();
            if (exampleController == null)
            {
                GameObject exampleObj = new GameObject("LLMUIExample");
                exampleController = exampleObj.AddComponent<LLMUIExample>();
                exampleController.layoutManager = uiLayoutManager;
                Debug.Log("[主控制器] 自动创建了LLMUIExample");
            }
        }
    }

    /// <summary>
    /// 创建示例UI界面
    /// </summary>
    void CreateExampleUI()
    {
        if (exampleController != null)
        {
            exampleController.CreateExampleLayout();
            Debug.Log("[主控制器] 示例UI界面已创建，按空格键可重新生成");
        }
    }

    /// <summary>
    /// 给LLM系统调用的主要接口
    /// </summary>
    /// <param name="jsonLayoutData">LLM生成的JSON布局数据</param>
    /// <returns>是否创建成功</returns>
    public bool CreateUILayout(string jsonLayoutData)
    {
        if (uiLayoutManager != null)
        {
            return uiLayoutManager.CreateLayoutFromJson(jsonLayoutData);
        }
        return false;
    }

    /// <summary>
    /// 清理当前UI布局
    /// </summary>
    public void ClearUI()
    {
        if (uiLayoutManager != null)
        {
            uiLayoutManager.ClearCurrentLayout();
        }
    }

    void Update()
    {
        // 按C键清理UI
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearUI();
            Debug.Log("[主控制器] UI已清理");
        }

        // 按R键重新创建示例
        if (Input.GetKeyDown(KeyCode.R))
        {
            CreateExampleUI();
        }
    }

    void OnGUI()
    {
        // 在屏幕上显示操作提示
        GUI.Box(new Rect(10, 10, 300, 80), "LLM UI系统控制面板");
        GUI.Label(new Rect(20, 35, 280, 20), "空格键: 创建示例UI");
        GUI.Label(new Rect(20, 55, 280, 20), "C键: 清理当前UI");
        GUI.Label(new Rect(20, 75, 280, 20), "R键: 重新创建示例");
    }
}
