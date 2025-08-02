using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CardGame.Cui.GameModes;
using CardGame.Cui.GameModes.LocalMachine;
using CardGame.Cui.GameModes.Network;
using CardGame.Cui.Page;
using CardGame.Cui.PlayerGathering;
using CardGame.Logic;
using CardGame.Logic.Game;
using CardGame.Logic.Game.Responses;

namespace CardGame.Cui
{
    // Number of cards
    // PlayerName: Number of cards, 5 spaces
    // Cards on table
    // Card name
    // (Player: card played, animated
    // (Bondi received by PlayerName (Number of cards)
    // OR
    // Round Ended!)
    // (Prompt: Press enter to cause bot to play their move.
    // OR
    // Prompt: Choose card (type /c to view cards in hand)

    // Need all player's hands for card count.
    // Need cards on table on GameSession

    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            // TODO: Feedback on validation failure.
            bool isNetworkGameMode = IsNetworkGameMode();
            if (isNetworkGameMode)
            {
                Console.WriteLine("\n1. Host Game");
                Console.WriteLine("2. Join Game");
                string input = UserInput.ReadDigitInRange(1, 2);

                if (input == "1") new NetworkGameMode().RunAsServer();
                else new NetworkGameMode().RunAsClient();
            }
            else
            {
                new LocalMachineGameMode().PlayGame();
            }
        }

        private static bool IsNetworkGameMode()
        {
            Console.WriteLine("Choose the game mode.");
            Console.WriteLine("1. Local Machine");
            Console.WriteLine("2. Network");

            string userInput = UserInput.ReadDigitInRange(1, 2);

            return userInput[0] == '2';
        }

    }
}
