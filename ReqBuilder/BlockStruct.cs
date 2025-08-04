using System.IO;

namespace ReqBlock
{
    public abstract class BlockStruct
    {
        public abstract uint BlockType { get; }
        public abstract void WriteData(BinaryWriter bw);
    }
}