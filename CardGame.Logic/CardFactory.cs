using System;
using System.Collections.Generic;
using System.Linq;
using CardGame.Logic.Helpers;

namespace CardGame.Logic
{
    public class CardFactory
    {
        public CardFactory()
        {
            var symbols = new[] 
            {
                new Symbol("Club", '♣'),
                new Symbol("Heart", '♥'),
                new Symbol("Diamond", '◆'),
                new Symbol("Spade", '♠', true),
            };

            var characters = new[]
            {
                new Character("2", 13),
                new Character("3", 12),
                new Character("4", 11),
                new Character("5", 10),
                new Character("6", 9),
                new Character("7", 8),
                new Character("8", 7),
                new Character("9", 6),
                new Character("10", 5),
                new Character("J", 4),
                new Character("Q", 3),
                new Character("K", 2),
                new Character("A", 1, true),
            };

            foreach (Symbol symbol in symbols)
            {
                foreach (Character character in characters)
                {
                    _cardDeck.Add(new Card(symbol, character));
                }
            }
        }

        private readonly List<Card> _cardDeck = new List<Card>();

        public List<List<Card>> GetHands(int numberOfPlayers)
        {
            var hands = new List<List<Card>>();
            for (int i = 0; i < numberOfPlayers; i++)
            {
                hands.Add(new List<Card>());
            }

            _cardDeck.Shuffle();

            int handIndex = 0;
            foreach (Card card in _cardDeck)
            {
                hands[handIndex].Add(card);
                handIndex = (handIndex + 1) % numberOfPlayers;
            }

            return hands;
        }
    }
}
