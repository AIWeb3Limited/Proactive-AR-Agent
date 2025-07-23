using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using ReAct.Models;
using ReAct.Prompts;

namespace ReAct.Agents
{
    public abstract class Agent
    {
        protected string agentName;
        protected PromptTemplate promptTemplate;
        protected List<Message> messages;
        
        public string AgentName => agentName;
        
        public Agent(string agentName, PromptTemplate promptTemplate)
        {
            this.agentName = agentName;
            this.promptTemplate = promptTemplate;
            this.messages = new List<Message>();
        }
        
        public virtual void RefreshSystem(Dictionary<string, string> parameters = null)
        {
            var systemPrompt = promptTemplate.GetFormattedSystemPrompt(parameters);
            
            if (messages.Count == 0)
            {
                messages.Add(new Message("system", systemPrompt));
            }
            else
            {
                messages[0] = new Message("system", systemPrompt);
            }
        }
        
        public virtual async Task<string> ResponseWithMemory(string query, Dictionary<string, string> parameters = null)
        {
            RefreshSystem(parameters);
            
            messages.Add(new Message("user", query));
            
            var response = await GenerateResponse(messages);
            
            messages.Add(new Message("assistant", response));
            
            return response;
        }
        
        public virtual async Task<string> ResponseWithoutMemory(string query, Dictionary<string, string> parameters = null)
        {
            RefreshSystem(parameters);
            
            var tempMessages = new List<Message>
            {
                new Message("system", promptTemplate.GetFormattedSystemPrompt(parameters)),
                new Message("user", query)
            };
            
            return await GenerateResponse(tempMessages);
        }
        
        protected abstract Task<string> GenerateResponse(List<Message> messages);
        
        public void ClearMemory()
        {
            messages.Clear();
        }
        
        public List<Message> GetMessages()
        {
            return new List<Message>(messages);
        }
        
        public void AddMessage(Message message)
        {
            messages.Add(message);
        }
    }
} 