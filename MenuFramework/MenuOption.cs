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
        /// <param name="closeOnSelection"></param>        
        public MenuOption(string text, Func<MenuOptionResult> command)
        {
            Text = text;
            Command = command;
        }
    }

    /// <summary>
    /// Represents a Menu Option with an object attached to it for "reactive text".
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MenuOption<T> : MenuOption
    {
        private T item;
        public MenuOption(Func<MenuOptionResult> command, T item) 
            : base (item.ToString(), command) 
        { 
            this.item = item;
        }

        /// <summary>
        /// The text to display for this menu option.
        /// </summary>
        /// <value></value>
        public override string Text
        {
            get { return item.ToString(); }
        }
    }

    /// <summary>
    /// All menu command methods return <see langword="abstract"/>MenuOptionResult informing the framework what to do next.
    /// </summary>
    public enum MenuOptionResult : int
    {
        /// <summary>
        /// The configuration of the overall menu will decide whether there will be a pause after selection.
        /// </summary>
        Default = 0,
        /// <summary>
        /// The progam will pause after execution of the menu command, and the user will need to press any key to continue.
        /// </summary>
        WaitAfterMenuSelection,
        /// <summary>
        /// The progam will NOT pause after execution of the menu command. The menu will be re-drawn immediately after the command is executed
        /// </summary>
        DoNotWaitAfterMenuSelection,
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