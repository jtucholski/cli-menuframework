using System;

namespace MenuFramework.Sample.UI
{
    public class SubMenu : ConsoleMenu
    {        
        public SubMenu()
        {
           this.AddOption("Forecast", DisplayTodaysWeather)
                .AddOption("Time", TellTheTime)
                .AddOption("Close", Close);
        }

        public MenuOptionResult DisplayTodaysWeather()
        {
            Console.WriteLine("The weather today is a perfect 85 degrees. Not a cloud in the sky!");
            return MenuOptionResult.Default;
        }

        public MenuOptionResult TellTheTime()
        {
            Console.WriteLine($"The day and time is {DateTime.Now}");
            return MenuOptionResult.Default;
        }
    }
}