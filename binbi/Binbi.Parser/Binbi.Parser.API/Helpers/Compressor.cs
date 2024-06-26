using System.IO.Compression;
using System.Text;

namespace Binbi.Parser.API.Helpers;

internal static class Compressor
{
    internal static string Decompress(byte[] data)
    {
        using var compressedStream = new MemoryStream(data);
        using var decompressedStream = new MemoryStream();
        using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
        
        gzipStream.CopyTo(decompressedStream);
        var decompressedData = decompressedStream.ToArray();
        return Encoding.UTF8.GetString(decompressedData);
    }
}