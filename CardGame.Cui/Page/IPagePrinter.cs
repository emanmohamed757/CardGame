using System.Collections.Generic;
using CardGame.Cui.GameModes.Network;
using CardGame.Logic;
using CardGame.Logic.Game;

namespace CardGame.Cui.Page
{
    internal interface IPagePrinter
    {
        void Print(
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
            bool displayWhoseTurn = false);
    }
}
