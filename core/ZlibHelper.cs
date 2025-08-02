using System.IO;
using System.IO.Compression;
using System.Collections.Generic;

public static class ZlibHelper
{
    public static byte[] Decompress(byte[] compressed, int uncompressedSize)
    {
        using var input = new MemoryStream(compressed);
        using var output = new MemoryStream();

        // Use ZLibStream, not DeflateStream
        using var zlib = new ZLibStream(input, CompressionMode.Decompress);
        zlib.CopyTo(output);

        return output.ToArray();
    }

    public static List<byte> CompressData(List<byte> input)
    {
        // Step 1: Raw Deflate
        using var raw = new MemoryStream();
        using (var deflate = new DeflateStream(raw, CompressionLevel.Optimal, true))
        {
            deflate.Write(input.ToArray(), 0, input.Count);
        }
        byte[] deflateData = raw.ToArray();

        // Step 2: Zlib header
        byte[] zlibHeader = new byte[] { 0x78, 0x9C };

        // Step 3: Adler32 checksum
        uint adler = Adler32(input.ToArray());
        byte[] adlerBytes = BitConverter.GetBytes(SwapEndian(adler));

        // Step 4: Build full zlib stream
        var result = new List<byte>();
        result.AddRange(zlibHeader);
        result.AddRange(deflateData);
        result.AddRange(adlerBytes);

        return result;
    }

    private static uint SwapEndian(uint value)
    {
        return ((value & 0x000000FF) << 24) |
               ((value & 0x0000FF00) << 8) |
               ((value & 0x00FF0000) >> 8) |
               ((value & 0xFF000000) >> 24);
    }

    private static uint Adler32(byte[] data)
    {
        const uint MOD_ADLER = 65521;
        uint a = 1, b = 0;

        foreach (byte c in data)
        {
            a = (a + c) % MOD_ADLER;
            b = (b + a) % MOD_ADLER;
        }

        return (b << 16) | a;
    }

    // Original C++ by MojoBojo
    // https://github.com/weighta/Mumbos-Motors
    public static uint CAFF_Checksum(List<byte> caffHeader)
    {
        uint r11 = 0;

        for (int i = 0; i < caffHeader.Count; i++)
        {
            uint r8 = caffHeader[i];
            uint r10 = r11 << 4;

            if ((r8 & 0x80) > 0)
            {
                r11 = 0xFFFFFF80 | r8;
            }
            else
            {
                r11 = r8;
            }

            r11 += r10;
            r10 = r11 & 0xF0000000;

            if (r10 != 0)
            {
                r8 = r10 >> 24;
                r10 |= r8;
                r11 ^= r10;
            }
        }
        return r11;
    }

    public static int NEAREST_MULTIPLE(int x, int s) => ((x + s - 1) / s) * s;
}