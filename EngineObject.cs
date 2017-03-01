using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.Linq;
using System.Security.RightsManagement;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Data;
using System.Text.RegularExpressions;
using System.Xml;

namespace EngineAPI
{
    public class EngineObject
    {

        protected XmlNode _innerXml { get; set; }
        protected Schema _schema;


        public EngineObject(XmlNode _xmlNode, XmlDocument _xmlSchema)
        {
            _innerXml = _xmlNode;
            _schema = new Schema(_xmlSchema);
            SetFullyQualifiedName();
        }

        public EngineObject(XmlNode _xmlNode, Schema Schema)
        {
            _innerXml = _xmlNode;
            _schema = Schema;
            SetFullyQualifiedName();
        }

        /// <summary>
        /// Parameterless constructor for inheriting classes
        /// </summary>
        protected EngineObject()
        {

        }

        protected void SetFullyQualifiedName()
        {
            //Search through the parent hirearchy and set the fully qualified name
            string parent = "";
            XmlNode currentNode = _innerXml;
            while(currentNode.ParentNode!=null)
            {
                if (currentNode.ParentNode.Name != "#document")
                {
                    parent = currentNode.ParentNode.Name + "." + parent;
                }
                currentNode = currentNode.ParentNode;
            }
            FullyQualifiedName = parent+_innerXml.Name;               
        }

        /// <summary>
        /// Search through the XML node and identify any parameters then add them to the list
        /// </summary>
        /// <returns>A list of parameters in this node</returns>
        public ParamList Parameters
        {
            get
            {
                ParamList _parameters = new ParamList();
                foreach (XmlNode _node in _innerXml.ChildNodes)
                {
                    if (IsParameter(_node))
                    {
                        var p = new Parameter() { Name = _node.Name, Value = _node.InnerText };
                        p.PropertyChanged += P_PropertyChanged;
                        _parameters.Add(p);
                    }
                }
                _parameters.PropertyChanged += _parameters_PropertyChanged;
                return _parameters;
            }
        }

        private void _parameters_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var doc = _innerXml.OwnerDocument;
            var param = ((ParamList)sender)[((ParamList)sender).Count - 1];
            var node = doc.CreateElement(param.Name);
            node.AppendChild(doc.CreateTextNode(param.Value.ToString()));
            _innerXml.AppendChild(node);
        }

        private void P_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Parameter s = (Parameter) sender;
            var node = _innerXml.SelectSingleNode(s.Name);
            if (node != null)
            {
                node.InnerText = s.Value.ToString();
            }
        }

        /// <summary>
        /// Search through XML node and identify any children then add them to the list
        /// </summary>
        /// <returns>A list of the children objects present in this node</returns>
        public List<EngineObject> Children
        {
            get
            {
                List<EngineObject> _children = new List<EngineObject>();
                foreach (XmlNode _node in _innerXml.ChildNodes)
                {
                    if (!IsParameter(_node) && !IsComment(_node) && !IsTable(_node))
                    {
                        _children.Add(new EngineObject(_node,_schema));
                    }
                }
                return _children;
            }
        }

        /// <summary>
        /// Search through the xml and return a list of tables if they exist
        /// </summary>
        /// <returns>A list of tables in this node</returns>
        public List<Table> Tables
        {
            get
            {
                List<Table> _tables = new List<Table>();
                foreach (XmlNode _node in _innerXml.ChildNodes)
                {
                    if (IsTable(_node))
                    {
                        _tables.Add(new Table(_node));
                    }
                }
                return _tables;
            }
        }


        public string Name => _innerXml.Name;

        /// <summary>
        /// The name of the object if it exists
        /// </summary>
        public string ObjectName => _innerXml.SelectSingleNode("./Name|./Params/Name")?.InnerText;

        public string ObjectType => _innerXml.SelectSingleNode("./Type|./Params/Type|./Class")?.InnerText;

        public string FullyQualifiedName { get; set; }

        public XmlNode SelectSingleNode(string xPath)
        {
            return _innerXml.SelectSingleNode(xPath);
        }

        public XmlNodeList SelectNodes(string xPath)
        {
            return _innerXml.SelectNodes(xPath);
        }

        /// <summary>
        /// Search through the Schema and return a list of addable submodels
        /// </summary>
        /// <returns>A list of addable submodels</returns>
        public List<ObjectDetails> AddableObjects()
        {
            //This is for addable objects e.g. ESG models/products/rebalance rules
            List<ObjectDetails> templist = new List<ObjectDetails>();
            try
            {
                XmlNode Params = _schema.GetObjectSchema(this);
                var ObjectList = Schema.GetObjectsFromXml(Params, Schema.ObjectClassifier.Optional);
                //Now we check if any objects have maxOccurs=1 and already exist

                foreach (var obj in ObjectList)
                {
                    if (obj.maxOccurs >= 1)
                    {
                        int count = Children.FindAll(x => x.Name == obj.NodeName).Count;
                        if (count < obj.maxOccurs)
                        {
                            templist.Add(obj);
                        }
                    }
                }
            }
            catch(Exception e)
            {

            }
            return templist;
        }


        public EngineObject AddObject(ObjectDetails AddObject)
        {
            //first check we can add the object
            if (AddableObjects().Find(x=>x.NodeName==AddObject.NodeName)!=null)
            {
                var node = _innerXml.OwnerDocument.ImportNode(AddObject.XmlSnippet, true);
                node.Attributes.RemoveAll();
                foreach (XmlElement el in node.SelectNodes(".//*"))
                {
                    if (el.Attributes[Schema.defaultValue] != null)
                    {
                        el.InnerText = el.Attributes[Schema.defaultValue].Value;
                    }
                    el.Attributes.RemoveAll();
                }
                _innerXml.AppendChild(node);
                return new EngineObject(node,_schema);
            }
            return null;
        }
        
        public List<ParameterDetails> AddableParameters()
        {
            //this is so we can see optional parameters
            
            XmlNode Params = _schema.GetObjectSchema(this);
            return Schema.GetParametersFromXml(Params);           
        }

        public void AddParameter(String pName,string pValue)
        {
            Parameters.Add(new Parameter() { Name = pName, Value = pValue });
        }

        public List<string> AddableValueTypes()
        {            
            //Get the models
            XmlNode Params = _schema.GetObjectSchema(this);            
            return Schema.GetValueTypesfromXml(Params);
        }

        protected void AddQuery(string ValueType)
        {
            throw new NotImplementedException();
        }

        protected void AddOperator(string Query,string OperatorType)
        {
            throw new NotImplementedException();
        }

        public void AddOutput(string ValueType, string OperatorType)
        {
            throw new NotImplementedException();
        }
        
        private static bool IsParameter(XmlNode node)
        {
            return ((node.ChildNodes.Count == 1 && node.FirstChild.Name == "#text") || node.ChildNodes.Count == 0);
        }

        private static bool IsComment(XmlNode node)
        {
            return (node.Name == "#comment");
        }

        private static bool IsTable(XmlNode node)
        {
            if (node.ChildNodes.Count<=1)
            {
                return false;
            }
            string lastname = node.FirstChild.Name;
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name!=lastname)
                {
                    return false;
                }
            }
            return true;
        }


        public EngineObject Parent => new EngineObject(_innerXml.ParentNode,_schema);

        /// <summary>
        /// Searches the XML and finds the first object that matches the node name
        /// </summary>
        /// <param name="nodename"></param>
        /// <returns></returns>
        public EngineObject FindObjectbyNodeName(string nodename)
        {
            string alternate = char.ToUpper(nodename[0]) + nodename.Substring(1);
            XmlNode temp = _innerXml.SelectSingleNode(".//" + nodename + "|.//"+alternate);
            return (temp != null) ? new EngineObject(temp,_schema) : null;
        }

        /// <summary>
        /// Search the XML and finds the objects that matches the node name
        /// </summary>
        /// <param name="nodename"></param>
        /// <returns></returns>
        public List<EngineObject> FindObjectsbyNodeName(string nodename)
        {
            string alternate = char.ToUpper(nodename[0]) + nodename.Substring(1);
            XmlNodeList temp = _innerXml.SelectNodes(".//" + nodename + "|.//" + alternate);
            List<EngineObject> _tempout = new List<EngineObject>();
            foreach(XmlNode node in temp)
            {
                _tempout.Add(new EngineObject(node,_schema));
            }
            return _tempout;
        }

        /// <summary>
        /// Searches the XML and finds the first object whose "Name" child node is name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EngineObject FindObjectbyName(string name)
        {
            XmlNode temp = _innerXml.SelectSingleNode(".//*[Name='" + name + "']");
            return (temp != null) ? new EngineObject(temp,_schema) : null;
        }

        /// <summary>
        /// Seacrhes the XML and find the objects whose "Name" child node is name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public List<EngineObject> FindObjectsbyName(string name)
        {
            XmlNodeList temp = _innerXml.SelectNodes(".//*[Name='" + name + "']");
            List<EngineObject> _tempout = new List<EngineObject>();
            foreach (XmlNode node in temp)
            {
                _tempout.Add(new EngineObject(node,_schema));
            }
            return _tempout;
        }

        public static string GetAlternate(string input)
        {
            if (input.Any(c => char.IsUpper(c)))
            {
                string alternate = "";
                switch (input)
                {
                    case "ProbabilityState1ToState2":
                        alternate = "probability_state1_state2";
                        break;
                    case "ProbabilityState2ToState1":
                        alternate = "probability_state2_state1";
                        break;
                    case "SweepCashFlows":
                        alternate = "sweep_cashflows";
                        break;
                    default:
                        {
                            alternate = input.Replace("ID", "_id");
                            alternate = Regex.Replace(alternate, @"(?<=.)([A-Z][a-z])", "_$1").ToLower();
                            break;
                        }
                }
                return alternate;
            }
            else
            {
                string alternate = "";
                switch (input)
                {
                    case "code":
                        alternate = input;
                        break;
                    case "probability_state1_state2":
                        alternate = "ProbabilityState1ToState2";
                        break;
                    case "probability_state2_state1":
                        alternate = "ProbabilityState2ToState1";
                        break;
                    case "sweep_cashflows":
                        alternate = "SweepCashFlows";
                        break;
                    default:
                        {
                            alternate = Regex.Replace(input, "_id", "ID");
                            alternate = Regex.Replace(alternate, @"((_[a-z]))", m => m.ToString().ToUpper().Trim('_'));
                            alternate = char.ToUpper(alternate[0]) + alternate.Substring(1);
                            break;
                        }
                }
                return alternate;
            }
        }

    }

    

}