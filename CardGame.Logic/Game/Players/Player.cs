using System.Collections.Generic;
using System.Linq;

namespace CardGame.Logic.Game
{
    public abstract class Player
    {
        protected readonly List<Card> _hand;

        public Player(List<Card> hand, string name, bool isHuman = false)
        {
            _hand = hand;
            _hand.Sort(Comparer<Card>.Create((a, b) => a.Symbol.Name.CompareTo(b.Symbol.Name)));
            IsHuman = isHuman;
            Name = name;
        }

        public bool IsHuman { get; }

        public string Name { get; set; }

        // TODO: Check if this can be put into another class.
        public Player NextPlayer { get; set; }

        public Player PreviousPlayer { get; set; }

        public int Id { get; set; }

        public int CardCount => _hand.Count;

        public List<Card> GetHand()
        {
            return _hand.ToList();
        }

        public virtual void TakeCards(List<Card> cardsOnTable)
        {
            _hand.AddRange(cardsOnTable);
            cardsOnTable.Clear();
        }

        public bool IsFirstPlayer() =>
            _hand.Any(card => card.IsStartCard);

        public bool HasSymbol(Symbol symbol) =>
            _hand.Any(card => card.Symbol == symbol);

        /// <returns>The card if the player has it, else null.</returns>
        public Card GetCardIfAvailable(string symbolName, string characterIdentifier) =>
            _hand.FirstOrDefault(card => card.Character == characterIdentifier && card.Symbol == symbolName);

        public virtual void PlayCard(Card card, List<Card> cardsOnTable)
        {
            _hand.Remove(card);
            cardsOnTable.Add(card);
        }

        public bool HasEmptyHand() => _hand.Count == 0;

        public override bool Equals(object obj)
        {
            if (obj is Player player)
            {
                return player.Name.Equals(Name);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}