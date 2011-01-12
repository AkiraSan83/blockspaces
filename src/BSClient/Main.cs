using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JollyBit.BS.Client.Rendering;

namespace JollyBit.BS.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(System.Reflection.Assembly.GetAssembly(typeof(Program)).CodeBase);
            RenderWrapper.RegisterUpdateCallback(UpdateCallback);
            RenderWrapper.RegisterMouseCallback(MouseCallback);
            RenderWrapper.RegisterKeyboardCallback(KeyboardCallback);
            RenderWrapper.Run();
            Console.ReadLine();
        }

        static void UpdateCallback()
        {

        }
        static void MouseCallback(EMouseButton mouseButton, KeyStatus keyStatus, int x, int y)
        {
        }
        static void KeyboardCallback(ushort character, EKey key, KeyStatus keyStatus)
        {
            Console.WriteLine(character);
        }
    }
}
