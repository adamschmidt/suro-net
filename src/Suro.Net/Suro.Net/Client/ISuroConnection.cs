using Suro.Net.Client.Thrift;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Msg = Suro.Net.Client.Message;

namespace Suro.Net.Client
{
    public interface ISuroConnection : IDisposable
    {
        void Connect();
        bool IsAlive { get; }
        string AppName { get; }
        Result Send(Msg.Message msg);
        Result Send(TMessageSet messageSet);
    }
}
