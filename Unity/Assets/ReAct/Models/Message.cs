using System;
using UnityEngine;

namespace ReAct.Models
{
    [Serializable]
    public class Message
    {
        [SerializeField] private string role;
        [SerializeField] private string content;
        
        public string Role 
        { 
            get => role; 
            set => role = value; 
        }
        
        public string Content 
        { 
            get => content; 
            set => content = value; 
        }
        
        public Message(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
        
        public Message() { }
        
        public override string ToString()
        {
            return $"{role}: {content}";
        }
    }
} 