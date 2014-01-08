using Newtonsoft.Json;
using Suro.Net.Client;
using Suro.Net.Client.Compression;
using Suro.Net.Client.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Suro.Net.Web
{
    /// <summary>
    /// Container class that encapsulates the logic for building and writing a Suro message during a web request.
    /// </summary>
    public class SuroContext
    {
        private const string RequestItemKey = "__BehaviourContext__";
        private IList<ContextItem> contextItems = new List<ContextItem>();

        private SuroContext()
        {
        }

        /// <summary>
        /// Get the number of items in the current context
        /// </summary>
        public int Size
        {
            get
            {
                return contextItems.Count;
            }
        }

        /// <summary>
        /// Gets the Suro context for the current request.
        /// </summary>
        public static SuroContext Current
        {
            get
            {
                var item = HttpContext.Current.Items[RequestItemKey] as SuroContext;
                if (item == null)
                {
                    item = new SuroContext();
                    HttpContext.Current.Items[RequestItemKey] = item;
                }
                return item;
            }
        }

        /// <summary>
        /// Add a data blob to the context.
        /// </summary>
        /// <param name="routingKey">Message routing key</param>
        /// <param name="raw">Data to be sent</param>
        public void Add(string routingKey, byte[] raw)
        {
            contextItems.Add(new ContextItem(routingKey, raw));
        }

        /// <summary>
        /// Add a text-based message to the context.
        /// </summary>
        /// <param name="routingKey">Message routing key</param>
        /// <param name="message">Data to be sent</param>
        public void Add(string routingKey, string message)
        {
            Add(routingKey, Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// Add an object message to the context. The object will be sent as JSON.
        /// </summary>
        /// <param name="routingKey">Message routing key</param>
        /// <param name="item">Data to be sent</param>
        public void Add(string routingKey, object item)
        {
            Add(routingKey, JsonConvert.SerializeObject(item, Formatting.None));
        }

        /// <summary>
        /// Add a 64-bit integer to the context. The integer bytes will be added as big-endian to suit the Java conversion
        /// on the server side.
        /// </summary>
        /// <param name="routingKey">Message routing key</param>
        /// <param name="item">Data to be sent</param>
        public void Add(string routingKey, long item)
        {
            var bytes = BitConverter.GetBytes(item);
            if(BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            Add(routingKey, bytes);
        }

        /// <summary>
        /// Add a 32-bit integer to the context. The integer bytes will be added as big-endian to suit the Java conversion
        /// on the server side.
        /// </summary>
        /// <param name="routingKey">Message routing key</param>
        /// <param name="item">Data to be sent</param>
        public void Add(string routingKey, int item)
        {
            var bytes = BitConverter.GetBytes(item);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            Add(routingKey, bytes);
        }

        /// <summary>
        /// Write the information gathered in the context (HttpRequest) to the Suro server. Calling this method sends the data
        /// uncompressed.
        /// </summary>
        /// <param name="connection">Suro server connection</param>
        /// <returns>true if the response from the server is OK</returns>
        public bool Write(ISuroConnection connection)
        {
            return Write(connection, false);
        }

        /// <summary>
        /// Write the information gathered in the context (HttpRequest) to the Suro server. If requested, the information
        /// will be sent to the server LZO-compressed.
        /// </summary>
        /// <param name="connection">Suro server connection</param>
        /// <param name="compressed">flag indicating if the data should be LZO-compressed before sending</param>
        /// <returns></returns>
        public bool Write(ISuroConnection connection, bool compressed)
        {
            connection.Connect(); //ensure the client is connected

            var bob = new MessageSetBuilder(connection.AppName)
                .WithCompression(compressed ? CompressionType.Lzo : CompressionType.None );

            foreach (var item in contextItems)
            {
                bob = bob.AddMessage(item.RoutingKey, item.Data);
            }

            var result = connection.Send(bob.Build());

            return result.ResultCode == Client.Thrift.ResultCode.OK;
        }

        /// <summary>
        /// Container object for TMessageSet items
        /// </summary>
        private class ContextItem
        {
            public string RoutingKey { get; private set; }
            public byte[] Data { get; private set; }

            public ContextItem(string routingKey, byte[] data)
            {
                RoutingKey = routingKey;
                Data = data;
            }
        }
    }
}
