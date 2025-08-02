using System.IO;
using System.Text;

namespace ReqBlock
{
    // RequirementBlock01

    public class RequirementBlock02 : BlockStruct // Blank Requirement
    {
        public override uint BlockType => 0x02;

        public override byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            BlockUtils.WriteHeader(bw);
            bw.Write(BlockType);
            BlockUtils.WriteFooter(bw);

            return ms.ToArray();
        }
    }

    // RequirementBlock03

    // RequirementBlock04

    // RequirementBlock05

    // RequirementBlock06

    public class RequirementBlock07 : BlockStruct // X is NOT in Garden
    {
        public override uint BlockType => 0x07;

        public override byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            BlockUtils.WriteHeader(bw);
            bw.Write(BlockType);
            bw.Write(0x0E);             // ?
            bw.Write((uint)0);          // ?
            bw.Write((uint)0);          // ?
            bw.Write((uint)1);          // ?
            bw.Write((uint)1);          // ?
            bw.Write((uint)0);          // ?
            bw.Write((uint)0);          // ?
            bw.Write((uint)1);          // ?
            bw.Write((uint)0);          // ?
            bw.Write((uint)0);          // ?
            bw.Write((uint)0);          // ?

            string fullTag = TagGroup.GetRandomEatTag(); // Target Item to Eat
            string[] tag = fullTag.Split(',');
            byte[] tagBytes = Encoding.ASCII.GetBytes(tag[0]);
            byte[] paddedTag = BlockUtils.PadTag(tagBytes, 144);  // Pad rest of Tag bytes
            bw.Write(paddedTag);

            BlockUtils.WriteFooter(bw);

            return ms.ToArray();
        }
    }

    /*public class RequirementBlock08 : BlockStruct // Currently Unknown
    {
        public override uint BlockType => 0x08;

        public override byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            return ms.ToArray();
        }
    }*/

    public class RequirementBlock09 : BlockStruct // Level X or Better
    {
        public override uint BlockType => 0x09;

        public override byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            BlockUtils.WriteHeader(bw);
            bw.Write(BlockType);
            bw.Write((uint)1);          // Level Required
            BlockUtils.WriteFooter(bw);

            return ms.ToArray();
        }
    }

    // RequirementBlock0A

    // RequirementBlock0B

    // RequirementBlock0C

    // RequirementBlock0D

    // RequirementBlock0E

    // RequirementBlock0F

    // RequirementBlock10

    // RequirementBlock11

    // RequirementBlock12

    // RequirementBlock13

    // RequirementBlock14

    // RequirementBlock15

    public class RequirementBlock16 : BlockStruct // Has Eaten X Item
    {
        private readonly string fullTag;

        public RequirementBlock16(string tag)
        {
            fullTag = tag;
        }

        public RequirementBlock16() : this(TagGroup.GetRandomEatTag()) { }

        public override uint BlockType => 0x16;

        public override byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            BlockUtils.WriteHeader(bw);
            bw.Write(BlockType);
            bw.Write((uint)1);          // ?
            bw.Write((uint)1);          // ?
            bw.Write((uint)9);          // ?
            bw.Write((uint)1);          // Amount required

            string[] tagParts = fullTag.Split(',');
            byte[] tagBytes = Encoding.ASCII.GetBytes(tagParts[0]); // Only write first part to bw
            byte[] paddedTag = BlockUtils.PadTag(tagBytes, 144);    // Pad to required length
            bw.Write(paddedTag);

            BlockUtils.WriteFooter(bw);

            return ms.ToArray();
        }
    }

    // RequirementBlock17

    // RequirementBlock18

    // RequirementBlock19

    // RequirementBlock1A

    // RequirementBlock1B

    // RequirementBlock1C

    // RequirementBlock1D

    // RequirementBlock1E

    // RequirementBlock1F

    // RequirementBlock20

    // RequirementBlock21

    // RequirementBlock22
}
