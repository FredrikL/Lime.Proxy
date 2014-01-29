using System.Collections.Generic;

namespace LimeProxy.Models
{
    public class TableQuery
    {
        public IEnumerable<string> Fields { get; set; }
        public IEnumerable<Condition> Conditions { get; set; } 
    }
}