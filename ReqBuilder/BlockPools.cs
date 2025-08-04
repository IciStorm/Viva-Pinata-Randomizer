using ReqBlock;
using System;
using System.Collections.Generic;

public static class BlockPools
{
    public static HashSet<Type> SingleOnlyBlockTypes = new()
    {
        typeof(RequirementBlock01), // Nighttime in the Garden
        typeof(RequirementBlock09), // Level X or better
        typeof(RequirementBlock1A)  // Garden Value
    };
    public static List<Func<BlockStruct>> AppearBlocks => new()
    {
        () => new RequirementBlock01(), // Nighttime in the Garden
        //() => new RequirementBlock02() // NULL Function (Disabled until I figure out how I want to use this)
        () => new RequirementBlock09(), // Level X or better
        () => new RequirementBlock0E() // Tag is in Garden
        //() => new RequirementBlock17(), // Days Elapsed in Game
        //() => new RequirementBlock19() // Surface Area with ID
    };

    public static List<Func<BlockStruct>> VisitBlocks => new()
    {
        //() => new RequirementBlock02() // NULL Function (Disabled until I figure out how I want to use this)
        //() => new RequirementBlock03() // OR Function
        () => new RequirementBlock09(), // Level X or better
        () => new RequirementBlock0E(), // Tag is in Garden
        //() => new RequirementBlock19() // Surface Area with ID
        () => new RequirementBlock1A(), // Garden Value
    };

    public static List<Func<BlockStruct>> ResidentBlocks => new()
    {
        //() => new RequirementBlock08() // Surface Area with Tag
        () => new RequirementBlock09(), // Level X or better
        () => new RequirementBlock0E(), // Tag is in Garden
        () => new RequirementBlock16(), // Has Eaten
        () => new RequirementBlock1A(), // Garden Value
        //() => new RequirementBlock1C(), // Has Romanced
        //() => new RequirementBlock1E() // Has Award
    };

    public static List<Func<BlockStruct>> MateBlocks => new()
    {
        () => new RequirementBlock16() // Has Eaten
    };

    public static List<Func<BlockStruct>> VariantBlocks => new()
    {
        () => new RequirementBlock16() // Has Eaten
    };

}
