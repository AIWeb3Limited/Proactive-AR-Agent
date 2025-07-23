using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ReAct.Utils
{
    /// <summary>
    /// 简单的 JSON 解析器，用于替代 Newtonsoft.Json
    /// 专门为 ReAct Agent 的响应格式设计
    /// </summary>
    public static class SimpleJsonParser
    {
        /// <summary>
        /// 解析 ReAct Agent 的 JSON 响应
        /// </summary>
        /// <param name="json">JSON 字符串</param>
        /// <returns>解析后的字典</returns>
        public static Dictionary<string, object> ParseReactResponse(string json)
        {
            try
            {
                var result = new Dictionary<string, object>();
                
                // 清理 JSON 字符串
                json = json.Trim();
                if (json.StartsWith("```json"))
                {
                    json = json.Substring(7);
                }
                if (json.EndsWith("```"))
                {
                    json = json.Substring(0, json.Length - 3);
                }
                json = json.Trim();
                
                // 提取 thought
                var thoughtMatch = Regex.Match(json, "\"thought\"\\s*:\\s*\"([^\"]+)\"");
                if (thoughtMatch.Success)
                {
                    result["thought"] = thoughtMatch.Groups[1].Value;
                }
                
                // 检查是否有 action (使用更健壮的方法)
                var actionMatch = Regex.Match(json, "\"action\"\\s*:\\s*\\{");
                if (actionMatch.Success)
                {
                    var startIndex = actionMatch.Index + actionMatch.Length - 1;
                    var braceCount = 1;
                    var endIndex = startIndex + 1;
                    
                    // 找到匹配的右大括号
                    while (endIndex < json.Length && braceCount > 0)
                    {
                        if (json[endIndex] == '{') braceCount++;
                        else if (json[endIndex] == '}') braceCount--;
                        if (braceCount > 0) endIndex++;
                    }
                    
                    if (braceCount == 0)
                    {
                        var actionContent = json.Substring(startIndex + 1, endIndex - startIndex - 1);
                        var actionDict = ParseActionObject(actionContent);
                        result["action"] = actionDict;
                    }
                }
                
                // 检查是否有 answer
                var answerMatch = Regex.Match(json, "\"answer\"\\s*:\\s*\"([^\"]+)\"");
                if (answerMatch.Success)
                {
                    result["answer"] = answerMatch.Groups[1].Value;
                }
                
                return result;
            }
            catch (Exception e)
            {
                Debug.LogError($"JSON 解析失败: {e.Message}");
                return new Dictionary<string, object>();
            }
        }
        
        /// <summary>
        /// 解析 action 对象
        /// </summary>
        /// <param name="actionContent">action 内容</param>
        /// <returns>解析后的字典</returns>
        private static Dictionary<string, object> ParseActionObject(string actionContent)
        {
            var result = new Dictionary<string, object>();
            
            // 提取 name
            var nameMatch = Regex.Match(actionContent, "\"name\"\\s*:\\s*\"([^\"]+)\"");
            if (nameMatch.Success)
            {
                result["name"] = nameMatch.Groups[1].Value;
            }
            
            // 提取 reason
            var reasonMatch = Regex.Match(actionContent, "\"reason\"\\s*:\\s*\"([^\"]+)\"");
            if (reasonMatch.Success)
            {
                result["reason"] = reasonMatch.Groups[1].Value;
            }
            
            // 提取 input (使用更健壮的方法)
            var inputMatch = Regex.Match(actionContent, "\"input\"\\s*:\\s*\\{");
            if (inputMatch.Success)
            {
                var startIndex = inputMatch.Index + inputMatch.Length - 1;
                var braceCount = 1;
                var endIndex = startIndex + 1;
                
                // 找到匹配的右大括号
                while (endIndex < actionContent.Length && braceCount > 0)
                {
                    if (actionContent[endIndex] == '{') braceCount++;
                    else if (actionContent[endIndex] == '}') braceCount--;
                    if (braceCount > 0) endIndex++;
                }
                
                if (braceCount == 0)
                {
                    var inputContent = actionContent.Substring(startIndex + 1, endIndex - startIndex - 1);
                    var inputDict = ParseInputObject(inputContent);
                    result["input"] = inputDict;
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// 解析 input 对象
        /// </summary>
        /// <param name="inputContent">input 内容</param>
        /// <returns>解析后的字典</returns>
        private static Dictionary<string, object> ParseInputObject(string inputContent)
        {
            var result = new Dictionary<string, object>();
            
            // 使用正则表达式提取键值对
            var matches = Regex.Matches(inputContent, "\"([^\"]+)\"\\s*:\\s*\"([^\"]+)\"");
            foreach (Match match in matches)
            {
                var key = match.Groups[1].Value;
                var value = match.Groups[2].Value;
                result[key] = value;
            }
            
            // 处理布尔值
            var boolMatches = Regex.Matches(inputContent, "\"([^\"]+)\"\\s*:\\s*(true|false)");
            foreach (Match match in boolMatches)
            {
                var key = match.Groups[1].Value;
                var value = bool.Parse(match.Groups[2].Value);
                result[key] = value;
            }
            
            return result;
        }
        
        /// <summary>
        /// 将字典转换为 JSON 字符串（简单实现）
        /// </summary>
        /// <param name="dict">要转换的字典</param>
        /// <returns>JSON 字符串</returns>
        public static string DictionaryToJson(Dictionary<string, object> dict)
        {
            var json = "{";
            var first = true;
            
            foreach (var kvp in dict)
            {
                if (!first) json += ",";
                json += $"\"{kvp.Key}\":";
                
                if (kvp.Value is string)
                {
                    json += $"\"{kvp.Value}\"";
                }
                else if (kvp.Value is bool)
                {
                    json += kvp.Value.ToString().ToLower();
                }
                else if (kvp.Value is Dictionary<string, object>)
                {
                    json += DictionaryToJson((Dictionary<string, object>)kvp.Value);
                }
                else
                {
                    json += $"\"{kvp.Value}\"";
                }
                
                first = false;
            }
            
            json += "}";
            return json;
        }
    }
} 