using MenuFramework.Sample.DAL;
using MenuFramework.Sample.Models;
using System;

namespace MenuFramework.Sample.UI
{
    public class ParkMenu : ConsoleMenu
    {
        private ParkDao parkDao;
        private Park park;

        public ParkMenu(ParkDao parkDao, Park park)
        {
            this.parkDao = parkDao;
            this.park = park;

            this.AddOption("Update Park", UpdatePark)
                .AddOption("Delete Park", Deletepark)
                .AddOption("Close", Close)
                .Configure(config =>
                {
                    config.SelectedItemBackgroundColor = ConsoleColor.White;
                    config.SelectedItemForegroundColor = ConsoleColor.DarkGreen;
                });

        }

        private void Deletepark()
        {
            Console.Write($"Are you sure you want to delete {park.Name}?");
            if (Console.ReadLine().ToLower() == "y")
            {
                parkDao.Delete(park.ParkId);
                Console.WriteLine("Park was deleted.");
            }
        }

        private void UpdatePark()
        {
            
            
            Park updatedPark = new Park(park.ParkId, park.Name, park.State);
            Console.Write("Name: ");
            updatedPark.Name = Console.ReadLine();

            Console.Write("State: ");
            updatedPark.State = Console.ReadLine();

            parkDao.Update(updatedPark);
            Console.WriteLine("Park was updated.");
        }
    }
}