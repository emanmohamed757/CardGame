using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;

namespace CardGame.Cui.GameModes.Network
{
    internal static class ExtensionMethods
    {
        public static string ReadString(this NetworkStream stream)
        {
            byte[] lengthPrefix = new byte[4];
            stream.Read(lengthPrefix, 0, lengthPrefix.Length);
            int totalLength = BitConverter.ToInt32(lengthPrefix, 0);

            var buffer = new byte[totalLength];
            int bytesRead = 0;

            while (bytesRead < totalLength)
            {
                int read = stream.Read(buffer, bytesRead, totalLength - bytesRead);
                if (read == 0) break; // Connection closed.
                bytesRead += read;
            }

            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        public static void WriteString(this NetworkStream stream, string value)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(value);
            byte[] lengthPrefix = BitConverter.GetBytes(stringBytes.Length);

            stream.Write(lengthPrefix, 0, lengthPrefix.Length);
            stream.Write(stringBytes, 0, stringBytes.Length);
        }

        public static void BroadcastString(this IEnumerable<GameClient> clients, string value)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(value);
            byte[] lengthPrefix = BitConverter.GetBytes(stringBytes.Length);
            foreach (var client in clients)
            {
                NetworkStream stream = client.TcpClient.GetStream();
                stream.Write(lengthPrefix, 0, lengthPrefix.Length);
                stream.Write(stringBytes, 0, stringBytes.Length);
            }
        }

        /// <summary>
        /// Signals the clients to stop waiting and to continue.
        /// </summary>
        public static void BroadcastByte(this IEnumerable<GameClient> clients)
        {
            foreach (var client in clients)
            {
                client.TcpClient.GetStream().WriteByte(1);
            }
        }

        // TODO: Maybe this should not be an extension method of the clients list. Maybe there should be a class for methods used by host.
        public static void BroadcastObject<T>(this IEnumerable<GameClient> clients, T value)
        {
            string serializedString = JsonConvert.SerializeObject(value);
            clients.BroadcastString(serializedString);
        }

        public static T ReadObject<T>(this NetworkStream stream)
        {
            string serializedString = stream.ReadString();
            return JsonConvert.DeserializeObject<T>(serializedString);
        }

        public static void WriteObject<T>(this NetworkStream stream, T value)
        {
            string serializedString = JsonConvert.SerializeObject(value);
            stream.WriteString(serializedString);
        }
    }
}
