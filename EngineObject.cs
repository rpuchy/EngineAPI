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
        protected XmlDocument _schema = new XmlDocument();
        protected string _schemaFilename;

        public EngineObject(XmlNode _xmlNode, XmlDocument _xmlSchema)
        {
            _innerXml = _xmlNode;
            _schema = _xmlSchema;
        }

        /// <summary>
        /// Parameterless constructor for inheriting classes
        /// </summary>
        protected EngineObject()
        {

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
                return _parameters;
            }
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

        /// <summary>
        /// Search through the Schema and return a list of addable submodels
        /// </summary>
        /// <returns>A list of addable submodels</returns>
        public List<ObjectDetails> AddableObjects()
        {
            //This is for addable objects e.g. ESG models/products/rebalance rules
            List<ObjectDetails> templist = new List<ObjectDetails>();
            XmlNode Params = _schema.SelectSingleNode("//Simulation//" + this.Name);
            foreach (XmlNode param in Params.ChildNodes)
            {
                if (param.Attributes["type"].Value == "container")
                {
                    templist.Add(ObjectDetails.LoadFromXml(param));
                }
            }
            return templist;
        }



        public List<ParameterDetails> AddableParameters()
        {
            //this is so we can see optional parameters
            List<ParameterDetails> templist = new List<ParameterDetails>();
            XmlNode Params = _schema.SelectSingleNode("//Simulation//" + this.Name);
            foreach (XmlNode param in Params.ChildNodes)
            {
                if (param.Attributes["type"].Value != "container")
                {
                   templist.Add(ParameterDetails.LoadFromXml(param));
                }
            }
            return templist;           
        }

        public List<string> AddableValueTypes()
        {
            List<string> templist = new List<string>();
            XmlNode Params = _schema.SelectSingleNode("//Simulation//" + this.Name);
            foreach (string val in Params.Attributes["ValueTypes"].Value.Split(','))
            {
                templist.Add(val);
            }
            return templist;
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
            return (node.ChildNodes.Count == 1 && node.FirstChild.Name == "#text");
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