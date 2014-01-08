using Suro.Net.Client.Thrift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suro.Net.Client
{
    public class PooledSuroConnection : ISuroConnection
    {
        private ObjectPool<ISuroConnection> _pool;
        private SuroConnection _connection;

        public PooledSuroConnection(ObjectPool<ISuroConnection> pool, SuroConnection connection)
        {
            _pool = pool;
            _connection = connection;
        }

        public void Dispose()
        {
            if (_pool.IsDisposed)
            {
                _connection.Dispose();
            }
            else
            {
                _pool.Release(this);
            }
        }

        public void Connect()
        {
            _connection.Connect();
        }

        public bool IsAlive
        {
            get { return _connection.IsAlive; }
        }


        public Result Send(Message.Message msg)
        {
            return _connection.Send(msg);
        }


        public Result Send(Thrift.TMessageSet messageSet)
        {
            return _connection.Send(messageSet);
        }


        public string AppName
        {
            get { return _connection.AppName; }
        }
    }
}
