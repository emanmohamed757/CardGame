using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardGame.Logic.Game;
using CardGame.Logic;

namespace CardGame.Cui.Page
{
    internal class PlainPagePrinter : IPagePrinter
    {
        private readonly string _twoNewLines = Environment.NewLine + Environment.NewLine;

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
            // TODO: "Bot: 56 cards" should not have line break.
            IEnumerable<string> cardCounts =
                players.Where(player => player.CardCount > 0).Select(player => $"{player.Name}: {player.CardCount} cards");
            string cardCountsFormatted = string.Join("     ", cardCounts);

            string cardsOnTableFormatted = cardsOnTable.Count > 0 ?
                _twoNewLines
                    + "Cards on Table:"
                    + Environment.NewLine
                    + string.Join(
                        Environment.NewLine,
                        cardsOnTable.Select(card =>
                        {
                            string cardName = card.ToString();
                            return cardName.Substring(Math.Min(cardName.Length, cardsOnTableNamesNegativeXShift));
                        }))
                : null;

            string whoseTurnText = displayWhoseTurn ? $"{_twoNewLines}{playerName}'s turn!" : null;

            string cardPlayedAnimation = null;
            if (cardPlayedAnimationProperties != null)
            {
                cardPlayedAnimation = $"{_twoNewLines}{cardPlayedAnimationProperties.PlayerName}:"
                    + new string(' ', cardPlayedAnimationProperties.CardPositionInLine)
                    + cardPlayedAnimationProperties.CardName;
            }

            string bondiReceivedText = null;
            if (bondiReceivedTextProperties != null)
            {
                bondiReceivedText = $"{_twoNewLines}Bondi received by "
                    + $"{bondiReceivedTextProperties.PlayerName} "
                    + $"({bondiReceivedTextProperties.NumberOfCards} cards)";
            }

            string roundEndedText = roundEnded ? $"{_twoNewLines}Round Ended!" : null;
            string namesOfPlayersWhoWonFormatted = namesOfPlayersWhoWon == null || namesOfPlayersWhoWon.Count() == 0 ?
                null
                : $"{_twoNewLines}Players who have won: " + string.Join(", ", namesOfPlayersWhoWon);

            string cardsInHandFormatted = cardsInHand == null ? null
                : _twoNewLines
                    + "Cards in Hand:"
                    + Environment.NewLine
                    + string.Join(Environment.NewLine, cardsInHand.Select(card => card.ToString()));

            string actionPromptText =
                actionPrompt == ActionPrompt.PlayMove ? $"{_twoNewLines}Choose card (Enter symbol and character seperated with a space)."
                    + $"{Environment.NewLine}Type /c to {(cardsInHand == null ? "view" : "hide")} cards in hand."
                : actionPrompt == ActionPrompt.CauseBotMove ? $"{_twoNewLines}Press Enter to cause bot to play their move."
                : null;

            ConsoleExtension.ClearConsoleAndWriteLine($"{Environment.NewLine}{cardCountsFormatted}"
                + $"{cardsOnTableFormatted}"
                + $"{whoseTurnText}"
                + $"{cardPlayedAnimation}"
                + $"{bondiReceivedText}"
                + $"{roundEndedText}"
                + $"{namesOfPlayersWhoWonFormatted}"
                + $"{cardsInHandFormatted}"
                + $"{actionPromptText}");
        }
    }
}

