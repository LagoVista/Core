// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: f6eb3605618f590adfbc4decd82c1e14e0ac05dead18e497a9ba23f3ca2af558
// IndexVersion: 1
// --- END CODE INDEX META ---
using System;

namespace LagoVista.Core.Diagnostics.ConsoleProxy
{
    public sealed class ConsoleWriter : IConsoleWriter
    {
        private readonly ConsoleColor _defaultForeground;
        private readonly ConsoleColor _warningForeground;
        private readonly ConsoleColor _errorForeground;

        public ConsoleWriter(ConsoleColor defaultForeground = ConsoleColor.Green, ConsoleColor warningForeground = ConsoleColor.Yellow, ConsoleColor errorForeground = ConsoleColor.Red)
        {
            _defaultForeground = defaultForeground;
            _warningForeground = warningForeground;
            _errorForeground = errorForeground;
        }

        public void WriteError(string message)
        {
            Console.ForegroundColor = _errorForeground;
            Console.WriteLine($"{DateTime.Now.ToString("T")}   {message}");
            Console.ResetColor();
        }

        public void WriteLine(string message)
        {
            Console.ForegroundColor = _defaultForeground;
            Console.WriteLine($"{DateTime.Now.ToString("T")}   {message}");
            Console.ResetColor();
        }

        public void WriteWarning(string message)
        {
            Console.ForegroundColor = _warningForeground;
            Console.WriteLine($"{DateTime.Now.ToString("T")}   {message}");
            Console.ResetColor();
        }
    }
}