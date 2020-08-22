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
        public string Text { get; }

        /// <summary>
        /// The command to invoke when the menu option is selected.
        /// </summary>
        /// <value></value>
        public Action Command { get; }

        /// <summary>
        /// Creates a new menu option.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="command"></param>
        public MenuOption(string text, Action command)
        {
            Text = text;
            Command = command;
        }
    }
}