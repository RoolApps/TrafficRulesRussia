using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesGenerator
{
    [Serializable]
    public class Resources
    {
        public Resource[] ResourcesArray { get; set; }

        public Resources() { }
    }

    [Serializable]
    public class Resource
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string DefaultValue { get; set; }

        public Resource() { }
    }

}
