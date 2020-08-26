using MenuFramework.DAL;
using System;

namespace MenuFramework
{
    public class MainMenu : ConsoleMenu
    {
        ParkDao parkDao;
        public MainMenu(ParkDao parkDao)
        {
            this.parkDao = parkDao;
            this.AddOption("Greeting", ShowGreeting)
                .AddOption("Today", (new SubMenu()).Show)
                .AddOption("Parks", new ParksMenu(parkDao).Show)
                .AddOption("Close", Close)
                .Configure(config => {
                    config.SelectedItemBackgroundColor = ConsoleColor.Red;
                    config.SelectedItemForegroundColor = ConsoleColor.White;
                });
        }

        public void ShowGreeting()
        {
            Console.WriteLine("Hello there!");
        }

        public void ParkSelected(Park selection)
        {
            Console.WriteLine($"The user selected {selection.Name}.");         
        }
    }
}