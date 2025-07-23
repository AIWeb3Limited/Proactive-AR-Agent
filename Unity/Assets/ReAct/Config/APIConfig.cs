using UnityEngine;

namespace ReAct.Config
{
    [CreateAssetMenu(fileName = "APIConfig", menuName = "ReAct/API Config")]
    public class APIConfig : ScriptableObject
    {
        [Header("OpenAI Configuration")]
        [SerializeField] private string openAIApiKey = "";
        [SerializeField] private string openAIBaseUrl = "https://api.openai.com/v1";
        [SerializeField] private string openAIModel = "gpt-3.5-turbo";
        
        [Header("Other AI Services")]
        [SerializeField] private string deepSeekApiKey = "";
        [SerializeField] private string deepSeekBaseUrl = "https://api.deepseek.com/v1";
        [SerializeField] private string deepSeekModel = "deepseek-chat";
        
        [Header("Azure OpenAI")]
        [SerializeField] private string azureApiKey = "";
        [SerializeField] private string azureEndpoint = "";
        [SerializeField] private string azureDeploymentName = "";
        
        public string OpenAIApiKey => openAIApiKey;
        public string OpenAIBaseUrl => openAIBaseUrl;
        public string OpenAIModel => openAIModel;
        
        public string DeepSeekApiKey => deepSeekApiKey;
        public string DeepSeekBaseUrl => deepSeekBaseUrl;
        public string DeepSeekModel => deepSeekModel;
        
        public string AzureApiKey => azureApiKey;
        public string AzureEndpoint => azureEndpoint;
        public string AzureDeploymentName => azureDeploymentName;
        
        private void OnValidate()
        {
            // 隐藏API密钥显示
            if (!string.IsNullOrEmpty(openAIApiKey) && openAIApiKey.Length > 8)
            {
                // 在Inspector中显示部分密钥
            }
        }
    }
} 