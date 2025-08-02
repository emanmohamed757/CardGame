using System.Collections.Generic;
using CardGame.Logic;
using CardGame.Logic.Game;

namespace CardGame.Cui.GameModes.Network
{
    internal class GameSessionDto
    {
        public List<PlayerDto> Players { get; set; }

        public List<Card> CardsOnTable { get; set; }

        public Card LargestCardInRound { get; set; }
    }
}
