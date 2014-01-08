using Suro.Net.Client.Compression;
using Suro.Net.Client.Thrift;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suro.Net.Client.Message
{

    public class MessageSetBuilder
    {
        private List<Message> _messages;
        private CompressionType _compressionType = CompressionType.None;
        private string _clientApp;

        public MessageSetBuilder()
            : this(Constants.DefaultApplicationName)
        {

        }
        public MessageSetBuilder (string clientApp)
        {
            _messages = new List<Message>();
            _clientApp = clientApp;
        }

        public MessageSetBuilder AddMessage(string routingKey, byte[] payload)
        {
            return AddMessage(new Message(routingKey, payload));
        }

        public MessageSetBuilder AddMessage(Message msg)
        {
            _messages.Add(msg);
            return this;
        }

        public MessageSetBuilder WithCompression(CompressionType compressionType)
        {
            _compressionType = compressionType;
            return this;
        }

        public TMessageSet Build()
        {
            try
            {
                var buf = CreatePayload(_messages, _compressionType);
                var crc = CalculateCrc(buf);

                return new TMessageSet
                {
                    Compression = (sbyte)_compressionType,
                    App = _clientApp,
                    Crc = crc,
                    NumMessages = _messages.Count(),
                    Messages = buf

                };
            }
            finally
            {
                _messages.Clear();
            }
        }

        private long CalculateCrc(byte[] buf)
        {
            return Convert.ToInt64(Crc32.Compute(buf));
        }

        private byte[] CreatePayload(List<Message> _messages, CompressionType _compressionType)
        {
            using(var stream = new MemoryStream())
            using(var writer = new BinaryWriter(stream))
            {

                foreach (var item in _messages)
                {
                    item.Write(writer);
                }

                ICompression comp = new NoCompression();

                if (_compressionType == CompressionType.Lzo)
                    comp = new LzfCompression();

                writer.Flush();
                return comp.Compress(stream.ToArray());
            }

        }
    }
}
