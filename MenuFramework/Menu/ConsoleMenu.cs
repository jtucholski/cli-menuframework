using System.Reflection;
using System;
using System.Collections.Generic;

namespace MenuFramework
{


    public class ConsoleMenu
    {
        // Private Instance Variable
        private List<MenuOption> menuOptions = new List<MenuOption>();
        private readonly MenuConfig config = new MenuConfig();

        public ConsoleMenu() { }

        public static void Close()
        {
            throw new InvalidOperationException("This method is not meant to be invoked.");
        }

        /// <summary>
        /// Adds a new option to the menu.
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="action">An action to invoke.</param>
        public ConsoleMenu AddOption(string text, Action action)
        {
            MenuOption option = new MenuOption(text, action);
            menuOptions.Add(option);
            return this;
        }

        /// <summary>
        /// Adds a new option to the menu.
        /// </summary>
        /// <param name="option">The MenuOption object.</param>
        public ConsoleMenu AddOption(MenuOption option)
        {
            menuOptions.Add(option);
            return this;
        }

        public ConsoleMenu AddOption<T>(string text, T item, Action<T> action)
        {
            AddOption(text, () => action(item));
            return this;
        }

        /// <summary>
        /// Adds a new option to the menu that is passed to the action upon selection.
        /// </summary>
        /// <param name="item">The item create as a menu option.</param>
        /// <param name="action">The method to invoke. The method must have a single argument of type T.</param>
        /// <returns></returns>
        public ConsoleMenu AddOption<T>(T item, Action<T> action)
        {
            AddOption(item.ToString(), () => action(item));
            return this;
        }

        /// <summary>
        /// Adds multiple options to the menu at once.
        /// </summary>
        /// <param name="items">A collection of items.</param>
        /// <param name="action">The method to invoke. The method must have a single argument of type T.</param>        
        /// <returns></returns>
        public ConsoleMenu AddOptionRange<T>(IEnumerable<T> items, Action<T> action)
        {
            foreach(T item in items)
            {
                AddOption(item.ToString(), () => action(item));
            }

            return this;
        }

        /// <summary>
        /// Display the menu options.
        /// </summary>
        public void Show()
        {
            ConsoleKey key;
            int currentSelectionIndex = 0;

            // Infinitely loop the menu
            while (true)
            {
                // At least once display the menu options,
                // when the user presses Enter, invoke the option associated
                // otherwise keep redrawing the screen as the "selection" changes
                do
                {
                    Console.Clear();

                    if (!String.IsNullOrEmpty(config.Title))
                    {
                        Console.WriteLine(config.Title);
                    }

                    // Print the options
                    for (int i = 0; i < menuOptions.Count; i++)
                    {
                        bool isSelectedOption = (currentSelectionIndex == i);
                        MenuOption option = menuOptions[i];
                        PrintMenuOption(option, isSelectedOption);
                        Console.WriteLine();
                    }

                    // Let the user press a key
                    key = Console.ReadKey().Key;

                    if (key == ConsoleKey.DownArrow)
                    {
                        // Select the next option
                        currentSelectionIndex = GetIndexOfNextItem(currentSelectionIndex);
                    }
                    else if (key == ConsoleKey.UpArrow)
                    {
                        // Select previous option
                        currentSelectionIndex = GetIndexOfPrevItem(currentSelectionIndex);
                    }

                } while (key != ConsoleKey.Enter);


                // User pressed enter, so invoke the command
                MenuOption selectedOption = menuOptions[currentSelectionIndex];

                if (config.ClearConsole)
                {
                    Console.Clear();
                }

                // If the action is Close
                if (selectedOption.Command == ConsoleMenu.Close)
                {
                    return;
                }

                // Invoke the associated option
                selectedOption.Command();

                // If the command referenced .Show(), we just left
                // another submenu
                bool wasInSubmenu = selectedOption.Command.Method == MethodInfo.GetCurrentMethod();

                // See if we want to wait for the user to resume
                if (config.WaitAfterMenuSelection && !wasInSubmenu)
                {
                    Console.ReadKey();
                }

                // Automatically close the menu?
                if (config.CloseMenuOnSelection)
                {
                    return;
                }

            }
        }

        public ConsoleMenu Configure(Action<MenuConfig> action)
        {
            action.Invoke(config);
            return this;
        }

        private int GetIndexOfNextItem(int currentIndex)
        {
            // If selected one is last in the list
            if (currentIndex + 1 == menuOptions.Count)
            {
                return 0;
            }
            else  // Otherwise selected one is currentIndex + 1
            {
                return currentIndex + 1;
            }
        }

        private int GetIndexOfPrevItem(int currentIndex)
        {
            // If selected one is first in the list
            if (currentIndex == 0)
            {
                return menuOptions.Count - 1;
            }
            else //Otherwise selected one is one of the later options in the list
            {
                return currentIndex - 1;
            }
        }

        private void PrintMenuOption(MenuOption option, bool isSelected)
        {
            string selector = config.Selector;
            string formattedText = option.Text.Replace("\n", "\n" + new string(' ', selector.Length));

            if (isSelected)
            {
                Console.BackgroundColor = config.SelectedItemBackgroundColor;
                Console.ForegroundColor = config.SelectedItemForegroundColor;
                Console.Write(selector);
                Console.Write(formattedText);
            }
            else
            {
                // Add Spacing to keep indentation consistent
                Console.Write(new string(' ', selector.Length));
                Console.Write(formattedText);
            }

            Console.BackgroundColor = config.ItemBackgroundColor;
            Console.ForegroundColor = config.ItemForegroundColor;
        }
    }

    /// <summary>
    /// This menu is used to display a list of data-driven options.
    /// The type must implement IMenuFormattable for display purposes.
    /// </summary>
    /// <typeparam name="T">Type of data to display in the menu.</typeparam>
    public class ConsoleMenu<T> : ConsoleMenu where T : IMenuFormattable
    {
        
        /// <summary>
        /// Adds multiple menu options.
        /// </summary>
        /// <param name="dataSource">The list of objects to add.</param>
        /// <param name="action">The method to invoke. The selected item is passed as the parameter.</param>
        /// <returns></returns>
        public ConsoleMenu AddOptionRange(List<T> dataSource, Action<T> action)
        {
            foreach (T item in dataSource)
            {
                AddOption(item.FormatAsMenuOption(), () => action(item));
            }

            return this;
        }        
    }
}