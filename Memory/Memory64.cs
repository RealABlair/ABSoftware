using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ABSoftware
{
    public class Memory64
    {
        #region Consts
        public const string KERNEL32 = "kernel32.dll";
        public const string USER32 = "user32.dll";

        public const uint WM_KEYDOWN = 0x0100;
        public const uint WM_KEYUP = 0x0101;
        public const uint WM_CHAR = 0x0102;
        #endregion

        #region Structs
        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION64
        {
            public ulong BaseAddress;
            public ulong AllocationBase;
            public int AllocationProtect;
            public int __alignment1;
            public ulong RegionSize;
            public int State;
            public int Protect;
            public int Type;
            public int __alignment2;
        }
        #endregion

        #region Enums
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
        #endregion

        #region Functions
        #region Kernel
        [DllImport(KERNEL32, SetLastError = true)]
        public static extern IntPtr OpenProcess(uint access, bool inheritHandle, int processId);
        public static IntPtr OpenProcess(Process process, ProcessAccessFlags flags) => OpenProcess((uint)flags, false, process.Id);
        [DllImport(KERNEL32)]
        public static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION64 lpBuffer, ulong dwLength);
        [DllImport(KERNEL32, SetLastError = true)]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);
        [DllImport(KERNEL32, SetLastError = true)]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int nSize, out int lpNumberOfBytesWritten);
        [DllImport(KERNEL32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);
        [DllImport(KERNEL32, ExactSpelling = true, SetLastError = true)]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, FreeType dwFreeType);
        [DllImport(KERNEL32)]
        public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, MemoryProtection flNewProtect, out uint lpflOldProtect);
        [DllImport(KERNEL32, CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport(KERNEL32, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);
        [DllImport(KERNEL32, SetLastError = true)]
        public static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);
        #endregion
        #region User
        [DllImport(USER32, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, uint wParam, int lParam);
        #endregion
        #endregion

        public string ProcessName { get; private set; }
        public int ProcessId { get; private set; }
        public IntPtr Handle { get; private set; }
        public IntPtr HWND { get; private set; }
        public ProcessModule[] modules = new ProcessModule[0];

        public bool Attach(string ProcessName)
        {
            Process[] procs = Process.GetProcessesByName(ProcessName);
            if (procs.Length < 1)
                return false;
            if (procs[0] == null)
                return false;

            this.ProcessName = ProcessName;
            this.ProcessId = procs[0].Id;
            this.Handle = OpenProcess(procs[0], ProcessAccessFlags.All);
            this.HWND = procs[0].MainWindowHandle;
            Array.Resize(ref modules, procs[0].Modules.Count);
            procs[0].Modules.CopyTo(modules, 0);

            return true;
        }

        public bool Attach(int ProcessId)
        {
            Process proc = Process.GetProcessById(ProcessId);
            if (proc == null)
                return false;

            this.ProcessName = ProcessName;
            this.ProcessId = proc.Id;
            this.Handle = OpenProcess(proc, ProcessAccessFlags.All);
            this.HWND = proc.MainWindowHandle;
            Array.Resize(ref modules, proc.Modules.Count);
            proc.Modules.CopyTo(modules, 0);

            return true;
        }

        public long FindSignature(byte[] signature, string mask, long minAddress = 0x11FFFFFFFFFF, long maxAddress = 0x7FFFFFFFFFFF)
        {
            MEMORY_BASIC_INFORMATION64 memory_basic_information;
            memory_basic_information.BaseAddress = (ulong)IntPtr.Zero;
            memory_basic_information.RegionSize = (ulong)IntPtr.Zero;
            long num = 0;
            long address = minAddress;
            while (address < maxAddress)
            {
                VirtualQueryEx(Handle, (IntPtr)address, out memory_basic_information, (ulong)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION64)));
                if (address == (((long)memory_basic_information.BaseAddress) + ((long)memory_basic_information.RegionSize)))
                {
                    break;
                }
                address = ((long)memory_basic_information.BaseAddress) + ((long)memory_basic_information.RegionSize);
                byte[] lpBuffer;
                try
                {
                    lpBuffer = new byte[(long)memory_basic_information.RegionSize];
                }
                catch (Exception)
                {
                    continue;
                }
                ReadProcessMemory(Handle, (IntPtr)memory_basic_information.BaseAddress, lpBuffer, lpBuffer.Length, out int lpNumberOfBytesRead);

                for (int i = 0; i < (lpBuffer.Length - signature.Length); i++)
                {
                    if ((lpBuffer[i] == signature[0] || mask[0] == '?') && (lpBuffer[i + 1] == signature[1] || mask[1] == '?'))
                    {
                        for (int j = 0; j < signature.Length; j++)
                        {
                            if ((mask[j] == '?') || (lpBuffer[i + j] == signature[j]))
                            {
                                num++;
                                if (num == signature.Length)
                                {
                                    long addr = ((long)memory_basic_information.BaseAddress) + i;
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

        public long[] FindSignatures(byte[] signature, string mask, long minAddress = 0x11FFFFFFFFFF, long maxAddress = 0x7FFFFFFFFFFF)
        {
            long[] sigs = new long[0];

            void AddSignature(long signatureAddress) { Array.Resize(ref sigs, sigs.Length + 1); sigs[sigs.Length - 1] = signatureAddress; }

            MEMORY_BASIC_INFORMATION64 memory_basic_information;
            memory_basic_information.BaseAddress = (ulong)IntPtr.Zero;
            memory_basic_information.RegionSize = (ulong)IntPtr.Zero;
            long num = 0;
            long address = minAddress;
            while (address < maxAddress)
            {
                VirtualQueryEx(Handle, (IntPtr)address, out memory_basic_information, (ulong)Marshal.SizeOf(typeof(MEMORY_BASIC_INFORMATION64)));
                if (address == (((long)memory_basic_information.BaseAddress) + ((long)memory_basic_information.RegionSize)))
                {
                    break;
                }
                address = ((long)memory_basic_information.BaseAddress) + ((long)memory_basic_information.RegionSize);
                byte[] lpBuffer;
                try
                {
                    lpBuffer = new byte[(long)memory_basic_information.RegionSize];
                }
                catch (Exception)
                {
                    continue;
                }
                ReadProcessMemory(Handle, (IntPtr)memory_basic_information.BaseAddress, lpBuffer, lpBuffer.Length, out int lpNumberOfBytesRead);

                for (int i = 0; i < (lpBuffer.Length - signature.Length); i++)
                {
                    if ((lpBuffer[i] == signature[0] || mask[0] == '?') && (lpBuffer[i + 1] == signature[1] || mask[1] == '?'))
                    {
                        for (int j = 0; j < signature.Length; j++)
                        {
                            if ((mask[j] == '?') || (lpBuffer[i + j] == signature[j]))
                            {
                                num++;
                                if (num == signature.Length)
                                {
                                    AddSignature(((long)memory_basic_information.BaseAddress) + i);
                                    num = 0;
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
            return sigs;
        }

        public long[] FindValues<T>(T value, long minAddress = 0x11FFFFFFFFFF, long maxAddress = 0x7FFFFFFFFFFF)
        {
            Type type = typeof(T);
            byte[] data = new byte[0];
            if (type == typeof(sbyte)) data = new byte[1] { (byte)(sbyte)(object)value };
            if (type == typeof(byte)) data = new byte[1] { (byte)(object)value };
            if (type == typeof(short)) data = new byte[2] { (byte)((short)(object)value & 0xFF), (byte)((short)(object)value >> 8 & 0xFF) };
            if (type == typeof(ushort)) data = new byte[2] { (byte)((ushort)(object)value & 0xFF), (byte)((ushort)(object)value >> 8 & 0xFF) };
            if (type == typeof(int)) data = new byte[4] { (byte)((int)(object)value & 0xFF), (byte)((int)(object)value >> 8 & 0xFF), (byte)((int)(object)value >> 16 & 0xFF), (byte)((int)(object)value >> 24 & 0xFF) };
            if (type == typeof(uint)) data = new byte[4] { (byte)((int)(object)value & 0xFF), (byte)((int)(object)value >> 8 & 0xFF), (byte)((int)(object)value >> 16 & 0xFF), (byte)((int)(object)value >> 24 & 0xFF) };
            if (type == typeof(long)) data = new byte[8] { (byte)((long)(object)value & 0xFF), (byte)((long)(object)value >> 8 & 0xFF), (byte)((long)(object)value >> 16 & 0xFF), (byte)((long)(object)value >> 24 & 0xFF), (byte)((long)(object)value >> 32 & 0xFF), (byte)((long)(object)value >> 40 & 0xFF), (byte)((long)(object)value >> 48 & 0xFF), (byte)((long)(object)value >> 56 & 0xFF) };
            if (type == typeof(ulong)) data = new byte[8] { (byte)((ulong)(object)value & 0xFF), (byte)((ulong)(object)value >> 8 & 0xFF), (byte)((ulong)(object)value >> 16 & 0xFF), (byte)((ulong)(object)value >> 24 & 0xFF), (byte)((ulong)(object)value >> 32 & 0xFF), (byte)((ulong)(object)value >> 40 & 0xFF), (byte)((ulong)(object)value >> 48 & 0xFF), (byte)((ulong)(object)value >> 56 & 0xFF) };

            if (type == typeof(float)) data = BitConverter.GetBytes((float)(object)value);
            if (type == typeof(double)) data = BitConverter.GetBytes((double)(object)value);

            if (data.Length < 1)
                return null;

            return FindSignatures(data, "".PadLeft(data.Length, 'x'), minAddress, maxAddress);
        }

        public bool Read(long address, int length, out byte[] data)
        {
            byte[] buffer = new byte[length];
            if (!ReadProcessMemory(Handle, (IntPtr)address, buffer, buffer.Length, out int lpNumberOfBytesRead))
            {
                data = null;
                return false;
            }

            data = buffer;
            return true;
        }

        public T Read<T>(long address)
        {
            Type type = typeof(T);
            byte[] data;
            int size = Marshal.SizeOf(default(T));
            if (!Read(address, size, out data))
                return default(T);

            if (type == typeof(sbyte)) return (T)(object)(sbyte)data[0];
            if (type == typeof(byte)) return (T)(object)data[0];
            if (type == typeof(short)) return (T)(object)(short)(data[1] << 8 | data[0]);
            if (type == typeof(ushort)) return (T)(object)(ushort)(data[1] << 8 | data[0]);
            if (type == typeof(int)) return (T)(object)(int)(data[3] << 24 | data[2] << 16 | data[1] << 8 | data[0]);
            if (type == typeof(uint)) return (T)(object)(uint)(data[3] << 24 | data[2] << 16 | data[1] << 8 | data[0]);
            if (type == typeof(long)) return (T)(object)(long)((long)data[7] << 56 | (long)data[6] << 48 | (long)data[5] << 40 | (long)data[4] << 32 | (long)data[3] << 24 | (long)data[2] << 16 | (long)data[1] << 8 | (long)data[0]);
            if (type == typeof(ulong)) return (T)(object)(ulong)((long)data[7] << 56 | (long)data[6] << 48 | (long)data[5] << 40 | (long)data[4] << 32 | (long)data[3] << 24 | (long)data[2] << 16 | (long)data[1] << 8 | (long)data[0]);

            if (type == typeof(float)) return (T)(object)BitConverter.ToSingle(data, 0);
            if (type == typeof(double)) return (T)(object)BitConverter.ToDouble(data, 0);

            return default(T);
        }

        public bool Write(long address, byte[] data)
        {
            return WriteProcessMemory(Handle, (IntPtr)address, data, data.Length, out int lpNumberOfBytesWritten);
        }

        public bool Write<T>(long address, T value)
        {
            Type type = typeof(T);
            if (type == typeof(sbyte)) return Write(address, new byte[1] { (byte)(sbyte)(object)value });
            if (type == typeof(byte)) return Write(address, new byte[1] { (byte)(object)value });
            if (type == typeof(short)) return Write(address, new byte[2] { (byte)((short)(object)value & 0xFF), (byte)((short)(object)value >> 8 & 0xFF) });
            if (type == typeof(ushort)) return Write(address, new byte[2] { (byte)((ushort)(object)value & 0xFF), (byte)((ushort)(object)value >> 8 & 0xFF) });
            if (type == typeof(int)) return Write(address, new byte[4] { (byte)((int)(object)value & 0xFF), (byte)((int)(object)value >> 8 & 0xFF), (byte)((int)(object)value >> 16 & 0xFF), (byte)((int)(object)value >> 24 & 0xFF) });
            if (type == typeof(uint)) return Write(address, new byte[4] { (byte)((int)(object)value & 0xFF), (byte)((int)(object)value >> 8 & 0xFF), (byte)((int)(object)value >> 16 & 0xFF), (byte)((int)(object)value >> 24 & 0xFF) });
            if (type == typeof(long)) return Write(address, new byte[8] { (byte)((long)(object)value & 0xFF), (byte)((long)(object)value >> 8 & 0xFF), (byte)((long)(object)value >> 16 & 0xFF), (byte)((long)(object)value >> 24 & 0xFF), (byte)((long)(object)value >> 32 & 0xFF), (byte)((long)(object)value >> 40 & 0xFF), (byte)((long)(object)value >> 48 & 0xFF), (byte)((long)(object)value >> 56 & 0xFF) });
            if (type == typeof(ulong)) return Write(address, new byte[8] { (byte)((ulong)(object)value & 0xFF), (byte)((ulong)(object)value >> 8 & 0xFF), (byte)((ulong)(object)value >> 16 & 0xFF), (byte)((ulong)(object)value >> 24 & 0xFF), (byte)((ulong)(object)value >> 32 & 0xFF), (byte)((ulong)(object)value >> 40 & 0xFF), (byte)((ulong)(object)value >> 48 & 0xFF), (byte)((ulong)(object)value >> 56 & 0xFF) });

            if (type == typeof(float)) return Write(address, BitConverter.GetBytes((float)(object)value));
            if (type == typeof(double)) return Write(address, BitConverter.GetBytes((double)(object)value));

            return false;
        }

        public long OffsetAddress(long address, int[] offsets)
        {
            byte[] buffer = new byte[8];
            long pointerAddress = address;
            ReadProcessMemory(Handle, (IntPtr)address, buffer, buffer.Length, out int lpNumberOfBytesRead);

            for(int i = 0; i < offsets.Length; i++)
            {
                long stepAddress = ((long)buffer[7] << 56 | (long)buffer[6] << 48 | (long)buffer[5] << 40 | (long)buffer[4] << 32 | (long)buffer[3] << 24 | (long)buffer[2] << 16 | (long)buffer[1] << 8 | (long)buffer[0]);
                stepAddress += offsets[i];
                pointerAddress = stepAddress;
                ReadProcessMemory(Handle, (IntPtr)stepAddress, buffer, buffer.Length, out lpNumberOfBytesRead);
                //pointerAddress = ((long)buffer[7] << 56 | (long)buffer[6] << 48 | (long)buffer[5] << 40 | (long)buffer[4] << 32 | (long)buffer[3] << 24 | (long)buffer[2] << 16 | (long)buffer[1] << 8 | (long)buffer[0]);
            }
            return pointerAddress;
        }

        public bool AllocateMemory(long address, uint size, MemoryProtection protection)
        {
            return VirtualAllocEx(Handle, (IntPtr)address, size, AllocationType.Commit | AllocationType.Reserve, protection) != IntPtr.Zero;
        }

        public bool AllocateMemory(long address, uint size, MemoryProtection protection, out IntPtr memoryBaseAddress)
        {
            memoryBaseAddress = VirtualAllocEx(Handle, (IntPtr)address, size, AllocationType.Commit | AllocationType.Reserve, protection);
            return memoryBaseAddress != IntPtr.Zero;
        }

        public bool SetMemoryProtection(long address, uint size, MemoryProtection newProtection)
        {
            return VirtualProtectEx(Handle, (IntPtr)address, size, newProtection, out uint lpflOldProtect);
        }

        public bool FreeMemory(long address)
        {
            return VirtualFreeEx(Handle, (IntPtr)address, 0, FreeType.Release);
        }

        public bool InjectDLL(string dllPath)
        {
            uint dwSize = (uint)((dllPath.Length + 1) * Marshal.SizeOf(typeof(char)));
            IntPtr handle = OpenProcess((uint)(ProcessAccessFlags.CreateThread | ProcessAccessFlags.VirtualMemoryOperation | ProcessAccessFlags.VirtualMemoryRead | ProcessAccessFlags.VirtualMemoryWrite | ProcessAccessFlags.QueryInformation), false, ProcessId);
            IntPtr procAddress = GetProcAddress(GetModuleHandle(KERNEL32), "LoadLibraryA");
            if (procAddress == null)
                return false;
            IntPtr alloc = VirtualAllocEx(handle, IntPtr.Zero, dwSize, AllocationType.Commit | AllocationType.Reserve, MemoryProtection.ReadWrite);
            if (alloc == null)
                return false;
            byte[] dllPathBytes = System.Text.Encoding.UTF8.GetBytes(dllPath);
            if (!WriteProcessMemory(handle, alloc, dllPathBytes, (int)dwSize, out int writtenAmount) || writtenAmount != dllPathBytes.Length + 1)
                return false;
            if (CreateRemoteThread(handle, IntPtr.Zero, 0U, procAddress, alloc, 0U, IntPtr.Zero) == IntPtr.Zero)
                return false;
            if (!CloseHandle(handle))
                return false;

            return true;
        }

        public void PressKey(uint key)
        {
            SendMessage(HWND, WM_KEYDOWN, key, 0);
        }

        public void ReleaseKey(uint key)
        {
            SendMessage(HWND, WM_KEYUP, key, 0);
        }

        public void SendChar(uint key)
        {
            SendMessage(HWND, WM_CHAR, key, 0);
        }
    }
}
