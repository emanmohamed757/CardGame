using System;
using System.Xml.Linq;

namespace CardGame.Logic
{
    public readonly struct Character
    {
        public Character(string identifier, int rank, bool isStartCharacter = false)
        {
            Identifier = identifier;
            Rank = rank;
            IsStartCharacter = isStartCharacter;
        }

        public string Identifier { get; }

        public int Rank { get; }

        public bool IsStartCharacter { get; }

        public override string ToString()
        {
            return Identifier.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is string symbolName)
            {
                return symbolName.Equals(Identifier, StringComparison.OrdinalIgnoreCase);
            }
            else if (obj is Character character)
            {
                return character.Identifier.Equals(Identifier, StringComparison.OrdinalIgnoreCase);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }

        public static bool operator ==(Character obj1, string obj2)
        {
            return obj1.Identifier.Equals(obj2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(Character obj1, string obj2)
        {
            return !obj1.Identifier.Equals(obj2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator ==(string obj2, Character obj1)
        {
            return obj1.Identifier.Equals(obj2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(string obj2, Character obj1)
        {
            return !obj1.Identifier.Equals(obj2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
