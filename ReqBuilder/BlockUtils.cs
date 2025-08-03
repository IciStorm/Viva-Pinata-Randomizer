using System.IO;

namespace ReqBlock
{
    public static class BlockUtils
    {
        public static void WriteHeader(BinaryWriter bw) // Start of block
        {
            bw.Write((uint)0x03); // Standard header
        }

        public static void WriteFooter(BinaryWriter bw) // End of block
        {
            bw.Write((uint)0x64); // Footer segment 1
            bw.Write((uint)0x04); // Footer segment 2
            bw.Write((uint)0x64); // Footer segment 3
            bw.Write((uint)0x00); // Footer padding
        }

        public static void WriteMiddle(BinaryWriter bw) // Used to bridge multiple blocks together
        {
            bw.Write((uint)0x64);
            bw.Write((uint)0x05);
        }
            
        public static byte[] PadTag(byte[] data, int totalLength)
        {
            // Get difference between character length and total length of padding and delete the stuff below

            byte[] result = new byte[totalLength];
            data.CopyTo(result, 0);
            return result;
        }
    }
}
