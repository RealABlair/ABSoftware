using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GTA5Casino
{
    public class Memory
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
        public static IntPtr OpenProcess(Process proc, ProcessAccessFlags flags)
        {
            return OpenProcess((uint)flags, false, proc.Id);
        }

        #region READ_PROC_MEM
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize,  out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        #endregion

        #region WRITE_PROC_MEM
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);
        #endregion

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        public enum BufferType : int
        {
            Byte = 1,
            Byte2 = 2,
            Byte4 = 4,
            Byte8 = 8,
            Float = 4,
            Double = 8
        }

        public string process_name { get; private set; }
        IntPtr handle = IntPtr.Zero;

        public Memory(string ProcessName)
        {
            process_name = ProcessName;
            Process p = Process.GetProcessesByName(ProcessName)[0];
            if (p != null)
            {
                handle = OpenProcess(p, ProcessAccessFlags.All);
            }
        }

        public int GetInt(byte[] data)
        {
            string ret = GetHex32(data);
            return Convert.ToInt32(ret, 16);
        }

        public long GetLong(byte[] data)
        {
            string ret = GetHex64(data);
            return Convert.ToInt64(ret, 16);
        }

        public int GetAddress32(byte[] data)
        {
            string ret = "";
            for (int i = data.Length - 1; i > -1; i--)
            {
                ret += data[i].ToString("X2");
            }
            return Convert.ToInt32(ret, 16);
        }

        public long GetAddress64(byte[] data)
        {
            string ret = "";
            for(int i = data.Length - 1; i > -1; i--)
            {
                ret += data[i].ToString("X2");
            }
            return Convert.ToInt64(ret, 16);
        }

        private string GetHex32(byte[] data)
        {
            string ret = "";
            for (int i = data.Length - 1; i > -1; i--)
            {
                ret += data[i].ToString("X2");
            }
            return ret;
        }

        private string GetHex64(byte[] data)
        {
            string ret = "";
            for (int i = data.Length - 1; i > -1; i--)
            {
                ret += data[i].ToString("X2");
            }
            return ret;
        }

        public int OffsetAddress(int address, int[] offsets)
        {
            byte[] buffer = new byte[(int)BufferType.Byte8];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr);
            for (int i = 0; i < offsets.Length; i++)
            {
                long addr = GetAddress32(buffer);
                addr += offsets[i];
                IpAddress = (IntPtr)addr;
                ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr);
            }
            return IpAddress.ToInt32();
        }

        public long OffsetAddress(long address, int[] offsets)
        {
            byte[] buffer = new byte[(int)BufferType.Byte8];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr);
            for(int i = 0; i < offsets.Length; i++)
            {
                long addr = GetAddress64(buffer);
                addr += offsets[i];
                IpAddress = (IntPtr)addr;
                ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr);
            }
            return IpAddress.ToInt64();
        }

        public int ReadByte(long address)
        {
            byte[] buffer = new byte[(int)BufferType.Byte];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr);
            return GetInt(buffer);
        }

        public int ReadByte2(long address)
        {
            byte[] buffer = new byte[(int)BufferType.Byte2];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr);
            return GetInt(buffer);
        }

        public int ReadByte4(long address)
        {
            byte[] buffer = new byte[(int)BufferType.Byte4];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr);
            return GetInt(buffer);
        }

        public long ReadByte8(long address)
        {
            byte[] buffer = new byte[(int)BufferType.Byte8];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr);
            return GetLong(buffer);
        }

        public float ReadFloat(long address)
        {
            byte[] buffer = new byte[(int)BufferType.Float];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr);
            return BitConverter.ToSingle(buffer, 0);
        }

        public double ReadDouble(long address)
        {
            byte[] buffer = new byte[(int)BufferType.Double];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr);
            return BitConverter.ToDouble(buffer, 0);
        }

        //WRITE MEMORY
        public void Write(long address, byte[] bytes)
        {
            IntPtr IpAddress = (IntPtr)address;
            IntPtr outTrash = IntPtr.Zero;
            WriteProcessMemory(handle, IpAddress, bytes, bytes.Length, out outTrash);
        }

        public class Convertation
        {
            public static byte[] GetBytes(int value)
            {
                return BitConverter.GetBytes(value);
            }

            public static byte[] GetBytes(double value)
            {
                return BitConverter.GetBytes(value);
            }

            public static byte[] GetBytes(float value)
            {
                return BitConverter.GetBytes(value);
            }
        }
    }
}