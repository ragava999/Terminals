using System;
using System.Collections.Generic;
using System.Text;

namespace DemoLib2
{
    public class Class2 : MarshalByRefObject
    {
        public void PrintDomain()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Hello world from DemoLib2.Class2");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Current appdomain: " + AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
