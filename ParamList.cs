using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EngineAPI
{
    public class ParamList : ObjectList<Parameter>
    {    
        public Object this[string name]
        {
            get
            {
                int index = InternalList.FindIndex(x => x.Name == name);
                if (index == -1)
                {
                    string alternate = EngineObject.GetAlternate(name);                        
                    index = InternalList.FindIndex(x => x.Name == alternate);                                    
                }
                if (index >= 0)
                {
                    return InternalList[index].Value;
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
                    int index = InternalList.FindIndex(x => x.Name == name);
                    if (index == -1)
                    {   
                        string alternate = EngineObject.GetAlternate(name);
                        index = InternalList.FindIndex(x => x.Name == alternate);
                    }
                    InternalList[index].Value = value.ToString();
                    
                }
                catch (Exception e)
                {
                    throw new Exception("Parameter : "+name+" could not be found");
                }                
            }
        }

    }
}
