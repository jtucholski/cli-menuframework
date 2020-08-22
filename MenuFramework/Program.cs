using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace MenuFramework
{
    class Program
    {
        static void ParkSelection(Park park)
        {
            Console.WriteLine($"You selected {park.Name} which is located in {park.State}.");
        }

        static void ColorSelection(string color)
        {
            Console.WriteLine($"You selected {color}.");
        }

        static void Main(string[] args)
        {
            Park olympic = new Park() { Name = "Olympic National Park", State = "WA" };
            Park cuyahoga = new Park() { Name = "Cuyahoga Valley National Park", State = "OH" };
            List<Park> parks = new List<Park>()
            {
                new Park() { Name = "Acadia National Park", State = "ME" },
                new Park() { Name = "Cuyahoga Valley National Park", State = "OH" },                
                new Park() { Name = "Glacier National Park", State = "MT" },                
                new Park() { Name = "Mount Rainier National Park", State = "WA" },
                new Park() { Name = "Olympic National Park", State = "WA" },            
                new Park() { Name = "Zion National Park", State = "UT" }
            };
          
            List<string> colors = new List<string>()
            {
                "Blue",
                "Green",
                "Red",
                "Yellow"
            };

            ConsoleMenu mainMenu = new ConsoleMenu()
                .AddOptionRange<Park>(parks, ParkSelection)
                .AddOption("Close", ConsoleMenu.Close)
                .Configure(config => { 
                    config.SelectedItemBackgroundColor = ConsoleColor.Red;
                    config.SelectedItemForegroundColor = ConsoleColor.White;
                    config.Selector = "* ";
                    config.Title = "PARK SELECTION";
                });

            mainMenu.Show();



            // SampleMenuUI ui = new SampleMenuUI();        

            // ConsoleMenu subMenu = new ConsoleMenu()
            //     .AddOption("Forecast", ui.DisplayTodaysWeather)
            //     .AddOption("Time", ui.TellTheTime)
            //     .AddOption("Close", ConsoleMenu.Close);

            // ConsoleMenu mainMenu = new ConsoleMenu()
            //     .AddOption("Greeting", ui.ShowGreeting)
            //     .AddOption("Today", subMenu.Show)
            //     .AddOption("Close", ConsoleMenu.Close )
            //     .Configure(config => {
            //         config.SelectedItemBackgroundColor = ConsoleColor.Red;
            //         config.SelectedItemForegroundColor = ConsoleColor.White;
            //     });

            //mainMenu.Show();

        }
    }
}
