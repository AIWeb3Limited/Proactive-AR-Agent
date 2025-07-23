using System.Threading.Tasks;
using UnityEngine;
using ReAct.Agents;
using ReAct.Models;
using ReAct.Config;

namespace ReAct
{
    public class ReactAgentTest : MonoBehaviour
    {
        [Header("Agent Settings")]
        public bool runOnStart = true;
        
        [Header("Model Configuration")]
        public ModelType modelType = ModelType.Mock;
        public APIConfig apiConfig;
        
        [Header("Manual API Settings (如果不使用APIConfig)")]
        [SerializeField] private string manualApiKey = "";
        
        [Header("Test Queries")]
        [TextArea(2, 4)]
        public string[] testQueries = {
            "统计Assets文件夹下有多少个C#脚本文件",
            "查找Assets文件夹中的所有prefab文件",
            "搜索项目中包含'Test'的脚本文件",
            "统计项目中的场景文件数量"
        };
        
        private ReactAgent agent;
        
        public enum ModelType
        {
            Mock,       // 模拟模型
            OpenAI      // OpenAI GPT
        }
        
        private void Start()
        {
            if (runOnStart)
            {
                InitializeAgent();
            }
        }
        
        private void InitializeAgent()
        {
            try
            {
                IGenerativeModel model = CreateModel();
                agent = new ReactAgent(model);
                Debug.Log($"ReAct Agent 初始化完成，使用模型: {model.ModelName}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Agent 初始化失败: {e.Message}");
                Debug.LogError("请检查API配置是否正确");
            }
        }
        
        private IGenerativeModel CreateModel()
        {
            switch (modelType)
            {
                case ModelType.Mock:
                    return new MockGenerativeModel();
                    
                case ModelType.OpenAI:
                    if (apiConfig != null)
                    {
                        return new OpenAIModel(apiConfig);
                    }
                    else if (!string.IsNullOrEmpty(manualApiKey))
                    {
                        return new OpenAIModel(manualApiKey);
                    }
                    else
                    {
                        throw new System.ArgumentException("使用OpenAI模型需要配置APIConfig或手动输入API Key");
                    }
                    
                default:
                    return new MockGenerativeModel();
            }
        }
        
        [ContextMenu("Execute Test Query 1")]
        public async void ExecuteTestQuery1()
        {
            await ExecuteQuery(testQueries[0]);
        }
        
        [ContextMenu("Execute Test Query 2")]
        public async void ExecuteTestQuery2()
        {
            await ExecuteQuery(testQueries[1]);
        }
        
        [ContextMenu("Execute Test Query 3")]
        public async void ExecuteTestQuery3()
        {
            await ExecuteQuery(testQueries[2]);
        }
        
        [ContextMenu("Execute Test Query 4")]
        public async void ExecuteTestQuery4()
        {
            await ExecuteQuery(testQueries[3]);
        }
        
        [ContextMenu("Execute Custom Query")]
        public async void ExecuteCustomQuery()
        {
            string customQuery = "查找所有的C#脚本文件";
            await ExecuteQuery(customQuery);
        }
        
        private async Task ExecuteQuery(string query)
        {
            if (agent == null)
            {
                InitializeAgent();
            }
            
            Debug.Log($"执行查询: {query}");
            
            try
            {
                var result = await agent.Execute(query);
                Debug.Log($"查询结果: {result}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"查询执行失败: {e.Message}");
            }
        }
        
        /// <summary>
        /// 用于外部调用的公共方法
        /// </summary>
        /// <param name="query">查询字符串</param>
        /// <returns>查询结果</returns>
        public async Task<string> ExecuteQueryAsync(string query)
        {
            if (agent == null)
            {
                InitializeAgent();
            }
            
            return await agent.Execute(query);
        }
        
        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 200, 30), "Initialize Agent"))
            {
                InitializeAgent();
            }
            
            if (GUI.Button(new Rect(10, 50, 200, 30), "Test Query 1"))
            {
                ExecuteTestQuery1();
            }
            
            if (GUI.Button(new Rect(10, 90, 200, 30), "Test Query 2"))
            {
                ExecuteTestQuery2();
            }
            
            if (GUI.Button(new Rect(10, 130, 200, 30), "Test Query 3"))
            {
                ExecuteTestQuery3();
            }
            
            if (GUI.Button(new Rect(10, 170, 200, 30), "Test Query 4"))
            {
                ExecuteTestQuery4();
            }
        }
    }
} 