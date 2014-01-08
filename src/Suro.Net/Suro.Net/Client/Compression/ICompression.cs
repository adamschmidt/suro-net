using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suro.Net.Client.Compression
{
    public interface ICompression
    {
        byte[] Compress(byte[] buf);
        byte[] Decompress(byte[] buf);
    }
}
