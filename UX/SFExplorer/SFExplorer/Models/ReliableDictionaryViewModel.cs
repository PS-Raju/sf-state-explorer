using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFExplorer.Models
{
    public class ReliableDictionaryViewModel
    {
        public string AppName;

        public string ServiceName;

        public string CollectionName;

        public List<string> KeyValuePairs { get; set; }
    }
}