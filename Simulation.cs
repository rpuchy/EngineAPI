using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.Win32;

namespace EngineAPI
{
    class Simulation :EngineObject
    {

        private readonly XmlDocument _xmlDoc = new XmlDocument();
        private readonly string _filename;


        public Simulation(string filename, string schemaFilename="") : base()
        {
            _filename = filename;
            if (schemaFilename != "")
            {
                try
                {
                    using (FileStream fileReader = new FileStream(schemaFilename, FileMode.Open))
                    using (XmlReader reader = XmlReader.Create(fileReader))
                    {
                        _schema.Load(reader);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: " + ex.Message + ex.ToString() +
                                                        Environment.NewLine);
                    throw;
                }
            }
            try
            {
                using (FileStream fileReader = new FileStream(filename, FileMode.Open))
                using (XmlReader reader = XmlReader.Create(fileReader))
                {                    
                    _xmlDoc.Load(reader);                    
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR: " + ex.Message + ex.ToString() +
                                                    Environment.NewLine);
                throw;
            }
            _innerXml = _xmlDoc;
        }

        public void AddScenarioFile(string ModelID)
        {
            throw new NotImplementedException();
        }

        public void SetoutputLocation(string Path)
        {
            throw new NotImplementedException();
        }

        public void AddTransactionLog(string filename, List<int> Scenarios)
        {
            throw new NotImplementedException();
        }

        public void RemoveOutputs()
        {
            throw new NotImplementedException();
        }



        public Simulation(XmlDocument doc) : base(doc,new XmlDocument())
        {
            _xmlDoc = doc;
        }

        public void Save()
        {
            if (_filename == "")
            {
                throw new Exception("There is no filename associated with this xml");
            }
            _xmlDoc.Save(_filename);
        }

        public void SaveAs(string filename)
        {
            _xmlDoc.Save(filename);
        }

    }
}
