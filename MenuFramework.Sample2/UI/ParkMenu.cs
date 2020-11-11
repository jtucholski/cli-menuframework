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

            this.AddOption("pdate Park", UpdatePark, "U")
                .AddOption("elete Park", Deletepark, "D")
                .AddOption("uit", Close, "Q")
                .Configure(config =>
                {
                    config.MenuSelectionMode = MenuSelectionMode.KeyString;
                    config.KeyStringTextSeparator = ")";
                    config.SelectedItemBackgroundColor = ConsoleColor.White;
                    config.SelectedItemForegroundColor = ConsoleColor.DarkGreen;
                });
        }

        protected override void OnBeforeShow()
        {
            Console.WriteLine($"Id: {park.ParkId}");
            Console.WriteLine($"Name: {park.Name}");
            Console.WriteLine($"State: {park.State}");
            Console.WriteLine();
        }

        protected override void OnAfterShow()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"--- Today's weather at {park.Name} is 69F and sunny ---");
        }

        private MenuOptionResult Deletepark()
        {
            bool delete = ConsoleMenu.GetBool($"Are you sure you want to delete {park.Name}?", false);

            if (delete)
            {
                parkDao.Delete(park.ParkId);
                Console.WriteLine("Park was deleted.");
                return MenuOptionResult.CloseMenuAfterSelection;
            }
            else
            {
                return MenuOptionResult.DoNotWaitAfterMenuSelection;
            }
        }

        private MenuOptionResult UpdatePark()
        {
            Park updatedPark = new Park(park.ParkId, park.Name, park.State);
         
            updatedPark.Name = ConsoleMenu.GetString("Name:", true);
            if (updatedPark.Name.Trim().Length == 0)
            {
                return MenuOptionResult.DoNotWaitAfterMenuSelection;
            }
            updatedPark.State = ConsoleMenu.GetString("State:");
            if (updatedPark.State.Trim().Length == 0)
            {
                return MenuOptionResult.DoNotWaitAfterMenuSelection;
            }

            parkDao.Update(updatedPark);
            Console.WriteLine("Park was updated.");
            
            return MenuOptionResult.CloseMenuAfterSelection;
        }
    }
}