using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardGame.Logic;

namespace CardGame.CommandLine
{
    internal class CardComparer : Comparer<Card>
    {
        public override int Compare(Card x, Card y)
        {
            if (x.Symbol == y.Symbol)
            {
                return x.Character.Rank.CompareTo(y.Character.Rank);
            }

            return x.Symbol.Name.CompareTo(y.Symbol.Name);
        }
    }
}
