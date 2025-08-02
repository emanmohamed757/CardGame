using System;

namespace CardGame.Logic
{
    public readonly struct Symbol
    {
        public Symbol(string name, char icon, bool isStartSymbol = false)
        {
            Name = name;
            Icon = icon;
            IsStartSymbol = isStartSymbol;
        }

        public string Name { get; }

        public char Icon { get; }

        public bool IsStartSymbol { get; }

        public override string ToString()
        {
            return Name.ToString();
        }

        public override bool Equals(object obj)
        {
            if (obj is string symbolName)
            {
                return symbolName.Equals(Name, StringComparison.OrdinalIgnoreCase);
            }
            else if (obj is Symbol symbol)
            {
                return symbol.Name.Equals(Name, StringComparison.OrdinalIgnoreCase);
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public static bool operator ==(Symbol obj1, Symbol obj2)
        {
            return obj1.Name == obj2.Name;
        }
        public static bool operator !=(Symbol obj1, Symbol obj2)
        {
            return obj1.Name != obj2.Name;
        }


        public static bool operator ==(Symbol obj1, string obj2)
        {
            return obj1.Name.Equals(obj2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(Symbol obj1, string obj2)
        {
            return !obj1.Name.Equals(obj2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator ==(string obj2, Symbol obj1)
        {
            return obj1.Name.Equals(obj2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool operator !=(string obj2, Symbol obj1)
        {
            return !obj1.Name.Equals(obj2, StringComparison.OrdinalIgnoreCase);
        }
    }
}
