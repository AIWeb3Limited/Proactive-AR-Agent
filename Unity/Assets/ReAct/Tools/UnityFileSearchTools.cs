using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ReAct.Tools
{
    public class UnityFileSearchTools
    {

        [ToolDescription("Elon的信息")]
        public static string ELONINFO(string param1)
        {
            return "Elon 是超高校级编程手！";
        }


        [ToolDescription("在指定路径下查找匹配模式的文件，并返回文件信息列表")]
        public static string FindFiles(string path = "Assets", string filePattern = "*", bool recursive = false)
        {
            try
            {
                var results = new List<FileInfo>();
                var searchPath = Path.GetFullPath(path);
                
                if (!Directory.Exists(searchPath))
                {
                    return $"路径不存在: {path}";
                }
                
                SearchOption searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                
                // 处理通配符模式
                string[] patterns = filePattern.Split(',');
                foreach (var pattern in patterns)
                {
                    var files = Directory.GetFiles(searchPath, pattern.Trim(), searchOption);
                    foreach (var file in files)
                    {
                        var fileInfo = new FileInfo(file);
                        results.Add(fileInfo);
                    }
                }
                
                // 排序并格式化结果
                var sortedResults = results.OrderBy(f => f.Name).ToList();
                var resultList = new List<string>();
                
                foreach (var file in sortedResults)
                {
                    resultList.Add($"文件名: {file.Name}, 路径: {GetRelativePath(file.FullName)}");
                }
                
                return $"找到 {resultList.Count} 个文件:\n" + string.Join("\n", resultList);
            }
            catch (Exception e)
            {
                return $"文件查找失败: {e.Message}";
            }
        }
        
        [ToolDescription("统计指定路径下匹配模式的文件数量")]
        public static string CountFiles(string path = "Assets", string filePattern = "*", bool recursive = false)
        {
            try
            {
                var searchPath = Path.GetFullPath(path);
                
                if (!Directory.Exists(searchPath))
                {
                    return $"路径不存在: {path}";
                }
                
                SearchOption searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                
                int totalCount = 0;
                var sampleFiles = new List<string>();
                
                // 处理通配符模式
                string[] patterns = filePattern.Split(',');
                foreach (var pattern in patterns)
                {
                    var files = Directory.GetFiles(searchPath, pattern.Trim(), searchOption);
                    totalCount += files.Length;
                    
                    // 保留前5个文件作为示例
                    foreach (var file in files.Take(5 - sampleFiles.Count))
                    {
                        sampleFiles.Add(GetRelativePath(file));
                        if (sampleFiles.Count >= 5) break;
                    }
                }
                
                var result = $"统计结果:\n" +
                           $"总数: {totalCount}\n" +
                           $"路径: {GetRelativePath(searchPath)}\n" +
                           $"模式: {filePattern}\n";
                
                if (sampleFiles.Any())
                {
                    result += $"示例文件:\n" + string.Join("\n", sampleFiles);
                }
                
                return result;
            }
            catch (Exception e)
            {
                return $"文件统计失败: {e.Message}";
            }
        }
        
        [ToolDescription("搜索 Unity 项目中的资源文件")]
        public static string SearchAssets(string assetType = "all", string namePattern = "*")
        {
            try
            {
                var results = new List<string>();
                
                // 根据资源类型搜索
                switch (assetType.ToLower())
                {
                    case "script":
                    case "cs":
                        results.AddRange(Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories));
                        break;
                    case "prefab":
                        results.AddRange(Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories));
                        break;
                    case "scene":
                        results.AddRange(Directory.GetFiles("Assets", "*.unity", SearchOption.AllDirectories));
                        break;
                    case "texture":
                    case "image":
                        results.AddRange(Directory.GetFiles("Assets", "*.png", SearchOption.AllDirectories));
                        results.AddRange(Directory.GetFiles("Assets", "*.jpg", SearchOption.AllDirectories));
                        results.AddRange(Directory.GetFiles("Assets", "*.jpeg", SearchOption.AllDirectories));
                        break;
                    case "material":
                        results.AddRange(Directory.GetFiles("Assets", "*.mat", SearchOption.AllDirectories));
                        break;
                    case "audio":
                        results.AddRange(Directory.GetFiles("Assets", "*.wav", SearchOption.AllDirectories));
                        results.AddRange(Directory.GetFiles("Assets", "*.mp3", SearchOption.AllDirectories));
                        results.AddRange(Directory.GetFiles("Assets", "*.ogg", SearchOption.AllDirectories));
                        break;
                    default:
                        results.AddRange(Directory.GetFiles("Assets", "*", SearchOption.AllDirectories));
                        break;
                }
                
                // 应用名称过滤
                if (namePattern != "*")
                {
                    results = results.Where(f => Path.GetFileNameWithoutExtension(f).Contains(namePattern)).ToList();
                }
                
                var formattedResults = results.Select(GetRelativePath).OrderBy(x => x).ToList();
                
                return $"找到 {formattedResults.Count} 个 {assetType} 资源:\n" + 
                       string.Join("\n", formattedResults.Take(20)); // 限制显示前20个
            }
            catch (Exception e)
            {
                return $"资源搜索失败: {e.Message}";
            }
        }
        
        [ToolDescription("查找包含特定内容的脚本文件")]
        public static string FindScripts(string contentPattern = "", string namePattern = "*")
        {
            try
            {
                var scriptFiles = Directory.GetFiles("Assets", "*.cs", SearchOption.AllDirectories);
                var results = new List<string>();
                
                foreach (var scriptFile in scriptFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(scriptFile);
                    
                    // 检查文件名是否匹配
                    if (namePattern != "*" && !fileName.Contains(namePattern))
                        continue;
                    
                    // 检查文件内容是否匹配
                    if (!string.IsNullOrEmpty(contentPattern))
                    {
                        var content = File.ReadAllText(scriptFile);
                        if (!content.Contains(contentPattern))
                            continue;
                    }
                    
                    results.Add(GetRelativePath(scriptFile));
                }
                
                return $"找到 {results.Count} 个脚本文件:\n" + string.Join("\n", results);
            }
            catch (Exception e)
            {
                return $"脚本搜索失败: {e.Message}";
            }
        }
        
        private static string GetRelativePath(string fullPath)
        {
            var projectPath = Directory.GetCurrentDirectory();
            
            // 兼容性实现，替代 Path.GetRelativePath
            var projectUri = new Uri(projectPath + Path.DirectorySeparatorChar);
            var fullUri = new Uri(fullPath);
            
            if (projectUri.Scheme != fullUri.Scheme) 
            {
                return fullPath; // 不同的驱动器或scheme
            }
            
            Uri relativeUri = projectUri.MakeRelativeUri(fullUri);
            return Uri.UnescapeDataString(relativeUri.ToString()).Replace('/', Path.DirectorySeparatorChar).Replace("\\", "/");
        }
    }
} 