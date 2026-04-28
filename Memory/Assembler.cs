using System;

namespace ABSoftware
{
    public class Assembler
    {
        ByteBuilder builder = new ByteBuilder();

        public Assembler()
        {
            this.is64Bit = false;
        }

        public Assembler(bool is64Bit)
        {
            this.is64Bit = is64Bit;
        }

        bool is64Bit;

        public const byte OPERAND_SIZE_PX = 0x66;
        public const byte REX_W = 0x48;
        public const byte REX_B = 0x41;
        public const byte REX_R = 0x44;

        private readonly AssemblerDictionary<string, int> labels = new AssemblerDictionary<string, int>();
        private readonly AssemblerList<Fix> fixes = new AssemblerList<Fix>();

        //(mov source, destination) = 0x89 | 0x88; (mov destination, source) = 0x8B | 0x8A; 
        public Assembler MOV(MemoryAddressRegisters a, Registers b, bool destinationFirst = true)
        {
            if (destinationFirst) EmitMR(0x8B, new Operand(a), new Operand(b), destinationFirst); 
            else EmitMR(0x89, new Operand(a), new Operand(b), destinationFirst);

            return this;
        }
        public Assembler MOV(Registers a, Registers b, bool destinationFirst = true)
        {
            if (destinationFirst) EmitMR(0x8B, new Operand(a), new Operand(b), destinationFirst);
            else EmitMR(0x89, new Operand(a), new Operand(b), destinationFirst);

            return this;
        }
        public Assembler MOV(MemoryAddressRegisters a, byte aDisplacement, Registers b, bool destinationFirst = true)
        {
            if(destinationFirst) EmitMR(0x8B, new Operand(a), new Operand(b), destinationFirst, aDisplacement);
            else EmitMR(0x89, new Operand(a), new Operand(b), destinationFirst, aDisplacement);

            return this;
        }
        public Assembler MOV(MemoryAddressRegisters a, int aDisplacement, Registers b, bool destinationFirst = true)
        {
            if (destinationFirst) EmitMR(0x8B, new Operand(a), new Operand(b), destinationFirst, aDisplacement);
            else EmitMR(0x89, new Operand(a), new Operand(b), destinationFirst, aDisplacement);

            return this;
        }
        public Assembler MOV(Registers a, byte value) { EmitImmediate(0xB0, new Operand(a), value); return this; }
        public Assembler MOV(Registers a, short value) { EmitImmediate(0xB8, new Operand(a), value); return this; }
        public Assembler MOV(Registers a, int value) { EmitImmediate(0xB8, new Operand(a), value); return this; }
        public Assembler MOV(Registers a, long value) { EmitImmediate(0xB8, new Operand(a), value); return this; }
        public Assembler MOV(MemoryAddressRegisters a, byte value) { EmitImmediate(0xC6, new Operand(a), value); return this; }
        public Assembler MOV(MemoryAddressRegisters a, int value) { EmitImmediate(0xC7, new Operand(a), value); return this; }
        public Assembler MOV(MemoryAddressRegisters a, byte aDisplacement, byte value) { EmitImmediate(0xC6, new Operand(a).SetSize(1), value, aDisplacement); return this; }
        public Assembler MOV(MemoryAddressRegisters a, int aDisplacement, byte value) { EmitImmediate(0xC6, new Operand(a).SetSize(1), value, aDisplacement); return this; }
        public Assembler MOV(MemoryAddressRegisters a, byte aDisplacement, int value) { EmitImmediate(0xC7, new Operand(a), value, aDisplacement); return this; }
        public Assembler MOV(MemoryAddressRegisters a, int aDisplacement, int value) { EmitImmediate(0xC7, new Operand(a), value, aDisplacement); return this; }
        public Assembler MOV_FROM_EAX(Address32 address) { builder.Append(new byte[] { 0xA3 }, address.Bytes()); return this; }
        public Assembler MOV_TO_EAX(Address32 address) { builder.Append(new byte[] { 0xA1 }, address.Bytes()); return this; }
        public Assembler MOV_FROM_RAX(Address64 address) { builder.Append(new byte[] { REX_W, 0xA3 }, address.Bytes()); return this; }
        public Assembler MOV_TO_RAX(Address64 address) { builder.Append(new byte[] { REX_W, 0xA1 }, address.Bytes()); return this; }
        //EmitImmediate(0x8B, new Operand(register), address);

        /// <param name="address">Address (x86) or offset in (x64) apps</param>
        public Assembler MOV(MemoryAddressRegisters register, Address32 address) { EmitMR(0x8B, new Operand(MemoryAddressRegisters.EAX), new Operand(register), false, address, true); return this; }

        public Assembler MOV(Address32 address, MemoryAddressRegisters register) { EmitMR(0x89, new Operand(MemoryAddressRegisters.EAX), new Operand(register), false, address, true); return this; }

        /*public void MOV(MemoryAddressRegisters register, long addressDistance)
        {
            if (register.Is64Bit())
                builder.Append(REX_W);
            builder.Append(new byte[] { 0x8B, (byte)(0x05 | (register.Value() << 3)) }, addressDistance.Bytes());
        }

        public void MOV(long addressDistance, MemoryAddressRegisters register)
        {
            if (register.Is64Bit())
                builder.Append(REX_W);
            builder.Append(new byte[] { 0x89, (byte)(0x05 | (register.Value() << 3)) }, addressDistance.Bytes());
        }*/

        public Assembler NOP(int count = 1)
        {
            byte[] bytes = new byte[count];
            for (int i = 0; i < count; i++)
                bytes[i] = 0x90;
            builder.Append(bytes);

            return this;
        }

        public Assembler JMP(int distance)
        {
            EmitJmp(0xEB, 0xE9, distance);

            return this;
        }

        public Assembler JZ(int distance) { EmitJmp(0x74, 0x84, distance); return this; }
        public Assembler JE(int distance) { return JZ(distance); }
        public Assembler JNZ(int distance) { EmitJmp(0x75, 0x85, distance); return this; }
        public Assembler JNE(int distance) { return JNZ(distance); }
        public Assembler JBE(int distance) { EmitJmp(0x76, 0x86, distance); return this; }
        public Assembler JNA(int distance) { return JBE(distance); }
        public Assembler JA(int distance) { EmitJmp(0x77, 0x87, distance); return this; } 
        public Assembler JNBE(int distance) { return JA(distance); }
        public Assembler JL(int distance) { EmitJmp(0x7C, 0x8C, distance); return this; }
        public Assembler JNGE(int distance) { return JL(distance); }
        public Assembler JNB(int distance) { EmitJmp(0x73, 0x83, distance); return this; }
        public Assembler JAE(int distance) { return JNB(distance); }
        public Assembler JNC(int distance) { return JAE(distance); }
        public Assembler JG(int distance) { EmitJmp(0x7F, 0x8F, distance); return this; }
        public Assembler JNLE(int distance) { return JG(distance); }
        public Assembler JGE(int distance) { EmitJmp(0x7D, 0x8D, distance); return this; }
        public Assembler JNL(int distance) { return JGE(distance); }
        public Assembler JLE(int distance) { EmitJmp(0x7E, 0x8E, distance); return this; }
        public Assembler JNG(int distance) { return JLE(distance); }
        public Assembler JB(int distance) { EmitJmp(0x72, 0x82, distance); return this; }
        public Assembler JNAE(int distance) { return JB(distance); }
        public Assembler JC(int distance) { return JB(distance); }
        public Assembler JS(int distance) { EmitJmp(0x78, 0x88, distance); return this; } // < 0
        public Assembler JNS(int distance) { EmitJmp(0x79, 0x89, distance); return this; } // >= 0

        public Assembler CALL(int distance) { EmitJmp(0xE8, 0xE8, distance); return this; }
        public Assembler CALL(Registers register) { EmitMR(0xFF, new Operand(register), new Operand(2), false); return this; }
        public Assembler RET() { builder.Append(0xC3); return this; }
        public Assembler RET(short bytesToRelease) { builder.Append(0xC2); builder.Append(bytesToRelease.Bytes()); return this; }

        public Assembler PUSH(Registers register) { EmitShort(0x50, new Operand(register)); return this; }
        public Assembler POP(Registers register) { EmitShort(0x58, new Operand(register)); return this; }

        public Assembler INC_Short(Registers register) { if (is64Bit) return INC(register); else EmitShort(0x40, new Operand(register)); return this; }
        public Assembler DEC_Short(Registers register) { if (is64Bit) return DEC(register); else EmitShort(0x48, new Operand(register)); return this; }
        public Assembler INC(Registers register)
        {
            Operand operand = new Operand(register);
            if (operand.Size == 1)
                EmitMR(0xFE, operand, new Operand((byte)0), false);
            else
                EmitMR(0xFF, operand, new Operand((byte)0), false);

            return this;
        }
        public Assembler DEC(Registers register)
        {
            Operand operand = new Operand(register);
            if (operand.Size == 1)
                EmitMR(0xFE, operand, new Operand((byte)0), false);
            else
                EmitMR(0xFF, operand, new Operand((byte)0), false);

            return this;
        }
        public Assembler INC(MemoryAddressRegisters register)
        {
            Operand operand = new Operand(register);
            if (operand.Size == 1)
                EmitMR(0xFE, operand, new Operand((byte)0), false);
            else
                EmitMR(0xFF, operand, new Operand((byte)0), false);

            return this;
        }
        public Assembler DEC(MemoryAddressRegisters register)
        {
            Operand operand = new Operand(register);
            if (operand.Size == 1)
                EmitMR(0xFE, operand, new Operand((byte)0), false);
            else
                EmitMR(0xFF, operand, new Operand((byte)0), false);

            return this;
        }

        public Assembler ADD(Registers destination, Registers source) { if (source.Size() == 1) EmitMR(0x00, new Operand(destination), new Operand(source), true); else EmitMR(0x01, new Operand(destination), new Operand(source), true); return this; }
        public Assembler ADD(MemoryAddressRegisters destination, Registers source) { if (source.Size() == 1) EmitMR(0x00, new Operand(destination), new Operand(source), true); else EmitMR(0x01, new Operand(destination), new Operand(source), true); return this; }
        public Assembler ADD(Registers destination, MemoryAddressRegisters source) { if (destination.Size() == 1) EmitMR(0x02, new Operand(destination), new Operand(source), true); else EmitMR(0x03, new Operand(destination), new Operand(source), true); return this; }
        public Assembler SUB(Registers destination, Registers source) { if (source.Size() == 1) EmitMR(0x28, new Operand(destination), new Operand(source), true); else EmitMR(0x29, new Operand(destination), new Operand(source), true); return this; }
        public Assembler SUB(MemoryAddressRegisters destination, Registers source) { if (source.Size() == 1) EmitMR(0x28, new Operand(destination), new Operand(source), true); else EmitMR(0x29, new Operand(destination), new Operand(source), true); return this; }
        public Assembler SUB(Registers destination, MemoryAddressRegisters source) { if (destination.Size() == 1) EmitMR(0x2A, new Operand(destination), new Operand(source), true); else EmitMR(0x2B, new Operand(destination), new Operand(source), true); return this; }

        public Assembler XOR(Registers destination, Registers source) { if (source.Size() == 1) EmitMR(0x30, new Operand(destination), new Operand(source), true); else EmitMR(0x31, new Operand(destination), new Operand(source), true); return this; }
        public Assembler XOR(MemoryAddressRegisters destination, Registers source) { if (source.Size() == 1) EmitMR(0x30, new Operand(destination), new Operand(source), true); else EmitMR(0x31, new Operand(destination), new Operand(source), true); return this; }
        public Assembler XOR(Registers destination, MemoryAddressRegisters source) { if (source.Size() == 1) EmitMR(0x32, new Operand(destination), new Operand(source), true); else EmitMR(0x33, new Operand(destination), new Operand(source), true); return this; }
        public Assembler CMP(Registers destination, Registers source) { if (source.Size() == 1) EmitMR(0x38, new Operand(destination), new Operand(source), true); else EmitMR(0x39, new Operand(destination), new Operand(source), true); return this; }
        public Assembler CMP(MemoryAddressRegisters destination, Registers source) { if (source.Size() == 1) EmitMR(0x38, new Operand(destination), new Operand(source), true); else EmitMR(0x39, new Operand(destination), new Operand(source), true); return this; }
        public Assembler CMP(Registers destination, MemoryAddressRegisters source) { if (source.Size() == 1) EmitMR(0x3A, new Operand(destination), new Operand(source), true); else EmitMR(0x3B, new Operand(destination), new Operand(source), true); return this; }
        public Assembler AND(Registers destination, Registers source) { if (source.Size() == 1) EmitMR(0x20, new Operand(destination), new Operand(source), true); else EmitMR(0x21, new Operand(destination), new Operand(source), true); return this; }
        public Assembler AND(MemoryAddressRegisters destination, Registers source) { if (source.Size() == 1) EmitMR(0x20, new Operand(destination), new Operand(source), true); else EmitMR(0x21, new Operand(destination), new Operand(source), true); return this; }
        public Assembler AND(Registers destination, MemoryAddressRegisters source) { if (source.Size() == 1) EmitMR(0x22, new Operand(destination), new Operand(source), true); else EmitMR(0x23, new Operand(destination), new Operand(source), true); return this; }
        public Assembler OR(Registers destination, Registers source) { if (source.Size() == 1) EmitMR(0x08, new Operand(destination), new Operand(source), true); else EmitMR(0x09, new Operand(destination), new Operand(source), true); return this; }
        public Assembler OR(MemoryAddressRegisters destination, Registers source) { if (source.Size() == 1) EmitMR(0x08, new Operand(destination), new Operand(source), true); else EmitMR(0x09, new Operand(destination), new Operand(source), true); return this; }
        public Assembler OR(Registers destination, MemoryAddressRegisters source) { if (source.Size() == 1) EmitMR(0x0A, new Operand(destination), new Operand(source), true); else EmitMR(0x0B, new Operand(destination), new Operand(source), true); return this; }
        public Assembler TEST(Registers destination, Registers source) { if (source.Size() == 1) EmitMR(0x84, new Operand(destination), new Operand(source), true); else EmitMR(0x85, new Operand(destination), new Operand(source), true); return this; }
        public Assembler TEST(MemoryAddressRegisters destination, Registers source) { if (source.Size() == 1) EmitMR(0x84, new Operand(destination), new Operand(source), true); else EmitMR(0x85, new Operand(destination), new Operand(source), true); return this; }

        public Assembler LEA(Registers destination, MemoryAddressRegisters source, int displacement = 0) { EmitMR(0x8D, new Operand(destination), new Operand(source), true, displacement); return this; }

        public Assembler ADD(Registers destination, int imm) { if (destination.Size() == 1) EmitImmediate(0x80, new Operand(destination), imm, extension: 0); else EmitImmediate(0x81, new Operand(destination), imm, extension: 0); return this; }
        public Assembler ADD(MemoryAddressRegisters destination, int imm, int displacement = 0) { if (destination.Size() == 1) EmitImmediate(0x80, new Operand(destination), imm, displacement, 0); else EmitImmediate(0x81, new Operand(destination), imm, displacement, 0); return this; }
        public Assembler OR(Registers destination, int imm) { if (destination.Size() == 1) EmitImmediate(0x80, new Operand(destination), imm, extension: 1); else EmitImmediate(0x81, new Operand(destination), imm, extension: 1); return this; }
        public Assembler AND(Registers destination, int imm) { if (destination.Size() == 1) EmitImmediate(0x80, new Operand(destination), imm, extension: 4); else EmitImmediate(0x81, new Operand(destination), imm, extension: 4); return this; }
        public Assembler SUB(Registers destination, int imm) { if (destination.Size() == 1) EmitImmediate(0x80, new Operand(destination), imm, extension: 5); else EmitImmediate(0x81, new Operand(destination), imm, extension: 5); return this; }
        public Assembler SUB(MemoryAddressRegisters destination, int imm, int displacement = 0) { if (destination.Size() == 1) EmitImmediate(0x80, new Operand(destination), imm, displacement, 5); else EmitImmediate(0x81, new Operand(destination), imm, displacement, 5); return this; }
        public Assembler XOR(Registers destination, int imm) { if (destination.Size() == 1) EmitImmediate(0x80, new Operand(destination), imm, extension: 6); else EmitImmediate(0x81, new Operand(destination), imm, extension: 6); return this; }
        public Assembler XOR(MemoryAddressRegisters destination, int imm, int displacement = 0) { if (destination.Size() == 1) EmitImmediate(0x80, new Operand(destination), imm, displacement, 6); else EmitImmediate(0x81, new Operand(destination), imm, displacement, 6); return this; }
        public Assembler CMP(Registers destination, int imm) { if (destination.Size() == 1) EmitImmediate(0x80, new Operand(destination), imm, extension: 7); else EmitImmediate(0x81, new Operand(destination), imm, extension: 7); return this; }
        public Assembler CMP(MemoryAddressRegisters destination, int imm, int displacement = 0) { if (destination.Size() == 1) EmitImmediate(0x80, new Operand(destination), imm, displacement, 7); else EmitImmediate(0x81, new Operand(destination), imm, displacement, 7); return this; }

        public Assembler JMP(string labelName)
        {
            EmitJmp(0xEB, 0xE9, labelName);

            return this;
        }

        public Assembler JZ(string labelName) { EmitJmp(0x74, 0x84, labelName); return this; }
        public Assembler JE(string labelName) { return JZ(labelName); }
        public Assembler JNZ(string labelName) { EmitJmp(0x75, 0x85, labelName); return this; }
        public Assembler JNE(string labelName) { return JNZ(labelName); }
        public Assembler JBE(string labelName) { EmitJmp(0x76, 0x86, labelName); return this; }
        public Assembler JNA(string labelName) { return JBE(labelName); }
        public Assembler JA(string labelName) { EmitJmp(0x77, 0x87, labelName); return this; }
        public Assembler JNBE(string labelName) { return JA(labelName); }
        public Assembler JL(string labelName) { EmitJmp(0x7C, 0x8C, labelName); return this; }
        public Assembler JNGE(string labelName) { return JL(labelName); }
        public Assembler JNB(string labelName) { EmitJmp(0x73, 0x83, labelName); return this; }
        public Assembler JAE(string labelName) { return JNB(labelName); }
        public Assembler JNC(string labelName) { return JAE(labelName); }
        public Assembler JG(string labelName) { EmitJmp(0x7F, 0x8F, labelName); return this; }
        public Assembler JNLE(string labelName) { return JG(labelName); }
        public Assembler JGE(string labelName) { EmitJmp(0x7D, 0x8D, labelName); return this; }
        public Assembler JNL(string labelName) { return JGE(labelName); }
        public Assembler JLE(string labelName) { EmitJmp(0x7E, 0x8E, labelName); return this; }
        public Assembler JNG(string labelName) { return JLE(labelName); }
        public Assembler JB(string labelName) { EmitJmp(0x72, 0x82, labelName); return this; }
        public Assembler JNAE(string labelName) { return JB(labelName); }
        public Assembler JC(string labelName) { return JB(labelName); }
        public Assembler JS(string labelName) { EmitJmp(0x78, 0x88, labelName); return this; } // < 0
        public Assembler JNS(string labelName) { EmitJmp(0x79, 0x89, labelName); return this; } // >= 0

        public Assembler CALL(string labelName) { EmitJmp(0xE8, 0xE8, labelName); return this; }

        public Assembler Label(string labelName)
        {
            if (labels.Contains(labelName))
                return this;

            labels[labelName] = GetPosition();
            return this;
        }

        public Assembler UpdateLabel(string labelName, int value)
        {
            if(labels.Contains(labelName))
                labels[labelName] = value;

            return this;
        }

        public void Clear()
        {
            labels.Clear();
            fixes.Clear();
            builder.Clear();
        }

        public int GetPosition()
        {
            return builder.Size;
        }

        public byte[] ToArray()
        {
            FixLabelData();

            return builder.ToArray();
        }

        public override string ToString()
        {
            FixLabelData();

            return builder.ToString();
        }

        void FixLabelData()
        {
            while (fixes.Size > 0)
            {
                if (!labels.TryGet(fixes[0].LabelName, out int labelPosition))
                    throw new Exception($"Label {fixes[0].LabelName} wasn't defined");

                int delta = labelPosition - (fixes[0].DistanceOffset + fixes[0].DistanceSize);

                byte[] deltaBytes = delta.Bytes();

                for (int i = 0; i < fixes[0].DistanceSize; i++)
                    builder[fixes[0].DistanceOffset + i] = deltaBytes[i];

                fixes.RemoveAt(0);
            }
        }

        public byte GetJMPLength<DT>()
        {
            Type type = typeof(DT);

            if (type == typeof(sbyte))
                return 3;
            if (type == typeof(short))
                return 4;
            if (type == typeof(int))
                return 5;

            return 0;
        }

        public DT GetJMPDistance<DT>(int to, int fromAssemblyBase)
        {
            Type dtType = typeof(DT);
            byte jmpLength = GetJMPLength<DT>();

            int value = (to - (fromAssemblyBase + GetPosition() + jmpLength));

            if (typeof(int) == dtType)
                return (DT)(object)value;
            else if (typeof(short) == dtType)
                return (DT)(object)(short)value;
            else if (typeof(sbyte) == dtType)
                return (DT)(object)(sbyte)value;

            return default(DT);
        }

        public enum Registers : int
        {
            AL,
            CL,
            DL,
            BL,
            AH,
            CH,
            DH,
            BH,

            AX,
            CX,
            DX,
            BX,
            SP,
            BP,
            SI,
            DI,

            EAX,
            ECX,
            EDX,
            EBX,
            ESP,
            EBP,
            ESI,
            EDI,

            MM0,
            MM1,
            MM2,
            MM3,
            MM4,
            MM5,
            MM6,
            MM7,

            XMM0,
            XMM1,
            XMM2,
            XMM3,
            XMM4,
            XMM5,
            XMM6,
            XMM7,

            ES,
            CS,
            SS,
            DS,
            FS,
            GS,
            //res
            //res

            CR0,
            //invd
            CR2,
            CR3,
            CR4,
            //invd
            //invd
            //invd

            DR0,
            DR1,
            DR2,
            DR3,
            DR4,
            DR5,
            DR6,
            DR7,

            //64bit

            SPL,
            BPL,
            SIL,
            DIL,

            R8B,
            R9B,
            R10B,
            R11B,
            R12B,
            R13B,
            R14B,
            R15B,

            R8W,
            R9W,
            R10W,
            R11W,
            R12W,
            R13W,
            R14W,
            R15W,

            R8D,
            R9D,
            R10D,
            R11D,
            R12D,
            R13D,
            R14D,
            R15D,

            RAX,
            RCX,
            RDX,
            RBX,
            RSP,
            RBP,
            RSI,
            RDI,

            R8,
            R9,
            R10,
            R11,
            R12,
            R13,
            R14,
            R15,

            XMM8,
            XMM9,
            XMM10,
            XMM11,
            XMM12,
            XMM13,
            XMM14,
            XMM15,

            CR8
            //invd
            //invd
            //invd
            //invd
            //invd
            //invd
            //invd

            //invd
            //invd
            //invd
            //invd
            //invd
            //invd
            //invd
            //invd
        }

        public enum MemoryAddressRegisters : int
        {
            EAX = Registers.EAX,
            ECX,
            EDX,
            EBX,
            ESP,
            EBP,
            ESI,
            EDI,

            RAX = Registers.RAX,
            RCX,
            RDX,
            RBX,
            RSP,
            RBP,
            RSI,
            RDI,

            R8 = Registers.R8,
            R9,
            R10,
            R11,
            R12,
            R13,
            R14,
            R15,
        }

        public readonly struct Operand
        {
            public readonly byte Value;
            public readonly int Size;
            public readonly bool IsMemory;
            public readonly bool IsExtended;
            public readonly bool IsNewLow;

            public Operand(byte value, int size, bool isMemory, bool isExtended, bool isNewLow)
            {
                this.Value = value;
                this.Size = size;
                this.IsMemory = isMemory;
                this.IsExtended = isExtended;
                this.IsNewLow = isNewLow;
            }

            public Operand(Registers reg)
            {
                this.Value = (byte)reg.Value();
                this.Size = reg.Size();
                this.IsMemory = false;
                this.IsExtended = reg.IsExtended();
                this.IsNewLow = reg >= Registers.SPL && reg <= Registers.DIL;
            }

            public Operand(MemoryAddressRegisters reg)
            {
                this.Value = (byte)reg.Value();
                this.Size = reg.Size();
                this.IsMemory = true;
                this.IsExtended = reg.IsExtended();
                this.IsNewLow = false;
            }

            public Operand(byte extensionValue)
            {
                this.Value = extensionValue;
                this.Size = 0;
                this.IsMemory = false;
                this.IsExtended = false;
                this.IsNewLow = false;
            }

            public Operand SetSize(int size)
            {
                return new Operand(this.Value, size, this.IsMemory, this.IsExtended, this.IsNewLow);
            }
        }

        void EmitMR(byte opCode, Operand destination, Operand source, bool destinationFirst = false, int displacement = 0, bool isStatic = false)
        {
            if(destinationFirst)
            {
                Operand temp = destination;
                destination = source;
                source = temp;
            }

            if (destination.Size == 2) builder.Append(OPERAND_SIZE_PX);

            byte REX = 0x40;
            if (destination.Size == 8) REX |= 0x08;     //W
            if (source.IsExtended) REX |= 0x04;         //R
            if (destination.IsExtended) REX |= 0x01;    //B

            if (REX > 0x40 || destination.IsNewLow || source.IsNewLow)
                builder.Append(REX);

            builder.Append(opCode);

            if(isStatic && destination.IsMemory)
            {
                builder.Append((byte)((source.Value << 3) | 0x05));

                builder.Append(displacement.Bytes());
            }
            else
            {
                byte mod;
                if (!destination.IsMemory) mod = 0xC0;
                else if (displacement == 0)
                {
                    if (destination.Value == 5) mod = 0x40;
                    else mod = 0x00;
                }
                else if (displacement >= sbyte.MinValue && displacement <= sbyte.MaxValue) mod = 0x40;
                else mod = 0x80;

                builder.Append((byte)(mod | (source.Value << 3) | destination.Value));

                if (destination.IsMemory)
                {
                    if (mod == 0x40)
                    {
                        builder.Append((byte)displacement);
                    }
                    else if (mod == 0x80)
                    {
                        builder.Append(displacement.Bytes());
                    }
                }
            }
        }

        void EmitShort(byte baseOpCode, Operand operand)
        {
            if (operand.Size == 2) builder.Append(OPERAND_SIZE_PX);

            byte REX = 0x40;
            if (operand.Size == 8) REX |= 0x08;     //W
            if (operand.IsExtended) REX |= 0x01;    //B

            if (REX > 0x40 || operand.IsNewLow)
                builder.Append(REX);

            builder.Append((byte)(baseOpCode | operand.Value));
        }

        void EmitImmediate(byte opCode, Operand destination, long imm, int displacement = 0, int extension = -1)
        {
            if (destination.Size == 2) builder.Append(OPERAND_SIZE_PX);

            byte REX = 0x40;
            if (destination.Size == 8) REX |= 0x08;     //W
            if (destination.IsExtended) REX |= 0x01;    //B

            if (REX > 0x40 || destination.IsNewLow)
                builder.Append(REX);

            if(extension != -1 || destination.IsMemory)
            {
                builder.Append(opCode);

                byte mod;
                if (!destination.IsMemory)
                    mod = 0xC0;
                else
                {
                    if (displacement == 0) mod = 0x00;
                    else if (displacement >= sbyte.MinValue && displacement <= sbyte.MaxValue) mod = 0x40;
                    else mod = 0x80;
                }

                byte reg = (byte)(extension == -1 ? 0 : extension);

                builder.Append((byte)(mod | (reg << 3) | destination.Value));

                if(displacement != 0)
                {
                    if (mod == 0x40) builder.Append((byte)displacement);
                    else builder.Append(displacement.Bytes());
                }

                byte[] bytes = imm.Bytes();
                int size = destination.Size > 4 ? 4 : destination.Size;
                for (int i = 0; i < size; i++)
                    builder.Append(bytes[i]);
            }
            else
            {
                builder.Append((byte)(opCode | destination.Value));

                byte[] bytes = imm.Bytes();
                for (int i = 0; i < destination.Size; i++)
                    builder.Append(bytes[i]);
            }
        }

        void EmitJmp(byte shortOp, byte nearOp, int distance)
        {
            if (distance >= sbyte.MinValue && distance <= sbyte.MaxValue)
            {
                builder.Append(shortOp);
                builder.Append((byte)distance);
            }
            else
            {
                builder.Append(0x0F);
                builder.Append(nearOp);
                builder.Append(BitConverter.GetBytes(distance));
            }
        }

        void EmitJmp(byte shortOp, byte nearOp, string labelName)
        {
            if (shortOp != 0xEB && shortOp != 0xE8) builder.Append(0x0F);
            
            builder.Append(nearOp);

            fixes.Add(new Fix() 
            {
                LabelName = labelName,
                DistanceSize = 4,
                DistanceOffset = builder.Size
            });

            builder.Append(new byte[] { 0, 0, 0, 0 });
        }
    }

    static class AssemblerExtensions
    {
        static byte[] RegisterOffset = new byte[]
        {
            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,

            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,

            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,

            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,

            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,

            0,
            1,
            2,
            3,
            4,
            5,
            //res
            //res

            0,
            //invd
            2,
            3,
            4,
            //invd
            //invd
            //invd

            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,

            //64bit

            4,
            5,
            6,
            7,

            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,

            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,

            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,

            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,

            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,

            0,
            1,
            2,
            3,
            4,
            5,
            6,
            7,

            0
        };

        static byte[] RegisterSize = new byte[]
        {
            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,

            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,

            4,
            4,
            4,
            4,
            4,
            4,
            4,
            4,

            8,
            8,
            8,
            8,
            8,
            8,
            8,
            8,

            16,
            16,
            16,
            16,
            16,
            16,
            16,
            16,

            2,
            2,
            2,
            2,
            2,
            2,
            //res
            //res

            4,
            //invd
            4,
            4,
            4,
            //invd
            //invd
            //invd

            4,
            4,
            4,
            4,
            4,
            4,
            4,
            4,

            //64bit

            1,
            1,
            1,
            1,

            1,
            1,
            1,
            1,
            1,
            1,
            1,
            1,

            2,
            2,
            2,
            2,
            2,
            2,
            2,
            2,

            4,
            4,
            4,
            4,
            4,
            4,
            4,
            4,

            8,
            8,
            8,
            8,
            8,
            8,
            8,
            8,

            8,
            8,
            8,
            8,
            8,
            8,
            8,
            8,

            16,
            16,
            16,
            16,
            16,
            16,
            16,
            16,

            8
            //invd
            //invd
            //invd
            //invd
            //invd
            //invd
            //invd

            //invd
            //invd
            //invd
            //invd
            //invd
            //invd
            //invd
            //invd
        };

        
        public static int Value(this Assembler.Registers register)
        {
            return RegisterOffset[(int)register];
        }

        public static bool Is64Bit(this Assembler.Registers register)
        {
            return (int)register >= (int)Assembler.Registers.SPL;
        }

        public static int Size(this Assembler.Registers register)
        {
            return RegisterSize[(int)register];
        }

        public static bool IsExtended(this Assembler.Registers register)
        {
            return (register >= Assembler.Registers.R8B && register <= Assembler.Registers.R15D) || (register >= Assembler.Registers.R8);
        }

        public static int Value(this Assembler.MemoryAddressRegisters register)
        {
            return RegisterOffset[(int)register];
        }

        public static bool Is64Bit(this Assembler.MemoryAddressRegisters register)
        {
            return (int)register >= (int)Assembler.Registers.SPL;
        }

        public static int Size(this Assembler.MemoryAddressRegisters register)
        {
            return RegisterSize[(int)register];
        }

        public static bool IsExtended(this Assembler.MemoryAddressRegisters register)
        {
            return register >= Assembler.MemoryAddressRegisters.R8;
        }

        #region Conversions
        public static byte[] Bytes(this sbyte value)
        {
            return new byte[1] { (byte)(sbyte)value };
        }

        public static byte[] Bytes(this byte value)
        {
            return new byte[1] { (byte)value };
        }

        public static byte[] Bytes(this short value)
        {
            return new byte[2] { (byte)(value & 0xFF), (byte)(value >> 8 & 0xFF) };
        }

        public static byte[] Bytes(this ushort value)
        {
            return new byte[2] { (byte)(value & 0xFF), (byte)(value >> 8 & 0xFF) };
        }

        public static byte[] Bytes(this int value)
        {
            return new byte[4] { (byte)(value & 0xFF), (byte)(value >> 8 & 0xFF), (byte)(value >> 16 & 0xFF), (byte)(value >> 24 & 0xFF) };
        }

        public static byte[] Bytes(this uint value)
        {
            return new byte[4] { (byte)(value & 0xFF), (byte)(value >> 8 & 0xFF), (byte)(value >> 16 & 0xFF), (byte)(value >> 24 & 0xFF) };
        }

        public static byte[] Bytes(this long value)
        {
            return new byte[8] { (byte)(value & 0xFF), (byte)(value >> 8 & 0xFF), (byte)(value >> 16 & 0xFF), (byte)(value >> 24 & 0xFF), (byte)(value >> 32 & 0xFF), (byte)(value >> 40 & 0xFF), (byte)(value >> 48 & 0xFF), (byte)(value >> 56 & 0xFF) };
        }

        public static byte[] Bytes(this ulong value)
        {
            return new byte[8] { (byte)(value & 0xFF), (byte)(value >> 8 & 0xFF), (byte)(value >> 16 & 0xFF), (byte)(value >> 24 & 0xFF), (byte)(value >> 32 & 0xFF), (byte)(value >> 40 & 0xFF), (byte)(value >> 48 & 0xFF), (byte)(value >> 56 & 0xFF) };
        }

        public static byte[] Bytes(this Address32 value)
        {
            return value.address.Bytes();
        }

        public static byte[] Bytes(this Address64 value)
        {
            return value.address.Bytes();
        }
        #endregion
    }

    public struct Address32
    {
        public int address { get; private set; }

        public Address32(int address)
        {
            this.address = address;
        }

        public static implicit operator int(Address32 address) => address.address;
        public static implicit operator Address32(int address) => new Address32(address);
        public static implicit operator uint(Address32 address) => (uint)address.address;
        public static implicit operator Address32(uint address) => new Address32((int)address);
    }

    public struct Address64
    {
        public long address { get; private set; }

        public Address64(long address)
        {
            this.address = address;
        }

        public static implicit operator long(Address64 address) => address.address;
        public static implicit operator Address64(long address) => new Address64(address);
        public static implicit operator Address64(int address) => new Address64(address);
    }
    
    struct Fix
    {
        public string LabelName;
        public int DistanceSize;
        public int DistanceOffset;
    }
    class AssemblerDictionary<Key, Value>
    {
        private AssemblerList<AssemblerList<Element>> buckets;

        public int Size { get; private set; }

        public AssemblerDictionary()
        {
            buckets = new AssemblerList<AssemblerList<Element>>();
            Size = 0;
        }

        public Value this[Key key]
        {
            get { return Get(key); }
            set { Set(key, value); }
        }

        public void Add(Key key, Value value)
        {
            if (key == null)
                throw new Exception("Supplied key is null.");

            int hash = key.GetHashCode();

            AssemblerList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
            {
                AssemblerList<Element> newBucket = new AssemblerList<Element>();
                buckets.Add(newBucket);
                newBucket.Add(new Element(hash, key, value));
                Size++;
                return;
            }
            else
            {
                if (bucket.Contains(e => { return e.key.Equals(key); }))
                    throw new Exception("Trying to add a duplicate.");
                else
                {
                    bucket.Add(new Element(hash, key, value));
                    Size++;
                }
            }
        }

        public bool TryAdd(Key key, Value value)
        {
            if (key == null)
                return false;

            int hash = key.GetHashCode();

            AssemblerList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
            {
                AssemblerList<Element> newBucket = new AssemblerList<Element>();
                buckets.Add(newBucket);
                newBucket.Add(new Element(hash, key, value));
                Size++;
                return true;
            }
            else
            {
                if (bucket.Contains(e => { return e.key.Equals(key); }))
                    return false;
                else
                {
                    bucket.Add(new Element(hash, key, value));
                    Size++;
                    return true;
                }
            }
        }

        public Value Get(Key key)
        {
            int hash = key.GetHashCode();

            AssemblerList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
                return default;
            else
                return bucket.FirstOrDefault(e => { return e.key.Equals(key); }).value;
        }

        public bool TryGet(Key key, out Value value)
        {
            int hash = key.GetHashCode();

            AssemblerList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
            {
                value = default;
                return false;
            }
            else
            {
                value = bucket.FirstOrDefault(e => { return e.key.Equals(key); }).value;
                return true;
            }
        }

        public void Set(Key key, Value value)
        {
            int hash = key.GetHashCode();

            AssemblerList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
            {
                AssemblerList<Element> newBucket = new AssemblerList<Element>();
                buckets.Add(newBucket);
                newBucket.Add(new Element(hash, key, value));
                Size++;
                return;
            }
            for (int i = 0; i < bucket.Size; i++)
            {
                if (bucket[i].key.Equals(key))
                {
                    bucket[i] = new Element(hash, key, value);
                    return;
                }
            }
            bucket.Add(new Element(hash, key, value));
            Size++;
        }

        public void Remove(Key key)
        {
            int hash = key.GetHashCode();

            AssemblerList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
                return;
            else
            {
                Size -= bucket.RemoveIf(e => { return e.key.Equals(key); });
            }
        }

        public void Clear()
        {
            buckets.Clear();
            Size = 0;
        }

        public bool Contains(Key key)
        {
            int hash = key.GetHashCode();

            AssemblerList<Element> bucket = buckets.FirstOrDefault(b => { return (b.Size > 0 && b[0].hash == hash); });
            if (bucket == null)
                return false;
            else
                return !bucket.FirstOrDefault(e => { return e.key.Equals(key); }).Equals(default(Element));
        }

        private struct Element
        {
            public int hash;
            public Key key;
            public Value value;

            public Element(int hash, Key key, Value value)
            {
                this.hash = hash;
                this.key = key;
                this.value = value;
            }
        }
    }
    class AssemblerList<T>
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

        public AssemblerList()
        {
            this.elements = new T[0];
            this.Size = 0;
        }

        public AssemblerList(T[] elements)
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
            for (int i = 0; i < Size; i++)
            {
                if (elements[i].Equals(element))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        public void RemoveAt(int id)
        {
            Array.Copy(this.elements, id + 1, this.elements, id, this.Size - id - 1);
            Size--;
        }

        public int RemoveIf(Func<T, bool> predicate)
        {
            int removedCount = 0;
            T[] stamp = GetElements();
            for (int i = 0; i < stamp.Length; i++)
            {
                if (predicate.Invoke(stamp[i]))
                {
                    removedCount++;
                    Remove(stamp[i]);
                }
            }
            return removedCount;
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
                if (predicate.Invoke(elements[i]))
                    return i;
            }
            return -1;
        }

        public AssemblerList<T> Copy()
        {
            T[] array = new T[Size];
            Array.Copy(elements, 0, array, 0, Size);
            AssemblerList<T> newList = new AssemblerList<T>(array);
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

                    if (sortingType > 0)
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
