using System;
using System.Linq;

namespace ABSoftware
{
    public class ArrayList<T>
    {
        T[] elements = null;

        public int Size { get { return elements.Length; } }

        public ArrayList()
        {
            this.elements = new T[0];
        }

        public ArrayList(T[] elements)
        {
            this.elements = elements;
        }

        public T[] GetElements()
        {
            return elements;
        }

        public T Get(int id)
        {
            return elements[id];
        }
        
        public T this[int id]
        {
            get { return elements[id]; }
            set { elements[id] = value; }
        }

        public void Add(T element)
        {
            Array.Resize(ref elements, elements.Length + 1);
            elements[elements.Length - 1] = element;
        }

        public void Remove(T element)
        {
            for (int i = 0; i < elements.Length; i++)
            {
                if (elements[i].Equals(element))
                {
                    RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveAt(int id)
        {
            elements = elements.Where((source, index) => index != id).ToArray();
        }

        public void Clear()
        {
            elements = new T[0];
        }
        
        public bool Contains(T element)
        {
            return elements.Contains(element);
        }
    }
}
