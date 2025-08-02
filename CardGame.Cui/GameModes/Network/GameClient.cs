using System.Net.Sockets;

namespace CardGame.Cui.GameModes.Network
{
    internal class GameClient
    {
        public TcpClient TcpClient { get; set; }

        public int PlayerId { get; set; }
    }
}
