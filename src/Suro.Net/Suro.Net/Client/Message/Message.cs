using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suro.Net.Client.Message
{
    public class Message
    {
        public string RoutingKey { get; private set; }
        public byte[] Payload { get; private set; }

        public Message(string routingKey, byte[] payload)
        {
            if (string.IsNullOrEmpty(routingKey))
                throw new ArgumentException("Routing key cannot be null or empty", "routingKey");

            RoutingKey = routingKey;
            Payload = payload;
        }

        public void Write(BinaryWriter writer)
        {
            var keyBytes = Encoding.UTF8.GetBytes(RoutingKey);
            var lengthBytes = BitConverter.GetBytes((short)keyBytes.Length);
            Array.Reverse(lengthBytes);
            writer.Write(lengthBytes);
            writer.Write(keyBytes);

            lengthBytes = BitConverter.GetBytes(Payload.Length);
            Array.Reverse(lengthBytes);
            writer.Write(lengthBytes);
            writer.Write(Payload);
        }

    }
}
