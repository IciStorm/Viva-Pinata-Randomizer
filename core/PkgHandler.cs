using Randomizer.PKG;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Text;
using BYTES = System.Collections.Generic.List<byte>;

public class Streams
{
    public BYTES VREF = new BYTES();
    public BYTES VDAT = new BYTES();
    public BYTES VGPU = new BYTES();
}

namespace Randomizer.Core
{
    public static class PkgHandler
    {
        public static BYTES GetVREFBYTES(BinaryReader reader, CAFF caff)
        {
            long currentPos = reader.BaseStream.Position;

            // Go to VREF offset inside the CAFF section
            long vrefOffset = caff.CAFF_INFO.Offset + caff.VREF_Offset;
            ByteUtils.SeekBeg(reader, vrefOffset);

            var compressedVREF = ByteUtils.ReadBytes(reader, (int)caff.VREF_Compressed_Size);

            // Restore original file position
            ByteUtils.SeekBeg(reader, currentPos);

            return ZlibHelper.Decompress(compressedVREF.ToArray(), (int)caff.VREF_Uncompressed_Size).ToList();
        }

        public static BYTES GetVDATBYTES(BinaryReader reader, Pkg package, int CAFFNumber)
        {
            long offset = package.VREFs[CAFFNumber].VDAT_Offset + package.Caff_Infos[CAFFNumber].Offset;
            ByteUtils.SeekBeg(reader, offset);
            var compressed = ByteUtils.ReadBytes(reader, (int)package.VREFs[CAFFNumber].VDAT_Compressed_Size);
            return ZlibHelper.Decompress(compressed.ToArray(), (int)package.VREFs[CAFFNumber].VDAT_Uncompressed_Size).ToList();
        }

        public static BYTES GetChunkVDATBYTES(BinaryReader reader, Pkg package, int CAFFNumber, int ChunkNumber)
        {
            var fullVDAT = GetVDATBYTES(reader, package, CAFFNumber);
            var chunk = ByteUtils.CopyBytes(fullVDAT, package.VREFs[CAFFNumber].ChunkInfos[ChunkNumber].VDAT_Offset, package.VREFs[CAFFNumber].ChunkInfos[ChunkNumber].VDAT_Size);
            return chunk;
        }

        public static BYTES GetVGPUBYTES(BinaryReader reader, Pkg package, int CAFFNumber)
        {
            long offset = package.VREFs[CAFFNumber].VGPU_Offset + package.Caff_Infos[CAFFNumber].Offset;
            ByteUtils.SeekBeg(reader, offset);
            var compressed = ByteUtils.ReadBytes(reader, (int)package.VREFs[CAFFNumber].VGPU_Compressed_Size);
            return ZlibHelper.Decompress(compressed.ToArray(), (int)package.VREFs[CAFFNumber].VGPU_Uncompressed_Size).ToList();
        }

        public static BYTES GetChunkVGPUBYTES(int CAFFNumber, int ChunkNumber, Pkg package, BinaryReader reader)
        {
            var fullVGPU = GetVGPUBYTES(reader, package, CAFFNumber);
            var chunk = ByteUtils.CopyBytes(fullVGPU,
                package.VREFs[CAFFNumber].ChunkInfos[ChunkNumber].VGPU_Offset,
                package.VREFs[CAFFNumber].ChunkInfos[ChunkNumber].VGPU_Size);
            return chunk;
        }

        public static FileType GetFileType(string ChunkName, BYTES VDAT, BYTES VGPU)
        {
            //if the first 4 bytes are "2E 64 64 64" then return DDS
            if (VDAT.Count > 25)
            {
                if (VDAT[25] == 0x60 || VDAT[25] == 0x10 || VDAT[25] == 0x30 || VDAT[25] == 0xB0)
                {
                    return FileType.DDS;
                }
                if (VDAT[26] == 0x40)//26 is TIP Raw
                {
                    return FileType.RawImage;
                }
                //First 4 bytes of VDAT are "XUIZ" then return XUI Scene ".xzp"
                if (VDAT[0] == 0x58 && VDAT[1] == 0x55 && VDAT[2] == 0x49 && VDAT[3] == 0x5A)
                {
                    return FileType.XUI_Scene;
                }
            }
            return FileType.Unknown;
        }

        public static Pkg ReadPKG(string path)
        {
            var pkg = new Pkg { Path = path };

            using var reader = new BinaryReader(File.OpenRead(path));

            // Read version (first 4 bytes)
            var versionBytes = reader.ReadBytes(4);
            uint version = ByteUtils.ToUInt32(versionBytes, false);
            pkg.Version = version;

            if (version > 3)
            {
                pkg.IsBigEndian = true;
                version = ByteUtils.ToUInt32(versionBytes, true);
                pkg.Version = version;
            }

            // Read CAFF count
            var caffCountBytes = reader.ReadBytes(4);
            pkg.CaffCount = ByteUtils.ToUInt32(caffCountBytes, pkg.IsBigEndian);

            // Read each CAFF_Info block (12 bytes per entry)
            for (int i = 0; i < pkg.CaffCount; i++)
            {
                var info = new CAFF_Info
                {
                    Number = (uint)(i + 1),
                    PKGPath = path,
                    Unknown = ByteUtils.ToUInt32(reader.ReadBytes(4), pkg.IsBigEndian),
                    Offset = ByteUtils.ToUInt32(reader.ReadBytes(4), pkg.IsBigEndian),
                    Size = ByteUtils.ToUInt32(reader.ReadBytes(4), pkg.IsBigEndian)
                };
                pkg.Caff_Infos.Add(info);
            }

            // Read CAFF Headers
            for (int i = 0; i < pkg.CaffCount; i++)
            {
                //Seek offset
                ByteUtils.SeekBeg(reader, pkg.Caff_Infos[i].Offset);
                var caff = new CAFF
                {
                    CAFF_INFO = pkg.Caff_Infos[i],
                };
                //CAFF Version (17 bytes)
                BYTES CAFF_Version_B = ByteUtils.ReadBytes(reader, 17);
                string CAFF_Version = "";
                CAFF_Version = ByteUtils.ConvertBytesToString(CAFF_Version_B);
                caff.CAFF_Version = CAFF_Version;

                //forward 3 bytes of null data
                ByteUtils.SeekCur(reader, 3);

                //VREF Offset (4 bytes)
                caff.VREF_Offset = ByteUtils.ToUInt32(ByteUtils.ReadBytes(reader, 4), pkg.IsBigEndian);

                //Skip 4 unknown bytes
                ByteUtils.SeekCur(reader, 4);

                //Chunk Amount (4 bytes)
                caff.ChunkCount = ByteUtils.ToUInt32(ByteUtils.ReadBytes(reader, 4), pkg.IsBigEndian);

                //chunk spread count (4 bytes)
                caff.ChunkSpreadCount = ByteUtils.ToUInt32(ByteUtils.ReadBytes(reader, 4), pkg.IsBigEndian);

                //skip 44 bytes
                ByteUtils.SeekCur(reader, 44);

                //VRef Uncompressed Size (4 bytes)
                caff.VREF_Uncompressed_Size = ByteUtils.ToUInt32(ByteUtils.ReadBytes(reader, 4), pkg.IsBigEndian);

                //skip 12 bytes
                ByteUtils.SeekCur(reader, 12);

                //VRef Compressed Size (4 bytes)
                caff.VREF_Compressed_Size = ByteUtils.ToUInt32(ByteUtils.ReadBytes(reader, 4), pkg.IsBigEndian);

                //VLUT Uncompressed Size (4 bytes)
                caff.VLUT_Uncompressed_Size = ByteUtils.ToUInt32(ByteUtils.ReadBytes(reader, 4), pkg.IsBigEndian);

                //skip 12 bytes

                ByteUtils.SeekCur(reader, 12);

                //VLUT Compressed Size (4 bytes)
                caff.VLUT_Compressed_Size = ByteUtils.ToUInt32(ByteUtils.ReadBytes(reader, 4), pkg.IsBigEndian);

                //VLUT Offset is VRef Offset + VRef Compressed Size
                caff.VLUT_Offset = caff.VREF_Offset + caff.VREF_Compressed_Size;

                pkg.CAFFs.Add(caff);
            }

            // Read VREFs
            for (int i = 0; i < pkg.CaffCount; i++)
            {
                var VREF = new VREF { };
                uint NameBlockSize;

                BYTES VREF_Uncompressed = GetVREFBYTES(reader, pkg.CAFFs[i]);

                //start at first value
                int offset = 9;

                VREF.VDAT_Uncompressed_Size = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);

                offset += 20;

                //VDAT Compressed Size (4 bytes)
                VREF.VDAT_Compressed_Size = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);

                offset = 42;

                //VGPU Uncompressed Size (4 bytes)
                VREF.VGPU_Uncompressed_Size = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);

                offset += 20;

                //VGPU Compressed Size (4 bytes)
                VREF.VGPU_Compressed_Size = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);

                //VLUT_OFFSET + VLUT_Compressed_Size
                VREF.VDAT_Offset = pkg.CAFFs[i].VLUT_Offset + pkg.CAFFs[i].VLUT_Compressed_Size;

                //VDAT_OFFSET + VDAT_Compressed_Size
                VREF.VGPU_Offset = VREF.VDAT_Offset + VREF.VDAT_Compressed_Size;

                offset = 81;

                //for each chunk
                for (int j = 0; j < (pkg.CAFFs[i].ChunkCount); j++)
                {
                    uint ChunkNameoffset;
                    uint NameSize;
                    BYTES ChunkName;

                    if (j == (pkg.CAFFs[i].ChunkCount - 1))
                    {
                        ChunkNameoffset = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);
                        offset = 77;
                        NameBlockSize = ByteUtils.ToUInt32(VREF_Uncompressed, 77, pkg.IsBigEndian);
                        NameSize = (NameBlockSize + (81 + (pkg.CAFFs[i].ChunkCount * 4))) - (ChunkNameoffset + (81 + (pkg.CAFFs[i].ChunkCount * 4)));
                        ChunkName = ByteUtils.CopyBytes(VREF_Uncompressed, ChunkNameoffset + (81 + (pkg.CAFFs[i].ChunkCount * 4)), NameSize);
                        offset += 4;
                        VREF.ChunkNames.Add(ByteUtils.ConvertBytesToString(ChunkName));

                        continue;
                    }

                    ChunkNameoffset = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);

                    uint NextChunkNameoffset = ByteUtils.ToUInt32(VREF_Uncompressed, offset + 4, pkg.IsBigEndian);

                    NameSize = (NextChunkNameoffset + (81 + (pkg.CAFFs[i].ChunkCount * 4))) - (ChunkNameoffset + (81 + (pkg.CAFFs[i].ChunkCount * 4)));

                    ChunkName = ByteUtils.CopyBytes(VREF_Uncompressed, ChunkNameoffset + (81 + (pkg.CAFFs[i].ChunkCount * 4)), NameSize);

                    if (ChunkName.Count > 0 && ChunkName[ChunkName.Count - 1] == 0x00)
                    {
                        ChunkName.RemoveAt(ChunkName.Count - 1);
                    }

                    offset += 4;

                    VREF.ChunkNames.Add(ByteUtils.ConvertBytesToString(ChunkName));
                }

                offset = 77;

                //NameBlockSize (4 bytes)
                NameBlockSize = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);

                //Beginning of InfoBlock
                offset = (int)(NameBlockSize + 81 + (pkg.CAFFs[i].ChunkCount * 4)) + 4;

                for (int j = 0; j < (pkg.CAFFs[i].ChunkCount - 1); j++)
                {
                    var chunkinfo = new ChunkInfo { };
                    var chunkinfooffsets = new ChunkInfoOffsets { };
                    chunkinfo.ChunkName = VREF.ChunkNames[j];
                    //chunk id (4 bytes)
                    chunkinfo.ID = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);
                    offset += 4;
                    uint VDATOFFSET = (uint)offset;
                    chunkinfooffsets.VDAT_Offset_Location = (uint)offset;
                    chunkinfo.VDAT_Offset = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);
                    offset += 4;
                    uint VDATSIZE = (uint)offset;
                    chunkinfooffsets.VDAT_Size_Location = (uint)offset;
                    chunkinfo.VDAT_Size = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);
                    offset += 4;
                    chunkinfo.VDAT_File_Data_1 = VREF_Uncompressed[offset];
                    offset += 1;
                    chunkinfo.VDAT_File_Data_2 = VREF_Uncompressed[offset];
                    offset += 1;
                    if (VREF_Uncompressed.Count < (offset + 4))
                    {
                        chunkinfo.OffsetLocations = chunkinfooffsets;
                        VREF.ChunkInfos.Add(chunkinfo);
                    }
                    else
                    {
                        uint NextID = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);
                        if (NextID == chunkinfo.ID)
                        {
                            chunkinfo.HasVGPU = true;
                            offset += 4;
                            int VGPUOFFSET = offset;
                            chunkinfooffsets.VGPU_Offset_Location = (uint)offset;
                            chunkinfo.VGPU_Offset = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);
                            offset += 4;
                            int VGPUSIZE = offset;
                            chunkinfooffsets.VGPU_Size_Location = (uint)offset;
                            chunkinfo.VGPU_Size = ByteUtils.ToUInt32(VREF_Uncompressed, offset, pkg.IsBigEndian);
                            offset += 4;
                            chunkinfo.VGPU_File_Data_1 = VREF_Uncompressed[offset];
                            offset += 1;
                            chunkinfo.VGPU_File_Data_2 = VREF_Uncompressed[offset];
                            offset += 1;
                            chunkinfo.OffsetLocations = chunkinfooffsets;
                            VREF.ChunkInfos.Add(chunkinfo);
                        }
                        else
                        {
                            chunkinfo.HasVGPU = false;
                            chunkinfo.OffsetLocations = chunkinfooffsets;
                            VREF.ChunkInfos.Add(chunkinfo);
                        }
                    }
                }
                pkg.VREFs.Add(VREF);
            }

            // Add check for .dds files here (unlikely I'll ever need it for rando)

            return pkg;
        }

        public static Streams UpdateStreams_WithDoubleOffsetSpacing(Pkg pkg, VREF vref, CAFF caff, ChunkInfo targetChunkInfo, BYTES newVDAT, BYTES newVGPU)
        {
            var patchedStreams = new Streams();
            BYTES originalVDAT, originalVGPU;

            using (FileStream fs = new FileStream(pkg.Path, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                patchedStreams.VREF = GetVREFBYTES(reader, caff);
                originalVDAT = GetVDATBYTES(reader, pkg, (int)caff.CAFF_INFO.Number - 1);
                originalVGPU = GetVGPUBYTES(reader, pkg, (int)caff.CAFF_INFO.Number - 1);
            }

            patchedStreams.VDAT = new BYTES();
            patchedStreams.VGPU = new BYTES();

            foreach (var chunk in vref.ChunkInfos)
            {
                patchedStreams.VDAT = ByteUtils.ResizeList(patchedStreams.VDAT, (int)(chunk.VDAT_Offset * 2));

                BYTES chunkVDAT = (chunk.ChunkName == targetChunkInfo.ChunkName) ? newVDAT : ByteUtils.CopyBytes(originalVDAT, chunk.VDAT_Offset, chunk.VDAT_Size);
                patchedStreams.VDAT.InsertRange((int)(chunk.VDAT_Offset * 2), chunkVDAT);
                ByteUtils.SetIntAtOffset(patchedStreams.VREF, (int)chunk.OffsetLocations.VDAT_Offset_Location, (int)(chunk.VDAT_Offset * 2), pkg.IsBigEndian);

                if (chunk.HasVGPU)
                {
                    patchedStreams.VGPU = ByteUtils.ResizeList(patchedStreams.VGPU, (int)(chunk.VGPU_Offset * 2));
                    BYTES chunkVGPU = (chunk.ChunkName == targetChunkInfo.ChunkName) ? newVGPU : ByteUtils.CopyBytes(originalVGPU, chunk.VGPU_Offset, chunk.VGPU_Size);
                    patchedStreams.VGPU.InsertRange((int)(chunk.VGPU_Offset * 2), chunkVGPU);
                    ByteUtils.SetIntAtOffset(patchedStreams.VREF, (int)chunk.OffsetLocations.VGPU_Offset_Location, (int)(chunk.VGPU_Offset * 2), pkg.IsBigEndian);
                }
            }

            return patchedStreams;
        }

        public static Streams UpdateStreams_WithOneOffsetSpacing(Pkg pkg, VREF vref, CAFF caff, ChunkInfo targetChunkInfo, BYTES newVDAT, BYTES newVGPU)
        {
            var patchedStreams = new Streams();

            BYTES originalVDAT;
            BYTES originalVGPU;
            using (var fs = new FileStream(pkg.Path, FileMode.Open, FileAccess.Read))
            using (var reader = new BinaryReader(fs))
            {
                patchedStreams.VREF = GetVREFBYTES(reader, caff);
                originalVDAT = GetVDATBYTES(reader, pkg, (int)caff.CAFF_INFO.Number - 1);
                originalVGPU = GetVGPUBYTES(reader, pkg, (int)caff.CAFF_INFO.Number - 1);
            }

            patchedStreams.VDAT = new List<byte>();
            patchedStreams.VGPU = new List<byte>();

            foreach (var chunk in vref.ChunkInfos)
            {
                // --- VDAT Processing ---
                BYTES chunkVDAT;
                if (chunk.ChunkName == targetChunkInfo.ChunkName)
                {
                    chunkVDAT = newVDAT;
                }
                else
                {
                    chunkVDAT = ByteUtils.CopyBytes(originalVDAT, chunk.VDAT_Offset, chunk.VDAT_Size);
                }

                int newVDATOffset = patchedStreams.VDAT.Count;
                patchedStreams.VDAT.AddRange(chunkVDAT);
                ByteUtils.SetIntAtOffset(patchedStreams.VREF, (int)chunk.OffsetLocations.VDAT_Offset_Location, newVDATOffset, pkg.IsBigEndian);

                // --- VGPU Processing ---
                if (chunk.HasVGPU)
                {
                    BYTES chunkVGPU;
                    if (chunk.ChunkName == targetChunkInfo.ChunkName)
                    {
                        chunkVGPU = newVGPU;
                    }
                    else
                    {
                        chunkVGPU = ByteUtils.CopyBytes(originalVGPU, chunk.VGPU_Offset, chunk.VGPU_Size);
                    }

                    int newVGPUOffset = patchedStreams.VGPU.Count;
                    patchedStreams.VGPU.AddRange(chunkVGPU);
                    ByteUtils.SetIntAtOffset(patchedStreams.VREF, (int)chunk.OffsetLocations.VGPU_Offset_Location, newVGPUOffset, pkg.IsBigEndian);
                }
            }
            return patchedStreams;
        }

        public static Streams UpdateStreams_AppendEnd(Pkg pkg, VREF vref, CAFF caff, ChunkInfo chunkInfo, BYTES newVDAT, BYTES newVGPU)
        {
            var patchedStreams = new Streams();

            using (FileStream fs = new FileStream(pkg.Path, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                patchedStreams.VREF = GetVREFBYTES(reader, caff);
                patchedStreams.VDAT = GetVDATBYTES(reader, pkg, (int)caff.CAFF_INFO.Number - 1);
                patchedStreams.VGPU = GetVGPUBYTES(reader, pkg, (int)caff.CAFF_INFO.Number - 1);
            }

            // --- VDAT PATCHING (append with leading 0x10 padding) ---
            patchedStreams.VDAT.AddRange(new byte[0x10]);        // Optional padding
            int newVDATOffset = patchedStreams.VDAT.Count;       // Offset BEFORE adding anything
            patchedStreams.VDAT.AddRange(newVDAT);               // Insert new data
            ByteUtils.SetIntAtOffset(patchedStreams.VREF, (int)chunkInfo.OffsetLocations.VDAT_Offset_Location, newVDATOffset, pkg.IsBigEndian);
            ByteUtils.SetIntAtOffset(patchedStreams.VREF, (int)chunkInfo.OffsetLocations.VDAT_Size_Location, newVDAT.Count, pkg.IsBigEndian);

            // --- VGPU PATCHING (if present) ---
            if (chunkInfo.HasVGPU)
            {
                patchedStreams.VGPU.AddRange(new byte[0x10]);
                int newVGPUOffset = patchedStreams.VGPU.Count;
                patchedStreams.VGPU.AddRange(newVGPU);
                ByteUtils.SetIntAtOffset(patchedStreams.VREF, (int)chunkInfo.OffsetLocations.VGPU_Offset_Location, newVGPUOffset, pkg.IsBigEndian);
                ByteUtils.SetIntAtOffset(patchedStreams.VREF, (int)chunkInfo.OffsetLocations.VGPU_Size_Location, newVGPU.Count, pkg.IsBigEndian);
            }

            return patchedStreams;
        }

        public static BYTES UpdateCAFF(Pkg pkg, VREF vref, CAFF caff, Streams newStreams, bool bigEndian)
        {
            BYTES caffHeader;
            BYTES vlutRaw;

            using (FileStream fs = new FileStream(pkg.Path, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                caffHeader = ByteUtils.ReadBytesAtOffset(reader, caff.CAFF_INFO.Offset, (int)caff.VREF_Offset);
                vlutRaw = ByteUtils.ReadBytesAtOffset(reader, caff.VLUT_Offset + caff.CAFF_INFO.Offset, (int)caff.VLUT_Compressed_Size);
            }

            int vrefUncompressedSize = newStreams.VREF.Count;
            int vdatUncompressedSize = newStreams.VDAT.Count;
            int vgpuUncompressedSize = newStreams.VGPU.Count;

            BYTES vdatCompressed = ZlibHelper.CompressData(newStreams.VDAT);
            BYTES vgpuCompressed = ZlibHelper.CompressData(newStreams.VGPU);

            BYTES modifiedVRef = new BYTES(newStreams.VREF);
            ByteUtils.SetIntAtOffset(modifiedVRef, 9, vdatUncompressedSize, bigEndian);
            ByteUtils.SetIntAtOffset(modifiedVRef, 29, vdatCompressed.Count, bigEndian);
            ByteUtils.SetIntAtOffset(modifiedVRef, 42, vgpuUncompressedSize, bigEndian);
            ByteUtils.SetIntAtOffset(modifiedVRef, 62, vgpuCompressed.Count, bigEndian);

            BYTES vrefCompressed = ZlibHelper.CompressData(modifiedVRef);
            BYTES vlutCompressed = vlutRaw;

            // Zero out old checksum in header
            for (int i = 0x18; i < 0x1C; i++)
                caffHeader[i] = 0;

            ByteUtils.SetIntAtOffset(caffHeader, 80, vrefUncompressedSize, bigEndian);
            ByteUtils.SetIntAtOffset(caffHeader, 96, vrefCompressed.Count, bigEndian);

            uint newChecksum = ZlibHelper.CAFF_Checksum(caffHeader);
            ByteUtils.SetIntAtOffset(caffHeader, 0x18, (int)newChecksum, bigEndian);

            var newCaff = new BYTES();
            newCaff.AddRange(caffHeader);
            newCaff.AddRange(vrefCompressed);
            newCaff.AddRange(vlutCompressed);
            newCaff.AddRange(vdatCompressed);
            newCaff.AddRange(vgpuCompressed);

            return newCaff;
        }

        public static BYTES UpdatePKG(Pkg pkg, BYTES newCAFF, int caffNumber)
        {
            BYTES pkgHeader;
            BYTES caffData;

            using (FileStream fs = new FileStream(pkg.Path, FileMode.Open, FileAccess.Read))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                pkgHeader = ByteUtils.ReadBytesAtOffset(reader, 0, (int)pkg.Caff_Infos[caffNumber].Offset);
            }

            
            BYTES newPkg = new BYTES();
            List<CAFF_Info> newCaffInfos = new();

            for (int i = 0; i < pkg.Caff_Infos.Count; i++)
            {
                var originalInfo = pkg.Caff_Infos[i];
                var newInfo = new CAFF_Info
                {
                    Number = originalInfo.Number,
                    Unknown = originalInfo.Unknown
                };

                if (i == 0)
                {
                    newPkg.AddRange(pkgHeader);
                }

                int currentOffset = newPkg.Count;
                int nextOffset = ZlibHelper.NEAREST_MULTIPLE(currentOffset, 2048);
                if (nextOffset > newPkg.Count)
                {
                    newPkg.AddRange(new byte[nextOffset - newPkg.Count]);
                }

                newInfo.Offset = (uint)nextOffset;

                if (originalInfo.Number - 1 == caffNumber)
                {
                    newPkg.AddRange(newCAFF);
                    newInfo.Size = (uint)newCAFF.Count;
                }
                else
                {
                    using (FileStream fs = new FileStream(pkg.Path, FileMode.Open, FileAccess.Read))
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        caffData = ByteUtils.ReadBytesAtOffset(reader, originalInfo.Offset, (int)originalInfo.Size);
                    }
                    newPkg.AddRange(caffData);
                    newInfo.Size = (uint)caffData.Count;
                }

                newCaffInfos.Add(newInfo);
            }

            for (int i = 0; i < newCaffInfos.Count; i++)
            {
                int baseOffset = (int)((newCaffInfos[i].Number - 1) * 12) + 8;
                ByteUtils.SetIntAtOffset(newPkg, baseOffset, (int)newCaffInfos[i].Unknown, pkg.IsBigEndian);
                ByteUtils.SetIntAtOffset(newPkg, baseOffset + 4, (int)newCaffInfos[i].Offset, pkg.IsBigEndian);
                ByteUtils.SetIntAtOffset(newPkg, baseOffset + 8, (int)newCaffInfos[i].Size, pkg.IsBigEndian);
            }

            return newPkg;
        }

        public static void ReplaceChunk(Pkg pkg, int caffNumber, string chunkName, ChunkType type, BYTES patchData, string newPkgExportPath, bool overridePkg, bool dds = false)
        {

            BYTES originalVGPU;
            BYTES originalVDAT;

            for (int vrefIndex = 0; vrefIndex < pkg.VREFs.Count; vrefIndex++)
            {
                var vref = pkg.VREFs[vrefIndex];
                var caff = pkg.CAFFs[vrefIndex];

                for (int chunkIndex = 0; chunkIndex < vref.ChunkInfos.Count; chunkIndex++)
                {
                    var chunk = vref.ChunkInfos[chunkIndex];
                    if (chunk.ChunkName != chunkName) continue;

                    using (FileStream fs = new FileStream(pkg.Path, FileMode.Open, FileAccess.Read))
                    using (BinaryReader reader = new BinaryReader(fs))
                    {
                        originalVGPU = chunk.HasVGPU ? GetChunkVGPUBYTES(vrefIndex, chunkIndex, pkg, reader) : new BYTES();
                        originalVDAT = GetChunkVDATBYTES(reader, pkg, vrefIndex, chunkIndex);
                    }

                    Streams updatedStreams = type switch
                    {
                        ChunkType.VDAT => UpdateStreams_AppendEnd(pkg, vref, caff, chunk, patchData, originalVGPU),
                        ChunkType.VGPU => UpdateStreams_AppendEnd(pkg, vref, caff, chunk, originalVDAT, patchData),
                        _ => throw new InvalidOperationException("Unsupported ChunkType")
                    };

                    BYTES newCaff = UpdateCAFF(pkg, vref, caff, updatedStreams, pkg.IsBigEndian);
                    BYTES newPkg = UpdatePKG(pkg, newCaff, vrefIndex);

                    string backupDir = Path.Combine("Backups", "PackageBundles");
                    string pkgFileName = Path.GetFileName(pkg.Path);
                    string backupPath = Path.Combine(backupDir, pkgFileName);
                    string exportPath = Path.Combine(newPkgExportPath, "Patched_" + pkgFileName);

                    Directory.CreateDirectory(backupDir);

                    if (overridePkg)
                    {
                        if (!File.Exists(backupPath))
                            File.Copy(pkg.Path, backupPath);

                        File.Delete(pkg.Path);
                        File.WriteAllBytes(pkg.Path, newPkg.ToArray());
                    }
                    else
                    {
                        File.WriteAllBytes(exportPath, newPkg.ToArray());
                    }

                    return; // Done after first match
                }
            }
        }

        public static void ReplaceMultipleChunks(Pkg pkg, int caffNumber, Dictionary<string, BYTES> patchMap, string exportPath, bool overridePkg)
        {
            var vref = pkg.VREFs[caffNumber];
            var caff = pkg.CAFFs[caffNumber];

            var patchedStreams = new Streams();

            using (var reader = new BinaryReader(File.OpenRead(pkg.Path)))
            {
                patchedStreams.VREF = GetVREFBYTES(reader, caff);
                patchedStreams.VDAT = GetVDATBYTES(reader, pkg, caffNumber);
                patchedStreams.VGPU = GetVGPUBYTES(reader, pkg, caffNumber);
            }

            foreach (var chunk in vref.ChunkInfos)
            {
                if (!patchMap.TryGetValue(chunk.ChunkName, out var newVDAT))
                    continue;

                patchedStreams.VDAT.AddRange(new byte[0x10]);
                int newOffset = patchedStreams.VDAT.Count;
                patchedStreams.VDAT.AddRange(newVDAT);

                ByteUtils.SetIntAtOffset(patchedStreams.VREF, (int)chunk.OffsetLocations.VDAT_Offset_Location, newOffset, pkg.IsBigEndian);
                ByteUtils.SetIntAtOffset(patchedStreams.VREF, (int)chunk.OffsetLocations.VDAT_Size_Location, newVDAT.Count, pkg.IsBigEndian);
            }

            // Rebuild and write once
            BYTES newCaff = UpdateCAFF(pkg, vref, caff, patchedStreams, pkg.IsBigEndian);
            BYTES newPkg = UpdatePKG(pkg, newCaff, caffNumber);

            string backupDir = Path.Combine("Backups", "PackageBundles");
            string pkgFileName = Path.GetFileName(pkg.Path);
            string backupPath = Path.Combine(backupDir, pkgFileName);


            if (overridePkg)
            {
                if (!File.Exists(backupPath))
                    File.Copy(pkg.Path, backupPath);

                File.Delete(pkg.Path);
                File.WriteAllBytes(pkg.Path, newPkg.ToArray());
            }
            else
            {
                File.WriteAllBytes(exportPath, newPkg.ToArray());
            }

            return; // Done after first match
        }
    }
}
