using System;
using System.Collections;
using System.Collections.Generic;

namespace ABSoftware
{
    public class ArrayList<T> : IEnumerable<T>
    {
        T[] elements = null;

        public int Capacity
        {
            get { return elements.Length; }
            private set
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
            this.elements = (T[])elements.Clone();
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

            if (index < this.Size)
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

                if (index < this.Size)
                {
                    Array.Copy(this.elements, index, this.elements, index + length, this.Size - index);
                }

                Array.Copy(elements, 0, this.elements, index, length);

                this.Size += length;
            }
        }

        public bool Remove(T element)
        {
            int index = FindIndex(e => { return EqualityComparer<T>.Default.Equals(e, element); });
            if(index != -1)
            {
                RemoveAt(index);
                return true;
            }

            return false;
        }

        public void RemoveAt(int id)
        {
            if (id >= Size || id < 0) throw new ArgumentOutOfRangeException();

            int shiftCount = Size - id - 1;
            if (shiftCount > 0)
            {
                Array.Copy(this.elements, id + 1, this.elements, id, shiftCount);
            }

            Size--;
            elements[Size] = default(T);
        }

        public int RemoveIf(Func<T, bool> predicate)
        {
            int removedCount = 0;
            int writeIndex = 0;

            for(int i = 0; i < Size; i++)
            {
                if(predicate(elements[i]))
                {
                    removedCount++;
                }
                else
                {
                    elements[writeIndex] = elements[i];
                    writeIndex++;
                }
            }

            for (int i = writeIndex; i < Size; i++)
                elements[i] = default;

            Size = writeIndex;

            return removedCount;
        }

        public void Clear()
        {
            Array.Clear(elements, 0, Size);
            Size = 0;
        }

        public bool Contains(T element)
        {
            for (int i = 0; i < Size; i++)
            {
                if (EqualityComparer<T>.Default.Equals(elements[i], element))
                    return true;
            }
            return false;
        }

        public bool Contains(Func<T, bool> predicate)
        {
            for (int i = 0; i < Size; i++)
            {
                if (predicate.Invoke(elements[i]))
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
                if (elements[i] == null)
                    continue;
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

            void QuickSort(int start, int end)
            {
                if (start >= end) return;

                T pivot = elements[start + ((end - start) / 2)];

                int i = start;
                int j = end;

                while (i <= j)
                {
                    while (comparison(elements[i], pivot) < 0) i++;
                    while (comparison(elements[j], pivot) > 0) j--;

                    if(i <= j)
                    {
                        T temp = elements[i];
                        elements[i] = elements[j];
                        elements[j] = temp;
                        i++;
                        j--;
                    }
                }

                if (start < j) QuickSort(start, j);
                if (i < end) QuickSort(i, end);
            }

            QuickSort(0, Size - 1);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for(int i = 0; i < Size; i++)
            {
                yield return elements[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
