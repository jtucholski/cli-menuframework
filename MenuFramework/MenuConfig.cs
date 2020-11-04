using System;

namespace MenuFramework
{
    /// <summary>
    /// Class that contains all the configurable options for a ConsoleMenu
    /// </summary>
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

        /// <summary>
        /// default: false
        /// </summary>
        public bool BeepOnError { get; set; } = false;

        /// <summary>default: true</summary>        
        public bool WaitAfterMenuSelection { get; set; } = true;

        /// <summary>default: ""</summary>                
        public string Title { get; set; } = "";
        /// <summary>
        /// default: true
        /// </summary>
        public bool CloseOnEscape { get; set; } = true;

        /// <summary>
        /// Determines how the user will select an option from the menu
        /// </summary>
        public MenuSelectionMode MenuSelectionMode { get; set; } = MenuSelectionMode.Arrow;
        /// <summary>
        /// When in KeyString mode, the text between the Key String and the Option Text
        /// </summary>
        public string KeyStringTextSeparator { get; set; } = " ";

        /// <summary>
        /// When in KeyString mode, the text to prompt the user for selecting an item.
        /// </summary>
        public string KeyStringPrompt { get; set; } = "Select an option: ";
    }

    /// <summary>
    /// Configuration option for howe the user selects an item from the menu.
    /// </summary>
    public enum MenuSelectionMode : int
    {
        /// <summary>
        /// In Arrow mode, the user uses the up and down arrow keys to highlight an option, and then presses Enter to select that option./>
        /// </summary>
        Arrow = 1,
        /// <summary>
        /// In KeyString mode, each menu option is assigned a unique string, used to identify the option, and there is a prompt to Select an option.  
        /// The user types the key string and presses Enter to select.
        /// </summary>
        KeyString
    }
}
