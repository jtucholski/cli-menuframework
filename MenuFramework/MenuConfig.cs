using System;

namespace MenuFramework
{
    public class MenuConfig
    {
        /// <summary>default Console.BackgroundColor</summary>        
        public ConsoleColor SelectedItemBackgroundColor { get; set; } = Console.BackgroundColor;
        
        /// <summary>default Console.ForegroundColor</summary>        
        public ConsoleColor SelectedItemForegroundColor { get; set; } = Console.ForegroundColor;
        
        /// <summary>default Console.BackgroundColor</summary>        
        public ConsoleColor ItemBackgroundColor { get; set; } = Console.BackgroundColor;
        
        /// <summary>default Console.ForegroundColor</summary>        
        public ConsoleColor ItemForegroundColor { get; set; } = Console.ForegroundColor;

        /// <summary>default: ">> "</summary>
        public string Selector { get; set; } = ">> ";

        /// <summary>default: true</summary>        
        public bool ClearConsole { get; set; } = true;

        public bool BeepOnError { get; set; } = false;

        /// <summary>default: true</summary>        
        public bool WaitAfterMenuSelection { get; set; } = true;

        /// <summary>default: ""</summary>                
        public string Title { get; set; } = "";
        /// <summary>
        /// default: true
        /// </summary>
        public bool CloseOnEscape { get; set; } = true;
    }
}