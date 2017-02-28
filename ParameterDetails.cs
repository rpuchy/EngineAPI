using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EngineAPI
{
    public class ParameterDetails
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string constraint { get; set; }
        public string depends { get; set; }
        public string subType { get; set; }
        public string metaType { get; set; }
        public string reference { get; set; }
        public string referenceField { get; set; }
        public string referenceSubType { get; set; }
        public string referenceMetaType { get; set; }
        public string minOccurs { get; set; }
        public string maxOccurs { get; set; }
        public string defaultval { get; set; }
        public string defaultType { get; set; }
        public string unique { get; set; }
        public string uniqueSearchRef { get; set; }
        public bool isUniqueScope { get; set; }
        public string UniqueScopeTo { get; set; }
        public string Description { get; set; }
        public string bounds { get; set; }

        public static ParameterDetails LoadFromXml(XmlNode param)
        {
            ParameterDetails t = new ParameterDetails();
            t.Name = param.Name;
            t.Description = param.Attributes["desc"]?.Value;
            t.Type = param.Attributes["type"]?.Value;
            t.UniqueScopeTo = param.Attributes["UniqueScopeTo"]?.Value;
            t.bounds = param.Attributes["bounds"]?.Value;
            t.constraint = param.Attributes["constraint"]?.Value;
            t.defaultType = param.Attributes["defaultType"]?.Value;
            t.defaultval = param.Attributes["default"]?.Value;
            t.depends = param.Attributes["depends"]?.Value;
            t.isUniqueScope = param.Attributes["isUniqueScope"]?.Value == "true";
            t.maxOccurs = param.Attributes["maxOccurs"]?.Value;
            t.minOccurs = param.Attributes["minOccurs"]?.Value;
            t.metaType = param.Attributes["metaType"]?.Value;
            t.reference = param.Attributes["reference"]?.Value;
            t.referenceField = param.Attributes["referenceField"]?.Value;
            t.referenceMetaType = param.Attributes["referenceMetaType"]?.Value;
            t.referenceSubType = param.Attributes["referenceSubType"]?.Value;
            t.subType = param.Attributes["subType"]?.Value;
            t.unique = param.Attributes["unique"]?.Value;
            t.uniqueSearchRef = param.Attributes["uniqueSearchRef"]?.Value;
            return t;
        }
    }
}
