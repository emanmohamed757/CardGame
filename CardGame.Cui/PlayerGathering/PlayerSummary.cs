using System.Collections.Generic;
using System.Net.Sockets;
using CardGame.Cui.GameModes.Network;

namespace CardGame.Cui.PlayerGathering
{
    internal class PlayerSummary
    {
        public string NameOfHost { get; set; }

        public int BotCount { get; set; }

        public int HumanCount { get; set; }

        public IList<string> HumanNames { get; set; }

        public List<GameClient> Clients { get; set; }

        public List<int> PlayerIds { get; set; }
    }
}
