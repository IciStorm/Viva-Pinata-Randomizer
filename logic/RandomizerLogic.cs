using Randomizer.Core;
using Randomizer.PKG;
using ReqBlock;
using System.IO;
using System.Text;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using BYTES = System.Collections.Generic.List<byte>;

public static class RandomizerLogic
{
    public static void RunFullRandomization(Pkg pkg, string exportPath, string logPath)
    {
        var patchMap = new Dictionary<string, BYTES>();

        using StreamWriter logWriter = new StreamWriter(logPath);
        logWriter.WriteLine("=== Viva Piñata Randomizer Log ===");
        logWriter.WriteLine("Version: 0.0.1");
        logWriter.WriteLine("Seed: To be Implemented (NONE)\n\n");

        HandleAppearChunks(pkg, patchMap, logWriter);
        HandleVisitChunks(pkg, patchMap, logWriter);
        HandleResidentChunks(pkg, patchMap, logWriter);
        //HandleMateChunks(pkg, patchMap, logWriter);

        PkgHandler.ReplaceMultipleChunks(pkg, caffNumber: 0, patchMap, exportPath, overridePkg: true);

        logWriter.WriteLine($"\nCompleted. Total patched chunks: {patchMap.Count}");
    }


    public static (byte[] data, List<BlockStruct> blocks) BuildRandomRequirementSet(List<Func<BlockStruct>> blockPool)
    {
        int count = Random.Shared.Next(1, 5);
        int attempts = 0;
        var builder = new PatchFileBuilder();
        var blocks = new List<BlockStruct>();

        var usedTypes = new HashSet<Type>();

        while (blocks.Count < count && attempts < 10)
        {
            var factory = blockPool[Random.Shared.Next(blockPool.Count)];
            var block = factory();
            var type = block.GetType();

            if (BlockPools.SingleOnlyBlockTypes.Contains(type) && usedTypes.Contains(type))
            {
                attempts++;
                continue;
            }

            blocks.Add(block);
            usedTypes.Add(type);
            builder.AddBlock(block);
        }

        return (builder.Build(), blocks);
    }

    private static void HandleAppearChunks(Pkg pkg, Dictionary<string, BYTES> patchMap, StreamWriter log)
    {
        foreach (string chunkName in TargetChunkNames.AppearChunks())
        {
            var (data, blocks) = BuildRandomRequirementSet(BlockPools.AppearBlocks);
            patchMap[chunkName] = data.ToList();

            string species = LogNames.getChunkNameFromReqspinata(chunkName);
            string displayName = LogNames.InternalToDisplay.GetValueOrDefault(species, species);

            log.WriteLine($"[Appear] {displayName}:");
            foreach (var block in blocks)
            {
                log.WriteLine($" - {RequirementDescriptions.LogText(block)}");
            }
        }
    }

    private static void HandleVisitChunks(Pkg pkg, Dictionary<string, BYTES> patchMap, StreamWriter log)
    {
        foreach (string chunkName in TargetChunkNames.EnterChunks())
        {
            var (data, blocks) = BuildRandomRequirementSet(BlockPools.VisitBlocks);
            patchMap[chunkName] = data.ToList();

            string species = LogNames.getChunkNameFromReqspinata(chunkName);
            string displayName = LogNames.InternalToDisplay.GetValueOrDefault(species, species);

            log.WriteLine($"[Visit] {displayName}:");
            foreach (var block in blocks)
            {
                log.WriteLine($" - {RequirementDescriptions.LogText(block)}");
            }
        }
    }

    private static void HandleResidentChunks(Pkg pkg, Dictionary<string, BYTES> patchMap, StreamWriter log)
    {
        foreach (string chunkName in TargetChunkNames.ResidentChunks())
        {
            var (data, blocks) = BuildRandomRequirementSet(BlockPools.ResidentBlocks);
            patchMap[chunkName] = data.ToList();

            string species = LogNames.getChunkNameFromReqspinata(chunkName);
            string displayName = LogNames.InternalToDisplay.GetValueOrDefault(species, species);

            log.WriteLine($"[Resident] {displayName}:");
            foreach (var block in blocks)
            {
                log.WriteLine($" - {RequirementDescriptions.LogText(block)}");
            }
        }
    }

    private static void HandleVariantChunks(Pkg pkg, Dictionary<string, BYTES> patchMap, StreamWriter log)
    {
        foreach (string chunkName in TargetChunkNames.VariantChunks())
        {
            var (data, blocks) = BuildRandomRequirementSet(BlockPools.VariantBlocks);
            patchMap[chunkName] = data.ToList();

            string species = LogNames.getChunkNameFromReqspinata(chunkName);
            string displayName = LogNames.InternalToDisplay.GetValueOrDefault(species, species);

            log.WriteLine($"[Variant] {displayName}:");
            foreach (var block in blocks)
            {
                log.WriteLine($" - {RequirementDescriptions.LogText(block)}");
            }
        }
    }



}