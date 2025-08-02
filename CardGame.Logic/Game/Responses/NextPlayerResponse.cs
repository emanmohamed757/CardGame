using System.Collections.Generic;

namespace CardGame.Logic.Game.Responses
{
    public class NextPlayerResponse
    {
        public string PlayerName { get; set; }

        public bool IsHuman { get; set; }

        public int PlayerId { get; set; }

        public List<Card> Hand { get; set; }
    }
}
