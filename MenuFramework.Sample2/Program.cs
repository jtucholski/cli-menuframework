using System;
using System.Collections.Generic;
using MenuFramework;
using MenuFramework.Sample.DAL;
using MenuFramework.Sample.UI;

namespace MenuFramework.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            ParkDao parkDao = new ParkDao("Connection string");

            MainMenu mainMenu = new MainMenu(parkDao);
            mainMenu.Show();
        }

    }
}
