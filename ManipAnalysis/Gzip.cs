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
            using (var compressedStream = new MemoryStream())
            using (var gZipCompressor = new GZipStream(compressedStream, CompressionMode.Compress))
            {
                var xml = new XmlSerializer(typeof(T));
                xml.Serialize(uncompressedStream, tObject);
                uncompressedStream.Position = 0;
                uncompressedStream.CopyTo(gZipCompressor);
                compressedData = compressedStream.ToArray();
            }

            return compressedData;
        }

        public static T DeCompress(byte[] data)
        {
            T decompressedData;

            using (var compressedStream = new MemoryStream(data))
            using (var uncompressedStream = new MemoryStream())
            using (var gZipDecompressor = new GZipStream(compressedStream, CompressionMode.Decompress))
            {
                gZipDecompressor.CopyTo(uncompressedStream);
                uncompressedStream.Position = 0;
                var xml = new XmlSerializer(typeof(T));
                decompressedData = (T)xml.Deserialize(uncompressedStream);
            }

            return decompressedData;
        }
    }
}