using System.Collections.Generic;

namespace CardGame.Logic.Game
{
    internal class Human : Player
    {
        public Human(List<Card> hand, string name) : base(hand, name, true)
        {
        }

        public Human(List<Card> hand, string name, int id)
            : this(hand, name)
        {
            Id = id;
        }
    }
}
