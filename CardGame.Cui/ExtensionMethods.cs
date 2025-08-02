using System.Collections.Generic;
using System.Linq;
using CardGame.Cui.GameModes.Network;
using CardGame.Logic.Game;

namespace CardGame.Cui
{
    internal static class ExtensionMethods
    {
        public static List<PlayerDto> ToPlayerDtos(this IEnumerable<Player> players) =>
            players
            .Select(player => new PlayerDto { Name = player.Name, CardCount = player.CardCount })
            .ToList();
    }
}
