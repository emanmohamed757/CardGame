using System.Collections.Generic;

namespace CardGame.Logic.Game
{
    internal class BotsPlayerView
    {
        public List<Card> CardsInHand { get; set; } = new List<Card>();

        public List<Card> CardsNotInHand { get; set; } = new List<Card>();

        public List<Symbol> SymbolsNotInHand { get; set; } = new List<Symbol>();
    }
}
