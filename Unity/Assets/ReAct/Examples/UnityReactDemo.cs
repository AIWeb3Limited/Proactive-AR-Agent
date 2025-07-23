using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using ReAct.Agents;
using ReAct.Models;

namespace ReAct.Examples
{
    public class UnityReactDemo : MonoBehaviour
    {
        [Header("UI References")]
        public InputField queryInput;
        public Button executeButton;
        public Text resultText;
        public ScrollRect scrollRect;
        
        [Header("Demo Settings")]
        public bool showDebugLogs = true;
        
        [Header("Predefined Queries")]
        [TextArea(2, 3)]
        public string[] predefinedQueries = {
            "统计Assets文件夹下有多少个C#脚本文件",
            "查找所有的prefab文件",
            "搜索包含'Test'的脚本文件",
            "统计项目中的场景文件数量",
            "查找所有的材质文件",
            "搜索音频文件"
        };
        
        private ReactAgent agent;
        private List<string> conversationHistory = new List<string>();
        
        private void Start()
        {
            InitializeUI();
            InitializeAgent();
        }
        
        private void InitializeUI()
        {
            if (executeButton != null)
            {
                executeButton.onClick.AddListener(OnExecuteClicked);
            }
            
            if (queryInput != null)
            {
                queryInput.onEndEdit.AddListener(OnQueryInputEndEdit);
            }
            
            UpdateResultText("ReAct Agent 已初始化，请输入查询...");
        }
        
        private void InitializeAgent()
        {
            agent = new ReactAgent(new MockGenerativeModel());
            
            if (showDebugLogs)
            {
                Debug.Log("Unity ReAct Agent 初始化完成");
            }
        }
        
        private void OnQueryInputEndEdit(string query)
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ExecuteQuery();
            }
        }
        
        private void OnExecuteClicked()
        {
            ExecuteQuery();
        }
        
        private async void ExecuteQuery()
        {
            if (agent == null)
            {
                UpdateResultText("Agent 未初始化");
                return;
            }
            
            string query = queryInput?.text ?? "";
            if (string.IsNullOrWhiteSpace(query))
            {
                UpdateResultText("请输入查询内容");
                return;
            }
            
            UpdateResultText("正在处理查询...");
            
            try
            {
                // 记录查询历史
                conversationHistory.Add($"用户: {query}");
                
                // 执行查询
                var result = await agent.Execute(query);
                
                // 记录结果历史
                conversationHistory.Add($"Agent: {result}");
                
                // 更新UI
                UpdateResultText($"查询: {query}\n\n结果: {result}");
                
                // 清空输入框
                if (queryInput != null)
                {
                    queryInput.text = "";
                }
                
                if (showDebugLogs)
                {
                    Debug.Log($"查询执行完成: {query} -> {result}");
                }
            }
            catch (System.Exception e)
            {
                var errorMessage = $"查询执行失败: {e.Message}";
                UpdateResultText(errorMessage);
                Debug.LogError(errorMessage);
            }
        }
        
        private void UpdateResultText(string text)
        {
            if (resultText != null)
            {
                resultText.text = text;
                
                // 滚动到底部
                if (scrollRect != null)
                {
                    Canvas.ForceUpdateCanvases();
                    scrollRect.verticalNormalizedPosition = 0f;
                }
            }
        }
        
        // 预定义查询按钮方法
        public void ExecutePredefinedQuery(int index)
        {
            if (index >= 0 && index < predefinedQueries.Length)
            {
                if (queryInput != null)
                {
                    queryInput.text = predefinedQueries[index];
                }
                ExecuteQuery();
            }
        }
        
        // 清空对话历史
        public void ClearHistory()
        {
            conversationHistory.Clear();
            UpdateResultText("对话历史已清空");
        }
        
        // 显示对话历史
        public void ShowHistory()
        {
            if (conversationHistory.Count == 0)
            {
                UpdateResultText("暂无对话历史");
                return;
            }
            
            var historyText = "对话历史:\n\n" + string.Join("\n\n", conversationHistory);
            UpdateResultText(historyText);
        }
        
        // 获取可用工具列表
        public void ShowAvailableTools()
        {
            var toolsInfo = @"可用工具:

1. COUNT_FILES - 统计文件数量
   参数: path, filePattern, recursive

2. FIND_FILES - 查找文件
   参数: path, filePattern, recursive

3. SEARCH_ASSETS - 搜索资源
   参数: assetType, namePattern

4. FIND_SCRIPTS - 查找脚本
   参数: contentPattern, namePattern

示例查询:
- 统计Assets文件夹下有多少个C#脚本文件
- 查找所有的prefab文件
- 搜索包含'Test'的脚本文件";
            
            UpdateResultText(toolsInfo);
        }
        
        // 重新初始化Agent
        public void ReinitializeAgent()
        {
            InitializeAgent();
            UpdateResultText("Agent 已重新初始化");
        }
        
        // 创建UI按钮（如果没有在Inspector中设置）
        private void OnGUI()
        {
            // 如果没有UI引用，使用OnGUI作为备用
            if (queryInput == null || executeButton == null)
            {
                GUILayout.BeginArea(new Rect(10, 10, 400, 300));
                
                GUILayout.Label("Unity ReAct Agent Demo", GUI.skin.label);
                
                GUILayout.Space(10);
                
                if (GUILayout.Button("执行预定义查询 1", GUILayout.Height(30)))
                {
                    ExecutePredefinedQuery(0);
                }
                
                if (GUILayout.Button("执行预定义查询 2", GUILayout.Height(30)))
                {
                    ExecutePredefinedQuery(1);
                }
                
                if (GUILayout.Button("执行预定义查询 3", GUILayout.Height(30)))
                {
                    ExecutePredefinedQuery(2);
                }
                
                if (GUILayout.Button("显示可用工具", GUILayout.Height(30)))
                {
                    ShowAvailableTools();
                }
                
                if (GUILayout.Button("显示对话历史", GUILayout.Height(30)))
                {
                    ShowHistory();
                }
                
                if (GUILayout.Button("清空历史", GUILayout.Height(30)))
                {
                    ClearHistory();
                }
                
                GUILayout.EndArea();
            }
        }
    }
} 