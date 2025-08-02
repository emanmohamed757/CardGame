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

namespace CardGame.Network
{
    public static class ExtensionMethods
    {
        public static string ReadString(this NetworkStream stream)
        {
            var buffer = new byte[1024];
            int numberOfBytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, numberOfBytesRead);
        }

        public static void WriteString(this NetworkStream stream, string value)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(value);
            stream.Write(stringBytes, 0, stringBytes.Length);
        }

        public static void BroadcastString(this List<GameClient> clients, string value)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(value);
            foreach (var client in clients)
            {
                client.GetStream().Write(stringBytes, 0 , stringBytes.Length);
            }
        }

        public static void BroadcastByte(this List<GameClient> clients)
        {
            foreach (var client in clients)
            {
                client.GetStream().WriteByte(1);
            }
        }

        // TODO: Maybe this should not be an extension method of the clients list. Maybe there should be a class for methods used by host.
        public static void BroadcastObject<T>(this List<GameClient> clients, T value)
        {
            string serializedString = JsonConvert.SerializeObject(value);
            BroadcastString(clients, serializedString);
        }

        public static T ReadObject<T>(this NetworkStream stream)
        {
            string serializedString = stream.ReadString();
            return JsonConvert.DeserializeObject<T>(serializedString);
        }
    }
}
