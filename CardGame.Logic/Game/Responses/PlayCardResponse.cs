using System.Collections.Generic;

namespace CardGame.Logic.Game.Responses
{
    public class PlayCardResponse
    {
        public string BondiRecipientName { get; set; }

        public List<Card> BondiCards { get; set; }

        public List<string> NameOfPlayersWhoWon { get; set; }

        public string NameOfPlayerWhoLost { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsInvalidMove { get; set; }

        public bool GameEnded { get; set; }

        public Card PlayedCard { get; set; }

        public bool RoundEnded { get; set; }
    }
}
