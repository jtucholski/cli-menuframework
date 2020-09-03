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
        protected List<MenuOption> menuOptions = new List<MenuOption>();
        private readonly MenuConfig config = new MenuConfig();

        public ConsoleMenu() { }

        #region Command methods that can be referred to by any derived menu for convenience
        /// <summary>
        /// Helper that can be added directly to a MenuOption to close (dismiss) the menu.
        /// </summary>
        /// <returns></returns>
        protected MenuOptionResult Close()
        {
            return MenuOptionResult.CloseMenuAfterSelection;
        }

        /// <summary>
        /// Helper that can be added directly to a MenuOption to Exit (dismiss this menu and  ALL parent menus).
        /// </summary>
        /// <returns></returns>
        protected MenuOptionResult Exit()
        {
            return MenuOptionResult.ExitAfterSelection;
        }

        /// <summary>
        /// Helper that can be added directly to a MenuOption to do a simple call to show a sub-menu
        /// </summary>
        /// <typeparam name="TMenu"></typeparam>
        /// <returns></returns>
        protected MenuOptionResult ShowMenu<TMenu>() where TMenu : ConsoleMenu, new()
        {
            TMenu menu = new TMenu();
            return menu.Show();
        }

        #endregion

        /// <summary>
        /// A derived menu can override this method if it needs to "dynamically" build menu options.
        /// For example, if the menu is data-driven based on a List, and that list may be modified 
        /// (items added, removed or updated) while the program runs, this method can be used to 
        /// re-build the options just before displaying to the user.
        /// </summary>
        virtual protected void RebuildMenuOptions() { }

        /// <summary>
        /// Adds a new option to the menu.
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="action">An action to invoke.</param>
        public ConsoleMenu AddOption(string text, Func<MenuOptionResult> action)
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
        public ConsoleMenu AddOption<T>(string text, Func<T, MenuOptionResult> action, T item)
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
        public ConsoleMenu AddOption<T>(T item, Func<T, MenuOptionResult> action)
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
        public ConsoleMenu AddOptionRange<T>(IEnumerable<T> items, Func<T, MenuOptionResult> action)
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
        public MenuOptionResult Show()
        {
            ConsoleKey key;
            int currentSelectionIndex = 0;

            // Infinitely loop the menu
            while (true)
            {
                // Call the virtual method to rebuild the options on the menu. Menus that are dynamic (data-driven) 
                // in nature should override and re-build menuOptions with the latest data.
                RebuildMenuOptions();

                // At least once display the menu options,
                // when the user presses Enter, invoke the option associated
                // otherwise keep redrawing the screen as the "selection" changes

                // Before the first draw - initialize some variables
                int previousSelectionIndex = -1;
                int menuTop = 0;
                int menuBottom = 0;
                // While we are inside the "drawing" loop, hide the cursor
                Console.CursorVisible = false;
                do
                {
                    // The first time in this loop is a full *re-draw* of the menu. After the first time, when the user presses the arrow keys,
                    // 
                    if (previousSelectionIndex < 0)
                    {
                        // Set previous index so we don't redraw again
                        previousSelectionIndex = 0;
                        Console.Clear();
                        OnBeforeShow();
                        Console.BackgroundColor = config.ItemBackgroundColor;
                        Console.ForegroundColor = config.ItemForegroundColor;

                        // Get the position where we start drawing the menu
                        menuTop = Console.CursorTop;

                        // Print the options
                        for (int i = 0; i < menuOptions.Count; i++)
                        {
                            bool isSelectedOption = (currentSelectionIndex == i);
                            MenuOption option = menuOptions[i];
                            PrintMenuOption(option, isSelectedOption);
                            Console.WriteLine();
                        }
                        // Get the position where the menu is finished drawing
                        menuBottom = Console.CursorTop;
                    }
                    else
                    {
                        // Draw a single item over the previously selected item, and the currently selected item
                        Console.CursorTop = menuTop + previousSelectionIndex;
                        Console.CursorLeft = 0;
                        PrintMenuOption(menuOptions[previousSelectionIndex], false);

                        Console.CursorTop = menuTop + currentSelectionIndex;
                        Console.CursorLeft = 0;
                        PrintMenuOption(menuOptions[currentSelectionIndex], true);

                        Console.CursorTop = menuBottom;
                        Console.CursorLeft = 0;
                    }

                    // Let the user press a key
                    key = Console.ReadKey(true).Key; // do not show the key press on the screen

                    if (key == ConsoleKey.Escape && config.CloseOnEscape)
                    {
                        // This is a HACK! It seems the ESC character somehow "swallows" the next character printed to the screen.  
                        // So I am printing a garbage character, never to be seen. Weird.
                        // Ok, now this seems to have gone away when I changed the ReadKeys to 'true'
                        //Console.WriteLine("X");

                        // Since we are exiting the method, we probably should re-show the cursor.
                        Console.CursorVisible = true;

                        return MenuOptionResult.DoNotWaitAfterMenuSelection;
                    }

                    if (key == ConsoleKey.DownArrow)
                    {
                        // Select the next option
                        previousSelectionIndex = currentSelectionIndex;
                        currentSelectionIndex = GetIndexOfNextItem(currentSelectionIndex);
                    }
                    else if (key == ConsoleKey.UpArrow)
                    {
                        // Select previous option
                        previousSelectionIndex = currentSelectionIndex;
                        currentSelectionIndex = GetIndexOfPrevItem(currentSelectionIndex);
                    }
                    else if (key != ConsoleKey.Enter)
                    {
                        // User pressed a key the menu is not expecting
                        if (config.BeepOnError)
                        {
                            // Let the user know they pressed a bad key
                            Console.Beep();
                        }
                    }


                } while (key != ConsoleKey.Enter);

                // Since we are going to execute a command, we should re-show the cursor.
                Console.CursorVisible = true;

                // User pressed enter, so invoke the command
                MenuOption selectedOption = menuOptions[currentSelectionIndex];

                if (config.ClearConsole)
                {
                    Console.Clear();
                }

                // Invoke the associated option
                MenuOptionResult result = selectedOption.Command();

                // Check Result and act accordingly

                // If the action returned Exit
                if (result == MenuOptionResult.ExitAfterSelection)
                {
                    return MenuOptionResult.ExitAfterSelection;
                }
                // If the action returned Close
                if (result == MenuOptionResult.CloseMenuAfterSelection)
                {
                    return MenuOptionResult.DoNotWaitAfterMenuSelection;
                }

                // Insert a pause so the user must press a key if directed to
                if (result == MenuOptionResult.WaitAfterMenuSelection)
                {
                    Console.ReadKey(true); // do not show the key press on the screen
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
        protected virtual void OnBeforeShow()
        {
            Console.WriteLine(config.Title);
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

        #region User Input Helper Methods - NOTE: these are all static public so they can be used from anywhere, not just derived menus
        /// <summary>
        /// This continually prompts the user until they enter a valid integer.
        /// </summary>
        /// <param name="message">The string to prompt the user with</param>
        /// <returns>A valid integer entered by the user</returns>
        static public int GetInteger(string message, int? defaultValue = null)
        {
            int resultValue = 0;
            while (true)
            {
                Console.Write(message + " ");

                // Check if there is a default option
                if (defaultValue.HasValue)
                {
                    Console.Write($"(Press ENTER for {defaultValue}) ");
                }

                string userInput = Console.ReadLine().Trim();

                // If the user pressed ENTER only, take the default
                if (userInput.Length == 0 && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (int.TryParse(userInput, out resultValue))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("!!! Invalid input. Please enter a valid whole number.");
                }
            }
            return resultValue;
        }

        /// <summary>
        /// This continually prompts the user until they enter a valid double.
        /// </summary>
        /// <param name="message">The string to prompt the user with</param>
        /// <returns>A valid double entered by the user</returns>
        static public double GetDouble(string message, double? defaultValue = null)
        {
            double resultValue = 0;
            while (true)
            {
                Console.Write(message + " ");

                // Check if there is a default option
                if (defaultValue.HasValue)
                {
                    Console.Write($"(Press ENTER for {defaultValue}) ");
                }

                string userInput = Console.ReadLine().Trim();

                // If the user pressed ENTER only, take the default
                if (userInput.Length == 0 && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (double.TryParse(userInput, out resultValue))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("!!! Invalid input. Please enter a valid decimal number.");
                }
            }
            return resultValue;
        }

        /// <summary>
        /// This continually prompts the user until they enter a valid decimal.
        /// </summary>
        /// <param name="message">The string to prompt the user with</param>
        /// <returns>A valid decimal entered by the user</returns>
        static public decimal GetDecimal(string message, decimal? defaultValue = null)
        {
            decimal resultValue = 0;
            while (true)
            {
                Console.Write(message + " ");

                // Check if there is a default option
                if (defaultValue.HasValue)
                {
                    Console.Write($"(Press ENTER for {defaultValue}) ");
                }

                string userInput = Console.ReadLine().Trim();

                // If the user pressed ENTER only, take the default
                if (userInput.Length == 0 && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (decimal.TryParse(userInput, out resultValue))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("!!! Invalid input. Please enter a valid decimal number.");
                }
            }
            return resultValue;
        }

        /// <summary>
        /// This continually prompts the user until they enter a valid date
        /// </summary>
        /// <param name="message">The string to prompt the user with</param>
        /// <returns>Date entered by the user</returns>
        static public DateTime GetDate(string message, DateTime? defaultValue = null)
        {
            DateTime resultValue;
            while (true)
            {
                Console.Write(message + " ");

                // Check if there is a default option
                if (defaultValue.HasValue)
                {
                    Console.Write($"(Press ENTER for {defaultValue.Value.ToShortDateString()}) ");
                }

                string userInput = Console.ReadLine().Trim();

                // If the user pressed ENTER only, take the default
                if (userInput.Length == 0 && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (DateTime.TryParse(userInput, out resultValue))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("!!! Invalid input. Please enter a valid Date.");
                }
            }
            return resultValue;
        }

        /// <summary>
        /// This continually prompts the user until they enter a valid bool.
        /// </summary>
        /// <param name="message">The string to prompt the user with</param>
        /// <returns>True or false.  The user can type Y or true for true values, N or false for false values.</returns>
        static public bool GetBool(string message, bool? defaultValue = null)
        {
            bool resultValue = false;
            while (true)
            {
                Console.Write(message + " ");

                // Check if there is a default option
                if (defaultValue.HasValue)
                {
                    Console.Write($"(Press ENTER for {defaultValue}) ");
                }

                string userInput = Console.ReadLine().Trim();

                // If the user pressed ENTER only, take the default
                if (userInput.Length == 0 && defaultValue.HasValue)
                {
                    return defaultValue.Value;
                }

                if (userInput.ToUpper() == "Y")
                {
                    resultValue = true;
                    break;
                }
                else if (userInput.ToUpper() == "N")
                {
                    resultValue = false;
                    break;
                }
                else if (bool.TryParse(userInput, out resultValue))
                {
                    break;
                }
                else
                {
                    Console.WriteLine("!!! Invalid input. Please enter [True, False, Y or N].");
                }
            }
            return resultValue;

        }

        /// <summary>
        /// This continually prompts the user until they enter a valid string (1 or more characters).
        /// </summary>
        /// <param name="message">The string to prompt the user with</param>
        /// <returns>String entered by the user</returns>
        static public string GetString(string message, bool allowEmptyString = false, string defaultValue = null)
        {
            while (true)
            {
                Console.Write(message + " ");

                // Check if there is a default option
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    Console.Write($"(Press ENTER for '{defaultValue}') ");
                }

                string userInput = Console.ReadLine().Trim();

                // If the user pressed ENTER only, take the default
                if (userInput.Length > 0)
                {
                    return userInput;
                }

                // Empty input, what to do...
                if (!String.IsNullOrEmpty(defaultValue))
                {
                    return defaultValue;
                }

                if (allowEmptyString)
                {
                    return userInput;
                }

                Console.WriteLine("!!! Invalid input. Please enter a valid string.");
            }
        }
        #endregion

        #region Change console colors

        protected void SetColor(ConsoleColor foregroundColor)
        {
            Console.ForegroundColor = foregroundColor;
        }

        protected void ResetColor()
        {
            Console.ForegroundColor = config.ItemForegroundColor;
        }
        #endregion
    }
}