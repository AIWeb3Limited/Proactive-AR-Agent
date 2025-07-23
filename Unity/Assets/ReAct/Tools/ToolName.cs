using System;

namespace ReAct.Tools
{
    public enum ToolName
    {
        COUNT_FILES,
        FIND_FILES,
        SEARCH_ASSETS,
        FIND_SCRIPTS,
        FIND_PREFABS,
        FIND_SCENES,
        ELONINFO
    }
    
    public static class ToolNameExtensions
    {
        public static string ToLowerString(this ToolName toolName)
        {
            return toolName.ToString().ToLower();
        }
    }
} 