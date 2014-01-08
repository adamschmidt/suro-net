using Msg = Suro.Net.Client.Message;
using Suro.Net.Client.Thrift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Thrift.Protocol;
using Thrift.Transport;

namespace Suro.Net.Client
{
    public class SuroConnection : ISuroConnection
    {
        private string _appName;
        private TFramedTransport _transport;
        private SuroServer.Client _client;
        private static readonly object locker = new object();
        private string _hostname;
        private int _port;
        public bool CompressMessages { get; set; }

        public SuroConnection(string appName, string hostname, int port)
        {
            _appName = appName;
            _hostname = hostname;
            _port = port;
            CompressMessages = false;
        }

        public void Connect()
        {
            if (_transport == null)
            {
                TcpClient tcpClient = null;

                try
                {
                    tcpClient = new TcpClient(_hostname, _port);
                }
                catch (SocketException e)
                {
                    throw new SuroException("Unable to contact Suro server", e);
                }
                
                tcpClient.NoDelay = true;
                tcpClient.LingerState = new LingerOption(true, 0);
                tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                var time = 300000UL;
                var interval = 1000UL;

                var input = new[]
            	{
            		(time == 0 || interval == 0) ? 0UL : 1UL, // on or off
					time,
					interval
				};

                const int BytesPerLong = 4; // 32 / 8
		        const int BitsPerByte = 8;

                // Pack input into byte struct.
				byte[] inValue = new byte[3 * BytesPerLong];
				for (int i = 0; i < input.Length; i++)
				{
					inValue[i * BytesPerLong + 3] = (byte)(input[i] >> ((BytesPerLong - 1) * BitsPerByte) & 0xff);
					inValue[i * BytesPerLong + 2] = (byte)(input[i] >> ((BytesPerLong - 2) * BitsPerByte) & 0xff);
					inValue[i * BytesPerLong + 1] = (byte)(input[i] >> ((BytesPerLong - 3) * BitsPerByte) & 0xff);
					inValue[i * BytesPerLong + 0] = (byte)(input[i] >> ((BytesPerLong - 4) * BitsPerByte) & 0xff);
				}

                tcpClient.Client.IOControl(IOControlCode.KeepAliveValues, inValue, BitConverter.GetBytes(0));
                
                TSocket thriftSocket = new TSocket(tcpClient);
                _transport = new TFramedTransport(thriftSocket);
            }
            
            if(!_transport.IsOpen)
                _transport.Open();
        }

        private SuroServer.Client Client
        {
            get
            {
                lock (locker)
                {
                    if (_client == null)
                    {
                        var protocol = new TBinaryProtocol(_transport, true, true);
                        _client = new SuroServer.Client(protocol);
                    }
                    if (!_transport.IsOpen)
                        _transport.Open();
                }
                return _client;
            }
        }

        public bool IsAlive
        {
            get
            {
                return _transport != null && _transport.IsOpen && Client.getStatus() == ServiceStatus.ALIVE;
            }
        }

        public void Dispose()
        {
            if (_transport != null && _transport.IsOpen)
            {
                _transport.Flush();
                _transport.Close();
            }
        }

        public string AppName
        {
            get
            {
                return _appName;
            }
        }

        public Result Send(Msg.Message msg)
        {
            return Send(
                new Msg.MessageSetBuilder(_appName)
                .AddMessage(msg)
                .Build()
                );
        }


        public Result Send(TMessageSet messageSet)
        {
            return Client.process(messageSet);
        }
    }
}
