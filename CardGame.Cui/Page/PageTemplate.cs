using System;
using System.Collections.Generic;
using System.Linq;
using CardGame.Logic;
using CardGame.Logic.Game;

namespace CardGame.Cui.Page
{
    internal class PageTemplate
    {
        private readonly string _newLine = Environment.NewLine + Environment.NewLine;

        public string GeneratePage(
            IEnumerable<Player> players,
            List<Card> cardsOnTable,
            string playerName = null,
            CardPlayedAnimationProperties cardPlayedAnimationProperties = null,
            BondiReceivedTextProperties bondiReceivedTextProperties = null,
            bool roundEnded = false,
            ActionPrompt actionPrompt = ActionPrompt.NoActionPrompt,
            List<Card> cardsInHand = null,
            int cardsOnTableNamesNegativeXShift = 0,
            IEnumerable<string> namesOfPlayersWhoWon = null)
        {
            // TODO: "Agent: 56 cards" should not have line break.
            IEnumerable<string> cardCounts = 
                players.Where(player => player.CardCount > 0).Select(player => $"{player.Name}: {player.CardCount} cards");
            string cardCountsFormatted = string.Join("     ", cardCounts);

            string cardsOnTableFormatted = cardsOnTable.Count > 0 ?
                _newLine
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

            string whoseTurnText = playerName == null ? null : $"{_newLine}{playerName}'s turn!";

            string cardPlayedAnimation = null;
            if (cardPlayedAnimationProperties != null)
            {
                cardPlayedAnimation = $"{_newLine}{cardPlayedAnimationProperties.PlayerName}:"
                    + new string(' ', cardPlayedAnimationProperties.CardPositionInLine)
                    + cardPlayedAnimationProperties.CardName;
            }

            string bondiReceivedText = null;
            if (bondiReceivedTextProperties != null)
            {
                bondiReceivedText = $"{_newLine}Bondi received by "
                    + $"{bondiReceivedTextProperties.PlayerName} "
                    + $"({bondiReceivedTextProperties.NumberOfCards} cards)";
            }

            string roundEndedText = roundEnded ? $"{_newLine}Round Ended!" : null;
            string namesOfPlayersWhoWonFormatted = namesOfPlayersWhoWon == null || namesOfPlayersWhoWon.Count() == 0 ?
                null
                : $"{_newLine}Players who have won: " + string.Join(", ", namesOfPlayersWhoWon);

            string cardsInHandFormatted = cardsInHand == null ? null 
                : _newLine 
                    + "Cards in Hand:"
                    + Environment.NewLine
                    + string.Join(Environment.NewLine, cardsInHand.Select(card => card.ToString()));

            string actionPromptText = 
                actionPrompt == ActionPrompt.PlayMove ? $"{_newLine}Choose card (Enter symbol and character seperated with a space)."
                    + $"{Environment.NewLine}Type /c to {(cardsInHand == null ? "view" : "hide")} cards in hand."
                : actionPrompt == ActionPrompt.CauseAgentMove ? $"{_newLine}Press Enter to cause agent to play their move."
                : null;

            return $"{Environment.NewLine}{cardCountsFormatted}"
                + $"{cardsOnTableFormatted}"
                + $"{whoseTurnText}"
                + $"{cardPlayedAnimation}"
                + $"{bondiReceivedText}"
                + $"{roundEndedText}"
                + $"{namesOfPlayersWhoWonFormatted}"
                + $"{cardsInHandFormatted}"
                + $"{actionPromptText}";
        }
    }
}
