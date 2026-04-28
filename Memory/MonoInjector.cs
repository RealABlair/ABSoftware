using System;
using System.Diagnostics;
using System.Text;
using static ABSoftware.Assembler;

namespace ABSoftware
{
    public class MonoInjector
    {
        public static void Inject(Memory memory, string libraryPath, string entryNamespace, string entryClass, string entryMethod)
        {
            PEScanner scanner = new PEScanner();
            ProcessModule module = null;
            for (int i = 0; i < memory.modules.Count; i++)
            {
                if (memory.modules[i].ModuleName.StartsWith("mono"))
                {
                    module = memory.modules[i];
                    Console.WriteLine($"Mono found! {module.ModuleName}");
                }
            }

            if (module != null)
                scanner.Scan(ref memory, (long)module.BaseAddress);

            long mono_get_root_domain = GetExportFunctionAddress(scanner, "mono_get_root_domain");
            long mono_thread_attach = GetExportFunctionAddress(scanner, "mono_thread_attach");
            long mono_domain_assembly_open = GetExportFunctionAddress(scanner, "mono_domain_assembly_open");
            long mono_assembly_get_image = GetExportFunctionAddress(scanner, "mono_assembly_get_image");
            long mono_class_from_name = GetExportFunctionAddress(scanner, "mono_class_from_name");
            long mono_class_get_method_from_name = GetExportFunctionAddress(scanner, "mono_class_get_method_from_name");
            long mono_runtime_invoke = GetExportFunctionAddress(scanner, "mono_runtime_invoke");

            Assembler assembler = new Assembler(scanner.is64Bit);
            if (scanner.is64Bit)
            {
                uint namespaceOffset = 256;
                uint classNameOffset = 384;
                uint methodNameOffset = 512;

                uint domainResultOffset = 640;
                uint assemblyResultOffset = 656;
                uint codeOffset = 700;

                long codeCave = memory.AllocateMemory(1000, Memory.MemoryProtection.ExecuteReadWrite).ToInt64();

                memory.Write(codeCave, Encoding.UTF8.GetBytes(libraryPath + '\0')); //0 - 255

                memory.Write(codeCave + namespaceOffset, Encoding.UTF8.GetBytes(entryNamespace + '\0'));
                memory.Write(codeCave + classNameOffset, Encoding.UTF8.GetBytes(entryClass + '\0'));
                memory.Write(codeCave + methodNameOffset, Encoding.UTF8.GetBytes(entryMethod + '\0'));

                /*assembler.SUB(Registers.RSP, 40);
                assembler.MOV(Registers.RAX, (long)mono_get_root_domain);
                assembler.CALL(Registers.RAX);
                assembler.ADD(Registers.RSP, 40);
                assembler.RET();*/

                assembler.SUB(Registers.RSP, 40);

                assembler.MOV(Registers.RAX, (long)mono_get_root_domain);
                assembler.CALL(Registers.RAX);

                assembler.MOV_FROM_RAX((Address64)(long)(codeCave + domainResultOffset));

                assembler.MOV(Registers.RCX, Registers.RAX);
                assembler.MOV(Registers.RAX, (long)mono_thread_attach);
                assembler.CALL(Registers.RAX);

                assembler.MOV(Registers.RAX, (codeCave + domainResultOffset));
                assembler.MOV(MemoryAddressRegisters.RAX, Registers.RCX, false);
                assembler.MOV(Registers.RDX, (long)codeCave);
                assembler.MOV(Registers.R8, 0);
                assembler.MOV(Registers.RAX, (long)mono_domain_assembly_open);
                assembler.CALL(Registers.RAX);

                assembler.MOV_FROM_RAX((Address64)(codeCave + assemblyResultOffset));

                assembler.MOV(Registers.RCX, Registers.RAX);
                assembler.MOV(Registers.RAX, (long)mono_assembly_get_image);
                assembler.CALL(Registers.RAX);

                assembler.MOV(Registers.RCX, Registers.RAX);
                assembler.MOV(Registers.RDX, (long)(codeCave + namespaceOffset));
                assembler.MOV(Registers.R8, (long)(codeCave + classNameOffset));
                assembler.MOV(Registers.RAX, (long)mono_class_from_name);
                assembler.CALL(Registers.RAX);

                assembler.MOV(Registers.RCX, Registers.RAX);
                assembler.MOV(Registers.RDX, (long)(codeCave + methodNameOffset));
                assembler.MOV(Registers.R8, 0);
                assembler.MOV(Registers.RAX, (long)mono_class_get_method_from_name);
                assembler.CALL(Registers.RAX);

                assembler.MOV(Registers.RCX, Registers.RAX);
                assembler.MOV(Registers.RDX, 0);
                assembler.MOV(Registers.R8, 0);
                assembler.MOV(Registers.R9, 0);
                assembler.MOV(Registers.RAX, (long)mono_runtime_invoke);
                assembler.CALL(Registers.RAX);

                assembler.ADD(Registers.RSP, 40);
                assembler.RET();

                byte[] shellcode = assembler.ToArray();
                memory.Write(codeCave + codeOffset, shellcode);

                Console.WriteLine($"{codeCave + codeOffset:X16}");
                Console.WriteLine(assembler.ToString());
                Console.ReadLine();

                IntPtr threadHandle = Memory.CreateRemoteThread(memory.getHandle(), IntPtr.Zero, 0, (IntPtr)(codeCave + codeOffset), IntPtr.Zero, 0, IntPtr.Zero);

                if (threadHandle == IntPtr.Zero)
                {
                    Console.WriteLine("An error occured while creating a thread");
                    Console.ReadLine();
                    return;
                }

                Memory.WaitForSingleObject(threadHandle, 0xFFFFFFFF);

                if (Memory.GetExitCodeThread(threadHandle, out uint exitCode))
                {
                    Console.WriteLine($"Thread has returned an exit code: {exitCode}");
                }

                Memory.CloseHandle(threadHandle);
            }
            else
            {
                uint namespaceOffset = 256;
                uint classNameOffset = 384;
                uint methodNameOffset = 512;

                uint domainResultOffset = 640;
                uint assemblyResultOffset = 644;
                uint codeOffset = 700;

                uint codeCave = (uint)memory.AllocateMemory(1000, Memory.MemoryProtection.ExecuteReadWrite).ToInt32();

                memory.Write(codeCave, Encoding.UTF8.GetBytes(libraryPath + '\0')); //0 - 255

                memory.Write(codeCave + namespaceOffset, Encoding.UTF8.GetBytes(entryNamespace + '\0'));
                memory.Write(codeCave + classNameOffset, Encoding.UTF8.GetBytes(entryClass + '\0'));
                memory.Write(codeCave + methodNameOffset, Encoding.UTF8.GetBytes(entryMethod + '\0'));

                assembler.MOV(Registers.EAX, (int)mono_get_root_domain);
                assembler.CALL(Registers.EAX);


                assembler.MOV_FROM_EAX((Address32)(int)(codeCave + domainResultOffset));

                assembler.PUSH(Registers.EAX);
                assembler.MOV(Registers.EAX, (int)mono_thread_attach);
                assembler.CALL(Registers.EAX);
                assembler.ADD(Registers.ESP, 4);

                assembler.PUSH(0);
                assembler.PUSH((int)codeCave);
                assembler.PUSH((Address32)(int)(codeCave + domainResultOffset));
                assembler.MOV(Registers.EAX, (int)mono_domain_assembly_open);
                assembler.CALL(Registers.EAX);
                assembler.ADD(Registers.ESP, 12);

                assembler.MOV_FROM_EAX((Address32)(int)(codeCave + assemblyResultOffset));

                assembler.PUSH(Registers.EAX);
                assembler.MOV(Registers.EAX, (int)mono_assembly_get_image);
                assembler.CALL(Registers.EAX);
                assembler.ADD(Registers.ESP, 4);

                assembler.PUSH((int)(codeCave + classNameOffset));
                assembler.PUSH((int)(codeCave + namespaceOffset));
                assembler.PUSH(Registers.EAX);
                assembler.MOV(Registers.EAX, (int)mono_class_from_name);
                assembler.CALL(Registers.EAX);
                assembler.ADD(Registers.ESP, 12);

                assembler.PUSH(0);
                assembler.PUSH((int)(codeCave + methodNameOffset));
                assembler.PUSH(Registers.EAX);
                assembler.MOV(Registers.EAX, (int)mono_class_get_method_from_name);
                assembler.CALL(Registers.EAX);
                assembler.ADD(Registers.ESP, 12);

                assembler.PUSH(0);
                assembler.PUSH(0);
                assembler.PUSH(0);
                assembler.PUSH(Registers.EAX);
                assembler.MOV(Registers.EAX, (int)mono_runtime_invoke);
                assembler.CALL(Registers.EAX);
                assembler.ADD(Registers.ESP, 16);

                assembler.RET();

                byte[] shellcode = assembler.ToArray();
                memory.Write(codeCave + codeOffset, shellcode);

                IntPtr threadHandle = Memory.CreateRemoteThread(memory.getHandle(), IntPtr.Zero, 0, (IntPtr)(codeCave + codeOffset), IntPtr.Zero, 0, IntPtr.Zero);

                if (threadHandle == IntPtr.Zero)
                {
                    Console.WriteLine("An error occured while creating a thread");
                    Console.ReadLine();
                    return;
                }

                Memory.WaitForSingleObject(threadHandle, 0xFFFFFFFF);

                if (Memory.GetExitCodeThread(threadHandle, out uint exitCode))
                {
                    Console.WriteLine($"Thread has returned an exit code: {exitCode}");
                }

                Memory.CloseHandle(threadHandle);
            }
        }

        static long GetExportFunctionAddress(PEScanner scanner, string exportFunctionName)
        {
            for (int i = 0; i < scanner.exportedFunctions.Length; i++)
            {
                if (scanner.exportedFunctions[i].Name.Equals(exportFunctionName))
                    return (long)(scanner.baseAddress + scanner.exportedFunctions[i].RvaAddress);
            }

            return 0;
        }
    }
}
