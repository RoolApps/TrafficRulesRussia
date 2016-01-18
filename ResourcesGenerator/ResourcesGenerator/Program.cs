using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ResourcesGenerator
{
    
    class Program
    {
        static int Main(string[] args)
        {
            if(args.Length != 2)
            {
                return 1;
            }

            String pathToResourceFile = args.First();
            String pathToGeneratedFile = args.Last();

            XmlSerializer serializer = new XmlSerializer(typeof(Resources));

            Resources resources = null;
            try
            {
                using (FileStream fs = new FileStream(pathToResourceFile, FileMode.OpenOrCreate))
                {
                    resources = serializer.Deserialize(fs) as Resources;
                }
            }
            catch(Exception)
            {
                return 1;
            }

            ResourcesTemplate template = new ResourcesTemplate(resources);
            String resourcesContent = template.TransformText();
            
            try
            {
                File.WriteAllText(pathToGeneratedFile, resourcesContent);
            }
            catch(Exception)
            {
                return 1;
            }

            return 0;

        }
    }
}
