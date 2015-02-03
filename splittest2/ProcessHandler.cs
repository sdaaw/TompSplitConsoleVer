using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace splittest2
{
    class ProcessHandler
    {
        const int TIMER_BYTES = 10;
        
        private enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern Int32 CloseHandle(IntPtr hProcess);

        public static byte[] ReadMemory(Process process, int address, int numOfBytes, out int bytesRead)
        {
            try
            {
                IntPtr hProc = OpenProcess(ProcessAccessFlags.All, false, process.Id);
                byte[] buffer = new byte[numOfBytes];
                ReadProcessMemory(hProc, new IntPtr(address), buffer, numOfBytes, out bytesRead);
                return buffer;
            }
            catch (NullReferenceException)
            {
                bytesRead = 0;
                return null;
            }
        }

        public static int getTombProcessId()
        {
            Process[] processArray = Process.GetProcesses();
            foreach (Process process in processArray)
            {
                Console.Write(".");
                int index = Game.possibleGames.FindIndex(name => name.ToUpper() == process.ProcessName.ToUpper());

                if (index != -1)
                    return index;
                
                System.Threading.Thread.Sleep(10);
            }

            return -1;
        }

        public static Process getTombProcess(string game)
        {
            return Process.GetProcessesByName(game).FirstOrDefault();
        }

        public static string getTimerString(Game process)
        {
            return getString(process, process.LastLevelTime, TIMER_BYTES);
        }

        private static string getString(Game process, int memoryAddress, int bytes)
        {
            int bytesRead;
            byte[] memoryValue = ProcessHandler.ReadMemory(process.ActualProcess, memoryAddress, bytes, out bytesRead);
            Encoding encoding = Encoding.Default;
            string result = encoding.GetString(memoryValue, 0, (int)bytes);
            result = result.TrimEnd('\0');
            return result;
        }
    }
}
