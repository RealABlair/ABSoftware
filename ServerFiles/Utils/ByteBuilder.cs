using System;

namespace ABSoftware.ServerFiles.Utils
{
    public class ByteBuilder
    {
        byte[] data = null;

        public int Size { get { return data.Length; } }
        public int LastIndex { get { return data.Length-1; } }

        #region Constructors
        public ByteBuilder()
        {
            data = new byte[0];
        }

        public ByteBuilder(int length)
        {
            data = new byte[length];
        }

        public ByteBuilder(byte[] data)
        {
            this.data = data;
        }
        #endregion

        public void Append(byte[] data)
        {
            int dataLength = this.data.Length;
            Array.Resize(ref this.data, this.data.Length + data.Length);
            Buffer.BlockCopy(data, 0, this.data, dataLength, data.Length);
        }

        public void Append(int startIndex, byte[] data)
        {
            int dataLength = this.data.Length;
            if (dataLength < data.Length + startIndex)
                dataLength = data.Length + startIndex;
            Array.Resize(ref this.data, dataLength);
            Buffer.BlockCopy(data, 0, this.data, startIndex, data.Length);
        }

        public void Fill(int startIndex, int endIndex, byte data)
        {
            byte[] array = new byte[endIndex-startIndex+1];
            for (int i = 0; i < array.Length; i++) array[i] = data;
            Buffer.BlockCopy(array, 0, this.data, startIndex, array.Length);
        }

        public void Clear(int size = 0)
        {
            this.data = new byte[size];
        }

        public byte[] GetRange(int startIndex, int length)
        {
            byte[] array = new byte[length];
            Buffer.BlockCopy(data, startIndex, array, 0, length);
            return array;
        }

        public byte[] ToArray()
        {
            return this.data;
        }

        public bool Contains(byte[] array)
        {
            int pos = -1;
            int num = 0;
            bool found = false;
            while (pos < this.data.Length - array.Length && !found)
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
            if (array.Length > this.data.Length)
                return -1;
            for (int i = 0; i < this.data.Length - array.Length+1; i++)
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

        public bool EndsWith(byte[] array)
        {
            int startIndex = this.data.Length - array.Length;
            int num = 0;
            for(int i = 0; i < array.Length; i++)
            {
                if (this.data[startIndex+i] == array[i])
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
            for(int i = 0; i < this.data.Length; i++)
            {
                if (i < this.data.Length-1)
                    text += $"{data[i].ToString("X2")} ";
                else
                    text += $"{data[i].ToString("X2")}";
            }
            return text;
        }
    }
}