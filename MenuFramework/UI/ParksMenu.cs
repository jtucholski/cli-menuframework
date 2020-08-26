using MenuFramework.DAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace MenuFramework
{
    class ParksMenu : ConsoleMenu
    {
        private ParkDao parkDao;
        public ParksMenu(ParkDao parkDao)
        {
            // NOTE: We do not add options here, because this is a dynamic, data-driven menu.  We build the options collection in the override of RebuildMenuOptions instead.
            this.parkDao = parkDao;
        }

        protected override void RebuildMenuOptions()
        {
            menuOptions.Clear();
            this.AddOptionRange<Park>(parkDao.GetList(), ShowParkMenu)
                .AddOption("Close", Close);
        }

        public void ShowParkMenu(Park park)
        {
            new ParkMenu(parkDao, park).Show();
        }
    }
}
