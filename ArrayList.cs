using System;

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
            Array.Copy(this.elements, id + 1, this.elements, id, this.Size - id - 1);
            Array.Resize(ref this.elements, this.Size - 1);
        }

        public void Clear()
        {
            elements = new T[0];
        }

        public bool Contains(T element)
        {
            for(int i = 0; i < elements.Length; i++)
            {
                if (elements[i].Equals(element))
                    return true;
            }
            return false;
        }

        public T FirstOrDefault(Func<T, bool> predicate)
        {
            for (int i = 0; i < Size; i++)
            {
                if (predicate.Invoke(elements[i]))
                    return elements[i];
            }
            return default;
        }

        public int FindIndex(Func<T, bool> predicate)
        {
            for (int i = 0; i < Size; i++)
            {
                if (predicate.Invoke(elements[i]))
                    return i;
            }
            return -1;
        }
    }
}
