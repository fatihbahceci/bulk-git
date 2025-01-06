using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bulk_git
{
    internal class CConsole : IDisposable
    {
        private ConsoleColor defaultForegroundColor;
        private ConsoleColor defaultBackgroundColor;

        /// <summary>
        /// Create a new instance of CConsole
        /// </summary>
        /// <returns></returns>
        public static CConsole i() => new CConsole();
        /// <summary>
        /// Create a new instance of CConsole <br />
        /// Default foreground color is set to the last specified color of the console <br />
        /// Default background color is set to the last specified color of the console
        /// </summary>
        public CConsole()
        {
            this.defaultForegroundColor = Console.ForegroundColor;
            this.defaultBackgroundColor = Console.BackgroundColor;
        }
        /// <summary>
        /// Create a new instance of CConsole
        /// </summary>
        /// <param name="defaultForegroundColor"> Default foreground color </param>
        /// <param name="defaultBackgroundColor"> Default background color </param>
        public CConsole(ConsoleColor defaultForegroundColor, ConsoleColor defaultBackgroundColor)
        {
            this.defaultForegroundColor = defaultForegroundColor;
            this.defaultBackgroundColor = defaultBackgroundColor;
        }
        /// <summary>
        /// Set the foreground color of the console
        /// </summary>
        /// <param name="foreground"></param>
        /// <returns></returns>
        public CConsole f(ConsoleColor foreground)
        {
            Console.ForegroundColor = foreground;
            return this;
        }
        /// <summary>
        /// Set the background color of the console
        /// </summary>
        /// <param name="background"></param>
        /// <returns></returns>
        public CConsole b(ConsoleColor background)
        {
            Console.BackgroundColor = background;
            return this;
        }
        /// <summary>
        /// Set the foreground and background color of the console
        /// </summary>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public CConsole c(ConsoleColor foreground, ConsoleColor background)
        {
            Console.ForegroundColor = foreground;
            Console.BackgroundColor = background;
            return this;
        }
        /// <summary>
        /// Reset the foreground and background color of the console to the default color that specified in the constructor
        /// </summary>
        /// <returns></returns>
        public CConsole r()
        {
            Console.ForegroundColor = defaultForegroundColor;
            Console.BackgroundColor = defaultBackgroundColor;
            return this;
        }
        /// <summary>
        /// Write a string to the console with the specified foreground and background color
        /// </summary>
        /// <param name="s"></param>
        /// <param name="foreground"> Set the foreground color of the console to this color permanently before writing the string </param>
        /// <param name="background"> Set the background color of the console to this color permanently before writing the string </param>
        /// <returns></returns>
        public CConsole wch(string s, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground != null)
                f(foreground.Value);
            if (background != null)
                b(background.Value);
            Console.Write(s);
            return this;
        }
        /// <summary>
        /// Write a string to the console with new line and the specified foreground and background color
        /// </summary>
        /// <param name="s"></param>
        /// <param name="foreground"> Set the foreground color of the console to this color permanently before writing the string </param>
        /// <param name="background"> Set the background color of the console to this color permanently before writing the string </param>
        /// <returns></returns>
        public CConsole wln(string s, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            if (foreground != null)
                f(foreground.Value);
            if (background != null)
                b(background.Value);
            Console.WriteLine(s);
            return this;
        }
        /// <summary>
        /// Write a new line to the console
        /// </summary>
        /// <returns></returns>
        public CConsole ln()
        {
            Console.WriteLine();
            return this;
        }
        /// <summary>
        /// Write a string to the console with the specified foreground and background color and reset the color to the default color after writing the string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public CConsole wcr(string s, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            wch(s, foreground, background);
            return r();
        }
        /// <summary>
        /// Write a string to the console with new line and the specified foreground and background color and reset the color to the default color after writing the string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="foreground"></param>
        /// <param name="background"></param>
        /// <returns></returns>
        public CConsole wlr(string s, ConsoleColor? foreground = null, ConsoleColor? background = null)
        {
            wln(s, foreground, background);
            return r();
        }
        /// <summary>
        /// Write a new line to the console and reset the color to the default color after writing the new line
        /// </summary>
        /// <returns></returns>
        public CConsole lnr()
        {
            ln();
            return r();
        }

        public void Dispose()
        {
            r();
        }
    }
}
