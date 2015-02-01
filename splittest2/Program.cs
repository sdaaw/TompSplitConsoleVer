using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using WindowsInput;
using System.ComponentModel;
using System.Threading;


namespace splittest2
{
    class Program
    {
        const int PROCESS_WM_READ = 0x0010;

        const int VK_SPLIT = 0x23;

        static uint old_level = 0;

        static bool loop = true;
        static bool idleMode = false;

        static Dictionary<string, string[]> levelNames = new Dictionary<string, string[]>()
        {
            {"tomb2", 
            new string[] {"Title",                                                          
            "The Great Wall",
            "Venice",                                 
            "Bartoli's Hideout",
            "Opera House",
            "Offshore Rig",
            "Diving Area",
            "40 Fathoms",
            "Wreck of the Maria Doria",
            "Living Quarters",
            "The Deck",
            "Tibetan Foothills",
            "Barkhang Monastery",
            "Catacombs of the Talion", 
            "Ice Palace",
            "Temple of Xian",
            "Floating Islands",
            "Dragon's Lair",
            "Home Sweet Home",
            "DEMO 1",
            "DEMO 2",
            "DEMO 3",
            "DEMO 4"}},

            {"tomb3", 
            new string[] {"Title",                                                          
            "Jungle",
            "Temple Ruins",                                 
            "The River Ganges",
            "Caves of Kaliya",
            "Coastal Village",
            "Crash Site",
            "Madubu Gorge",
            "Temple of Puna",
            "Thames Wharf",
            "Aldwych",
            "Lud's Gate",
            "City",
            "Nevada Desert",
            "High Security Compound",
            "Area 51",
            "Antarctica",
            "RX-Tech Mines",
            "Lost City of Tinnos",
            "Meteorite Cavern",
            "All Hallows"
            }},

            {"tr3gold", 
            new string[] {"Title",                                                          
            "Highland Fling",
            "Willard's Lair",                                 
            "Shakespeare Cliff",
            "Sleeping with the Fishes",
            "It's a Madhouse!",
            "Reunion"}}
        };

        public enum ProcessAccessFlags : uint
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
        static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern Int32 CloseHandle(IntPtr hProcess);

        public static void Main()
        {
            Console.Title = "~ TOMPING MODE ~";
            ShowMenu();
        }


        public static void ShowMenu()
        {

            Console.Clear();

            string option;
            int optionInt;

            Process[] getTompProcess = Process.GetProcesses();
            for (int i = 0; i < getTompProcess.Length; i++)
            {
                Console.Write(".");
                if (getTompProcess[i].ProcessName == "Tomb2")
                {
                    TheLoop(0);
                    break;
                }
                if (getTompProcess[i].ProcessName == "Tomb3")
                {
                    TheLoop(1);
                    break;
                }
                if (getTompProcess[i].ProcessName == "Tomb3g")
                {
                    TheLoop(2);
                    break;
                }
                System.Threading.Thread.Sleep(10);
            }

            Console.WriteLine("\nNo tomps running!\nWhat tomp are you planning to urn?");
            Console.WriteLine("=============================================================================");
            Console.WriteLine(" ");
            Console.WriteLine("1. Tomb Raider II");
            Console.WriteLine("2. Tomb Raider III");
            Console.WriteLine("3. Tomb Raider The Lost Artifact");

            option = Console.ReadLine();
            optionInt = Convert.ToInt32(option) - 1;

            TheLoop(optionInt);

            Console.WriteLine("ERROR: Invalid option.");
            Console.ReadKey();
            ShowMenu();

            // TOMP 2 LEVEL ADDRESS int 0x4D9EB0;
            // TOMP 3 LEVEL ADDRESS int 0x4C561C;
            // TOMP 3 GOLD LEVEL ADDRESS int 0x4C05FE;
            // TOMP 2 TITLE SCREEN FLAG ADDRESS int 0x51BD90;
            // TOMP 3 TITLE SCREEN FLAG ADDRESS int 0x6A1B78;
            // TOMP 3 GOLD TITLE SCREEN FLAG ADDRESS int 0x69AA04;
        }

        public static void EnterKeyBinds()
        {

        }

        public static string GetString(int memoryAddress, int bytes)
        {
            string[] processName = new string[] { "tomb2", "tomb3", "tr3gold" };
            Process process = Process.GetProcessesByName(processName[0]).FirstOrDefault();
            int bytesRead;
            //byte[] memoryValue = ReadMemory((IntPtr)memoryAddress, bytes, out bytesRead);
            byte[] memoryValue = ReadMemory(process, memoryAddress, bytes, out bytesRead);
            Encoding encoding = Encoding.Default;
            string result = encoding.GetString(memoryValue, 0, (int)bytes);
            result = result.TrimEnd('\0');
            return result;
        }

        public static byte[] ReadMemory(Process process, int address, int numOfBytes, out int bytesRead)
        {
            IntPtr hProc = OpenProcess(ProcessAccessFlags.All, false, process.Id);
            byte[] buffer = new byte[numOfBytes];
            ReadProcessMemory(hProc, new IntPtr(address), buffer, numOfBytes, out bytesRead);
            return buffer;
        }

        public static void TheLoop(int gameID)
        {
            /* gameID
             * 0 = tomp2
             * 1 = tomp3
             * 2 = tomp3g
            */
            int lastLevelTimeAddress = 0x00521724;
            int[] currentLevel = new int[] { 0x4D9EB0, 0x4C561C, 0x4C05FE };
            int[] titleFlag = new int[] { 0x51BD90, 0x6A1B78, 0x69AA04 };
            string[] processName = new string[] { "tomb2", "tomb3", "tr3gold" };
            string[] levels = levelNames[processName[gameID]];

            InputSimulator input = new InputSimulator();
            Process process = Process.GetProcessesByName(processName[gameID]).FirstOrDefault();

            if (process == null)
            {
                Console.Beep();
                Console.WriteLine(processName[gameID] + " is not running, run it now and launch the program.");
                Console.ReadKey();
                Environment.Exit(0);
            }

            int bytesRead;
            Console.Clear();
            Console.WriteLine(processName[gameID] + " loaded");
            Console.WriteLine(" ");
            Console.WriteLine(" ");
            Thread pThread = new Thread(KeyInput);
            pThread.Start();

            while (loop)
            {
                if(idleMode)
                {
                    Console.Title = "~ IDLE MODE ~";
                    Console.ReadKey();
                    Console.Title = "~ TOMPING MODE ~";
                    idleMode = false;
                }
                byte[] value = ReadMemory(process, currentLevel[gameID], 1, out bytesRead);
                byte[] isTitle = ReadMemory(process, titleFlag[gameID], 1, out bytesRead);
                string lastLevelTime = GetString(lastLevelTimeAddress, 10);
                System.Threading.Thread.Sleep(10);

                if ((int)value[0] != old_level && (int)isTitle[0] == 0) // split
                {
                    input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.F1);
                    System.Threading.Thread.Sleep(100);
                    input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.F1);
                    //Console.Clear();
                    //Console.WriteLine("{0}({1}) -> {2}({3})", 
                    //   levels[old_level], old_level, levels[(int)value[0]], (int)value[0]);
                    if (old_level > 0)
                        Console.WriteLine(levels[old_level] + ": " + lastLevelTime);
                    else
                        Console.WriteLine("ok gogo");

                    old_level = value[0];
                }
                else if ((int)isTitle[0] == 1 && old_level != 0) // reset
                {
                    input.Keyboard.KeyDown(WindowsInput.Native.VirtualKeyCode.F2);
                    System.Threading.Thread.Sleep(100);
                    input.Keyboard.KeyUp(WindowsInput.Native.VirtualKeyCode.F2);
                    System.Threading.Thread.Sleep(200);
                    old_level = 0;
                    Console.Clear();
                    Console.WriteLine("reset");
                }
            }
        }

        static void KeyInput()
        {
            if(!idleMode)
            {
                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.F5)
                {
                    idleMode = true;
                }
            }
            System.Threading.Thread.Sleep(200);
            KeyInput();
        }
    }
}
