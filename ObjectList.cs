using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace EngineAPI
{
    public class ObjectList<T>: INotifyPropertyChanged, IEnumerable<T>
    {
        protected List<T> InternalList = new List<T>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string ParamChanged, T Changed)
        {
            if (PropertyChanged != null)
            {
                this.PropertyChanged(Changed, new PropertyChangedEventArgs(ParamChanged));
            }
        }

        public T this[int index]
        {
            get
            {
                return InternalList[index];
            }
            set
            {
                InternalList[index] = value;
            }
        }

        public void Add(T _obj)
        {
            InternalList.Add(_obj);
            NotifyPropertyChanged("New", _obj);
        }

        public void Remove(T _obj)
        {
            if (!InternalList.Remove(_obj))
            {
                throw new Exception("object  not found "+_obj.ToString());
            }
            NotifyPropertyChanged("Remove", _obj);
        }

        public int Count
        {
            get
            { return InternalList.Count(); }
        }

        public ObjectList<T> FindAll(Predicate<T> match)
        {
            ObjectList<T> Temp = new ObjectList<T>();
            foreach(var t in InternalList.FindAll(match))
            {
                Temp.Add(t);
            }
            return Temp;
        }

        public T Find(Predicate<T> match)
        {
            return InternalList.Find(match);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }
    }
}
