using System;
using System.Collections.Generic;

namespace Randomizer.PKG
{
    public enum FileType
    {
        Unknown,
        DDS,
        RawImage,
        XUI_Scene
    }
    
    public enum ChunkType
    {
        Unknown,
        VDAT,
        VGPU,
        DDS
    }
    
    public class CAFF_Info
    {
        public string PKGPath { get; set; }
        public uint Offset { get; set; }
        public uint Size { get; set; }
        public uint Number { get; set; }
        public uint Unknown { get; set; }
    }

    public class CAFF
    {
        public CAFF_Info CAFF_INFO { get; set; }
        public string CAFF_Version { get; set; }
        public UInt32 ChunkCount { get; set; }
        public UInt32 ChunkSpreadCount { get; set; }
        public UInt32 VREF_Offset { get; set; }
        public UInt32 VREF_Uncompressed_Size { get; set; }
        public UInt32 VREF_Compressed_Size { get; set; }
        public UInt32 VLUT_Offset { get; set; }
        public UInt32 VLUT_Uncompressed_Size { get; set; }
        public UInt32 VLUT_Compressed_Size { get; set; }
        public bool isBigEndian { get; set; }
    }
    public class ChunkInfoOffsets
    {
        public UInt32 VDAT_Offset_Location { get; set; }
        public UInt32 VDAT_Size_Location { get; set; }
        public UInt32 VGPU_Offset_Location { get; set; }
        public UInt32 VGPU_Size_Location { get; set; }
    }

    public class ChunkInfo
    {
        public string ChunkName { get; set; }
        public UInt32 ID { get; set; }
        public UInt32 VDAT_Offset { get; set; }
        public UInt32 VDAT_Size { get; set; }
        public byte VDAT_File_Data_1 { get; set; }
        public byte VDAT_File_Data_2 { get; set; }
        public bool HasVGPU { get; set; }
        public UInt32 VGPU_Offset { get; set; }
        public UInt32 VGPU_Size { get; set; }
        public byte VGPU_File_Data_1 { get; set; }
        public byte VGPU_File_Data_2 { get; set; }
        public ChunkInfoOffsets OffsetLocations { get; set; }

        // FileType Type = FileType::Unknown;
        public byte DebugData { get; set; }

    }

    public class VREF
    {
        public CAFF_Info CAFF { get; set; }
        public List<string> ChunkNames { get; set; } = new();
        public UInt32 VGPU_Offset { get; set; }
        public UInt32 VGPU_Compressed_Size { get; set; }
        public UInt32 VGPU_Uncompressed_Size { get; set; }
        public UInt32 VDAT_Offset { get; set; }
        public UInt32 VDAT_Compressed_Size { get; set; }
        public UInt32 VDAT_Uncompressed_Size { get; set; }
        public UInt32 NameBlockOffset { get; set; }
        public UInt32 InfoBlockOffset { get; set; }
        public List<ChunkInfo> ChunkInfos { get; set; } = new();
    }
}

