using System;
using System.Collections.Generic;
using CardGame.Cui.Page;
using CardGame.Logic.Game;
using System.Threading;
using CardGame.Logic.Game.Responses;
using CardGame.Logic;

namespace CardGame.Cui
{
    internal static class UserInterface
    {
        public static void PlayCardAnimation(
            string playedCardName, 
            string playerName, 
            IPagePrinter pagePrinter, 
            List<PlayerDto> players, 
            List<Card> cardsOnTableCopy, 
            Card largestCardInRound)
        {
            // Play card animation.
            for (int i = 0; i < 7; i++)
            {
                var cardPlayedAnimationProperties = new CardPlayedAnimationProperties
                {
                    CardName = playedCardName,
                    CardPositionInLine = i,
                    PlayerName = playerName
                };
                pagePrinter.Print(
                    players,
                    cardsOnTableCopy,
                    playerName,
                    cardPlayedAnimationProperties: cardPlayedAnimationProperties,
                    largestCardInRound: largestCardInRound);
                Thread.Sleep(10);
            }
            Thread.Sleep(1000);
        }
    }
}
