using System;
using System.Collections.Generic;
using System.Linq;
using CardGame.Logic.Enums;
using CardGame.Logic.Helpers;

namespace CardGame.Logic.Game
{
    internal class Bot : Player
    {
        private readonly BotMind _botMind;

        private readonly Dictionary<Symbol, int> _countBySymbol = new Dictionary<Symbol, int>();

        public Bot(List<Card> hand, string name, BotMind botMind)
            : base(hand, name)
        {
            _botMind = botMind;

            foreach (Card card in hand)
            {
                _countBySymbol.TryGetValue(card.Symbol, out int count);
                _countBySymbol[card.Symbol] = count + 1;
            }
        }

        public Card ChooseCard(Card largestCardOnTable, Player firstPlayerInRound, bool isFirstMovePlayed)
        {
            if (!isFirstMovePlayed)
            {
                return _hand.First(card => card.IsStartCard);
            }
            if (largestCardOnTable == null)
            {
                IEnumerable<Symbol> symbolsInHand = _countBySymbol.Keys
                    .Where(symbol => _countBySymbol[symbol] > 0)
                    .OrderBy(symbol => _countBySymbol[symbol]);

                foreach (Symbol symbol in symbolsInHand)
                {
                    IEnumerable<Card> sortedCardsOfSymbol = _hand
                    .Where(card => card.Symbol == symbol)
                    .OrderBy(card => card.Character.Rank);

                    foreach (Card cardBeingEvaluated in sortedCardsOfSymbol)
                    {
                        Player subsequentPlayer = NextPlayer;
                        bool isSafe = false;
                        CardViability viability = CardViability.Danger;

                        while (subsequentPlayer != firstPlayerInRound && !isSafe)
                        {
                            BotsPlayerView playerView = _botMind.GetPlayerView(subsequentPlayer.Name);

                            if (playerView.SymbolsNotInHand.Contains(cardBeingEvaluated.Symbol))
                            {
                                if (viability == CardViability.Danger
                                    || viability == CardViability.UnknownDanger)
                                {
                                    viability = CardViability.Bondi;
                                }
                                else
                                {
                                    viability = CardViability.UnknownBondi;
                                    break;
                                }
                            }
                            else if (playerView.CardsInHand
                                .Any(playerCard => playerCard.Symbol == cardBeingEvaluated.Symbol
                                    && playerCard.Character.Rank > cardBeingEvaluated.Character.Rank
                                    && viability != CardViability.UnknownSafe))
                            {
                                viability = CardViability.Danger;
                            }
                            // Cards in bot hand, cards not in player hand, cards passed
                            else if (PlayerHasNoHigherCards(cardBeingEvaluated, subsequentPlayer.Name)
                                && viability != CardViability.UnknownSafe)
                            {
                                viability = CardViability.UnknownDanger;
                            }
                            else if (PlayerHasNoLowerCards(cardBeingEvaluated, subsequentPlayer.Name))
                            {
                                //if (playerView.CardsInHand.Any(card => 
                                //    card.Symbol == cardBeingEvaluated.Symbol
                                //    && card.Character.Rank < cardBeingEvaluated.Character.Rank))
                                //{
                                //    viability = CardViability.Safe;
                                isSafe = true;
                                break;
                                //}

                                //viability = CardViability.UnknownSafe;
                            }

                            if (viability == CardViability.Bondi)
                            {
                                break;
                            }

                            subsequentPlayer = subsequentPlayer.NextPlayer;
                        }

                        if (viability != CardViability.Bondi)
                        {
                            return cardBeingEvaluated;
                        }
                    }
                }

                Console.WriteLine("Malfunction???"); // TODO: REMOVE.
                return _hand[0];
                // Bot is first player in round.
            }
            else if (!_countBySymbol.Keys.Contains(largestCardOnTable.Symbol) 
                || _countBySymbol[largestCardOnTable.Symbol] == 0)
            {
                // Bondi.
                Symbol leastNumerousSymbol = _countBySymbol.Keys
                    .Where(symbol => _countBySymbol[symbol] > 0)
                    .MinBy(symbol => _countBySymbol[symbol]);

                return _hand
                    .Where(card => card.Symbol == leastNumerousSymbol)
                    .MinBy(card => card.Character.Rank);
            }
            else
            {
                // Bot has to play the symbol on table.
                // Safe
                // UnknownSafe
                // Unknown
                // UnknownDanger
                // UnknownSafeBondi,
                // UnknownBondi,
                // UnknownDangerBondi,
                // Bondi
                IEnumerable<Card> sortedCardsOfSymbol = _hand
                    .Where(card => card.Symbol == largestCardOnTable.Symbol)
                    .OrderBy(card => card.Character.Rank);

                foreach (Card cardBeingEvaluated in sortedCardsOfSymbol)
                {
                    Player subsequentPlayer = NextPlayer;
                    bool isSafe = false;
                    CardViability viability = CardViability.Danger;

                    if (cardBeingEvaluated.Symbol == largestCardOnTable.Symbol
                        && cardBeingEvaluated.Character.Rank > largestCardOnTable.Character.Rank)
                    {
                        return cardBeingEvaluated;
                    }

                    while (subsequentPlayer != firstPlayerInRound && !isSafe)
                    {
                        BotsPlayerView playerView = _botMind.GetPlayerView(subsequentPlayer.Name);

                        if (playerView.SymbolsNotInHand.Contains(cardBeingEvaluated.Symbol))
                        {
                            if (viability == CardViability.Danger
                                || viability == CardViability.UnknownDanger)
                            {
                                viability = CardViability.Bondi;
                            }
                            else
                            {
                                viability = CardViability.UnknownBondi;
                                break;
                            }
                        }
                        else if (playerView.CardsInHand
                            .Any(playerCard => playerCard.Symbol == cardBeingEvaluated.Symbol
                                && playerCard.Character.Rank > cardBeingEvaluated.Character.Rank
                                && viability != CardViability.UnknownSafe))
                        {
                            viability = CardViability.Danger;
                        }
                        // Cards in bot hand, cards not in player hand, cards passed
                        else if (PlayerHasNoHigherCards(cardBeingEvaluated, subsequentPlayer.Name)
                            && viability != CardViability.UnknownSafe)
                        {
                            viability = CardViability.UnknownDanger;
                        }
                        else if (PlayerHasNoLowerCards(cardBeingEvaluated, subsequentPlayer.Name))
                        {
                            //if (playerView.CardsInHand.Any(card => 
                            //    card.Symbol == cardBeingEvaluated.Symbol
                            //    && card.Character.Rank < cardBeingEvaluated.Character.Rank))
                            //{
                            //    viability = CardViability.Safe;
                            isSafe = true;
                            break;
                            //}

                            //viability = CardViability.UnknownSafe;
                        }

                        subsequentPlayer = subsequentPlayer.NextPlayer;
                    }

                    if (viability != CardViability.Bondi)
                    {
                        return cardBeingEvaluated;
                    }
                }

                return sortedCardsOfSymbol.Last();
            }
        }

        public override void PlayCard(Card card, List<Card> cardsOnTable)
        {
            base.PlayCard(card, cardsOnTable);
            _countBySymbol[card.Symbol]--;
        }

        public override void TakeCards(List<Card> cardsOnTable)
        {
            foreach (Card card in cardsOnTable)
            {
                _hand.Add(card);
                _countBySymbol.TryGetValue(card.Symbol, out int count);
                _countBySymbol[card.Symbol] = count + 1;
            }

            cardsOnTable.Clear();
        }

        private bool PlayerHasNoHigherCards(Card cardBeingEvaluated, string playerName)
        {
            List<Card> cardsNotInPlayersHand = _botMind.CardsNotInPlayersHand(playerName, _hand, cardBeingEvaluated.Symbol);
            int possiblyOwnedHigherCardCount = cardBeingEvaluated.Character.Rank - 1;

            foreach (Card card in cardsNotInPlayersHand)
            {
                if (card.Character.Rank < cardBeingEvaluated.Character.Rank)
                {
                    if (--possiblyOwnedHigherCardCount == 0) return true;
                }

            }

            return false;
        }

        private bool PlayerHasNoLowerCards(Card cardBeingEvaluated, string playerName)
        {
            List<Card> cardsNotInPlayersHand = _botMind.CardsNotInPlayersHand(playerName, _hand, cardBeingEvaluated.Symbol);
            int possiblyOwnedLowerCardCount = 13 - cardBeingEvaluated.Character.Rank; // TODO: Magic number 13 should not be magic.

            foreach (Card card in cardsNotInPlayersHand)
            {
                if (card.Character.Rank > cardBeingEvaluated.Character.Rank)
                {
                    if (--possiblyOwnedLowerCardCount == 0) return true;
                }

            }

            return false;
        }

        // Decisions Bot will make:
        // Do I have the cardBeingEvaluated?
        //  Do I give a larger or smaller cardBeingEvaluated?
        // Which cardBeingEvaluated do I give as bondi?

        // Things Bot should consider:
        // Which players will give bondi for the cards I have?
        // Can I put another player at a disadvantage?

        // Internal representation of cards of all players and the cards they don't have.
        // Position of all players, including the player who will give bondi

        // Needs:
        // Number of players who have played in round
        // Cards on table
        // TrackBondi method with bondi recepient and deliverer details
    }
}
