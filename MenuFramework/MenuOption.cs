using System;

namespace MenuFramework
{
    /// <summary>
    /// Represents a single option in a menu.
    /// </summary>
    public class MenuOption
    {
        /// <summary>
        /// The text to display for this menu option.
        /// </summary>
        /// <value></value>
        public virtual string Text { get; }

        /// <summary>
        /// The command to invoke when the menu option is selected.
        /// </summary>
        /// <value></value>
        public Func<MenuOptionResult> Command { get; }

        /// <summary>
        /// Creates a new menu option.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="command"></param>
        public MenuOption(string text, Func<MenuOptionResult> command)
        {
            Text = text;
            Command = command;
        }
    }

    /// <summary>
    /// Represents a Menu Option with an object attached to it for "reactive text".
    /// </summary>
    /// <typeparam name="T">The Type of the attached item</typeparam>
    public class MenuOption<T> : MenuOption
    {
        private T item;
        private Func<T, string> getMenuText;
        /// <summary>
        /// Constructor for a selection on a menu
        /// </summary>
        /// <param name="command">The method to run when this item is selected</param>
        /// <param name="item">An item to pass into the "command" method</param>
        /// <param name="getMenuText">A method to call to get the display text for this option. If null, item.ToString is called.</param>
        public MenuOption(Func<MenuOptionResult> command, T item, Func<T, string> getMenuText = null)
            : base("", command)
        {
            this.item = item;
            this.getMenuText = getMenuText;
        }

        /// <summary>
        /// The text to display for this menu option.
        /// </summary>
        /// <value></value>
        public override string Text
        {
            get 
            {
                if (getMenuText == null) return item.ToString();
                return getMenuText(item);
            }
        }
    }

    /// <summary>
    /// All menu command methods return <see langword="abstract"/>MenuOptionResult informing the framework what to do next.
    /// </summary>
    public enum MenuOptionResult : int
    {
        /// <summary>
        /// The progam will pause after execution of the menu command, and the user will need to press any key to continue.
        /// </summary>
        WaitAfterMenuSelection,
        /// <summary>
        /// The progam will NOT pause after execution of the menu command. The menu will be re-drawn immediately after the command is executed
        /// </summary>
        DoNotWaitAfterMenuSelection,
        /// <summary>
        /// The menu containing the selected option will pause after execution of the command and then close.
        /// </summary>
        WaitThenCloseAfterSelection,
        /// <summary>
        /// The menu containing the selected option will close after execution of the command.
        /// </summary>
        CloseMenuAfterSelection,
        /// <summary>
        /// The menu, and all parent menus, will close after execution of the command. (This is not yet implemented so it works like Close)
        /// </summary>
        ExitAfterSelection
    }
}