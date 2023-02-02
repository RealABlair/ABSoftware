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
            T[] newArray = new T[elements.Length];
            Array.Copy(this.elements, 0, newArray, 0, this.elements.Length);
            return newArray;
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

        public void RemoveIf(Func<T, bool> predicate)
        {
            for (int i = 0; i < Size; i++)
            {
                if (predicate.Invoke(elements[i]))
                    Remove(elements[i]);
            }
        }

        public void Clear()
        {
            elements = new T[0];
        }

        public bool Contains(T element)
        {
            for (int i = 0; i < elements.Length; i++)
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

        public ArrayList<T> Copy()
        {
            ArrayList<T> newList = new ArrayList<T>();
            Array.Copy(elements, 0, newList.elements, 0, Size);
            return newList;
        }

        public void Sort(Func<T, T, int> comparison)
        {
            if (Size <= 1)
                return;
            for (int i = 0; i < Size; i++)
            {
                for (int j = i + 1; j < Size; j++)
                {
                    int sortingType = comparison.Invoke(elements[i], elements[j]);

                    if (sortingType < -1)
                        sortingType = -1;
                    if (sortingType < -1)
                        sortingType = -1;

                    switch (sortingType)
                    {
                        case -1:
                            {
                                T buffer = elements[j];
                                elements[j] = elements[i];
                                elements[i] = buffer;
                            }
                            break;
                        case 0:
                            break;
                        case 1:
                            {
                                elements[i] = elements[i];
                                elements[j] = elements[j];
                            }
                            break;
                    }
                }
            }
        }
    }
}
