using System;
using System.Collections.Generic;

namespace MenuFramework
{
    /// <summary>
    /// A Console Menu that supports selectable menu options.
    /// </summary>
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
        /// <param name="option">A MenuOption object.</param>
        public ConsoleMenu AddOption(MenuOption option)
        {
            menuOptions.Add(option);
            return this;
        }

        /// <summary>
        /// Adds an option to the menu. The item is passed back to the callback method.
        /// </summary>
        /// <param name="text">Text to display.</param>
        /// <param name="action">The method to invoke. The method must allow a single argument of type T.</param>
        /// <param name="item">The object to pass the method when invoked.</param>
        /// <returns></returns>
        public ConsoleMenu AddOption<T>(string text, Action<T> action, T item)
        {
            AddOption(text, () => action(item));
            return this;
        }

        /// <summary>
        /// Adds a data bound option to the menu. The item is passed back to the callback method.
        /// </summary>
        /// <param name="item">The item create as a menu option.</param>
        /// <param name="action">The method to invoke. The method must allow a single argument of type T.</param>
        /// <returns></returns>
        public ConsoleMenu AddOption<T>(T item, Action<T> action)
        {
            AddOption(new MenuOption<T>(() => action(item), item));
            return this;
        }

        /// <summary>
        /// Adds multiple options to the menu at once.
        /// </summary>
        /// <param name="items">A collection of items to create as menu options.</param>
        /// <param name="action">The method to invoke. The method must have a single argument of type T.</param>        
        /// <returns></returns>
        public ConsoleMenu AddOptionRange<T>(IEnumerable<T> items, Action<T> action)
        {
            foreach (T item in items)
            {
                AddOption(new MenuOption<T>(() => action(item), item));
            }

            return this;
        }

        /// <summary>
        /// Displays the menu.
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

                    OnBeforeShow();

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

                CheckForWait(selectedOption);

                // Automatically close the menu?
                if (config.CloseMenuOnSelection)
                {
                    return;
                }

            }
        }

        /// <summary>
        /// Enables the ability to overwrite any of the default configuration settings.
        /// </summary>
        /// <param name="action">An Action that accepts a MenuConfig arugment.</param>
        /// <returns></returns>
        public ConsoleMenu Configure(Action<MenuConfig> action)
        {
            action.Invoke(config);
            return this;
        }

        /// <summary>
        /// Override this if specific code needs to execute before the menu is shown.
        /// </summary>
        protected virtual void OnBeforeShow() { }        

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

        private void CheckForWait(MenuOption option)
        {
            //We allow local menu options to override the configure wait
            if (config.WaitAfterMenuSelection)
            {
                Console.ReadKey();
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

}