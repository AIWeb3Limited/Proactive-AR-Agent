using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace ReAct.Utils
{
    /// <summary>
    /// 简单的HTTP客户端，用于调用AI API
    /// </summary>
    public static class SimpleHttpClient
    {
        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="jsonData">JSON数据</param>
        /// <param name="headers">请求头</param>
        /// <returns>响应内容</returns>
        public static async Task<string> PostAsync(string url, string jsonData, Dictionary<string, string> headers = null)
        {
            try
            {
                using (var request = new UnityWebRequest(url, "POST"))
                {
                    // 设置请求体
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.downloadHandler = new DownloadHandlerBuffer();
                    
                    // 设置请求头
                    request.SetRequestHeader("Content-Type", "application/json");
                    
                    if (headers != null)
                    {
                        foreach (var header in headers)
                        {
                            request.SetRequestHeader(header.Key, header.Value);
                        }
                    }
                    
                    // 发送请求
                    var operation = request.SendWebRequest();
                    
                    // 等待请求完成
                    while (!operation.isDone)
                    {
                        await Task.Yield();
                    }
                    
                    // 检查错误
                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        Debug.LogError($"HTTP请求失败: {request.error}");
                        Debug.LogError($"响应代码: {request.responseCode}");
                        Debug.LogError($"响应内容: {request.downloadHandler.text}");
                        throw new Exception($"HTTP请求失败: {request.error}");
                    }
                    
                    return request.downloadHandler.text;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"HTTP请求异常: {e.Message}");
                throw;
            }
        }
        
        /// <summary>
        /// 创建OpenAI API请求的JSON数据
        /// </summary>
        /// <param name="messages">消息列表</param>
        /// <param name="model">模型名称</param>
        /// <param name="temperature">温度参数</param>
        /// <returns>JSON字符串</returns>
        public static string CreateOpenAIRequestJson(List<object> messages, string model = "gpt-3.5-turbo", float temperature = 0.7f)
        {
            var request = new
            {
                model = model,
                messages = messages,
                temperature = temperature,
                max_tokens = 2000
            };
            
            return JsonUtility.ToJson(request);
        }
        
        /// <summary>
        /// 解析OpenAI API响应
        /// </summary>
        /// <param name="responseJson">响应JSON</param>
        /// <returns>消息内容</returns>
        public static string ParseOpenAIResponse(string responseJson)
        {
            try
            {
                // 使用简单的字符串解析，因为我们只需要content字段
                var contentMatch = System.Text.RegularExpressions.Regex.Match(
                    responseJson, 
                    "\"content\"\\s*:\\s*\"([^\"]+)\""
                );
                
                if (contentMatch.Success)
                {
                    return contentMatch.Groups[1].Value;
                }
                
                Debug.LogError($"无法解析OpenAI响应: {responseJson}");
                return "";
            }
            catch (Exception e)
            {
                Debug.LogError($"解析OpenAI响应失败: {e.Message}");
                return "";
            }
        }
    }
} 