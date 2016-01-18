using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourcesGenerator
{
    public partial class ResourcesTemplate
    {
        Resources Resources { get; set; }

        public ResourcesTemplate(Resources resources)
        {
            Resources = resources;
        }
    }
}
