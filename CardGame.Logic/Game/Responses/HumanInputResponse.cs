namespace CardGame.Logic.Game.Responses
{
    public class HumanInputResponse : NextMoveResponse
    {
        public bool IsCorrectMove { get; set; }

        public string ErrorMessage { get; set; }
    }
}
