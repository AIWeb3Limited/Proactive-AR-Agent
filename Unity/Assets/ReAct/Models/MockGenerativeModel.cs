using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace ReAct.Models
{
    public class MockGenerativeModel : IGenerativeModel
    {
        public string ModelName => "Mock Model";
        
        public async Task<string> GenerateAsync(List<Message> messages)
        {
            // 模拟异步调用延迟
            await Task.Delay(100);
            
            var lastMessage = messages[messages.Count - 1];
            
            // 简单的模拟响应逻辑
            if (lastMessage.Content.ToLower().Contains("count") || lastMessage.Content.ToLower().Contains("统计"))
            {
                return @"{
    ""thought"": ""用户想要统计文件，我需要使用 count_files 工具"",
    ""action"": {
        ""name"": ""COUNT_FILES"",
        ""reason"": ""统计指定目录下的文件数量"",
        ""input"": {
            ""path"": ""Assets"",
            ""filePattern"": ""*.cs"",
            ""recursive"": true
        }
    }
}";
            }
            
            if (lastMessage.Content.ToLower().Contains("find") || lastMessage.Content.ToLower().Contains("查找"))
            {
                return @"{
    ""thought"": ""用户想要查找文件，我需要使用 find_files 工具"",
    ""action"": {
        ""name"": ""FIND_FILES"",
        ""reason"": ""查找指定模式的文件"",
        ""input"": {
            ""path"": ""Assets"",
            ""filePattern"": ""*.cs"",
            ""recursive"": true
        }
    }
}";
            }
            
            // 默认响应
            return @"{
    ""thought"": ""用户的查询比较简单，我可以直接回答"",
    ""answer"": ""我是一个ReAct Agent，可以帮助您搜索和统计Unity项目中的文件。请告诉我您想要查找什么文件或进行什么统计。""
}";
        }
    }
} 