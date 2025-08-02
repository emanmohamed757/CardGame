using System.Runtime.InteropServices;
using System;
using CardGame.Logic;
using CardGame.Logic.Game;
using CardGame.Logic.Game.Responses;

namespace CardGame.CommandLine
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Number of humans:");
            int humanCount = int.Parse(Console.ReadLine());

            var humanNames = new string[humanCount];
            Console.WriteLine("Enter names of humans:");
            for (int i = 0; i < humanCount; i++)
            {
                humanNames[i] = Console.ReadLine();
            }

            Console.WriteLine("Enter number of bots:");
            int botCount = int.Parse(Console.ReadLine());

            var game = new GameSession(humanCount, botCount, humanNames);

            Console.WriteLine("Players:");
            foreach (string name in game.GetPlayerNames())
            {
                Console.WriteLine(name);
            }

            while (!game.HasGameEnded)
            {
                NextPlayerResponse nextPlayer = game.GetNextPlayer();

                PlayCardResponse playCard = null;
                while (true)
                {
                    Console.WriteLine($"\n{nextPlayer.PlayerName}'s turn!");
                    if (nextPlayer.IsHuman)
                    {
                        Console.WriteLine("Your cards:");
                        nextPlayer.Hand.Sort(new CardComparer());
                        foreach (var card in nextPlayer.Hand)
                        {
                            Console.WriteLine(card);
                        }
                        Console.WriteLine("Enter symbol and character seperated with a space:");
                        string input = Console.ReadLine();
                        string[] symbolAndCharacter = input.Split();
                        playCard = game.PlayCard(symbolAndCharacter[0], symbolAndCharacter[1]);
                    }
                    else
                    {
                        Console.WriteLine("Press Enter for AI to make move.");
                        Console.ReadLine();
                        playCard = game.PlayCard();
                    }

                    if (playCard.IsInvalidMove)
                    {
                        Console.WriteLine(playCard.ErrorMessage);
                    }
                    else
                    {
                        Console.WriteLine($"Played card: {playCard.PlayedCard.Symbol} {playCard.PlayedCard.Character}");
                        if (playCard.BondiCards != null && playCard.BondiCards.Count > 0)
                        {
                            Console.WriteLine($"Bondi received by {playCard.BondiRecipientName}");
                        }

                        if (playCard.NameOfPlayersWhoWon != null && playCard.NameOfPlayersWhoWon.Count > 0)
                        {
                            Console.WriteLine("Below players have won:");
                            foreach (string player in playCard.NameOfPlayersWhoWon)
                            {
                                Console.WriteLine(player);
                            }
                        }
                        break;
                    }
                }

                if (playCard.RoundEnded)
                {
                    Console.WriteLine();
                    Console.WriteLine("Round Ended!");
                    Console.WriteLine();
                }
            }
        }
    }
}
