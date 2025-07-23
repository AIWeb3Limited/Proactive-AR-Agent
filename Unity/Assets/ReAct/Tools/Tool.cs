using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ReAct.Tools
{
    public class Tool
    {
        private ToolName name;
        private MethodInfo method;
        private object instance;
        
        public ToolName Name => name;
        public string Description { get; private set; }
        
        public Tool(ToolName name, MethodInfo method, object instance = null)
        {
            this.name = name;
            this.method = method;
            this.instance = instance;
            
            // 获取方法的描述信息
            var descriptionAttribute = method.GetCustomAttribute<ToolDescriptionAttribute>();
            Description = descriptionAttribute?.Description ?? "No description available";
        }
        
        public string ToolUse(Dictionary<string, object> parameters = null)
        {
            try
            {
                // 获取方法参数
                var parameterInfos = method.GetParameters();
                object[] args = new object[parameterInfos.Length];
                
                if (parameters != null)
                {
                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        var paramInfo = parameterInfos[i];
                        if (parameters.ContainsKey(paramInfo.Name))
                        {
                            args[i] = ConvertParameter(parameters[paramInfo.Name], paramInfo.ParameterType);
                        }
                        else if (paramInfo.HasDefaultValue)
                        {
                            args[i] = paramInfo.DefaultValue;
                        }
                    }
                }
                
                // 调用方法
                var result = method.Invoke(instance, args);
                return result?.ToString() ?? "";
            }
            catch (Exception e)
            {
                Debug.LogError($"Tool {name} execution error: {e.Message}");
                return $"Error: {e.Message}";
            }
        }
        
        private object ConvertParameter(object value, Type targetType)
        {
            if (value == null) return null;
            
            if (targetType == typeof(string))
                return value.ToString();
            
            if (targetType == typeof(bool))
                return Convert.ToBoolean(value);
            
            if (targetType == typeof(int))
                return Convert.ToInt32(value);
            
            if (targetType == typeof(float))
                return Convert.ToSingle(value);
            
            return value;
        }
        
        public string GetToolInfo()
        {
            return $"tool_name:{name.ToLowerString()}|tool_description:{Description}";
        }
    }
    
    [AttributeUsage(AttributeTargets.Method)]
    public class ToolDescriptionAttribute : Attribute
    {
        public string Description { get; }
        
        public ToolDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
} 