using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Suro.Net.Client
{
    public class SuroConnectionPool : ObjectPool<ISuroConnection>
    {
        public string AppName { get; private set; }
        public string Hostname { get; private set; }
        public int Port { get; private set; }

        public SuroConnectionPool(string appName, string hostname, int port, int poolSize)
            : base(
                poolSize, 
                (pool) => new PooledSuroConnection(pool, new SuroConnection(appName, hostname, port))
            )
        {
            AppName = appName;
            Hostname = hostname;
            Port = port;
        }
    }
}
