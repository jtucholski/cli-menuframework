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
                config.MenuSelectionMode = MenuSelectionMode.KeyString;
                config.KeyStringTextSeparator = " ... ";
                config.SelectedItemBackgroundColor = ConsoleColor.Yellow;
                config.SelectedItemForegroundColor = ConsoleColor.Black;
            });
        }

        protected override void OnBeforeShow()
        {
            /*
              _   _       _   _                   _   ____            _        
             | \ | | __ _| |_(_) ___  _ __   __ _| | |  _ \ __ _ _ __| | _____ 
             |  \| |/ _` | __| |/ _ \| '_ \ / _` | | | |_) / _` | '__| |/ / __|
             | |\  | (_| | |_| | (_) | | | | (_| | | |  __/ (_| | |  |   <\__ \
             |_| \_|\__,_|\__|_|\___/|_| |_|\__,_|_| |_|   \__,_|_|  |_|\_\___/
            */
            Console.WriteLine(@" _   _       _   _                   _   ____            _        ");
            Console.WriteLine(@"| \ | | __ _| |_(_) ___  _ __   __ _| | |  _ \ __ _ _ __| | _____ ");
            Console.WriteLine(@"|  \| |/ _` | __| |/ _ \| '_ \ / _` | | | |_) / _` | '__| |/ / __|");
            Console.WriteLine(@"| |\  | (_| | |_| | (_) | | | | (_| | | |  __/ (_| | |  |   <\__ \");
            Console.WriteLine(@"|_| \_|\__,_|\__|_|\___/|_| |_|\__,_|_| |_|   \__,_|_|  |_|\_\___/");
            Console.WriteLine();
        }

        protected override void RebuildMenuOptions()
        {
            base.ClearOptions();
            this.AddOptionRange<Park>(parkDao.GetList(), ShowParkMenu, GetMenuText, p => $"{p.ParkId}")
                .AddOption("Close", Close, "Q");
        }

        public MenuOptionResult ShowParkMenu(Park park)
        {
            return new ParkMenu(parkDao, park).Show();
        }

        private string GetMenuText(Park park)
        {
            return $"{park.Name.ToUpper()}, {park.State}";
        }

    }
}
