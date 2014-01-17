using System;
using System.Collections.Generic;
using System.IO;
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
            return Encode(buf, 0, buf.Length);
        }

        private byte[] Encode(byte[] data, int offset, int length)
        {
            var outBuf = new byte[data.Length * 2];
            var encBytes = new LzfCompressor().Compress(data, data.Length, outBuf, outBuf.Length);

            using (var str = new MemoryStream())
            {

                int left = data.Length;
                int chunkLen = Math.Min(LzfChunk.MAX_CHUNK_LEN, left);
                LzfChunk first = LzfChunkEncoder.Encode(data, offset, chunkLen);
                left -= chunkLen;
                // shortcut: if it all fit in, no need to coalesce:
                if (left < 1)
                {
                    return first.Data;
                }
                // otherwise need to get other chunks:
                int resultBytes = first.Data.Length;
                offset += chunkLen;
                LzfChunk last = first;

                do
                {
                    chunkLen = Math.Min(left, LzfChunk.MAX_CHUNK_LEN);
                    LzfChunk chunk = LzfChunkEncoder.Encode(data, offset, chunkLen);
                    offset += chunkLen;
                    left -= chunkLen;
                    resultBytes += chunk.Data.Length;
                    last.Next = chunk;
                    last = chunk;
                } while (left > 0);
                // and then coalesce returns into single contiguous byte array
                byte[] result = new byte[resultBytes];
                int ptr = 0;
                for (; first != null; first = first.Next)
                {
                    ptr = first.CopyTo(result, ptr);
                }
                return result;
            }
        }

        public byte[] Decompress(byte[] buf)
        {
            throw new NotImplementedException();
        }

        private class LzfChunkEncoder
        {
            const int MinimumCompressionBytes = 16;

            public static LzfChunk Encode(byte[] data, int offset, int length)
            {
                if (length >= MinimumCompressionBytes)
                {
                    /* If we have non-trivial block, and can compress it by at least
                     * 2 bytes (since header is 2 bytes longer), let's compress:
                     */
                    var outBuf = new byte[data.Length * 2];

                    int compLen = new LzfCompressor().Compress(data, data.Length, outBuf, outBuf.Length);
                    if (compLen < (length - 2))
                    { // nah; just return uncompressed
                        return LzfChunk.CreateCompressed(length, outBuf, 0, compLen);
                    }
                }
                // Otherwise leave uncompressed:
                return LzfChunk.CreateUncompressed(data, offset, length);
            }
        }

        private class LzfChunk
        {
            public const int MAX_CHUNK_LEN = 0xFFFF;
            const int HEADER_LEN_COMPRESSED = 7;
            const int HEADER_LEN_NOT_COMPRESSED = 5;
    
            const byte BYTE_Z = (byte)'Z';
            const byte BYTE_V = (byte)'V';

            const int BLOCK_TYPE_NON_COMPRESSED = 0;
            const int BLOCK_TYPE_COMPRESSED = 1;

            public byte[] Data { get; private set; }
            public LzfChunk Next { get; set; }

            public LzfChunk(byte[] data)
            {
                Data = data;
            }

            internal static LzfChunk CreateCompressed(int length, byte[] outBuf, int p, int compLen)
            {
                byte[] result = new byte[compLen + HEADER_LEN_COMPRESSED];
                result[0] = BYTE_Z;
                result[1] = BYTE_V;
                result[2] = BLOCK_TYPE_COMPRESSED;
                result[3] = (byte)(compLen >> 8);
                result[4] = (byte)compLen;
                result[5] = (byte)(length >> 8);
                result[6] = (byte)length;
                Array.Copy(outBuf, p, result, HEADER_LEN_COMPRESSED, compLen);
                return new LzfChunk(result);
            }

            internal static LzfChunk CreateUncompressed(byte[] data, int offset, int length)
            {
                byte[] result = new byte[length + HEADER_LEN_NOT_COMPRESSED];
                result[0] = BYTE_Z;
                result[1] = BYTE_V;
                result[2] = BLOCK_TYPE_NON_COMPRESSED;
                result[3] = (byte)(length >> 8);
                result[4] = (byte)length;
                Array.Copy(data, offset, result, HEADER_LEN_NOT_COMPRESSED, length);
                return new LzfChunk(result);
            }

            public int CopyTo(byte[] dst, int ptr)
            {
                int len = Data.Length;
                Array.Copy(Data, 0, dst, ptr, len);
                return ptr + len;
            }
        }
    }
}
