using System;
using System.Collections.Generic;

namespace ABSoftware
{
    public class KLINArray
    {
        public List<object> Values = new List<object>();

        public int Count { get { return Values.Count; } }

        public KLINArray(string klin = null)
        {
            if (!Parse(klin))
                return;
        }

        public KLINArray(object[] objects)
        {
            this.Values = new List<object>(objects);
        }

        public bool Parse(string klin)
        {
            if (klin == null || !(klin.Contains("[") || klin.Contains("]")))
                return false;

            Dispose();

            string splitKlin = klin.Split(new char[] { '[', ']' })[1];
            string[] objects = splitKlin.Split(',');

            for(int i = 0; i < objects.Length; i++)
            {
                string currObject = objects[i];
                if(currObject.Contains("\""))
                {
                    currObject = currObject.Replace(" ", "");
                    Values.Add(currObject.Split(new char[] { '\"', '\"' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                    continue;
                }

                if (currObject.Contains("."))
                {
                    Values.Add(double.Parse(currObject.Replace(".", ",")));
                    continue;
                }
                else
                {
                    Values.Add(int.Parse(currObject));
                    continue;
                }
            }

            return true;
        }

        public void Add(object Value)
        {
            this.Values.Add(Value);
        }

        public bool TryGet<T>(int index, out T value)
        {
            T t;
            try
            {
               t = (T)Values[index];
            }
            catch(Exception)
            {
                value = default(T);
                return false;
            }
            value = t;
            return true;
        }

        public T Get<T>(int index)
        {
            return (T)Values[index];
        }

        public void Dispose()
        {
            Values.Clear();
        }

        public override string ToString()
        {
            string klinArray = "[";
            for(int i = 0; i < Values.Count; i++)
            {
                if(Values[i].GetType().Equals(typeof(String)))
                    klinArray += "\"" + Values[i] + "\"";   
                else
                    klinArray += Values[i].ToString().Replace(",", ".");

                if (i < Values.Count - 1)
                    klinArray += ", ";
            }
            klinArray += "]";
            return klinArray;
        }

        #region Utils
        #endregion
    }
}