using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// Default constructor
        /// </summary>
        public ConsoleMenu() { }

        #region Command methods that can be referred to by any derived menu for convenience
        /// <summary>
        /// Clears the available menu options, allowing them to be updated.
        /// </summary>
        protected void ClearOptions()
        {
            menuOptions.Clear();
        }

        /// <summary>
        /// Helper that can be added directly to a MenuOption to close (dismiss) the menu.
        /// </summary>
        /// <returns>A MenuOptionResult that tells the owning menu to Close.</returns>
        public static MenuOptionResult Close()
        {
            return MenuOptionResult.CloseMenuAfterSelection;
        }

        /// <summary>
        /// Helper that can be added directly to a MenuOption to Exit (dismiss this menu and  ALL parent menus).
        /// </summary>
        /// <returns>A MenuOptionResult that tells the owning menu to Exit.</returns>
        public static MenuOptionResult Exit()
        {
            return MenuOptionResult.ExitAfterSelection;
        }

        /// <summary>
        /// Helper that can be added directly to a MenuOption to do a simple call to show a sub-menu
        /// </summary>
        /// <typeparam name="TMenu">The type of menu (a ConsoleMenu) that should be created and displayed.</typeparam>
        /// <returns>Passes back the return value from the menu once it closes.</returns>
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

        #region AddOption method overloads
        /// <summary>
        /// Adds a new option to the menu.
        /// </summary>
        /// <param name="text">The text to display</param>
        /// <param name="action">An action to invoke.</param>
        /// <param name="keyString">When in MenuSelectionMode.KeyString, the text of the string the user must type to select this option.</param>
        /// <returns>Returns this menu, so that this method can be used in a method chain.</returns>
        public ConsoleMenu AddOption(string text, Func<MenuOptionResult> action, string keyString = null)
        {
            MenuOption option = new MenuOption(text, action, keyString);
            menuOptions.Add(option);
            return this;
        }

        /// <summary>
        /// Adds a new option to the menu.
        /// </summary>
        /// <param name="option">A MenuOption object.</param>
        /// <returns>Returns this menu, so that this method can be used in a method chain.</returns>
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
        /// <param name="keyString">When in MenuSelectionMode.KeyString, the text of the string the user must type to select this option.</param>
        /// <returns>Returns this menu, so that this method can be used in a method chain.</returns>
        public ConsoleMenu AddOption<T>(string text, Func<T, MenuOptionResult> action, T item, string keyString = null)
        {
            AddOption(text, () => action(item), keyString);
            return this;
        }

        /// <summary>
        /// Adds a data bound option to the menu. The item is passed back to the callback method.
        /// </summary>
        /// <param name="item">The item created as a menu option.</param>
        /// <param name="action">The method to invoke. The method must allow a single argument of type into which the item will be passed.</param>
        /// <param name="getMenuText">A method to get the display text for the menu item. It should take a parameter which is the item, and return a string which is the display text.  If this argument is null, item.ToString will be called to get the menu text.</param>
        /// <param name="getKeyString">When in MenuSelectionMode.KeyString, a method to call to get the text of the string the user must type to select this option.</param>
        /// <returns>Returns this menu, so that this method can be used in a method chain.</returns>
        public ConsoleMenu AddOption<T>(T item, Func<T, MenuOptionResult> action, Func<T, string> getMenuText = null, Func<T, string> getKeyString = null)
        {
            AddOption(new MenuOption<T>(() => action(item), item, getMenuText, getKeyString));
            return this;
        }

        /// <summary>
        /// Adds multiple options to the menu at once.
        /// </summary>
        /// <param name="items">A collection of items to create as menu options.</param>
        /// <param name="action">The method to invoke. The method must allow a single argument of type into which the item will be passed.</param>
        /// <param name="getMenuText">A method to get the display text for the menu item. It should take a parameter which is the item, and return a string which is the display text.  If this argument is null, item.ToString will be called to get the menu text.</param>
        /// <param name="getKeyString">When in MenuSelectionMode.KeyString, a method to call to get the text of the string the user must type to select this option.</param>
        /// <returns>Returns this menu, so that this method can be used in a method chain.</returns>
        public ConsoleMenu AddOptionRange<T>(IEnumerable<T> items, Func<T, MenuOptionResult> action, Func<T, string> getMenuText = null, Func<T, string> getKeyString = null)
        {
            foreach (T item in items)
            {
                AddOption(new MenuOption<T>(() => action(item), item, getMenuText, getKeyString));
            }

            return this;
        }

        #endregion

        /// <summary>
        /// Displays the menu.
        /// </summary>
        public MenuOptionResult Show()
        {
            MenuOption selectedOption = null;
            // Infinitely loop the menu
            while (true)
            {
                // Call the virtual method to rebuild the options on the menu. Menus that are dynamic (data-driven) 
                // in nature should override and re-build menuOptions with the latest data.
                RebuildMenuOptions();

                switch (config.MenuSelectionMode)
                {
                    case MenuSelectionMode.Arrow:
                        selectedOption = DrawMenuAndGetSelection_ArrowMode(selectedOption);
                        break;

                    case MenuSelectionMode.KeyString:
                        selectedOption = DrawMenuAndGetSelection_KeyStringMode(selectedOption);
                        break;
                }

                // We now have a selectedOption

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

                // If the action says to wait before closing
                if (result == MenuOptionResult.WaitThenCloseAfterSelection)
                {
                    Console.ReadKey(intercept: true);
                    return MenuOptionResult.DoNotWaitAfterMenuSelection;
                }


                // Insert a pause so the user must press a key if directed to
                if (result == MenuOptionResult.WaitAfterMenuSelection)
                {
                    Console.ReadKey(intercept: true);
                }
            }
        }

        private MenuOption DrawMenuAndGetSelection_KeyStringMode(MenuOption selectedOption)
        {
            // Draw all the menu items
            Console.BackgroundColor = config.ItemBackgroundColor;
            Console.ForegroundColor = config.ItemForegroundColor;
            Console.Clear();
            OnBeforeShow();

            // Print the options
            int maxKeyStringLength = 0;
            int nextSerialNumber = 1;
            
            // Loop once to build a dictionary and do some calculations
            Dictionary<string, MenuOption> optionDictionary = new Dictionary<string, MenuOption>(StringComparer.OrdinalIgnoreCase);
            foreach (MenuOption option in menuOptions)
            {
                // Figure out the KeyString for the item
                string keyString = option.KeyString ?? nextSerialNumber++.ToString();

                // Calculate the current Max length of all key strings
                maxKeyStringLength = Math.Max(maxKeyStringLength, keyString.Length);

                // Add the item to the dictionary (throw if duplicate)
                if (optionDictionary.ContainsKey(keyString))
                {
                    throw new Exception($"A duplicate menu key-string value was detected: {keyString} ");
                }
                optionDictionary.Add(keyString, option);
            }

            // Now loop the dictionary to print the menu 
            foreach(var kvp in optionDictionary)
            {
                string menuLine = $"{kvp.Key.PadLeft(maxKeyStringLength)}{config.KeyStringTextSeparator}{kvp.Value.Text}";
                if (selectedOption == kvp.Value)
                {
                    // Print the selected item in the configured color
                    Console.BackgroundColor = config.SelectedItemBackgroundColor;
                    Console.ForegroundColor = config.SelectedItemForegroundColor;
                    Console.WriteLine(menuLine);
                    Console.BackgroundColor = config.ItemBackgroundColor;
                    Console.ForegroundColor = config.ItemForegroundColor;

                }
                else
                {
                    Console.WriteLine(menuLine);
                }
            }

            // This is a hook where the derived class can print something under the menu text
            OnAfterShow();

            // Display the prompt (in the Selected Item color)
            Console.BackgroundColor = config.SelectedItemBackgroundColor;
            Console.ForegroundColor = config.SelectedItemForegroundColor;

            // Wait for the user to enter a command (make sure it's in the dictionary)
            string key = GetString(config.KeyStringPrompt, false, null, optionDictionary.Keys);

            Console.BackgroundColor = config.ItemBackgroundColor;
            Console.ForegroundColor = config.ItemForegroundColor;

            // Get the option the user selected
            return optionDictionary[key];
        }

        private MenuOption DrawMenuAndGetSelection_ArrowMode(MenuOption selectedOption)
        {
            int currentSelectionIndex = 0;
            if (selectedOption != null)
            {
                currentSelectionIndex = menuOptions.IndexOf(selectedOption);
                if (currentSelectionIndex < 0)
                {
                    currentSelectionIndex = 0;
                }
            }

            ConsoleKey key;

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

                    // This is a hook where the derived class can print something under the menu text
                    OnAfterShow();
                    Console.BackgroundColor = config.ItemBackgroundColor;
                    Console.ForegroundColor = config.ItemForegroundColor;

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

                // Let the user press a key but don't show it
                key = Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.Escape && config.CloseOnEscape)
                {
                    // Since we are exiting the method, we probably should re-show the cursor.
                    Console.CursorVisible = true;
                    return new MenuOption("", Close);
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
            return menuOptions[currentSelectionIndex];

        }

        /// <summary>
        /// Enables the ability to overwrite any of the default configuration settings.
        /// </summary>
        /// <param name="action">An Action that accepts a MenuConfig arugment.</param>
        /// <returns>Returns this menu, so that this method can be used in a method chain.</returns>
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

        /// <summary>
        /// Override this if specific code needs to execute after the menu is shown, but before user input is received
        /// </summary>
        protected virtual void OnAfterShow() 
        {
            Console.WriteLine();
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
        /// <param name="message">The string to prompt the user with.</param>
        /// <param name="defaultValue">An optional default value for the user to confirm.</param>
        /// <param name="allowableValues">An optional range of allowable values to validate against.</param>
        /// <returns>A valid integer entered by the user</returns>
        static public int GetInteger(string message, int? defaultValue = null, IEnumerable<int> allowableValues = null)
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
                    if (allowableValues != null && !allowableValues.Contains(resultValue))
                    {
                        Console.WriteLine($"!!! Invalid input. Must provide one of the following values {String.Join(", ", allowableValues)}");
                    }
                    else
                    {
                        break;
                    }
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
        /// <param name="message">The string to prompt the user with.</param>
        /// <param name="defaultValue">An optional default value for the user to confirm.</param>
        /// <param name="allowableValues">An optional range of allowable values to validate against.</param>
        /// <returns>A valid double entered by the user</returns>
        static public double GetDouble(string message, double? defaultValue = null, IEnumerable<double> allowableValues = null)
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
                    if (allowableValues != null && !allowableValues.Contains(resultValue))
                    {
                        Console.WriteLine($"!!! Invalid input. Must provide one of the following values {String.Join(", ", allowableValues)}");
                    }
                    else
                    {
                        break;
                    }
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
        /// <param name="message">The string to prompt the user with.</param>
        /// <param name="defaultValue">An optional default value for the user to confirm.</param>
        /// <param name="allowableValues">An optional range of allowable values to validate against.</param>
        /// <returns>A valid decimal entered by the user</returns>
        static public decimal GetDecimal(string message, decimal? defaultValue = null, IEnumerable<decimal> allowableValues = null)
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
                    if (allowableValues != null && !allowableValues.Contains(resultValue))
                    {
                        Console.WriteLine($"!!! Invalid input. Must provide one of the following values {String.Join(", ", allowableValues)}");
                    }
                    else
                    {
                        break;
                    }
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
        /// <param name="message">The string to prompt the user with.</param>
        /// <param name="defaultValue">An optional default value for the user to confirm.</param>
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
        /// <param name="message">The string to prompt the user with.</param>
        /// <param name="defaultValue">An optional default value for the user to confirm.</param>
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
        /// <param name="message">The string to prompt the user with.</param>
        /// <param name="allowEmptyString">If the user is allowed to provide no value (default false).</param>
        /// <param name="defaultValue">An optional default value for the user to confirm.</param>
        /// <param name="allowableValues">Collection of values to constrain for. If null, all values are allowed.</param>
        /// <returns>String entered by the user</returns>
        static public string GetString(string message, bool allowEmptyString = false, 
            string defaultValue = null, IEnumerable<string> allowableValues = null)
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

                if (userInput.Length == 0)
                {
                    if (allowEmptyString)
                    {
                        return userInput;
                    }
                    else if (!String.IsNullOrEmpty(defaultValue))
                    {
                        userInput = defaultValue;
                    }
                    else
                    {
                        Console.WriteLine("!!! Empty string is not allowed.");
                        continue;
                    }
                }

                if (allowableValues != null && !allowableValues.Any( v => v.ToLower() == userInput.ToLower()))
                {
                    Console.WriteLine($"!!! '{userInput}' is not in the list of valid values.");
                    continue;
                }

                // If the user pressed ENTER only, take the default
                return userInput;
            }
        }
        #endregion

        #region Change console colors

        /// <summary>
        /// Temporarily change the foreground color of the Console before a Write. To change the color back, 
        /// use ResetColor()
        /// </summary>
        /// <param name="foregroundColor">The new foreground color for the Console</param>
        protected void SetColor(ConsoleColor foregroundColor)
        {
            Console.ForegroundColor = foregroundColor;
        }

        /// <summary>
        /// After having called SetColor(color), this method resets the foreground color of the Console to the default value.
        /// </summary>
        protected void ResetColor()
        {
            Console.ForegroundColor = config.ItemForegroundColor;
        }
        #endregion
    }
}