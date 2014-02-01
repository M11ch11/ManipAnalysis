using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ManipAnalysis_v2.MongoDb;

namespace ManipAnalysis_v2
{
    class Gzip
    {
        public static byte[] Compress(List<PositionContainer> list)
        {
            byte[] compressedData;

            using (var uncompressedStream = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(typeof (List<PositionContainer>));
                xml.Serialize(uncompressedStream, list);
                uncompressedStream.Position = 0;
                using (var compressedStream = new MemoryStream())
                {
                    using (var gZipCompressor = new GZipStream(compressedStream, CompressionMode.Compress))
                    {
                        uncompressedStream.CopyTo(gZipCompressor);
                    }
                    compressedData = compressedStream.ToArray();
                }
            }
            return compressedData;
        }

        public static List<PositionContainer> DeCompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            using (var uncompressedStream = new MemoryStream())
            {
                using (var gZipDecompressor = new GZipStream(compressedStream, CompressionMode.Decompress))
                {
                    gZipDecompressor.CopyTo(uncompressedStream);
                }
                uncompressedStream.Position = 0;
                XmlSerializer xml = new XmlSerializer(typeof(List<PositionContainer>));
                return (List<PositionContainer>)xml.Deserialize(uncompressedStream);
            }
        }
    }
}
