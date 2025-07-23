using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using ReAct.Config;
using ReAct.Utils;

namespace ReAct.Models
{
    public class OpenAIModel : IGenerativeModel
    {
        private APIConfig config;
        private string apiKey;
        private string baseUrl;
        private string model;
        
        public string ModelName => model ?? "OpenAI GPT";
        
        public OpenAIModel(APIConfig config)
        {
            this.config = config;
            this.apiKey = config.OpenAIApiKey;
            this.baseUrl = config.OpenAIBaseUrl;
            this.model = config.OpenAIModel;
            
            ValidateConfiguration();
        }
        
        public OpenAIModel(string apiKey, string baseUrl = "https://api.openai.com/v1", string model = "gpt-3.5-turbo")
        {
            this.apiKey = apiKey;
            this.baseUrl = baseUrl;
            this.model = model;
            
            ValidateConfiguration();
        }
        
        private void ValidateConfiguration()
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentException("OpenAI API Key 不能为空。请在APIConfig中设置或通过构造函数传入。");
            }
            
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentException("OpenAI Base URL 不能为空。");
            }
            
            Debug.Log($"OpenAI模型已初始化: {ModelName}");
        }
        
        public async Task<string> GenerateAsync(List<Message> messages)
        {
            try
            {
                // 构建请求URL
                var url = $"{baseUrl.TrimEnd('/')}/chat/completions";
                
                // 构建请求JSON
                var requestJson = CreateRequestJson(messages);
                
                // 设置请求头
                var headers = new Dictionary<string, string>
                {
                    ["Authorization"] = $"Bearer {apiKey}"
                };
                
                Debug.Log($"发送OpenAI请求到: {url}");
                Debug.Log($"请求内容: {requestJson}");
                
                // 发送请求
                var responseJson = await SimpleHttpClient.PostAsync(url, requestJson, headers);
                
                Debug.Log($"OpenAI响应: {responseJson}");
                
                // 解析响应
                var content = ParseResponse(responseJson);
                
                if (string.IsNullOrEmpty(content))
                {
                    throw new Exception("OpenAI API 返回空响应");
                }

                Debug.Log($"当前内容为: {content}");
                return content;
            }
            catch (Exception e)
            {
                Debug.LogError($"OpenAI API 调用失败: {e.Message}");
                
                // 返回错误处理的默认响应
                return CreateErrorResponse(e.Message);
            }
        }
        
        private string CreateRequestJson(List<Message> messages)
        {
            // 直接从Message对象构建JSON，避免使用dynamic
            var messagesJson = string.Join(",", messages.Select(m => 
                $"{{\"role\":\"{m.Role}\",\"content\":\"{EscapeJsonString(m.Content)}\"}}"));
            
            return $"{{" +
                   $"\"model\":\"{this.model}\"," +
                   $"\"messages\":[{messagesJson}]," +
                   $"\"temperature\":0.7," +
                   $"\"max_tokens\":2000" +
                   $"}}";
        }
        
        private string EscapeJsonString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;
            
            return input
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
        }

        private string ParseResponse(string responseJson)
        {
            try
            {
                // 提取content字段的值（处理嵌套JSON）
                var pattern = @"""content""\s*:\s*""((?:[^""\\]|\\.)*)""";
                var match = System.Text.RegularExpressions.Regex.Match(responseJson, pattern);

                if (match.Success)
                {
                    var contentString = match.Groups[1].Value;

                    // 反转义
                    contentString = contentString
                        .Replace("\\\"", "\"")
                        .Replace("\\n", "\n")
                        .Replace("\\r", "\r")
                        .Replace("\\t", "\t")
                        .Replace("\\\\", "\\");

                    // 提取action部分
                    var actionPattern = @"""action""\s*:\s*\{[^}]*(?:\{[^}]*\}[^}]*)*\}";
                    var actionMatch = System.Text.RegularExpressions.Regex.Match(contentString, actionPattern);

                    if (actionMatch.Success)
                    {
                        return "{" + actionMatch.Value + "}";
                    }

                    return contentString;
                }

                return responseJson;
            }
            catch (Exception e)
            {
                Debug.LogError($"解析OpenAI响应失败: {e.Message}");
                return "";
            }
        }

        private string CreateErrorResponse(string errorMessage)
        {
            // 返回一个表示错误的JSON响应
            return $"{{" +
                   $"\"thought\": \"发生错误: {EscapeJsonString(errorMessage)}\"," +
                   $"\"answer\": \"抱歉，AI服务暂时不可用。错误信息: {EscapeJsonString(errorMessage)}\"" +
                   $"}}";
        }
    }
} 