using System;

namespace ABSoftware
{
    public class ByteBuilder
    {
        byte[] data = null;

        public int Capacity
        {
            get { return data.Length; }
            set
            {
                byte[] newArray = new byte[value];
                Array.Copy(data, 0, newArray, 0, Size);
                data = newArray;
            }
        }

        void ControlCapacity(int minCapacity)
        {
            if (this.data.Length < minCapacity)
            {
                int newCapacity = (data.Length == 0) ? 4 : (data.Length * 2);

                if (newCapacity > int.MaxValue - 8)
                    newCapacity = int.MaxValue - 8;
                if (newCapacity < minCapacity)
                    newCapacity = minCapacity;

                this.Capacity = newCapacity;
            }
        }

        public int Size { get; private set; }

        public int LastIndex { get { return Size - 1; } }

        #region Constructors
        public ByteBuilder()
        {
            data = new byte[0];
            Size = 0;
        }

        public ByteBuilder(int length)
        {
            data = new byte[length];
            ControlCapacity(length);
            Size = length;
        }

        public ByteBuilder(byte[] data)
        {
            this.data = data;
            ControlCapacity(data.Length);
            Size = data.Length;
        }
        #endregion

        public void Append(byte data)
        {
            if (Size == this.data.Length)
                ControlCapacity(Size + 1);
            this.data[Size] = data;
            Size++;
        }

        public void Append(byte[] data)
        {
            ControlCapacity(Size + data.Length);
            Buffer.BlockCopy(data, 0, this.data, Size, data.Length);
            Size += data.Length;
        }

        public void Append(params byte[][] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                Append(data[i]);
            }
        }

        public void Append(int startIndex, byte[] data)
        {
            if (Size + data.Length <= this.data.Length)
                ControlCapacity(Size + data.Length + startIndex);
            Buffer.BlockCopy(data, 0, this.data, startIndex, data.Length);
        }

        public void Fill(int startIndex, int endIndex, byte data)
        {
            for(int i = startIndex; i <= endIndex; i++)
            {
                this.data[i] = data;
            }
        }

        public void Clear()
        {
            if (this.Size > 0)
                Array.Clear(data, 0, Size);
        }

        public byte[] GetRange(int startIndex, int length)
        {
            byte[] array = new byte[length];
            Buffer.BlockCopy(data, startIndex, array, 0, length);
            return array;
        }

        public byte[] ToArray()
        {
            byte[] copy = new byte[Size];
            Array.Copy(this.data, 0, copy, 0, Size);
            return copy;
        }

        public bool Contains(byte[] array)
        {
            int pos = -1;
            int num = 0;
            bool found = false;
            while (pos < Size - array.Length && !found)
            {
                pos++;
                if (this.data[pos] == array[0] && this.data[pos + 1] == array[1])
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        if (this.data[pos + i] == array[i])
                        {
                            num++;
                            if (num.Equals(array.Length))
                            {
                                found = true;
                                break;
                            }
                        }
                        else
                        {
                            num = 0;
                        }
                    }
                }
            }
            return found;
        }

        public int IndexOf(byte[] array)
        {
            if (array.Length > Size)
                return -1;
            for (int i = 0; i < Size - array.Length + 1; i++)
            {
                bool found = true;
                for (int j = 0; j < array.Length; j++)
                {
                    if (this.data[i + j] != array[j])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return i;
                }
            }
            return -1;
        }

        public void RemoveFirstElements(int count)
        {
            byte[] oldArray = data;
            byte[] newArray = new byte[data.Length - count];
            Buffer.BlockCopy(oldArray, count, newArray, 0, data.Length - count);
            this.data = newArray;
        }

        public void RemoveAt(int index)
        {
            Array.Copy(this.data, index + 1, this.data, index, this.Size - index - 1);
            Size--;
        }

        public void Remove(int startIndex, int length)
        {
            Array.Copy(this.data, startIndex + length, this.data, startIndex, this.Size - startIndex - length);
            Size -= length;
        }

        public bool EndsWith(byte[] array)
        {
            int startIndex = Size - array.Length;
            int num = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (this.data[startIndex + i] == array[i])
                {
                    num++;
                    if (num.Equals(array.Length))
                    {
                        return true;
                    }
                }
                else
                {
                    num = 0;
                }
            }
            return false;
        }

        public static string ToString(byte[] array)
        {
            string text = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (i < array.Length - 1)
                    text += $"{array[i].ToString("X2")} ";
                else
                    text += $"{array[i].ToString("X2")}";
            }
            return text;
        }

        public override string ToString()
        {
            string text = "";
            for (int i = 0; i < Size; i++)
            {
                if (i < Size - 1)
                    text += $"{data[i].ToString("X2")} ";
                else
                    text += $"{data[i].ToString("X2")}";
            }
            return text;
        }
    }
}
