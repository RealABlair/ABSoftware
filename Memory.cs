using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace ABSoftware
{
    public class Memory
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(uint processAccess, bool bInheritHandle, int processId);
        public static IntPtr OpenProcess(Process proc, ProcessAccessFlags flags)
        {
            return OpenProcess((uint)flags, false, proc.Id);
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public IntPtr BaseAddress;
            public IntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [Flags]
        public enum FreeType : uint
        {
            Decommit = 16384U,
            Release = 32768U
        }

        [Flags]
        public enum AllocationType : uint
        {
            Commit = 4096U,
            Reserve = 8192U,
            Decommit = 16384U,
            Release = 32768U,
            Reset = 524288U,
            Physical = 4194304U,
            TopDown = 1048576U,
            WriteWatch = 2097152U,
            LargePages = 536870912U
        }

        [Flags]
        public enum MemoryProtection : uint
        {
            Execute = 16U,
            ExecuteRead = 32U,
            ExecuteReadWrite = 64U,
            ExecuteWriteCopy = 128U,
            NoAccess = 1U,
            ReadOnly = 2U,
            ReadWrite = 4U,
            WriteCopy = 8U,
            GuardModifierflag = 256U,
            NoCacheModifierflag = 512U,
            WriteCombineModifierflag = 1024U
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

        // VirtualQueryEx
        [DllImport("kernel32.dll")]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        [DllImport("kernel32.dll")]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, ulong dwLength);

        // VirtualFreeEx
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, FreeType dwFreeType);

        // VirtualAllocEx
        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        #region READ_PROC_MEM
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out, MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, IntPtr lpBuffer, int dwSize, out IntPtr lpNumberOfBytesRead);
        #endregion

        #region WRITE_PROC_MEM
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, Int32 nSize, out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [MarshalAs(UnmanagedType.AsAny)] object lpBuffer, int dwSize, out IntPtr lpNumberOfBytesWritten);
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
        public int process_id { get; private set; }
        IntPtr handle = IntPtr.Zero;
        public List<ProcessModule> modules = new List<ProcessModule>();


        public bool Attach(string ProcessName)
        {
            process_name = ProcessName;
            Process[] processes = Process.GetProcessesByName(ProcessName);
            if (processes.Length < 1)
                return false;
            Process p = processes[0];
            if (p != null)
            {
                process_id = p.Id;
                handle = OpenProcess(p, ProcessAccessFlags.All);
            }
            else
            {
                return false;
            }
            modules.Add(p.MainModule);
            foreach (ProcessModule m in p.Modules)
            {
                modules.Add(m);
            }
            return true;
        }

        public bool Attach(int ProcessId)
        {
            Process p = Process.GetProcessById(ProcessId);
            if (p != null)
            {
                process_name = p.ProcessName;
                process_id = p.Id;
                handle = OpenProcess(p, ProcessAccessFlags.All);
            }
            else
            {
                return false;
            }
            modules.Add(p.MainModule);
            foreach (ProcessModule m in p.Modules)
            {
                modules.Add(m);
            }
            return true;
        }

        public IntPtr getHandle()
        {
            return handle;
        }

        public int ReadSignature(byte[] signature, string mask, int minAddress = 0x11ffffff, int maxAddress = 0x7f000000)
        {
            MEMORY_BASIC_INFORMATION memory_basic_information;
            memory_basic_information.BaseAddress = IntPtr.Zero;
            memory_basic_information.RegionSize = IntPtr.Zero;
            int num = 0;
            int address = minAddress;
            while (address < maxAddress)
            {
                VirtualQueryEx(handle, (IntPtr)address, out memory_basic_information, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if (address == (((long)memory_basic_information.BaseAddress) + ((long)memory_basic_information.RegionSize)))
                {
                    break;
                }
                address = (((int)memory_basic_information.BaseAddress) + ((int)memory_basic_information.RegionSize));
                byte[] lpBuffer = new byte[(int)memory_basic_information.RegionSize];
                int lpNumberOfBytesRead = 0;
                ReadProcessMemory(handle, memory_basic_information.BaseAddress, lpBuffer, lpBuffer.Length, out lpNumberOfBytesRead);
                for (int i = 0; i < (lpBuffer.Length - signature.Length); i++)
                {
                    if ((lpBuffer[i] == signature[0]) && (lpBuffer[i + 1] == signature[1]))
                    {
                        for (int j = 0; j < signature.Length; j++)
                        {
                            if ((lpBuffer[i + j] == signature[j]) || (mask[j] == '?'))
                            {
                                num++;
                                if (num == signature.Length)
                                {
                                    int addr = ((int)memory_basic_information.BaseAddress) + i;
                                    return addr;
                                }
                            }
                            else
                            {
                                num = 0;
                            }
                        }
                    }
                }
                lpBuffer = null;
            }
            return 0;
        }

        public List<int> ReadSignatures(byte[] signature, string mask, int minAddress = 0x11ffffff, int maxAddress = 0x7f000000)
        {
            List<int> l = new List<int>();
            MEMORY_BASIC_INFORMATION memory_basic_information;
            memory_basic_information.BaseAddress = IntPtr.Zero;
            memory_basic_information.RegionSize = IntPtr.Zero;
            int num = 0;
            int address = minAddress;
            while (address < maxAddress)
            {
                VirtualQueryEx(handle, (IntPtr)address, out memory_basic_information, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if (address == (((long)memory_basic_information.BaseAddress) + ((long)memory_basic_information.RegionSize)))
                {
                    break;
                }
                address = (((int)memory_basic_information.BaseAddress) + ((int)memory_basic_information.RegionSize));
                byte[] lpBuffer = new byte[(int)memory_basic_information.RegionSize];
                int lpNumberOfBytesRead = 0;
                ReadProcessMemory(handle, memory_basic_information.BaseAddress, lpBuffer, lpBuffer.Length, out lpNumberOfBytesRead);
                for (int i = 0; i < (lpBuffer.Length - signature.Length); i++)
                {
                    if ((lpBuffer[i] == signature[0]) && (lpBuffer[i + 1] == signature[1]))
                    {
                        for (int j = 0; j < signature.Length; j++)
                        {
                            if ((lpBuffer[i + j] == signature[j]) || (mask[j] == '?'))
                            {
                                num++;
                                if (num == signature.Length)
                                {
                                    int addr = ((int)memory_basic_information.BaseAddress) + i;
                                    l.Add(addr);
                                }
                            }
                            else
                            {
                                num = 0;
                            }
                        }
                    }
                }
                lpBuffer = null;
            }
            return l;
        }

        public List<long> ReadSignatures(byte[] signature, string mask, long minAddress = 0x11fffffff, long maxAddress = 0x7f0000000)
        {
            List<long> l = new List<long>();
            MEMORY_BASIC_INFORMATION memory_basic_information;
            memory_basic_information.BaseAddress = IntPtr.Zero;
            memory_basic_information.RegionSize = IntPtr.Zero;
            long num = 0;
            long address = minAddress;
            while (address < maxAddress)
            {
                VirtualQueryEx(handle, (IntPtr)address, out memory_basic_information, (ulong)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if (address == (((long)memory_basic_information.BaseAddress) + ((long)memory_basic_information.RegionSize)))
                {
                    break;
                }
                address = (((long)memory_basic_information.BaseAddress) + ((long)memory_basic_information.RegionSize));
                byte[] lpBuffer;
                try
                {
                    lpBuffer = new byte[(long)memory_basic_information.RegionSize];
                }
                catch(Exception ex)
                {
                    continue;
                }
                IntPtr lpNumberOfBytesRead = IntPtr.Zero;
                ReadProcessMemory(handle, memory_basic_information.BaseAddress, lpBuffer, lpBuffer.Length, out lpNumberOfBytesRead);
                for (long i = 0; i < (lpBuffer.Length - signature.Length); i++)
                {
                    if ((lpBuffer[i] == signature[0]) && (lpBuffer[i + 1] == signature[1]))
                    {
                        for (int j = 0; j < signature.Length; j++)
                        {
                            if ((lpBuffer[i + j] == signature[j]) || (mask[j] == '?'))
                            {
                                num++;
                                if (num == signature.Length)
                                {
                                    long addr = ((long)memory_basic_information.BaseAddress) + i;
                                    l.Add(addr);
                                }
                            }
                            else
                            {
                                num = 0;
                            }
                        }
                    }
                }
                lpBuffer = null;
            }
            return l;
        }

        public List<int> ReadInts(int scanObject, int minAddress = 0x00000000, int maxAddress = 0x7f000000)
        {
            List<int> l = new List<int>();
            int address = minAddress;
            byte[] data = Convertation.GetBytes(scanObject);
            MEMORY_BASIC_INFORMATION _BASIC_INFORMATION;
            _BASIC_INFORMATION.RegionSize = IntPtr.Zero;
            _BASIC_INFORMATION.RegionSize = IntPtr.Zero;
            while (address <= maxAddress)
            {
                VirtualQueryEx(handle, (IntPtr)address, out _BASIC_INFORMATION, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if (address != (uint)_BASIC_INFORMATION.BaseAddress + (uint)_BASIC_INFORMATION.RegionSize)
                {
                    byte[] buffer = new byte[(uint)_BASIC_INFORMATION.RegionSize];
                    int Dammy = 0;
                    ReadProcessMemory(handle, (IntPtr)_BASIC_INFORMATION.BaseAddress, buffer, (int)_BASIC_INFORMATION.RegionSize, out Dammy);
                    for (int i = 0; i < buffer.Length - data.Length; i++)
                    {
                        if (buffer[i] == data[0])
                        {
                            if (buffer[i + 1] == data[1])
                            {
                                if (buffer[i + 2] == data[2])
                                {
                                    if (buffer[i + 3] == data[3])
                                    {
                                        int MyAddress = (int)_BASIC_INFORMATION.BaseAddress + i;
                                        l.Add(MyAddress);
                                    }
                                }
                            }
                        }
                    }
                }
                address = (int)_BASIC_INFORMATION.BaseAddress + (int)_BASIC_INFORMATION.RegionSize;
            }
            return l;
        }

        public List<int> ReadLongs(long scanObject, int minAddress = 0x00000000, int maxAddress = 0x7f000000)
        {
            List<int> l = new List<int>();
            int address = minAddress;
            byte[] data = Convertation.GetBytes(scanObject);
            MEMORY_BASIC_INFORMATION _BASIC_INFORMATION;
            _BASIC_INFORMATION.RegionSize = IntPtr.Zero;
            _BASIC_INFORMATION.RegionSize = IntPtr.Zero;
            while (address <= maxAddress)
            {
                VirtualQueryEx(handle, (IntPtr)address, out _BASIC_INFORMATION, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if (address != (uint)_BASIC_INFORMATION.BaseAddress + (uint)_BASIC_INFORMATION.RegionSize)
                {
                    byte[] buffer = new byte[(uint)_BASIC_INFORMATION.RegionSize];
                    int Dammy = 0;
                    ReadProcessMemory(handle, (IntPtr)_BASIC_INFORMATION.BaseAddress, buffer, (int)_BASIC_INFORMATION.RegionSize, out Dammy);
                    for (int i = 0; i < buffer.Length - data.Length; i++)
                    {
                        if (buffer[i] == data[0])
                        {
                            if (buffer[i + 1] == data[1])
                            {
                                if (buffer[i + 2] == data[2])
                                {
                                    if (buffer[i + 3] == data[3])
                                    {
                                        if (buffer[i + 4] == data[4])
                                        {
                                            if (buffer[i + 5] == data[5])
                                            {
                                                if (buffer[i + 6] == data[6])
                                                {
                                                    if (buffer[i + 7] == data[7])
                                                    {
                                                        int MyAddress = (int)_BASIC_INFORMATION.BaseAddress + i;
                                                        l.Add(MyAddress);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                address = (int)_BASIC_INFORMATION.BaseAddress + (int)_BASIC_INFORMATION.RegionSize;
            }
            return l;
        }

        public List<int> ReadDoubles(double scanObject, int minAddress = 0x00000000, int maxAddress = 0x7f000000)
        {
            List<int> l = new List<int>();
            int address = minAddress;
            byte[] data = Convertation.GetBytes(scanObject);
            MEMORY_BASIC_INFORMATION _BASIC_INFORMATION;
            _BASIC_INFORMATION.RegionSize = IntPtr.Zero;
            _BASIC_INFORMATION.RegionSize = IntPtr.Zero;
            while (address <= maxAddress)
            {
                VirtualQueryEx(handle, (IntPtr)address, out _BASIC_INFORMATION, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if (address != (uint)_BASIC_INFORMATION.BaseAddress + (uint)_BASIC_INFORMATION.RegionSize)
                {
                    byte[] buffer = new byte[(uint)_BASIC_INFORMATION.RegionSize];
                    int Dammy = 0;
                    ReadProcessMemory(handle, (IntPtr)_BASIC_INFORMATION.BaseAddress, buffer, (int)_BASIC_INFORMATION.RegionSize, out Dammy);
                    for (int i = 0; i < buffer.Length - data.Length; i++)
                    {
                        if (buffer[i] == data[0])
                        {
                            if (buffer[i + 1] == data[1])
                            {
                                if (buffer[i + 2] == data[2])
                                {
                                    if (buffer[i + 3] == data[3])
                                    {
                                        if (buffer[i + 4] == data[4])
                                        {
                                            if (buffer[i + 5] == data[5])
                                            {
                                                if (buffer[i + 6] == data[6])
                                                {
                                                    if (buffer[i + 7] == data[7])
                                                    {
                                                        int MyAddress = (int)_BASIC_INFORMATION.BaseAddress + i;
                                                        l.Add(MyAddress);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                address = (int)_BASIC_INFORMATION.BaseAddress + (int)_BASIC_INFORMATION.RegionSize;
            }
            return l;
        }

        public List<int> ReadFloats(float scanObject, int minAddress = 0x00000000, int maxAddress = 0x7f000000)
        {
            List<int> l = new List<int>();
            int address = minAddress;
            byte[] data = Convertation.GetBytes(scanObject);
            MEMORY_BASIC_INFORMATION _BASIC_INFORMATION;
            _BASIC_INFORMATION.RegionSize = IntPtr.Zero;
            _BASIC_INFORMATION.RegionSize = IntPtr.Zero;
            while (address <= maxAddress)
            {
                VirtualQueryEx(handle, (IntPtr)address, out _BASIC_INFORMATION, (uint)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION)));
                if (address != (uint)_BASIC_INFORMATION.BaseAddress + (uint)_BASIC_INFORMATION.RegionSize)
                {
                    byte[] buffer = new byte[(uint)_BASIC_INFORMATION.RegionSize];
                    int Dammy = 0;
                    ReadProcessMemory(handle, (IntPtr)_BASIC_INFORMATION.BaseAddress, buffer, (int)_BASIC_INFORMATION.RegionSize, out Dammy);
                    for (int i = 0; i < buffer.Length - data.Length; i++)
                    {
                        if (buffer[i] == data[0])
                        {
                            if (buffer[i + 1] == data[1])
                            {
                                if (buffer[i + 2] == data[2])
                                {
                                    if (buffer[i + 3] == data[3])
                                    {
                                        int MyAddress = (int)_BASIC_INFORMATION.BaseAddress + i;
                                        l.Add(MyAddress);
                                    }
                                }
                            }
                        }
                    }
                }
                address = (int)_BASIC_INFORMATION.BaseAddress + (int)_BASIC_INFORMATION.RegionSize;
            }
            return l;
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
            for (int i = data.Length - 1; i > -1; i--)
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
            byte[] buffer = new byte[(int)BufferType.Byte4];
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
            for (int i = 0; i < offsets.Length; i++)
            {
                long addr = GetAddress64(buffer);
                addr += offsets[i];
                IpAddress = (IntPtr)addr;
                ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr);
            }
            return IpAddress.ToInt64();
        }

        public bool ReadByteArray(long address, int length, out byte[] value)
        {
            byte[] buffer = new byte[length];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            if (ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr))
            {
                value = buffer;
                return true;
            }
            value = null;
            return true;
        }

        public bool ReadByte(long address, out byte value)
        {
            byte[] buffer = new byte[(int)BufferType.Byte];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            if (ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr))
            {
                value = (byte)GetInt(buffer);
                return true;
            }
            value = 0;
            return false;
        }

        public bool ReadByte2(long address, out short value)
        {
            byte[] buffer = new byte[(int)BufferType.Byte2];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            if (ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr))
            {
                value = (short)GetInt(buffer);
                return true;
            }
            value = 0;
            return false;
        }

        public bool ReadByte4(long address, out int value)
        {
            byte[] buffer = new byte[(int)BufferType.Byte4];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            if (ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr))
            {
                value = GetInt(buffer);
                return true;
            }
            value = 0;
            return false;
        }

        public bool ReadByte8(long address, out long value)
        {
            byte[] buffer = new byte[(int)BufferType.Byte8];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            if (ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr))
            {
                value = GetLong(buffer);
                return true;
            }
            value = 0;
            return false;
        }

        public bool ReadFloat(long address, out float value)
        {
            byte[] buffer = new byte[(int)BufferType.Float];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            if (ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr))
            {
                value = BitConverter.ToSingle(buffer, 0);
                return true;
            }
            value = 0f;
            return false;
        }

        public bool ReadDouble(long address, out double value)
        {
            byte[] buffer = new byte[(int)BufferType.Double];
            IntPtr IpAddress = (IntPtr)address;
            int outIntPtr = 0; //I DON`T NEED IT
            if (ReadProcessMemory(handle, IpAddress, buffer, buffer.Length, out outIntPtr))
            {
                value = BitConverter.ToDouble(buffer, 0);
                return true;
            }
            value = 0;
            return false;
        }

        //WRITE MEMORY
        public bool Write(long address, byte[] bytes)
        {
            IntPtr IpAddress = (IntPtr)address;
            IntPtr outTrash = IntPtr.Zero;
            return WriteProcessMemory(handle, IpAddress, bytes, bytes.Length, out outTrash);
        }

        public bool InjectDll(string dllPath)
        {
            uint dwSize = (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char)));
            IntPtr hndl = OpenProcess((uint)(ProcessAccessFlags.CreateThread | ProcessAccessFlags.VirtualMemoryOperation | ProcessAccessFlags.VirtualMemoryRead | ProcessAccessFlags.VirtualMemoryWrite | ProcessAccessFlags.QueryInformation), false, process_id);
            IntPtr procAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");
            if (procAddress == null)
            {
                return false;
            }
            IntPtr intptr = VirtualAllocEx(hndl, IntPtr.Zero, dwSize, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);
            if (intptr == null)
            {
                return false;
            }
            Thread.Sleep(500);
            byte[] dllPathBytes = Encoding.UTF8.GetBytes(dllPath);
            IntPtr intptr1;
            if (!WriteProcessMemory(hndl, intptr, dllPathBytes, (int)dwSize, out intptr1) || intptr1.ToInt32() != dllPathBytes.Length + 1)
            {
                return false;
            }
            if (CreateRemoteThread(hndl, IntPtr.Zero, 0U, procAddress, intptr, 0U, IntPtr.Zero) == IntPtr.Zero)
            {
                return false;
            }
            if (!CloseHandle(hndl))
            {
                return false;
            }

            return true;
        }

        public class Convertation
        {
            public static byte[] GetBytes(int value)
            {
                return BitConverter.GetBytes(value);
            }

            public static byte[] GetBytes(long value)
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
