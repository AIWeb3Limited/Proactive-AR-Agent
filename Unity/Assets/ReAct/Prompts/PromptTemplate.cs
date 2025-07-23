using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ReAct.Prompts
{
    [Serializable]
    public class Example
    {
        public string input;
        public string output;
        
        public Example(string input, string output)
        {
            this.input = input;
            this.output = output;
        }
    }
    
    public abstract class PromptTemplate
    {
        protected string prefix = "";
        protected List<Example> examples = new List<Example>();
        protected string suffix = "";
        protected string user = "";
        
        public virtual string GetFormattedSystemPrompt(Dictionary<string, string> parameters = null)
        {
            var formattedPrefix = FormatPrompt(prefix, parameters);
            var examplesStr = SetExamplesString();
            var formattedSuffix = FormatPrompt(suffix, parameters);
            
            return formattedPrefix + examplesStr + formattedSuffix;
        }
        
        public virtual string GetFormattedUserPrompt(Dictionary<string, string> parameters = null)
        {
            return FormatPrompt(user, parameters);
        }
        
        protected string SetExamplesString()
        {
            if (examples.Count == 0)
                return "";
            
            var examplesStr = new StringBuilder("\n\nExamples:\n");
            for (int i = 0; i < examples.Count; i++)
            {
                var example = examples[i];
                examplesStr.AppendLine($"Example {i + 1}:");
                examplesStr.AppendLine($"Input:\n{example.input}");
                examplesStr.AppendLine($"Output:\n{example.output}\n");
            }
            
            return examplesStr.ToString();
        }
        
        protected string FormatPrompt(string prompt, Dictionary<string, string> parameters)
        {
            if (string.IsNullOrEmpty(prompt) || parameters == null)
                return prompt;
            
            var result = prompt;
            foreach (var parameter in parameters)
            {
                var placeholder = "${" + parameter.Key + "}";
                result = result.Replace(placeholder, parameter.Value);
            }
            
            return result;
        }
        
        public void AddExample(string input, string output)
        {
            examples.Add(new Example(input, output));
        }
        
        public void ClearExamples()
        {
            examples.Clear();
        }
    }
} 