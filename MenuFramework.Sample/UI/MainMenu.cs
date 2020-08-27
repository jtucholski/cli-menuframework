using MenuFramework.Sample.DAL;
using MenuFramework.Sample.Models;
using System;

namespace MenuFramework.Sample.UI
{
    public class MainMenu : ConsoleMenu
    {
        ParkDao parkDao;
        public MainMenu(ParkDao parkDao)
        {
            this.parkDao = parkDao;
            this.AddOption("Hello World", HelloWorld, true)
                .AddOption("Today", (new SubMenu()).Show)
                .AddOption("Parks", new ParksMenu(parkDao).Show)
                .AddOption("Close", Close)
                .Configure(config => {
                    config.SelectedItemBackgroundColor = ConsoleColor.Red;
                    config.SelectedItemForegroundColor = ConsoleColor.White;
                    config.WaitAfterMenuSelection = false;
                });
        }

        private static void HelloWorld()
        {
            Console.WriteLine("Hey there, Hello World!");
        }

        public void ParkSelected(Park selection)
        {
            Console.WriteLine($"The user selected {selection.Name}.");         
        }
    }
}