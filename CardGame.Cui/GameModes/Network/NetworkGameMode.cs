using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CardGame.Cui.Page;
using CardGame.Cui.PlayerGathering;
using CardGame.Logic.Game;
using CardGame.Logic.Game.Responses;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CardGame.Logic;

namespace CardGame.Cui.GameModes.Network
{
    internal class NetworkGameMode
    {
        public void RunAsServer()
        {
            IPAddress serverIPAddress = Dns.GetHostAddresses(Dns.GetHostName())
                .FirstOrDefault(iPAddress => iPAddress.AddressFamily == AddressFamily.InterNetwork); // IPv4 address.
            var server = new TcpListener(serverIPAddress, 0); // 0 means a random available port number.

            PlayerSummary playerSummary = new PlayerGatheringService().HostGame(server);
            List<GameClient> clients = playerSummary.Clients;
            string name = playerSummary.NameOfHost;
            ConsoleExtension.ClearConsole();

            var gameSession = new GameSession(playerSummary.HumanCount, playerSummary.BotCount, playerSummary.HumanNames, playerSummary.PlayerIds);

            IPagePrinter pagePrinter = new ColorPagePrinter(true);

            while (!gameSession.HasGameEnded)
            {
                PlayCardResponse playCard = null;
                NextPlayerResponse currentPlayer = gameSession.GetNextPlayer();
                clients.BroadcastObject(currentPlayer);

                // Send game session information.
                List<PlayerDto> playerDtos = gameSession.Players.ToPlayerDtos();
                var gameSessionInformation = new GameSessionDto
                {
                    CardsOnTable = gameSession.CardsOnTable,
                    LargestCardInRound = gameSession.LargestCardInRound,
                    Players = playerDtos
                };

                clients.BroadcastObject(gameSessionInformation);

                bool isHostCurrentPlayer = currentPlayer.PlayerId == 1; // Host always has hardcoded Id of 1.
                ActionPrompt actionPrompt = isHostCurrentPlayer ? ActionPrompt.PlayMove : ActionPrompt.NoActionPrompt;
                pagePrinter.Print(
                    playerDtos,
                    gameSession.CardsOnTable,
                    currentPlayer.PlayerName,
                    null,
                    null,
                    false,
                    actionPrompt,
                    isHostCurrentPlayer ? currentPlayer.Hand : null,
                    largestCardInRound: gameSession.LargestCardInRound,
                    displayWhoseTurn: true);

                // For use in animations.
                List<Card> cardsOnTableCopy = gameSession.CardsOnTable.ToList();

                TcpClient playerClient = null;
                while (true)
                {
                    if (isHostCurrentPlayer)
                    {
                        while (true)
                        {
                            string input = Console.ReadLine();
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
                    else if (!currentPlayer.IsHuman)
                    {
                        Thread.Sleep(1000);
                        playCard = gameSession.PlayCard();
                    }
                    else
                    {
                        if (playerClient == null)
                        {
                            playerClient = clients
                                .FirstOrDefault(client => client.PlayerId == currentPlayer.PlayerId)
                                .TcpClient;
                        }

                        string playedCard = playerClient.GetStream().ReadString();
                        string[] symbolAndCharacter = playedCard.Split();
                        playCard = gameSession.PlayCard(symbolAndCharacter[0], symbolAndCharacter[1]);
                    }

                    if (playCard.IsInvalidMove)
                    {
                        if (currentPlayer.PlayerId == 1)
                        {
                            Console.WriteLine(playCard.ErrorMessage);
                        }
                        else if (currentPlayer.IsHuman)
                        {
                            playerClient.GetStream().WriteObject(new PlayCardDto
                            {
                                IsInvalidMove = true,
                                ErrorMessage = playCard.ErrorMessage
                            });
                        }
                    }
                    else
                    {
                        // TODO: Is broadcast here necessary?
                        if (playerClient != null)
                        {
                            playerClient.GetStream().WriteObject(new PlayCardDto // TODO: Instead, send PlayCardResponse itself.
                            {
                                IsInvalidMove = false,
                            });
                        }

                        clients.Where(client => client.TcpClient != playerClient).BroadcastByte();
                        break;
                    }
                }

                List<PlayerDto> playerDtoList = gameSession.Players.ToPlayerDtos();
                clients.BroadcastObject(playCard);
                clients.BroadcastObject(new GameSessionDto
                {
                    CardsOnTable = cardsOnTableCopy,
                    LargestCardInRound = gameSession.LargestCardInRound,
                    Players = playerDtoList,
                });

                RunAnimations(
                    gameSession.LargestCardInRound,
                    pagePrinter,
                    playCard,
                    currentPlayer,
                    cardsOnTableCopy,
                    playerDtoList);
            }

            Console.ReadLine();
        }

        public void RunAsClient()
        {
            (TcpClient client, string name, int clientId)  = new PlayerGatheringService().JoinGame();
            NetworkStream stream = client.GetStream();
            ConsoleExtension.ClearConsole();

            IPagePrinter pagePrinter = new ColorPagePrinter(true);

            // Game loop.
            while (true)
            {
                var currentPlayer = stream.ReadObject<NextPlayerResponse>();
                var gameSessionInformation = stream.ReadObject<GameSessionDto>();

                bool isClientTheCurrentPlayer = currentPlayer.PlayerId == clientId;
                ActionPrompt actionPrompt = isClientTheCurrentPlayer ? ActionPrompt.PlayMove : ActionPrompt.NoActionPrompt;
                pagePrinter.Print(
                    gameSessionInformation.Players,
                    gameSessionInformation.CardsOnTable,
                    currentPlayer.PlayerName,
                    null,
                    null,
                    false,
                    actionPrompt,
                    isClientTheCurrentPlayer ? currentPlayer.Hand : null,
                    largestCardInRound: gameSessionInformation.LargestCardInRound,
                    displayWhoseTurn: true);

                // User input.
                while (true)
                {
                    if (isClientTheCurrentPlayer)
                    {
                        string input = Console.ReadLine();
                        string[] symbolAndCharacter = input.Split();
                        if (symbolAndCharacter.Length < 2)
                        {
                            Console.WriteLine("Invalid card name. Try again!");
                        }
                        else
                        {
                            stream.WriteString(input);
                            var playCardInformation = stream.ReadObject<PlayCardDto>();
                            if (!playCardInformation.IsInvalidMove) break;

                            Console.WriteLine(playCardInformation.ErrorMessage);
                        }
                    }
                    else
                    {
                        stream.ReadByte();
                        break;
                    }
                }

                PlayCardResponse playCard = stream.ReadObject<PlayCardResponse>();
                var gameSessionInfo = stream.ReadObject<GameSessionDto>();

                RunAnimations(
                    gameSessionInfo.LargestCardInRound,
                    pagePrinter,
                    playCard,
                    currentPlayer,
                    gameSessionInformation.CardsOnTable,
                    gameSessionInfo.Players);
            }

        }

        private static void RunAnimations(Card largestCardInRound, IPagePrinter pagePrinter, PlayCardResponse playCard, NextPlayerResponse currentPlayer, List<Card> cardsOnTableCopy, List<PlayerDto> playerDtoList)
        {
            UserInterface.PlayCardAnimation(
                                playCard.PlayedCard.ToString(),
                                currentPlayer.PlayerName,
                                pagePrinter,
                                playerDtoList,
                                cardsOnTableCopy,
                                largestCardInRound);

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
                        playerDtoList,
                        cardsOnTableCopy,
                        currentPlayer.PlayerName,
                        roundEnded: true,
                        cardsOnTableNamesNegativeXShift: i,
                        bondiReceivedTextProperties: bondiReceivedTextProperties,
                        namesOfPlayersWhoWon: playCard.NameOfPlayersWhoWon,
                        largestCardInRound: largestCardInRound);

                    if (i == 0)
                    {
                        Thread.Sleep(2000);
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
                        playerDtoList,
                        cardsOnTableCopy,
                        currentPlayer.PlayerName,
                        roundEnded: true,
                        cardsOnTableNamesNegativeXShift: i,
                        namesOfPlayersWhoWon: playCard.NameOfPlayersWhoWon,
                        largestCardInRound: largestCardInRound);
                    Thread.Sleep(animationDelay);
                }

                Thread.Sleep(1000 - animationDelay);
            }
        }
    }
}
