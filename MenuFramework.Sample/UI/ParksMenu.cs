using MenuFramework.Sample.DAL;
using MenuFramework.Sample.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MenuFramework.Sample.UI
{
    class ParksMenu : ConsoleMenu
    {
        private ParkDao parkDao;
        public ParksMenu(ParkDao parkDao)
        {
            // NOTE: We do not add options here, because this is a dynamic, data-driven menu.  We build the options collection in the override of RebuildMenuOptions instead.
            this.parkDao = parkDao;
            this.Configure(config =>
            {
                config.SelectedItemBackgroundColor = ConsoleColor.Yellow;
                config.SelectedItemForegroundColor = ConsoleColor.Black;
            });
        }

        protected override void RebuildMenuOptions()
        {
            menuOptions.Clear();
            this.AddOptionRange<Park>(parkDao.GetList(), ShowParkMenu)
                .AddOption("Close", Close);
        }

        public MenuOptionResult ShowParkMenu(Park park)
        {
            return new ParkMenu(parkDao, park).Show();
        }
    }
}
