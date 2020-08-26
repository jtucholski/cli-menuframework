using System;
using System.Collections.Generic;
using MenuFramework;

namespace MenuFramework.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleMenu menu = new ConsoleMenu()
                .AddOption("Hello World", HelloWorld)
                .AddOption("Date and Time", DateAndTime)
                .AddOption("Close", ConsoleMenu.Close);

            menu.Show();
        }

        private static void HelloWorld()
        {
            Console.WriteLine("Hey there, Hello World!");
        }

        private static void DateAndTime()
        {
            Console.WriteLine($"The current date and time is {DateTime.Now}");
        }
    }
}
