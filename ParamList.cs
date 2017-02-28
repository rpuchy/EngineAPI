using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EngineAPI
{
    public class ParamList : List<Parameter>
    {
        public object this[string name]
        {
            get
            {
                int index = FindIndex(x => x.Name == name);
                if (index == -1)
                {
                    string alternate = EngineObject.GetAlternate(name);                        
                    index = FindIndex(x => x.Name == alternate);
                                    
                }
                if (index >= 0)
                {
                    return this[index].Value;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    int index = FindIndex(x => x.Name == name);
                    if (index == -1)
                    {   
                        string alternate = EngineObject.GetAlternate(name);
                        index = FindIndex(x => x.Name == alternate);
                    }
                    this[index].Value = value;
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }                
            }
        }
    }
}
