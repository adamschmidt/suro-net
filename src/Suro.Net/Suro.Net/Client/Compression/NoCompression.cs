using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suro.Net.Client.Compression
{
    public class NoCompression : ICompression
    {
        public byte[] Compress(byte[] buf)
        {
            return buf;
        }

        public byte[] Decompress(byte[] buf)
        {
            return buf;
        }
    }
}
