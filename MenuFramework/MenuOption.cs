using System;

namespace MenuFramework
{
    /// <summary>
    /// Represents a single option in a menu.
    /// </summary>
    public class MenuOption
    {
        /// <summary>
        /// When in MenuSelectionMode.KeyString, the text of the string the user must type to select this option. Must be unique in the menu.
        /// </summary>3
        public virtual string KeyString { get; }

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
        /// <param name="keyString">The text the user needs to type to select this option, when in KeyString mode</param>
        public MenuOption(string text, Func<MenuOptionResult> command, string keyString = null)
        {
            Text = text;
            Command = command;
            KeyString = keyString;
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
        private Func<T, string> getKeyString;
        /// <summary>
        /// Constructor for a selection on a menu
        /// </summary>
        /// <param name="command">The method to run when this item is selected</param>
        /// <param name="item">An item to pass into the "command" method</param>
        /// <param name="getMenuText">A method to call to get the display text for this option. If null, item.ToString is called.</param>
        /// <param name="getKeyString">A method to call to get the key string to display if the menu is in KeyString mode. If null, the menu will auto-number.</param>
        public MenuOption(Func<MenuOptionResult> command, T item, Func<T, string> getMenuText = null, Func<T, string> getKeyString = null)
            : base("", command)
        {
            this.item = item;
            this.getMenuText = getMenuText;
            this.getKeyString = getKeyString;
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

        /// <summary>
        /// The text to display as the item Key String if the menu is in KeyString mode
        /// </summary>
        public override string KeyString
        {
            get
            {
                if (getKeyString == null) return null;  // menu will auto-number
                return getKeyString(item);
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