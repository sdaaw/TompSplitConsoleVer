using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace splittest2
{
    class InputHandler
    {
        private static InputSimulator input = new InputSimulator();
        private static VirtualKeyCode splitKey = VirtualKeyCode.F1;
        private static VirtualKeyCode resetKey = VirtualKeyCode.F2;

        public static void Split()
        {
            PressKey(splitKey);
        }

        public static void Reset()
        {
            PressKey(resetKey);
        }

        private static void PressKey(VirtualKeyCode key)
        {
            input.Keyboard.KeyDown(key);
            System.Threading.Thread.Sleep(100);
            input.Keyboard.KeyUp(key);
        }

        public static bool EnterIdleMode()
        {
            ConsoleKeyInfo key = Console.ReadKey();
            return key.Key == ConsoleKey.F5;
        }
    }
}
