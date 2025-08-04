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

            BlockUtils.WriteHeader(bw);

            for (int i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                bool isLast = (i == blocks.Count - 1);

                block.WriteData(bw);

                if (isLast)
                    BlockUtils.WriteFooter(bw);
                else
                    BlockUtils.WriteMiddle(bw);
            }

            return ms.ToArray();
        }
    }

}
