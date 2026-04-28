using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace ABSoftware
{
    public class PEScanner
    {
        public IMAGE_DOS_HEADER imageDosHeader { get; private set; }
        public IMAGE_FILE_HEADER imageFileHeader { get; private set; }

        public IMAGE_OPTIONAL_HEADER32 imageOptionalHeader32 { get; private set; }
        public IMAGE_OPTIONAL_HEADER64 imageOptionalHeader64 { get; private set; }

        public IMAGE_SECTION_HEADER[] sections { get; private set; }

        public IMAGE_EXPORT_DIRECTORY imageExportDirectory { get; private set; }

        public bool is64Bit { get; private set; }

        public ExportFunction[] exportedFunctions { get; private set; }

        public long baseAddress { get; private set; }

        public void Scan(ref Memory memory, long baseAddress)
        {
            this.baseAddress = baseAddress;

            long baseOffset = 0;

            imageDosHeader = ReadStruct<IMAGE_DOS_HEADER>(memory, baseAddress + baseOffset, ref baseOffset);

            //stream.Seek(imageDosHeader.e_lfanew, SeekOrigin.Begin);

            baseOffset = imageDosHeader.e_lfanew;

            uint peSignature = ReadWO<uint>(memory, baseAddress + baseOffset, ref baseOffset);
            if (peSignature != 0x00004550)
                throw new Exception("PE header is corrupted");

            imageFileHeader = ReadStruct<IMAGE_FILE_HEADER>(memory, baseAddress + baseOffset, ref baseOffset);

            if (imageFileHeader.Machine == 0x8664)
            {
                is64Bit = true;
                imageOptionalHeader64 = ReadStruct<IMAGE_OPTIONAL_HEADER64>(memory, baseAddress + baseOffset, ref baseOffset);
            }
            else
            {
                is64Bit = false;
                imageOptionalHeader32 = ReadStruct<IMAGE_OPTIONAL_HEADER32>(memory, baseAddress + baseOffset, ref baseOffset);
            }

            sections = new IMAGE_SECTION_HEADER[imageFileHeader.NumberOfSections];
            for (int i = 0; i < imageFileHeader.NumberOfSections; i++)
                sections[i] = ReadStruct<IMAGE_SECTION_HEADER>(memory, baseAddress + baseOffset, ref baseOffset);

            uint exportRVA = is64Bit ? imageOptionalHeader64.DataDirectory[0].VirtualAddress : imageOptionalHeader32.DataDirectory[0].VirtualAddress;

            //stream.Seek(offset, SeekOrigin.Begin);
            baseOffset = exportRVA;

            imageExportDirectory = ReadStruct<IMAGE_EXPORT_DIRECTORY>(memory, baseAddress + baseOffset, ref baseOffset);

            uint namesOffset = imageExportDirectory.AddressOfNames;
            uint ordinalsOffset = imageExportDirectory.AddressOfNameOrdinals;
            uint functionsOffset = imageExportDirectory.AddressOfFunctions;

            exportedFunctions = new ExportFunction[imageExportDirectory.NumberOfNames];

            for (uint i = 0; i < imageExportDirectory.NumberOfNames; i++)
            {
                baseOffset = namesOffset + (i * 4);
                uint nameRva = ReadWO<uint>(memory, baseAddress + baseOffset, ref baseOffset);

                string name = ReadStringAtRva(memory, ref baseAddress, nameRva);

                baseOffset = ordinalsOffset + (i * 2);
                ushort ordinal = ReadWO<ushort>(memory, baseAddress + baseOffset, ref baseOffset);

                baseOffset = functionsOffset + (ordinal * 4);
                uint funcRva = ReadWO<uint>(memory, baseAddress + baseOffset, ref baseOffset);

                exportedFunctions[i] = new ExportFunction
                {
                    Name = name,
                    RvaAddress = funcRva,
                    Ordinal = (ushort)(imageExportDirectory.Base + ordinal)
                };
            }
        }

        public void Scan(string fileName)
        {
            using(FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using(BinaryReader reader = new BinaryReader(stream))
            {
                imageDosHeader = ReadStruct<IMAGE_DOS_HEADER>(reader);

                stream.Seek(imageDosHeader.e_lfanew, SeekOrigin.Begin);

                uint peSignature = reader.ReadUInt32();
                if (peSignature != 0x00004550)
                    throw new Exception("PE header is corrupted");

                imageFileHeader = ReadStruct<IMAGE_FILE_HEADER>(reader);

                if(imageFileHeader.Machine == 0x8664)
                {
                    is64Bit = true;
                    imageOptionalHeader64 = ReadStruct<IMAGE_OPTIONAL_HEADER64>(reader);
                }
                else
                {
                    is64Bit = false;
                    imageOptionalHeader32 = ReadStruct<IMAGE_OPTIONAL_HEADER32>(reader);
                }

                sections = new IMAGE_SECTION_HEADER[imageFileHeader.NumberOfSections];
                for (int i = 0; i < imageFileHeader.NumberOfSections; i++)
                    sections[i] = ReadStruct<IMAGE_SECTION_HEADER>(reader);

                uint exportRVA = is64Bit ? imageOptionalHeader64.DataDirectory[0].VirtualAddress : imageOptionalHeader32.DataDirectory[0].VirtualAddress;
                uint offset = RVAToOffset(exportRVA);

                stream.Seek(offset, SeekOrigin.Begin);

                imageExportDirectory = ReadStruct<IMAGE_EXPORT_DIRECTORY>(reader);

                uint namesOffset = RVAToOffset(imageExportDirectory.AddressOfNames);
                uint ordinalsOffset = RVAToOffset(imageExportDirectory.AddressOfNameOrdinals);
                uint functionsOffset = RVAToOffset(imageExportDirectory.AddressOfFunctions);

                exportedFunctions = new ExportFunction[imageExportDirectory.NumberOfNames];

                for (uint i = 0; i < imageExportDirectory.NumberOfNames; i++)
                {
                    reader.BaseStream.Seek(namesOffset + (i * 4), SeekOrigin.Begin);
                    uint nameRva = reader.ReadUInt32();

                    string name = ReadStringAtRva(reader, nameRva);

                    reader.BaseStream.Seek(ordinalsOffset + (i * 2), SeekOrigin.Begin);
                    ushort ordinal = reader.ReadUInt16();

                    reader.BaseStream.Seek(functionsOffset + (ordinal * 4), SeekOrigin.Begin);
                    uint funcRva = reader.ReadUInt32();

                    exportedFunctions[i] = new ExportFunction
                    {
                        Name = name,
                        RvaAddress = funcRva,
                        Ordinal = (ushort)(imageExportDirectory.Base + ordinal)
                    };
                }
            }
        }

        T ReadWO<T>(Memory memory, long address, ref long offset)
        {
            offset += Marshal.SizeOf(typeof(T));
            return memory.Read<T>(address);
        }

        T ReadStruct<T>(BinaryReader reader) where T : struct
        {
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        T ReadStruct<T>(Memory memory, long address, ref long offset) where T : struct
        {
            memory.ReadByteArray(address, Marshal.SizeOf(typeof(T)), out byte[] bytes);
            offset += bytes.Length;
            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                return (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            }
            finally
            {
                handle.Free();
            }
        }

        uint RVAToOffset(uint rva)
        {
            for(int i = 0; i < sections.Length; i++)
            {
                if (rva >= sections[i].VirtualAddress && rva < sections[i].VirtualAddress + sections[i].VirtualSize)
                    return (rva - sections[i].VirtualAddress) + sections[i].PointerToRawData;
            }

            return 0;
        }

        string ReadStringAtRva(BinaryReader reader, uint rva)
        {
            uint fileOffset = RVAToOffset(rva);
            if (fileOffset == 0) return string.Empty;

            long oldPos = reader.BaseStream.Position;
            reader.BaseStream.Seek(fileOffset, SeekOrigin.Begin);

            List<byte> bytes = new List<byte>();
            byte b;

            while ((b = reader.ReadByte()) != 0)
            {
                bytes.Add(b);
                if (bytes.Count > 512) break;
            }

            reader.BaseStream.Seek(oldPos, SeekOrigin.Begin);
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        string ReadStringAtRva(Memory memory, ref long address, uint rva)
        {
            uint fileOffset = rva;
            if (fileOffset == 0) return string.Empty;

            long oldPos = address;
            address += fileOffset;

            List<byte> bytes = new List<byte>();
            byte b;

            while ((b = ReadWO<byte>(memory, address, ref address)) != 0)
            {
                bytes.Add(b);
                if (bytes.Count > 512) break;
            }

            address = oldPos;
            return Encoding.UTF8.GetString(bytes.ToArray());
        }
    }

    public struct ExportFunction
    {
        public string Name;
        public uint RvaAddress;
        public ushort Ordinal;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_DOS_HEADER
    {
        public ushort e_magic;
        public ushort e_cblp;
        public ushort e_cp;
        public ushort e_crlc;
        public ushort e_cparhdr;
        public ushort e_minalloc;
        public ushort e_maxalloc;
        public ushort e_ss;
        public ushort e_sp;
        public ushort e_csum;
        public ushort e_ip;
        public ushort e_cs;
        public ushort e_lfarlc;
        public ushort e_ovno;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] e_res;
        public ushort e_oemid;
        public ushort e_oeminfo;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] e_res2;
        public int e_lfanew;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IMAGE_FILE_HEADER
    {
        public ushort Machine;
        public ushort NumberOfSections;
        public uint TimeDateStamp;
        public uint PointerToSymbolTable;
        public uint NumberOfSymbols;
        public ushort SizeOfOptionalHeader;
        public ushort Characteristics;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IMAGE_OPTIONAL_HEADER32
    {
        public ushort Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;
        public uint BaseOfData;

        public uint ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Win32VersionValue;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;
        public ushort Subsystem;
        public ushort DllCharacteristics;
        public uint SizeOfStackReserve;
        public uint SizeOfStackCommit;
        public uint SizeOfHeapReserve;
        public uint SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public IMAGE_DATA_DIRECTORY[] DataDirectory;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IMAGE_OPTIONAL_HEADER64
    {
        public ushort Magic;
        public byte MajorLinkerVersion;
        public byte MinorLinkerVersion;
        public uint SizeOfCode;
        public uint SizeOfInitializedData;
        public uint SizeOfUninitializedData;
        public uint AddressOfEntryPoint;
        public uint BaseOfCode;

        public ulong ImageBase;
        public uint SectionAlignment;
        public uint FileAlignment;
        public ushort MajorOperatingSystemVersion;
        public ushort MinorOperatingSystemVersion;
        public ushort MajorImageVersion;
        public ushort MinorImageVersion;
        public ushort MajorSubsystemVersion;
        public ushort MinorSubsystemVersion;
        public uint Win32VersionValue;
        public uint SizeOfImage;
        public uint SizeOfHeaders;
        public uint CheckSum;
        public ushort Subsystem;
        public ushort DllCharacteristics;
        public ulong SizeOfStackReserve;
        public ulong SizeOfStackCommit;
        public ulong SizeOfHeapReserve;
        public ulong SizeOfHeapCommit;
        public uint LoaderFlags;
        public uint NumberOfRvaAndSizes;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public IMAGE_DATA_DIRECTORY[] DataDirectory;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IMAGE_DATA_DIRECTORY
    {
        public uint VirtualAddress;
        public uint Size;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct IMAGE_SECTION_HEADER
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Name;
        public uint VirtualSize;
        public uint VirtualAddress;
        public uint SizeOfRawData;
        public uint PointerToRawData;
        public uint PointerToRelocations;
        public uint PointerToLinenumbers;
        public ushort NumberOfRelocations;
        public ushort NumberOfLinenumbers;
        public uint Characteristics;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_EXPORT_DIRECTORY
    {
        public uint Characteristics;
        public uint TimeDateStamp;
        public ushort MajorVersion;
        public ushort MinorVersion;
        public uint Name;
        public uint Base;
        public uint NumberOfFunctions;
        public uint NumberOfNames;
        public uint AddressOfFunctions;
        public uint AddressOfNames;
        public uint AddressOfNameOrdinals;
    }
}
