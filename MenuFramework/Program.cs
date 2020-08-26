using System.ComponentModel;
using System;
using System.Collections.Generic;
using MenuFramework.DAL;

namespace MenuFramework
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
