using Randomizer.Core;
using Randomizer.PKG;
using ReqBlock;
using System.IO;
using BYTES = System.Collections.Generic.List<byte>;

public static class RandomizerLogic
{
    public static void RunFullRandomization(Pkg pkg, string exportPath, string logPath)
    {
        var patchMap = new Dictionary<string, BYTES>();
        using StreamWriter logWriter = new StreamWriter(logPath);

        logWriter.WriteLine($"Full Randomization Log - {DateTime.Now}");
        logWriter.WriteLine($"PKG: {Path.GetFileName(pkg.Path)}\n");

        // Collect all chunk names
        var allChunks = new List<string>();
        allChunks.AddRange(TargetChunkNames.ResidentChunks());
        allChunks.AddRange(TargetChunkNames.MateChunks());

        foreach (string chunkName in allChunks)
        {
            string tag = TagGroup.GetRandomEatTag();
            var block = new RequirementBlock16(tag);
            var patch = block.ToBytes();

            patchMap[chunkName] = patch.ToList();
            logWriter.WriteLine($"Replaced: {chunkName} with RequirementBlock16 (Tag: {tag})");
        }

        PkgHandler.ReplaceMultipleChunks(pkg, caffNumber: 0, patchMap, exportPath, true);
        logWriter.WriteLine($"\nCompleted. Total patched chunks: {patchMap.Count}");
    }


    private static void HandleResidentChunks(Pkg pkg, Dictionary<string, BYTES> patchMap, StreamWriter log)
    {
        foreach (string chunkName in TargetChunkNames.ResidentChunks())
        {
            string tag = TagGroup.GetRandomEatTag();
            var block = new RequirementBlock16();
            patchMap[chunkName] = block.ToBytes().ToList();
            log.WriteLine($"[Resident] Replaced: {chunkName} with Tag: {tag}");
        }
    }
}