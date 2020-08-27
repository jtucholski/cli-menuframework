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

        /// <summary>
        /// Helper that can be added directly to a MenuOption to close (dismiss) the menu.
        /// </summary>
        /// <returns></returns>
        public MenuOptionResult Close()
        {
            return MenuOptionResult.CloseMenuAfterSelection;
        }

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
        public void Show()
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

                    if (key == ConsoleKey.Escape && config.CloseOnEscape)
                    {
                        // This is a HACK! It seems the ESC character somehow "swallows" the next character printed to the screen.  
                        // So I am printing a garbage character, never to be seen. Weird.
                        Console.WriteLine("X");
                        return;
                    }

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

                // Invoke the associated option
                MenuOptionResult result = selectedOption.Command();

                // Check Result and act accordingly

                // If the action returned Close
                if (result == MenuOptionResult.CloseMenuAfterSelection)
                {
                    return;
                }

                CheckForWait(result);
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

        private void CheckForWait(MenuOptionResult result)
        {
            // Start with the value configured in the menu
            bool wait = config.WaitAfterMenuSelection;

            // Check the result of the command for an override
            switch (result)
            {
                case MenuOptionResult.CloseMenuAfterSelection:
                case MenuOptionResult.DoNotWaitAfterMenuSelection:
                case MenuOptionResult.ExitAfterSelection:
                    wait = false;
                    break;

                case MenuOptionResult.WaitAfterMenuSelection:
                    wait = true;
                    break;
            }
            if (wait)
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