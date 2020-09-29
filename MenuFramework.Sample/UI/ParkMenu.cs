﻿using MenuFramework.Sample.DAL;
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

        protected override void OnBeforeShow()
        {
            Console.WriteLine($"Id: {park.ParkId}");
            Console.WriteLine($"Name: {park.Name}");
            Console.WriteLine($"State: {park.State}");
            Console.WriteLine();
        }

        private MenuOptionResult Deletepark()
        {
            bool delete = ConsoleMenu.GetBool($"Are you sure you want to delete {park.Name}?");

            if (delete)
            {
                parkDao.Delete(park.ParkId);
                Console.WriteLine("Park was deleted.");
            }

            return MenuOptionResult.CloseMenuAfterSelection;
        }

        private MenuOptionResult UpdatePark()
        {
            Park updatedPark = new Park(park.ParkId, park.Name, park.State);
         
            updatedPark.Name = ConsoleMenu.GetString("Name:");
            updatedPark.State = ConsoleMenu.GetString("State:");

            parkDao.Update(updatedPark);
            Console.WriteLine("Park was updated.");
            
            return MenuOptionResult.CloseMenuAfterSelection;
        }
    }
}