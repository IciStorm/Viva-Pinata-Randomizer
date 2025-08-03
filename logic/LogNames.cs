public static class LogNames
{
    public static readonly Dictionary<string, string> InternalToDisplay = new()
    {
        { "ant", "Raisant" },
        { "badger", "Badgesicle" },
        { "bat", "Sherbat" },
        { "batpest", "Sour Sherbat" },
        { "bear", "Fizzlybear" },
        { "beaver", "Sweetooth" },
        { "bee", "Buzzlegum" },
        { "blackbutterfly", "Black Flutterscotch" },
        { "bluebottle", "Taffly" },
        { "bluebutterfly", "Blue Flutterscotch" },
        { "boomslang", "Twingersnap" },
        { "brownbutterfly", "Brown Flutterscotch" },
        { "bushbaby", "Galagoogoo" },
        { "buzzard", "Buzzenge" },
        { "canary", "Candary" },
        { "cat", "Kittyfloss" },
        { "chameleon", "Jameleon" },
        { "chicken", "Cluckles" },
        { "cow", "Moozipan" },
        { "crocodile", "Cocoadile" },
        { "crocodilepest", "Sour Cocoadile" },
        { "crow", "Crowla" },
        { "crowpest", "Sour Crowla" },
        { "deer", "Doenut" },
        { "dog", "Barkbark" },
        { "dragon", "Dragonache" },
        { "dragonfly", "Dragumfly" },
        { "duck", "Quackberry" },
        { "eagle", "Eaglair" },
        { "elephant", "Elephanilla" },
        { "firefly", "Reddhott" },
        { "firesalamander", "Salamango" },
        { "flyingpig", "Pigxie" },
        { "fox", "Pretztail" },
        { "frog", "Lickatoad" },
        { "goose", "Juicygoose" },
        { "grasssnake", "Syrupent" },
        { "greenbutterfly", "Green Flutterscotch" },
        { "hedgehog", "Fudgehog" },
        { "hippo", "Chippopotamus" },
        { "horse", "Horstachio" },
        { "hydra", "Fourheads" },
        { "lion", "Roario" },
        { "mandrill", "Bonboon" },
        { "mandrillpest", "Sour Bonboon" },
        { "mole", "Profitamole" },
        { "molepest", "Sour Profitamole" },
        { "monkey", "Cinnamonkey" },
        { "moth", "Mothdrop" },
        { "mouse", "Mousemallow" },
        { "newt", "Newtgat" },
        { "orangebutterfly", "Orange Flutterscotch" },
        { "parrot", "Parrybo" },
        { "pig", "Rashberry" },
        { "pigeon", "Pudgeon" },
        { "pinkbutterfly", "Pink Flutterscotch" },
        { "poisonfrog", "Lackatoad" },
        { "pony", "Ponocky" },
        { "purplebutterfly", "Purple Flutterscotch" },
        { "rabbit", "Bunnycomb" },
        { "raccoon", "Macaraccoon" },
        { "raccoonpest", "Sour Macaraccoon" },
        { "redbutterfly", "Red Flutterscotch" },
        { "sheep", "Goobaa" },
        { "snail", "Shellybean" },
        { "snailpest", "Sour Shellybean" },
        { "sparrow", "Sparrowmint" },
        { "spider", "Arocknid" },
        { "squirrel", "Squazzil" },
        { "swan", "Swanana" },
        { "unicorn", "Chewnicorn" },
        { "whitebutterfly", "White Flutterscotch" },
        { "wolf", "Mallowolf" },
        { "wolfpest", "Sour Mallowolf" },
        { "worm", "Whirlm" },
        { "yellowbutterfly", "Yellow Flutterscotch" },
        { "zebra", "Zumbug" },
    };

    public static string GetDisplayName(string internalName)
    {
        return InternalToDisplay.TryGetValue(internalName.ToLower(), out var display)
            ? display
            : internalName; // fallback if no match found
    }

    public static string getChunkNameFromReqspinata(string chunkName)
    {
        var parts = chunkName.Split('_');
        return parts[parts.Length - 2];
    }

    public static string getChunkNameFromTaggroup(string chunkName)
    {
        var parts = chunkName.Split('_');
        return parts[parts.Length - 1];
    }
}