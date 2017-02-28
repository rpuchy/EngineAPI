using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EngineAPI
{
    public class ObjectDetails
    {
        public List<ParameterDetails> Parameters { get; set; }
        public List<ObjectDetails> SubObjects { get; set; }
        public List<TableDetails> Tables { get; set; }
        public List<String> ValueTypes { get; set; } 

        public static ObjectDetails LoadFromXml(XmlNode node)
        {
            return null;
        }
    }
}
