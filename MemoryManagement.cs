using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace EzTrigger
{
    class MemoryManagment
    {

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, ref IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(int hObject);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        public static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        private const uint PROCESS_ALL_ACCESS = 2035711;

        public long BytesRead { get; private set; }
        private IntPtr processHandle = IntPtr.Zero;
        Process[] MyProcess;
        public IntPtr clientBaseAddres = IntPtr.Zero;



        public IntPtr DllImageAddress(string dllname)
        {
            ProcessModuleCollection modules = MyProcess[0].Modules;

            foreach (ProcessModule procmodule in modules)
            {
                if (dllname == procmodule.ModuleName)
                {
                    return procmodule.BaseAddress;
                }
            }
            return IntPtr.Zero;

        }


        public bool Initialize(string processName = "csgo", string windowName = "Counter-Strike: Global Offensive")
        {
            if (processName == "")
            {
                
                return false;
            }


            while (MyProcess == null || MyProcess.Length == 0)
            {
                MyProcess = Process.GetProcessesByName(processName);
                Thread.Sleep(250);
            }
            if ((processHandle = OpenProcess(PROCESS_ALL_ACCESS, false, MyProcess[0].Id)) == IntPtr.Zero)
            {
                Console.WriteLine("ERROR: Cant open process! Make sure it is running. Also try running as Admin.");
                return false;
            }

       
            while (FindWindowByCaption(IntPtr.Zero, windowName) == IntPtr.Zero)
                Thread.Sleep(250);

   
            while ((clientBaseAddres = DllImageAddress("client.dll")) == IntPtr.Zero)
                Thread.Sleep(250);

            Console.WriteLine("");
            Console.WriteLine("");

            Console.WriteLine("> Successfully loaded!");
            return true;
        }

        public T Read<T>(IntPtr address)
        {
            IntPtr numBytes = IntPtr.Zero;
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            if (ReadProcessMemory(processHandle, address, buffer, size, ref numBytes))
                return BytesToT<T>(buffer);

            throw new Exception("Cant read from address!"); // Patch Here
        }

        public unsafe T BytesToT<T>(byte[] data, T defVal = default(T)) 
        {
            T structure = defVal;
            GCHandle gcHandle = GCHandle.Alloc(data, GCHandleType.Pinned);
            structure = (T)Marshal.PtrToStructure(gcHandle.AddrOfPinnedObject(), typeof(T));
            gcHandle.Free();
            return structure;
        }
    }
}
