using System.IO;
using System.IO.Compression;
using System.Xml.Serialization;

namespace ManipAnalysis_v2
{
    internal static class Gzip<T>
    {
        public static byte[] Compress(T tObject)
        {
            byte[] compressedData;

            using (var uncompressedStream = new MemoryStream())
            {
                var xml = new XmlSerializer(typeof(T));
                xml.Serialize(uncompressedStream, tObject);
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

        public static T DeCompress(byte[] data)
        {
            using (var compressedStream = new MemoryStream(data))
            {
                using (var uncompressedStream = new MemoryStream())
                {
                    using (var gZipDecompressor = new GZipStream(compressedStream, CompressionMode.Decompress))
                    {
                        gZipDecompressor.CopyTo(uncompressedStream);
                    }
                    uncompressedStream.Position = 0;
                    var xml = new XmlSerializer(typeof(T));
                    return (T)xml.Deserialize(uncompressedStream);
                }
            }
        }
    }
}