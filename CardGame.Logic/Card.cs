namespace CardGame.Logic
{
    public class Card
    {
        public Card(Symbol symbol, Character character)
        {
            Symbol = symbol;
            Character = character;
        }

        public Symbol Symbol { get; }

        public Character Character { get; }

        public bool IsStartCard => Symbol.IsStartSymbol && Character.IsStartCharacter;

        public override string ToString()
        {
            return $"{Symbol} {Character}";
        }

        public bool ValuesEquals(Card other)
        {
            return other != null && Character.Equals(other.Character) && Symbol.Equals(other.Symbol);
        }
    }
}
