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
    public class ParamList : INotifyPropertyChanged
    {

        protected List<Parameter> InternalList = new List<Parameter>();
        
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
                    Console.WriteLine(e);
                    throw;
                }                
            }
        }

        public Parameter this[int index]
        {
            get
            {
                return InternalList[index];
            }
            set
            {
              InternalList[index].Value = value.ToString();
            }
        }

        public void Add(Parameter param)
        {
            InternalList.Add(param);
            NotifyPropertyChanged("NewParam");
        }

        public int Count
        {
            get
            { return InternalList.Count(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string ParamChanged)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(ParamChanged));
            }
        }
    }
}
