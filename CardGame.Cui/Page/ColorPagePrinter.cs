using System;
using System.Collections.Generic;
using System.Linq;
using CardGame.Cui.GameModes.Network;
using CardGame.Logic;
using CardGame.Logic.Game;

namespace CardGame.Cui.Page
{
    internal class ColorPagePrinter : IPagePrinter
    {
        private readonly bool _isNetworkGameMode;

        public ColorPagePrinter(bool isNetworkGameMode = false)
        {
            _isNetworkGameMode = isNetworkGameMode;
        }

        public void Print(
            List<PlayerDto> players,
            List<Card> cardsOnTable,
            string playerName,
            CardPlayedAnimationProperties cardPlayedAnimationProperties = null,
            BondiReceivedTextProperties bondiReceivedTextProperties = null,
            bool roundEnded = false,
            ActionPrompt actionPrompt = ActionPrompt.NoActionPrompt,
            List<Card> cardsInHand = null,
            int cardsOnTableNamesNegativeXShift = 0,
            IEnumerable<string> namesOfPlayersWhoWon = null,
            Card largestCardInRound = null,
            bool displayWhoseTurn = false)
        {
            ConsoleExtension.ClearConsole();
            Console.WriteLine();

            players = players.Where(player => player.CardCount > 0).ToList();

            // Player names and card counts.
            List<string> cardCounts = players
                .Select(player => $"{player.Name}: {player.CardCount} cards")
                .ToList();
            for (int i = 0; i < cardCounts.Count; i++)
            {
                bool isCurrentPlayer = players[i].Name == playerName;
                bool isBondiReceivedPlayer = bondiReceivedTextProperties != null 
                    && players[i].Name == bondiReceivedTextProperties.PlayerName;
                if (isCurrentPlayer)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
                else if (isBondiReceivedPlayer)
                {
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.Write(cardCounts[i]);
                if (isCurrentPlayer || isBondiReceivedPlayer) Console.ResetColor();

                if (i != cardCounts.Count - 1) Console.Write("     ");
            }

            Console.WriteLine();

            // Cards on table.
            if (cardsOnTable != null && cardsOnTable.Count > 0)
            {
                Console.WriteLine(Environment.NewLine + "Cards on Table:");
                foreach (Card card in cardsOnTable)
                {
                    if (card.ValuesEquals(largestCardInRound)) Console.ForegroundColor = ConsoleColor.Red;

                    string cardName = card.ToString();
                    string cardNameFormatted = cardName.Substring(Math.Min(cardName.Length, cardsOnTableNamesNegativeXShift));
                    Console.WriteLine(cardNameFormatted);

                    if (card.ValuesEquals(largestCardInRound)) Console.ResetColor();
                }
            }

            // Whose turn.
            if (displayWhoseTurn) Console.WriteLine($"{Environment.NewLine}{playerName}'s turn!");

            // Card play animation.
            if (cardPlayedAnimationProperties != null)
            {
                Console.WriteLine($"{Environment.NewLine}{cardPlayedAnimationProperties.PlayerName}:"
                    + new string(' ', cardPlayedAnimationProperties.CardPositionInLine)
                    + cardPlayedAnimationProperties.CardName);
            }

            // Bondi text.
            if (bondiReceivedTextProperties != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{Environment.NewLine}Bondi received by "
                    + $"{bondiReceivedTextProperties.PlayerName} "
                    + $"({bondiReceivedTextProperties.NumberOfCards} cards)");
                Console.ResetColor();
            }

            // Round ended text.
            if (roundEnded)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{Environment.NewLine}Round Ended!");
                Console.ResetColor();
            }

            // Players who have won text.
            if (namesOfPlayersWhoWon != null && namesOfPlayersWhoWon.Count() > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{Environment.NewLine}Players who have won: " + string.Join(", ", namesOfPlayersWhoWon));
                Console.ResetColor();
            }
            
            // Cards in hand.
            if (cardsInHand != null)
            {
                Console.WriteLine(Environment.NewLine
                    + "Cards in Hand:"
                    + Environment.NewLine
                    + string.Join(Environment.NewLine, cardsInHand.Select(card => card.ToString())));
            }

            // Action prompts (single player)
            if (actionPrompt == ActionPrompt.PlayMove)
            {
                Console.WriteLine($"{Environment.NewLine}Choose card (Enter symbol and character seperated with a space)."
                    + (_isNetworkGameMode ? string.Empty : $"{Environment.NewLine}Type /c to {(cardsInHand == null ? "view" : "hide")} cards in hand."));
            }
            else if (actionPrompt == ActionPrompt.CauseBotMove)
            {
                Console.WriteLine($"{Environment.NewLine}Press Enter to cause bot to play their move.");
            }
        }
    }
}
