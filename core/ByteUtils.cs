using System;
using System.IO;
using System.Text;
using BYTES = System.Collections.Generic.List<byte>;

namespace Randomizer.Core
{
    public static class ByteUtils
    {
        public static string BytesToAscii(BYTES data)
        {
            return System.Text.Encoding.ASCII.GetString(data.ToArray());
        }

        public static string ConvertBytesToString(BYTES data)
        {
            return Encoding.ASCII.GetString(data.ToArray());
        }

        public static void SeekBeg(BinaryReader reader, long offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
        }

        public static void SeekCur(BinaryReader reader, long offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.Current);
        }

        public static void SeekEnd(BinaryReader reader, long offset)
        {
            reader.BaseStream.Seek(offset, SeekOrigin.End);
        }

        public static BYTES ReadBytesAtOffset(BinaryReader reader, long offset, int size)
        {
            long currentPos = reader.BaseStream.Position;
            reader.BaseStream.Seek(offset, SeekOrigin.Begin);
            byte[] data = reader.ReadBytes(size);
            reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);
            return data.ToList();
        }

        // Reads 'size' bytes from current position
        public static BYTES ReadBytes(BinaryReader reader, int size)
        {
            return reader.ReadBytes(size).ToList();
        }

        public static List<byte> CopyBytes(List<byte> data, uint startOffset, uint length)
        {
            var result = new List<byte>((int)length);
            for (uint i = startOffset; i < startOffset + length; i++)
            {
                if (i < data.Count)
                {
                    result.Add(data[(int)i]);
                }
            }
            return result;
        }

        public static uint ToUInt32(IEnumerable<byte> data, bool isBigEndian)
        {
            if (data.ToArray().Length < 4) return 0;
            if (isBigEndian) Array.Reverse(data.ToArray());
            return BitConverter.ToUInt32(data.ToArray(), 0);
        }

        public static uint ToUInt32(IEnumerable<byte> data, int offset, bool isBigEndian)
        {
            if (data.ToArray().Length < offset + 4) return 0;
            byte[] sub = data.Skip(offset).Take(4).ToArray();
            if (isBigEndian) Array.Reverse(sub);
            return BitConverter.ToUInt32(sub, 0);
        }

        public static float ToFloat(IEnumerable<byte> data, bool isBigEndian)
        {
            if (data.ToArray().Length < 4) return 0;
            byte[] copy = data.Take(4).ToArray();
            if (isBigEndian) Array.Reverse(copy);
            return BitConverter.ToSingle(copy, 0);
        }

        public static float ToFloat(IEnumerable<byte> data, int offset, bool isBigEndian)
        {
            if (data.ToArray().Length < offset + 4) return 0;
            byte[] sub = data.Skip(offset).Take(4).ToArray();
            if (isBigEndian) Array.Reverse(sub);
            return BitConverter.ToSingle(sub, 0);
        }

        public static short ToInt16(IEnumerable<byte> data, bool isBigEndian)
        {
            if (data.ToArray().Length < 2) return 0;
            if (isBigEndian) Array.Reverse(data.ToArray());
            return BitConverter.ToInt16(data.ToArray(), 0);
        }

        public static short ToInt16(IEnumerable<byte> data, int offset, bool isBigEndian)
        {
            if (data.ToArray().Length < offset + 2) return 0;
            byte[] sub = data.Skip(offset).Take(2).ToArray();
            if (isBigEndian) Array.Reverse(sub);
            return BitConverter.ToInt16(sub, 0);
        }

        public static byte[] FromUInt32(uint value, bool isBigEndian)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (isBigEndian) Array.Reverse(data);
            return data;
        }

        public static byte[] FromFloat(float value, bool isBigEndian)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (isBigEndian) Array.Reverse(data);
            return data;
        }

        public static byte[] FromInt16(short value, bool isBigEndian)
        {
            byte[] data = BitConverter.GetBytes(value);
            if (isBigEndian) Array.Reverse(data);
            return data;
        }

        public static void SetIntAtOffset(List<byte> data, int offset, int value, bool bigEndian)
        {
            byte[] intBytes = BitConverter.GetBytes(value);

            if (bigEndian)
                Array.Reverse(intBytes);

            // Ensure the list has enough space
            while (data.Count < offset + 4)
                data.Add(0x00);

            // Overwrite bytes at the correct offset
            for (int i = 0; i < 4; i++)
                data[offset + i] = intBytes[i];
        }

        public static List<byte> ResizeList(List<byte> input, int newSize)
        {
            if (newSize < input.Count)
                return input.Take(newSize).ToList();
            input.AddRange(Enumerable.Repeat((byte)0, newSize - input.Count));
            return input;
        }
    }
}
