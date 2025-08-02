using System;
using System.Text;

namespace CardGame.Cui
{
    internal static class ConsoleExtension
    {
        public static void ClearConsoleAndWriteLine(string value)
        {
            ClearConsole();
            Console.WriteLine(value);
        }

        public static void ClearConsole()
        {
            Console.SetCursorPosition(0, 0);
            
            string emptyLine = new string(' ', Console.WindowWidth) + '\n';
            var stringBuilder = new StringBuilder();
            for (int i = 0; i < Console.WindowHeight; i++)
            {
                stringBuilder.Append(emptyLine);
            }

            Console.WriteLine(stringBuilder.ToString());
            Console.SetCursorPosition(0, 0);
        }
    }
}
