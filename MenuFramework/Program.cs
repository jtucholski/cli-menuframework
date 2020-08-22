using System.ComponentModel;
using System;
using System.Collections.Generic;

namespace MenuFramework
{
    class Program
    {


        static void Main(string[] args)
        {            
            SampleMenuUI ui = new SampleMenuUI();        

            ConsoleMenu subMenu = new ConsoleMenu()
                .AddOption("Forecast", ui.DisplayTodaysWeather)
                .AddOption("Time", ui.TellTheTime)
                .AddOption("Close", ConsoleMenu.Close);

            ConsoleMenu mainMenu = new ConsoleMenu()
                .AddOption("Greeting", ui.ShowGreeting)
                .AddOption("Today", subMenu.Show)
                .AddOption("Close", ConsoleMenu.Close )
                .Configure(config => {
                    config.SelectedItemBackgroundColor = ConsoleColor.Red;
                    config.SelectedItemForegroundColor = ConsoleColor.White;
                });

            mainMenu.Show();

        }
    }
}
