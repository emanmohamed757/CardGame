using System;
using System.Linq;

namespace CardGame.Cui
{
    internal static class UserInput
    {

        public static string ReadDigitInRange(int start, int end)
        {
            while (true)
            {
                string input = Console.ReadLine().Trim();   
                for (int i = start; i <= end; i++)
                {
                    string number = i.ToString();
                    if (input == number || input == number + ".") return input;

                }

                Console.WriteLine($"Enter either {string.Join(" or ", Enumerable.Range(start, end).Select(num => $"\"{num}\""))}.");
            }
        }
    }
}
