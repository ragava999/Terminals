using System;
using System.Collections.Generic;
using System.Text;
using DemoLib1;

namespace RpxDemo2
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Running Rpx Demo 2");
                
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine("Loading class from additional assembly DemoLib1");

                Class1 item = new Class1();

                item.Print(args);

                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("FAILED");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Unexpected exception: " + ex.Message);
            }
            finally
            {
                Console.ResetColor();
            }
        }
    }
}
