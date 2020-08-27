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
            this.AddOption("Hello World", HelloWorld)
                .AddOption("Today", ShowMenu<TodayMenu>)
                .AddOption("Parks", ShowParksMenu)
                .AddOption("Close", Close)
                .Configure(config => {
                    config.SelectedItemBackgroundColor = ConsoleColor.Red;
                    config.SelectedItemForegroundColor = ConsoleColor.White;
                    config.WaitAfterMenuSelection = false;
                });
        }

        private static MenuOptionResult HelloWorld()
        {
            Console.WriteLine("Hey there, Hello World!");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult ShowParksMenu()
        {
            return new ParksMenu(parkDao).Show();
        }

        public void ParkSelected(Park selection)
        {
            Console.WriteLine($"The user selected {selection.Name}.");         
        }
    }
}