using CardGame.Cui.Page;
using System.Collections.Generic;
using System.Threading;
using System;
using CardGame.Cui.PlayerGathering;
using CardGame.Logic.Game.Responses;
using CardGame.Logic.Game;
using CardGame.Logic;
using System.Linq;
using CardGame.Cui.GameModes.Network;

namespace CardGame.Cui.GameModes.LocalMachine
{
    internal class LocalMachineGameMode
    {
        public void PlayGame()
        {
            PlayerSummary playerSummary = new PlayerGatheringService().GetLocalMachineGameModePlayerSummary();
            var gameSession = new GameSession(playerSummary.HumanCount, playerSummary.BotCount, playerSummary.HumanNames);

            IPagePrinter pagePrinter = new ColorPagePrinter();
            while (!gameSession.HasGameEnded)
            {
                NextPlayerResponse nextPlayer = gameSession.GetNextPlayer();
                PlayCardResponse playCard = null;
                ActionPrompt actionPrompt = nextPlayer.IsHuman ? ActionPrompt.PlayMove : ActionPrompt.CauseBotMove;
                pagePrinter.Print(
                    gameSession.Players.ToPlayerDtos(),
                    gameSession.CardsOnTable,
                    nextPlayer.PlayerName,
                    null,
                    null,
                    false,
                    actionPrompt,
                    largestCardInRound: gameSession.LargestCardInRound,
                    displayWhoseTurn: true);

                // For use in animations.
                List<Card> cardsOnTableCopy = gameSession.CardsOnTable.ToList();

                bool areCardsDisplayed = false;
                while (true) // For retrying played card validation
                {
                    if (nextPlayer.IsHuman)
                    {
                        while (true)
                        {
                            string input = Console.ReadLine();
                            if (input == "/c")
                            {
                                pagePrinter.Print(
                                    gameSession.Players.ToPlayerDtos(),
                                    gameSession.CardsOnTable,
                                    nextPlayer.PlayerName,
                                    null,
                                    null,
                                    false,
                                    actionPrompt,
                                    areCardsDisplayed ? null : nextPlayer.Hand,
                                    largestCardInRound: gameSession.LargestCardInRound,
                                    displayWhoseTurn: true);

                                areCardsDisplayed = !areCardsDisplayed;
                            }
                            else
                            {
                                string[] symbolAndCharacter = input.Split();
                                if (symbolAndCharacter.Length < 2)
                                {
                                    Console.WriteLine("Invalid card name. Try again!");
                                    continue;
                                }

                                playCard = gameSession.PlayCard(symbolAndCharacter[0], symbolAndCharacter[1]);
                                break;
                            }
                        }
                    }
                    else
                    {
                        Console.ReadLine(); // Wait for input before causing bot move.
                        playCard = gameSession.PlayCard();
                    }

                    if (playCard.IsInvalidMove)
                    {
                        Console.WriteLine(playCard.ErrorMessage);
                        continue;
                    }

                    break;
                }

                UserInterface.PlayCardAnimation(
                    playCard.PlayedCard.ToString(),
                    nextPlayer.PlayerName,
                    pagePrinter,
                    gameSession.Players.ToPlayerDtos(),
                    cardsOnTableCopy,
                    gameSession.LargestCardInRound);
                //// Play card animation.
                //for (int i = 0; i < 7; i++)
                //{
                //    var cardPlayedAnimationProperties = new CardPlayedAnimationProperties
                //    {
                //        CardName = playCard.PlayedCard.ToString(),
                //        CardPositionInLine = i,
                //        PlayerName = nextPlayer.PlayerName
                //    };
                //    pagePrinter.Print(
                //        gameSession.Players,
                //        cardsOnTableCopy,
                //        nextPlayer.PlayerName,
                //        cardPlayedAnimationProperties: cardPlayedAnimationProperties,
                //        largestCardInRound: gameSession.LargestCardInRound);
                //    Thread.Sleep(10);
                //}
                //Thread.Sleep(1000);

                // Bondi animation.
                if (playCard.BondiCards != null && playCard.BondiCards.Count > 0)
                {
                    cardsOnTableCopy.Add(playCard.PlayedCard);
                    int longestCardNameOnTable = cardsOnTableCopy.Select(card => card.ToString().Length).Max();
                    int animationDelay = 1000 / longestCardNameOnTable; // So that cards dissapear in exactly 1 second.
                    var bondiReceivedTextProperties = new BondiReceivedTextProperties
                    {
                        NumberOfCards = playCard.BondiCards.Count,
                        PlayerName = playCard.BondiRecipientName
                    };


                    for (int i = 0; i <= longestCardNameOnTable; i++)
                    {
                        pagePrinter.Print(
                            gameSession.Players.ToPlayerDtos(),
                            cardsOnTableCopy,
                            nextPlayer.PlayerName,
                            roundEnded: true,
                            cardsOnTableNamesNegativeXShift: i,
                            bondiReceivedTextProperties: bondiReceivedTextProperties,
                            namesOfPlayersWhoWon: playCard.NameOfPlayersWhoWon,
                            largestCardInRound: gameSession.LargestCardInRound);

                        if (i == 0)
                        {
                            Console.WriteLine(Environment.NewLine + "Press Enter to continue.");
                            Console.ReadLine();
                        }

                        Thread.Sleep(animationDelay);
                    }
                }
                else if (playCard.RoundEnded) // Round end animation.
                {
                    cardsOnTableCopy.Add(playCard.PlayedCard);
                    int longestCardNameOnTable = cardsOnTableCopy.Select(card => card.ToString().Length).Max();
                    int animationDelay = 1000 / longestCardNameOnTable; // So that cards dissapear in exactly 1 second.
                    for (int i = 0; i <= longestCardNameOnTable; i++)
                    {
                        pagePrinter.Print(
                            gameSession.Players.ToPlayerDtos(),
                            cardsOnTableCopy,
                            nextPlayer.PlayerName,
                            roundEnded: true,
                            cardsOnTableNamesNegativeXShift: i,
                            namesOfPlayersWhoWon: playCard.NameOfPlayersWhoWon,
                            largestCardInRound: gameSession.LargestCardInRound);
                        Thread.Sleep(animationDelay);
                    }

                    Thread.Sleep(1000 - animationDelay);
                }
            }
        }
    }
}
