using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using CardGame.Cui.GameModes.Network;

namespace CardGame.Cui.PlayerGathering
{
    internal class PlayerGatheringService
    {
        private static readonly List<GameClient> _clients = new List<GameClient>();
        private static readonly List<string> _playerNames = new List<string>();
        private static readonly List<int> _playerIds = new List<int> { 1 };

        public PlayerSummary GetLocalMachineGameModePlayerSummary()
        {
            int humanCount;
            Console.WriteLine("Enter number of humans:");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out humanCount)) break;

                Console.WriteLine("Invalid number. Try again!");
            }

            var humanNames = new string[humanCount];
            Console.WriteLine("\nEnter names of humans (one by one):");
            for (int i = 0; i < humanCount; i++)
            {
                string name;
                while (true)
                {
                    Console.Write($"{i + 1}. ");
                    name = Console.ReadLine();
                    if (name != string.Empty) break;

                    Console.WriteLine("Player name must have at least one character. Try again!");
                }
                humanNames[i] = name;
            }

            int botCount;
            Console.WriteLine("\nEnter number of bots:");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out botCount)) break;

                Console.WriteLine("Invalid number. Try again!");
            }

            return new PlayerSummary
            {
                BotCount = botCount,
                HumanCount = humanCount,
                HumanNames = humanNames
            };
        }

        // TODO: Need to save player ID's.
        public PlayerSummary HostGame(TcpListener tcpListener)
        {
            ConsoleExtension.ClearConsole();
            Console.WriteLine("Enter your name:");
            string name;
            while (true)
            {
                name = Console.ReadLine();
                if (name != string.Empty) break;

                Console.WriteLine("Player name must contain at least one character.");
            }
            _playerNames.Add(name);

            tcpListener.Start();
            Clipboard.SetText(tcpListener.LocalEndpoint.ToString());
            Console.WriteLine($"\nSocket address game is hosted on:\n{tcpListener.LocalEndpoint}\nIt has been copied to your clipboard.\nPlayers must enter this address to join the game.");
            Console.WriteLine("\n\nWaiting for players...\nPress [Enter] to stop accepting players\n");

            var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;
            Task.Run(() => AcceptPlayers(tcpListener, cancellationToken, name));

            Console.ReadLine();
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            tcpListener.Stop();

            Console.WriteLine("Entry closed.");
            _clients.BroadcastString($"{_clients.Count + 1} players are here. Please wait while the host adds optional bots.");

            int botCount;
            Console.WriteLine("\nEnter the number of bots:");
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out botCount)) break;

                Console.WriteLine("Invalid number. Try again!");
            }

            _clients.BroadcastByte();

            return new PlayerSummary
            {
                BotCount = botCount,
                Clients = _clients,
                HumanCount = _clients.Count + 1,
                HumanNames = _playerNames, // TODO: Can this conversion be removed?
                PlayerIds = _playerIds
            };
        }

        public (TcpClient, string nameOfClient, int clientId) JoinGame()
        {
            ConsoleExtension.ClearConsole();
            Console.WriteLine("Enter your name:");
            string name;
            while (true)
            {
                name = Console.ReadLine();
                if (name != string.Empty) break;

                Console.WriteLine("Player name must contain at least one character.");
            }

            TcpClient tcpClient = new TcpClient(); ;
            Console.WriteLine("\nEnter host address:");
            while (true)
            {
                string hostAddress = Console.ReadLine();
                string[] hostAddressParts = hostAddress.Split(':');
                if (hostAddressParts.Length == 2)
                {
                    IPAddress serverIPAddress = Dns.GetHostAddresses(hostAddressParts[0])[0];
                    int port = int.Parse(hostAddressParts[1]);
                    //if (!int.TryParse(hostAddressParts[1], out int port)
                    //    || !IPAddress.TryParse(hostAddressParts[0], out IPAddress serverIPAddress))
                    //{
                    //    Console.WriteLine("Invalid address. Try again!");
                    //    continue;
                    //}

                    // TODO: When server closes entry, client must be properly informed.
                    tcpClient.Connect(new IPEndPoint(serverIPAddress, port));
                    break;
                }
            }

            NetworkStream stream = tcpClient.GetStream();
            string hostName = stream.ReadString();
            stream.WriteString(name);
            int clientId = stream.ReadObject<int>();
            Console.WriteLine($"\nConnected to game hosted by {hostName}.\nWaiting for other players to join...");

            Console.WriteLine("\n" + stream.ReadString()); // Here the server mentions that entry is closed.
            stream.ReadByte();
            return (tcpClient, name, clientId);
        }

        private static void AcceptPlayers(TcpListener server, CancellationToken cancellationToken, string hostName)
        {
            while (true)
            {
                Task<TcpClient> task = server.AcceptTcpClientAsync();

                while (true)
                {
                    if (cancellationToken.IsCancellationRequested) break;
                    if (task.IsCompleted)
                    {
                        NetworkStream stream = task.Result.GetStream();
                        stream.WriteString(hostName);
                        string name = stream.ReadString();

                        int clientId = _clients.Count + 2;
                        var gameClient = new GameClient
                        {
                            PlayerId = clientId,
                            TcpClient = task.Result
                        };
                        _clients.Add(gameClient);
                        _playerNames.Add(name);
                        _playerIds.Add(clientId);

                        // Give client their Id.
                        stream.WriteObject(clientId);

                        Console.WriteLine(name + " joined. " + task.Result.Client.RemoteEndPoint);

                        break;
                    }

                    Thread.Sleep(100);
                }

                if (cancellationToken.IsCancellationRequested) return;
            }
        }
    }
}
