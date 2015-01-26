using System;
using System.Collections.Generic;
using System.Text;
using DemoLib2;

namespace RpxDemo3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Running Rpx Demo 3");
            Console.WriteLine();

            AppDomain domain = null;

            try
            {
                try 
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Test 1");

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Creates a new instance of the type RpxDemo3.Class1 from the current assembly");
                    Console.WriteLine("within the current AppDomain");
                    Console.WriteLine();

                    Class1 @class = new Class1();

                    @class.PrintDomain();

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("OK");
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
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.ResetColor();
                }

                string appDomainName = "TestDomain";

                domain = AppDomain.CreateDomain(appDomainName);

                try 
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Test 2");

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Creates a new instance of the type DemoLib2.Class2 from a additional assembly");
                    Console.WriteLine("within a new AppDomain"); 
                    Console.WriteLine();

                    Class2 @class = (Class2)domain.CreateInstanceAndUnwrap(typeof(Class2).Assembly.FullName, typeof(Class2).FullName);

                    @class.PrintDomain();
                    
                    Console.ForegroundColor = ConsoleColor.Green;

                    Console.WriteLine("OK"); 
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
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.ResetColor();
                }

                try
                {
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("Test 3");

                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine("Creates a new instance of the type RpxDemo3.Class1 from the current assembly");
                    Console.WriteLine("within a new AppDomain. This will fail because the assembly RpxDemo3 which");
                    Console.WriteLine("holds Class1 cannot be found in the file system");
                    Console.WriteLine();

                    Class1 @class = (Class1)domain.CreateInstanceAndUnwrap(typeof(Class1).Assembly.FullName, typeof(Class1).FullName);

                    @class.PrintDomain();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("OK"); 
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("FAILED");
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("Expected exception: " + ex.Message);
                }
                finally
                {
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.ResetColor();
                }

                if (domain != null)
                {
                    try
                    {
                        AppDomain.Unload(domain);
                    }
                    catch { } 
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("Unexpected exception in test program: " + ex.Message); 
            }
            finally
            {
                if (domain != null)
                {
                    AppDomain.Unload(domain);
                }
            }

            Console.ResetColor();
        }

        static System.Reflection.Assembly domain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("AssemblyResolve: " + args.Name); 

            if (args.Name == typeof(Class1).Assembly.FullName)
            {
                return typeof(Class1).Assembly;
            }
            else if (args.Name == typeof(Class2).Assembly.FullName)
            {
                return typeof(Class2).Assembly;
            }
            else
            {
                return null;
            }
        }
    }

    public class Class1 : MarshalByRefObject
    {
        public void PrintDomain()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("Hello world from RpxDemo3.Class1");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Current appdomain: " + AppDomain.CurrentDomain.FriendlyName);
        }
    }
}
