using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CardGame.Logic;

namespace CardGame.Cui.GameModes.Network
{
    /// <summary>
    /// Informs clients whether they can exit the play card loop or if their move was invalid.
    /// </summary>
    internal class PlayCardDto
    {
        public bool IsInvalidMove { get; set; }

        public string ErrorMessage { get; set; }
    }
}
