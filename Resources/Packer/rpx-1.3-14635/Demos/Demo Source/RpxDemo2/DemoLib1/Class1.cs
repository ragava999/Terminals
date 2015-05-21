using System;
using System.Collections.Generic;
using System.Text;

namespace DemoLib1
{
    public class Class1
    {
        public void Print(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Hello world from DemoLib1.Class1"); 

            ConsoleColor col = ConsoleColor.White;
            
            foreach (string str in args) 
            {
                Console.ForegroundColor = col;
                Console.WriteLine(str); 

                if (col == ConsoleColor.White)
                    col = ConsoleColor.DarkGreen; 
                else 
                    col = ConsoleColor.White;
            }
        }
    }
}
