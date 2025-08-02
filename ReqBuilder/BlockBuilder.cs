using System.Collections.Generic;
using System.IO;

namespace ReqBlock
{
    public class PatchFileBuilder
    {
        private readonly List<BlockStruct> blocks = new();

        public void AddBlock(BlockStruct block) => blocks.Add(block);

        public byte[] Build()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            foreach (var block in blocks)
                bw.Write(block.ToBytes());
            return ms.ToArray();
        }
    }
}
