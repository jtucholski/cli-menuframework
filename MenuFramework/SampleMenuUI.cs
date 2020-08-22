using System;

namespace MenuFramework
{
    public class SampleMenuUI
    {        
        public void DisplayTodaysWeather()
        {
            Console.WriteLine("The weather today is a perfect 85 degrees. Not a cloud in the sky!");
        }

        public void ShowGreeting()
        {
            Console.WriteLine("Hello there!");
        }

        public void TellTheTime()
        {
            Console.WriteLine($"The day and time is {DateTime.Now}");        
        }

        public void ParkSelected(Park selection)
        {
            Console.WriteLine($"The user selected {selection.Name}.");         
        }
    }
}