using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EngineAPI
{
    class Simulation :EngineObject
    {

        private XmlDocument _xmlDoc = new XmlDocument();

        public Simulation(string filename) : base()
        {
            try
            {
                using (FileStream file_reader = new FileStream(filename, FileMode.Open))
                using (XmlReader reader = XmlReader.Create(file_reader))
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

        public Simulation(XmlDocument doc) : base(doc)
        {
            _xmlDoc = doc;
        }
        
    }
}
