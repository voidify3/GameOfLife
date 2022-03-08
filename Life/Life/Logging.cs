using System;

namespace Life
{
    /// <summary>
    /// Some static methods for console messages. 
    /// Developed from provided part 1 solution
    /// (I added comments for consistency and renamed a parameter but nothing else is mine)
    /// </summary>
    /// <author>CAB201 teaching team</author>
    /// <date>September 2020</date>
    static class Logging
    {
        /// <summary>
        /// The current time with an accuracy of 1 millisecond
        /// </summary>
        private static string FormattedTime
        {
            get { return DateTime.Now.ToString("[hh:mm:ss:fff]"); }
        }

        /// <summary>
        /// Success message (green with the prefix Success)
        /// </summary>
        /// <param name="message">Message content</param>
        public static void Success(string message)
        {
            Message(message, "Success", ConsoleColor.Green);
        }

        /// <summary>
        /// Warning message (yellow with the prefix Warning)
        /// </summary>
        /// <param name="message">Message content</param>
        public static void Warning(string message)
        {
            Message(message, "Warning", ConsoleColor.Yellow);
        }

        /// <summary>
        /// Error message (red with the prefix Error)
        /// </summary>
        /// <param name="message">Message content</param>
        public static void Error(string message)
        {
            Message(message, "Error", ConsoleColor.Red);
        }

        /// <summary>
        /// Print a message to the console prefixed by the formatted time 
        /// Use the specified prefix and colour, or else no prefix and white
        /// </summary>
        /// <param name="message">The message to print</param>
        /// <param name="prefix">Message prefix (default null)</param>
        /// <param name="colour">Text colour (default white)</param>
        public static void Message(string message, string prefix = null,
            ConsoleColor colour = ConsoleColor.White)
        {
            Console.ForegroundColor = colour;
            Console.WriteLine($"{FormattedTime}{(prefix != null ? $" {prefix}: " : " ")}{message}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
