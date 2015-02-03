using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;

using System.ComponentModel;
using System.Threading;


namespace splittest2
{
    class Program
    {
        private static uint oldLevel = 0;

        private static bool loop = true;
        private static bool idleMode = false;

        public static void Main()
        {
            Console.Title = "~ TOMPING MODE ~";
            ShowMenu();
        }

        public static void ShowMenu()
        {
            Console.Clear();

            int gameId = ProcessHandler.getTombProcessId();

            if (gameId != -1)
                StartTheLoop(gameId);

            Console.WriteLine("\nNo tomps running!\nWhat tomp are you planning to urn?");
            Console.WriteLine("=============================================================================");
            Console.WriteLine(" ");
            Console.WriteLine("1. Tomb Raider II");
            Console.WriteLine("2. Tomb Raider III");
            Console.WriteLine("3. Tomb Raider The Lost Artifact");

            string option = Console.ReadLine();
            int optionInt = Convert.ToInt32(option) - 1;

            if (optionInt >= 0 && optionInt < 3)
                StartTheLoop(optionInt);

            Console.WriteLine("ERROR: Invalid option.");
            ResetProgram();
        }

        public static void TheLoop(Game game)
        {
            /* gameID
             * 0 = tomp2
             * 1 = tomp3
             * 2 = tomp3g
            */
            CheckForProcessInstance(game);

            Console.Clear();
            Console.WriteLine(game.ProcessName + " loaded\n\n");
            loop = true;
            Thread pThread = new Thread(CheckForIdleModeWish);
            pThread.IsBackground = true;
            pThread.Start();
            try
            {
                int bytesRead;
                while (loop)
                {
                    CheckForIdleMode();

                    byte[] currentLevel = ProcessHandler.ReadMemory(game.ActualProcess, game.CurrentLevel, 1, out bytesRead);
                    byte[] isTitle = ProcessHandler.ReadMemory(game.ActualProcess, game.TitleFlag, 1, out bytesRead);
                    string lastLevelTime = ProcessHandler.getTimerString(game);
                    System.Threading.Thread.Sleep(10);

                    if ((int)currentLevel[0] != oldLevel && (int)isTitle[0] == 0) // split
                    {
                        InputHandler.Split();

                        if (oldLevel > 0)
                            Console.WriteLine(game.Levels[oldLevel] + ": " + lastLevelTime);
                        else
                            Console.WriteLine("ok gogo");

                        oldLevel = currentLevel[0];
                    }
                    else if ((int)isTitle[0] == 1 && oldLevel != 0) // reset
                    {
                        InputHandler.Reset();
                        System.Threading.Thread.Sleep(200);

                        oldLevel = 0;
                        Console.Clear();
                        Console.WriteLine("reset");
                    }
                }
            }
            catch (NullReferenceException)
            {
                loop = false;
                pThread.Join();
                CheckForProcessInstance(game);
                TheLoop(game);
            }
        }

        private static void ResetProgram()
        {
            Console.ReadKey();
            Console.Clear();
            ShowMenu();
        }

        private static void CheckForProcessInstance(Game game)
        {
            if (game.ActualProcess == null)
            {
                Console.Beep();
                Console.WriteLine(game.ProcessName + " is not running, run it now and launch the program.");
                ResetProgram();
            }
        }

        private static void CheckForIdleMode()
        {
            if (idleMode)
            {
                Console.Title = "~ IDLE MODE ~";
                Console.ReadKey();
                Console.Title = "~ TOMPING MODE ~";
                idleMode = false;
            }
        }

        private static void CheckForIdleModeWish()
        {
            while (loop)
            {
                if (!idleMode)
                {
                    idleMode = InputHandler.EnterIdleMode();
                }
                System.Threading.Thread.Sleep(200);
            }
        }

        private static void StartTheLoop(int gameId)
        {
            Game game = new Game(gameId);
            TheLoop(game);
        }
    }
}
