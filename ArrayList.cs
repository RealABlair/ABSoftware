using System;

namespace ABSoftware
{
    public class ArrayList<T>
    {
        T[] elements = null;

        public int Capacity
        {
            get { return elements.Length; }
            set
            {
                T[] newArray = new T[value];
                Array.Copy(elements, 0, newArray, 0, Size);
                elements = newArray;
            }
        }
        public int Size { get; private set; }

        public ArrayList()
        {
            this.elements = new T[0];
            this.Size = 0;
        }

        public ArrayList(T[] elements)
        {
            this.elements = elements;
            this.Size = elements.Length;
        }

        public T[] GetElements()
        {
            T[] newArray = new T[Size];
            Array.Copy(this.elements, 0, newArray, 0, Size);
            return newArray;
        }

        public T Get(int id)
        {
            if (id < Size)
                return elements[id];
            else
                throw new ArgumentOutOfRangeException("Element id is out of bounds!");
        }

        public T this[int id]
        {
            get { if (id < Size) return elements[id]; else throw new ArgumentOutOfRangeException("Element id is out of bounds!"); }
            set { elements[id] = value; }
        }

        void ControlCapacity(int minCapacity)
        {
            if (this.elements.Length < minCapacity)
            {
                int newCapacity = (elements.Length == 0) ? 4 : (elements.Length * 2);

                if (newCapacity > int.MaxValue - 8)
                    newCapacity = int.MaxValue - 8;
                if (newCapacity < minCapacity)
                    newCapacity = minCapacity;

                this.Capacity = newCapacity;
            }
        }

        public void Add(T element)
        {
            if (Size == elements.Length)
                ControlCapacity(Size + 1);
            elements[Size] = element;
            Size++;
        }

        public void Add(T[] elements)
        {
            Insert(this.Size, elements);
        }

        public void Insert(int index, T element)
        {
            if (index > this.Size)
                return;

            ControlCapacity(this.Size + 1);

            if(index < this.Size)
            {
                Array.Copy(this.elements, index, this.elements, index + 1, this.Size - index);
            }

            this.elements[index] = element;
            this.Size++;
        }

        public void Insert(int index, T[] elements)
        {
            if (index > this.Size)
                return;

            int length = elements.Length;
            if (length > 0)
            {
                ControlCapacity(this.Size + length);

                if(index < this.Size)
                {
                    Array.Copy(this.elements, index, this.elements, index + length, this.Size - index);
                }

                Array.Copy(elements, 0, this.elements, index, length);

                this.Size += length;
            }
        }

        public void Remove(T element)
        {
            for (int i = 0; i < Size; i++)
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
            Size--;
        }

        public void RemoveIf(Func<T, bool> predicate)
        {
            T[] stamp = GetElements();
            for (int i = 0; i < stamp.Length; i++)
            {
                if (predicate.Invoke(stamp[i]))
                    Remove(stamp[i]);
            }
        }

        public void Clear()
        {
            elements = new T[0];
            Size = 0;
            ControlCapacity(0);
        }

        public bool Contains(T element)
        {
            for (int i = 0; i < Size; i++)
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
            T[] array = new T[Size];
            Array.Copy(elements, 0, array, 0, Size);
            ArrayList<T> newList = new ArrayList<T>(array);
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

                    if(sortingType > 0)
                    {
                        T buffer = elements[j];
                        elements[j] = elements[i];
                        elements[i] = buffer;
                    }
                }
            }
        }
    }
}
