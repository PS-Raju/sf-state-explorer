using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFExplorer.Models
{
    public class StatefulServiceViewModel
    {
        public string AppName;

        public string ServiceName;

        public List<Collection> Collections { get; set; }
    }
}