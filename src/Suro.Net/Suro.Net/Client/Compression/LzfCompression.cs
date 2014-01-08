using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suro.Net.Client.Compression
{
    public class LzfCompression : ICompression
    {
        private readonly long[] HashTable = new long[HSIZE];

        private const uint HLOG = 14;
        private const uint HSIZE = (1 << 14);
        private const uint MAX_LIT = (1 << 5);
        private const uint MAX_OFF = (1 << 13);
        private const uint MAX_REF = ((1 << 8) + (1 << 3));

        public byte[] Compress(byte[] buf)
        {
            var output = new byte[buf.Length * 2];
            var size = new LzfCompressor().Compress(buf, buf.Length, output, output.Length);

            byte[] result = new byte[size + 7];
            result[0] = (byte)'Z';
            result[1] = (byte)'V';
            result[2] = (byte)1;

            //encoded length
            var lenBytes = BitConverter.GetBytes((short)size);
            Array.Reverse(lenBytes);

            result[3] = (byte)(lenBytes[0] >> 8);
            result[4] = lenBytes[1];

            //original length
            lenBytes = BitConverter.GetBytes((short)buf.Length);
            Array.Reverse(lenBytes);

            result[5] = (byte)(lenBytes[0] >> 8);
            result[6] = lenBytes[1];

            for (int i = 0; i < size; i++)
            {
                result[i + 7] = output[i];
            }


            return result;
        }

        public byte[] Decompress(byte[] buf)
        {
            throw new NotImplementedException();
        }
    }
}
