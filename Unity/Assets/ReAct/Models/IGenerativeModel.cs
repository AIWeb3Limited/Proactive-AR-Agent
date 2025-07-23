using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReAct.Models
{
    public interface IGenerativeModel
    {
        Task<string> GenerateAsync(List<Message> messages);
        string ModelName { get; }
    }
} 