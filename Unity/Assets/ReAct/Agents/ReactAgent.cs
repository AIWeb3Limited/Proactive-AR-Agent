using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using ReAct.Models;
using ReAct.Prompts;
using ReAct.Tools;
using ReAct.Utils;

namespace ReAct.Agents
{
    public class ReactAgent : Agent
    {
        private int maxIteration = 5;
        private int currentIteration = 0;
        private Dictionary<ToolName, Tool> tools;
        private List<Message> history;
        private IGenerativeModel generativeModel;
        
        public ReactAgent(IGenerativeModel model = null) 
            : base("ReactAgent", new ReactPrompt())
        {
            this.generativeModel = model ?? new MockGenerativeModel();
            this.tools = new Dictionary<ToolName, Tool>();
            this.history = new List<Message>();
            
            // 自动注册工具
            RegisterDefaultTools();
        }
        
        private void RegisterDefaultTools()
        {
            var toolsType = typeof(UnityFileSearchTools);
            var methods = toolsType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            
            foreach (var method in methods)
            {
                var descriptionAttr = method.GetCustomAttribute<ToolDescriptionAttribute>();
                if (descriptionAttr != null)
                {
                    switch (method.Name)
                    {
                        case "FindFiles":
                            SetTool(ToolName.FIND_FILES, method);
                            break;
                        case "CountFiles":
                            SetTool(ToolName.COUNT_FILES, method);
                            break;
                        case "SearchAssets":
                            SetTool(ToolName.SEARCH_ASSETS, method);
                            break;
                        case "FindScripts":
                            SetTool(ToolName.FIND_SCRIPTS, method);
                            break;
                        case "ElonInFo":
                            SetTool(ToolName.ELONINFO, method);
                            break;
                    }
                }
            }
        }
        
        public void SetTool(ToolName name, MethodInfo method, object instance = null)
        {
            tools[name] = new Tool(name, method, instance);
        }
        
        public async Task<string> Execute(string query)
        {
            currentIteration = 0;
            history.Clear();
            
            await Think(query);
            
            var finalAnswer = history.LastOrDefault()?.Content ?? "无法生成答案";
            Debug.Log($"Final Answer: {finalAnswer}");
            
            return finalAnswer;
        }
        
        private async Task Think(string query)
        {
            currentIteration++;
            Debug.Log($"Iteration {currentIteration}");
            
            if (currentIteration >= maxIteration)
            {
                SetHistory("think", "system", "达到最大迭代次数，停止运行");
                return;
            }
            
            var parameters = new Dictionary<string, string>
            {
                ["query"] = query,
                ["history"] = GetHistory(),
                ["tools"] = GetToolsInfo()
            };
            
            var queryTemplate = promptTemplate.GetFormattedUserPrompt(parameters);
            Debug.Log($"Query Template: {queryTemplate}");
            
            var response = await ResponseWithoutMemory(queryTemplate);
            SetHistory("think", "assistant", response);
            
            await Decide(response, query);
        }
        
        private async Task Decide(string response, string query)
        {
            try
            {
                // 递归解析嵌套JSON字符串
                var jsonResponse = TryParseNestedJson(response);
                
                if (jsonResponse.ContainsKey("action"))
                {
                    var actionObj = jsonResponse["action"] as Dictionary<string, object>;
                    if (actionObj != null && actionObj.ContainsKey("name"))
                    {
                        var toolName = actionObj["name"].ToString().ToUpper();
                        
                        if (Enum.TryParse<ToolName>(toolName, out var toolEnum))
                        {
                            if (tools.ContainsKey(toolEnum))
                            {
                                SetHistory("decide", "assistant", $"使用工具: {toolEnum}");
                                var inputObj = actionObj.ContainsKey("input") ? actionObj["input"] as Dictionary<string, object> : new Dictionary<string, object>();
                                await Act(toolEnum, query, inputObj ?? new Dictionary<string, object>());
                            }
                            else
                            {
                                SetHistory("decide", "assistant", $"工具不存在: {toolEnum}");
                                await Think(query);
                            }
                        }
                        else
                        {
                            SetHistory("decide", "assistant", $"无效的工具名称: {toolName}");
                            await Think(query);
                        }
                    }
                    else
                    {
                        SetHistory("decide", "assistant", "Action 对象格式错误");
                        await Think(query);
                    }
                }
                else if (jsonResponse.ContainsKey("answer"))
                {
                    SetHistory("decide", "assistant", jsonResponse["answer"].ToString());
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"JSON解析错误: {e.Message}");
                SetHistory("decide", "assistant", $"解析错误，重新思考: {e.Message}");
                await Think(query);
            }
        }
        
        private async Task Act(ToolName toolName, string query, Dictionary<string, object> inputParams)
        {
            if (tools.ContainsKey(toolName))
            {
                var tool = tools[toolName];
                var observation = tool.ToolUse(inputParams);
                
                SetHistory("act", "tool_result", $"工具 {toolName} 的结果: {observation}");
                await Think(query);
            }
            else
            {
                SetHistory("act", "assistant", $"工具 {toolName} 不存在！");
                await Think(query);
            }
        }
        
        private void SetHistory(string step, string role, string content)
        {
            Debug.Log($"[{step}] {role}: {content}");
            history.Add(new Message(role, content));
        }
        
        private string GetHistory()
        {
            if (history.Count == 0)
                return "暂无历史记录";
            
            return string.Join("\n", history.Take(10).Select(m => m.ToString()));
        }
        
        private string GetToolsInfo()
        {
            return string.Join(", ", tools.Values.Select(tool => tool.GetToolInfo()));
        }
        
        protected override async Task<string> GenerateResponse(List<Message> messages)
        {
            return await generativeModel.GenerateAsync(messages);
        }

        // 新增递归解析嵌套JSON字符串的方法
        private Dictionary<string, object> TryParseNestedJson(string json)
        {
            var parsed = SimpleJsonParser.ParseReactResponse(json);
            // 如果只解析出一个 "content" 字段，且它本身是JSON，则递归解析
            if (parsed.Count == 1 && parsed.ContainsKey("content"))
            {
                var inner = parsed["content"] as string;
                if (!string.IsNullOrEmpty(inner) && inner.TrimStart().StartsWith("{"))
                {
                    return TryParseNestedJson(inner);
                }
            }
            return parsed;
        }
    }
} 