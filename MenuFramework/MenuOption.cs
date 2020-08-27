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
        public Action Command { get; }

        public bool? WaitAfterSelection { get; } = null;

        /// <summary>
        /// Creates a new menu option.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="command"></param>
        /// <param name="closeOnSelection"></param>        
        public MenuOption(string text, Action command, bool? waitAfterSelection = null)
        {
            Text = text;
            Command = command;
            WaitAfterSelection = waitAfterSelection;
        }
    }

    /// <summary>
    /// Represents a Menu Option with an object attached to it for "reactive text".
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MenuOption<T> : MenuOption
    {
        private T item;
        public MenuOption(Action command, T item, bool? waitAfterSelection = null) 
            : base (item.ToString(), command, waitAfterSelection) 
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
}