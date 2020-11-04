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
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            this.parkDao = parkDao;
            this.AddOption("Joke Menu", ShowJokeMenu)
                .AddOption("Hello World", HelloWorld)
                .AddOption("Today", ShowMenu<TodayMenu>)
                .AddOption("Parks", ShowParksMenu)
                .AddOption("Close", Close)
                .Configure(config => {
                    config.ItemBackgroundColor = ConsoleColor.White;
                    config.ItemForegroundColor = ConsoleColor.Black;
                    config.SelectedItemBackgroundColor = ConsoleColor.White;
                    config.SelectedItemForegroundColor = ConsoleColor.Black;
                    config.BeepOnError = true;
                });
        }

        private MenuOptionResult ShowJokeMenu()
        {
            // Create an "on-the-fly" menu
            ConsoleMenu jokeMenu = new ConsoleMenu()
                .AddOption("Cross the road jokes", DisplayCrossTheRoadJokes)
                .AddOption("Bar jokes", DisplayBarJokes)
                .AddOption("Computer jokes", DisplayComputerJokes)
                .AddOption("Close joke menu", ConsoleMenu.Close)
                .AddOption("Exit program", ConsoleMenu.Exit);
            jokeMenu.Configure(cfg => { cfg.SelectedItemForegroundColor = ConsoleColor.Red;});

            return jokeMenu.Show();
        }

        private MenuOptionResult DisplayComputerJokes()
        {
            Console.WriteLine("I tried to change my password to \"14days\"...");
            Console.ReadKey();
            SetColor(ConsoleColor.Green);
            Console.WriteLine("It was rejected as \"two week\"!");
            Console.ReadKey();
            ResetColor();
            Console.Clear();
            Console.WriteLine("How easy is it to count in binary?");
            Console.ReadKey();
            SetColor(ConsoleColor.Green);
            Console.WriteLine("As easy as 01 10 11.");

            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult DisplayBarJokes()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("It's a 5 minute walk from my house to the pub.");
            Console.WriteLine("It's a 35 minute walk from the pub to my house.");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("The difference is staggering.");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.WriteLine("A man was staggering home drunk in the early hours of the morning when he was stopped by a police officer.");
            Console.WriteLine("\"What are you doing out at this time of night?\" asked the officer.");
            Console.WriteLine("\"I'm going to a lecture,\" said the drunk.");
            Console.WriteLine("\"And who's going to be giving a lecture at this hour?\"");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\"My wife.\"");

            return MenuOptionResult.WaitAfterMenuSelection;
        }

        private MenuOptionResult DisplayCrossTheRoadJokes()
        {
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Why did the chicken cross the road?");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("He was practicing good Social Distancing.");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Clear();
            Console.WriteLine("Why did the chicken cross the road, roll in the dirt, and cross the road again?");
            Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Because he was a dirty double-crosser!");

            return MenuOptionResult.WaitAfterMenuSelection;
        }

        protected override void OnBeforeShow()
        {
            /*
              __  __       _         __  __                  
             |  \/  | __ _(_)_ __   |  \/  | ___ _ __  _   _ 
             | |\/| |/ _` | | '_ \  | |\/| |/ _ \ '_ \| | | |
             | |  | | (_| | | | | | | |  | |  __/ | | | |_| |
             |_|  |_|\__,_|_|_| |_| |_|  |_|\___|_| |_|\__,_|
            */
            Console.WriteLine(@" __  __       _         __  __                  ");
            Console.WriteLine(@"|  \/  | __ _(_)_ __   |  \/  | ___ _ __  _   _ ");
            Console.WriteLine(@"| |\/| |/ _` | | '_ \  | |\/| |/ _ \ '_ \| | | |");
            Console.WriteLine(@"| |  | | (_| | | | | | | |  | |  __/ | | | |_| |");
            Console.WriteLine(@"|_|  |_|\__,_|_|_| |_| |_|  |_|\___|_| |_|\__,_|");
            Console.WriteLine();
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