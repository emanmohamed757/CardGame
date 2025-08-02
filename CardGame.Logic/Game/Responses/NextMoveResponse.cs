namespace CardGame.Logic.Game.Responses
{
    public class NextMoveResponse
    {
        public Card CardPlayed { get; set; }

        public string PlayerName { get; set; }

        public bool InputRequired => CardPlayed == null;

        public PlayCardResponse BondiResponse { get; set; }
    }
}
