using ReqBlock;
using System.IO;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ReqBlock
{
    public class RequirementBlock01 : BlockStruct // Time of Day
    {
        public override uint BlockType => 0x07;

        public override byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            BlockUtils.WriteHeader(bw);
            bw.Write(BlockType);
            bw.Write(0xA441); // Need to be in Big Endian, or swapped somehow
            bw.Write(0x9040); // Need to be in Big Endian, or swapped somehow

            BlockUtils.WriteFooter(bw);

            return ms.ToArray();
        }
    }

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
        public string Tag { get; }

        public RequirementBlock07(string tag)
        {
            Tag = tag;
        }

        public RequirementBlock07()
        {
            Tag = TagGroup.GetRandomEatTag(); // TEMP, CHANGE AFTER TESTING AHHHHHHH
        }

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

            byte[] tagBytes = Encoding.ASCII.GetBytes(Tag);
            byte[] paddedTag = BlockUtils.PadTag(tagBytes, 144);  // Pad rest of Tag bytes
            bw.Write(paddedTag);

            BlockUtils.WriteFooter(bw);

            return ms.ToArray();
        }
    }

    public class RequirementBlock08 : BlockStruct // Currently Unknown
    {
        public override uint BlockType => 0x08;

        public override byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            return ms.ToArray();
        }
    }

    public class RequirementBlock09 : BlockStruct // Level X or Better
    {
        public int Level { get; }

        public RequirementBlock09(int level)
        {
            Level = 1; // TEMP, CHANGE AFTER TESTING AHHHHHHH
        }

        public RequirementBlock09()
        {
            Level = 1; // TEMP, CHANGE AFTER TESTING AHHHHHHH
        }

        public override uint BlockType => 0x09;

        public override byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            BlockUtils.WriteHeader(bw);
            bw.Write(BlockType);
            bw.Write((uint)Level);          // Level Required
            BlockUtils.WriteFooter(bw);

            return ms.ToArray();
        }
    }

    // RequirementBlock0A

    // RequirementBlock0B

    // RequirementBlock0C

    // RequirementBlock0D

    public class RequirementBlock0E : BlockStruct
    {
        public string Tag { get; }
        public int Amount { get; }

        public RequirementBlock0E(string tag, int amount)
        {
            Tag = tag;
            Amount = 1; // TEMP, CHANGE AFTER TESTING AHHHHHHH
        }

        public RequirementBlock0E()
        {
            Tag = TagGroup.GetRandomEatTag();
            Amount = 1;  // TEMP, CHANGE AFTER TESTING AHHHHHHH
        }

        public override uint BlockType => 0x16;

        public override byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            BlockUtils.WriteHeader(bw);
            bw.Write(BlockType);
            bw.Write((uint)0);          // ?
            bw.Write((uint)0);          // ?
            bw.Write((uint)1);          // ?
            bw.Write((uint)2);          // ?
            bw.Write((uint)0);          // ?
            bw.Write((uint)1);          // ?
            bw.Write((uint)Amount);     // Amount Required
            bw.Write((uint)0);          // ?
            bw.Write((uint)0);          // ?
            bw.Write((uint)0);          // ?

            byte[] tagBytes = Encoding.ASCII.GetBytes(Tag); // Only write first part to bw
            byte[] paddedTag = BlockUtils.PadTag(tagBytes, 264);    // Pad to required length
            bw.Write(paddedTag);

            bw.Write((uint)1);          // ? Known to be Variable

            BlockUtils.WriteFooter(bw);
            return ms.ToArray();
        }
    }

    // RequirementBlock0F

    // RequirementBlock10

    // RequirementBlock11

    // RequirementBlock12

    // RequirementBlock13

    // RequirementBlock14

    // RequirementBlock15

    public class RequirementBlock16 : BlockStruct
    {
        public string Tag { get; }
        public int Amount { get; }

        public RequirementBlock16(string tag, int amount)
        {
            Tag = tag;
            Amount = 1; // TEMP, CHANGE AFTER TESTING AHHHHHHH
        }

        public RequirementBlock16()
        {
            Tag = TagGroup.GetRandomEatTag();
            Amount = 1;  // TEMP, CHANGE AFTER TESTING AHHHHHHH
        }

        public override uint BlockType => 0x16;

        public override byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            BlockUtils.WriteHeader(bw);
            bw.Write(BlockType);
            bw.Write((uint)1);          // ?
            bw.Write((uint)1);          // ? Known to be Variable
            bw.Write((uint)9);          // ? Known to be Variable
            bw.Write((uint)Amount);     // Amount required

            byte[] tagBytes = Encoding.ASCII.GetBytes(Tag); // Only write first part to bw
            byte[] paddedTag = BlockUtils.PadTag(tagBytes, 144);    // Pad to required length
            bw.Write(paddedTag);

            BlockUtils.WriteFooter(bw);
            return ms.ToArray();
        }
    }

    // RequirementBlock17

    // RequirementBlock18

    // RequirementBlock19

    public class RequirementBlock1A : BlockStruct // Garden Value
    {
        public int Value { get; }

        public RequirementBlock1A(int Value)
        {
            Value = 1; // TEMP, CHANGE AFTER TESTING AHHHHHHH
        }

        public RequirementBlock1A()
        {
            Value = 1; // TEMP, CHANGE AFTER TESTING AHHHHHHH
        }

        public override uint BlockType => 0x09;

        public override byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            BlockUtils.WriteHeader(bw);
            bw.Write(BlockType);
            bw.Write((uint)Value);          // Value Required
            BlockUtils.WriteFooter(bw);

            return ms.ToArray();
        }
    }

    // RequirementBlock1B

    // RequirementBlock1C

    // RequirementBlock1D

    // RequirementBlock1E

    // RequirementBlock1F

    // RequirementBlock20

    // RequirementBlock21

    // RequirementBlock22
}

public static class RequirementDescriptions
{
    public static string LogText(BlockStruct block)
    {
        return block switch
        {
            RequirementBlock01 rb01 => "It is nighttime in the garden.",
            RequirementBlock02 rb02 => "None.",
            RequirementBlock07 rb07 => $"There are no {LogNames.GetDisplayName(LogNames.getChunkNameFromTaggroup(Strip(rb07.Tag)))} in garden.",
            RequirementBlock09 rb09 => $"You are a level {rb09.Level} gardener or better.",
            //RequirementBlock0E rb0E => $"There is/are {rb0E.amount} {LogNames.GetDisplayName(LogNames.getChunkNameFromTaggroup(Strip(rb0E.Tag)))} in the garden."
            RequirementBlock16 rb16 => $"Has eaten {rb16.Amount} {LogNames.GetDisplayName(LogNames.getChunkNameFromTaggroup(Strip(rb16.Tag)))}.",
            RequirementBlock1A rb1A => $"The garden is worth {rb1A.Value} chocolate coins.",
            
            _ => $"ERROR! ID: 0x{block.BlockType:X2}" // Something went wrong, figure it out
        };
    }

    private static string Strip(string s)
    {
        string[] tag = s.Split(',');
        return tag[0];
    }
}