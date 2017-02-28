using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Shapes;
using System.ComponentModel;
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

        public EngineObject(XmlNode _xmlNode)
        {
            _innerXml = _xmlNode;    
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
                    if (isParameter(_node))
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
            throw new NotImplementedException();
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
                    if (!isParameter(_node) && !isComment(_node) && !isTable(_node))
                    {
                        _children.Add(new EngineObject(_node));
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
                    if (isTable(_node))
                    {
                        _tables.Add(new Table(_node));
                    }
                }
                return _tables;
            }
        }


        public string Name
        {
            get { return _innerXml.Name; }
        }

        /// <summary>
        /// The name of the object if it exists
        /// </summary>
        public string ObjectName
        {
            get { return _innerXml.SelectSingleNode("./Name|./Params/Name")?.InnerText; }
        }

        /// <summary>
        /// Search through the Schema and return a list of addable submodels
        /// </summary>
        /// <returns>A list of addable submodels</returns>
        public List<String> AddableObjects()
        {
            return null;
        }
        
        private static bool isParameter(XmlNode _node)
        {
            return (_node.ChildNodes.Count == 1 && _node.FirstChild.Name == "#text");
        }

        private static bool isComment(XmlNode _node)
        {
            return (_node.Name == "#comment");
        }

        private static bool isTable(XmlNode _node)
        {
            if (_node.ChildNodes.Count<=1)
            {
                return false;
            }
            string lastname = _node.FirstChild.Name;
            foreach (XmlNode child in _node.ChildNodes)
            {
                if (child.Name!=lastname)
                {
                    return false;
                }
            }
            return true;
        }


        public EngineObject Parent
        {
            get
            {
                return new EngineObject(_innerXml.ParentNode);
            }
        }
        /// <summary>
        /// Searches the XML and finds the first object that matches the node name
        /// </summary>
        /// <param name="nodename"></param>
        /// <returns></returns>
        public EngineObject FindObjectbyNodeName(string nodename)
        {
            string alternate = char.ToUpper(nodename[0]) + nodename.Substring(1);
            XmlNode temp = _innerXml.SelectSingleNode(".//" + nodename + "|.//"+alternate);
            return (temp != null) ? new EngineObject(temp) : null;
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
                _tempout.Add(new EngineObject(node));
            }
            return (_tempout != null) ? _tempout : null;
        }

        /// <summary>
        /// Searches the XML and finds the first object whose "Name" child node is name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public EngineObject FindObjectbyName(string name)
        {
            XmlNode temp = _innerXml.SelectSingleNode(".//*[Name='" + name + "']");
            return (temp != null) ? new EngineObject(temp) : null;
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
                _tempout.Add(new EngineObject(node));
            }
            return (_tempout != null) ? _tempout : null;
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