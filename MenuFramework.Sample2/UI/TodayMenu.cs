using System;

namespace MenuFramework.Sample.UI
{
    public class TodayMenu : ConsoleMenu
    {        
        public TodayMenu()
        {
            this.AddOption("Forecast", DisplayTodaysWeather)
                 .AddOption("Time", TellTheTime)
                 .AddOption("Close", Close, "Q")
                 .AddOption("Exit", Exit, "X")
                 .Configure(config =>
                 {
                     config.MenuSelectionMode = MenuSelectionMode.KeyString;
                     config.Title = "*** Today ***";
                 });

        }

        public MenuOptionResult DisplayTodaysWeather()
        {
            Console.WriteLine("The weather today is a perfect 85 degrees. Not a cloud in the sky!");
            return MenuOptionResult.WaitAfterMenuSelection;
        }

        public MenuOptionResult TellTheTime()
        {
            Console.WriteLine($"The day and time is {DateTime.Now}");
            return MenuOptionResult.WaitAfterMenuSelection;
        }
    }
}