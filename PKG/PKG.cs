using System;
using System.Collections.Generic;
using Randomizer.PKG;

namespace Randomizer.PKG
{
    public class Pkg
    {
        public string Path { get; set; }
        public uint Version { get; set; }
        public bool IsBigEndian { get; set; }
        public uint CaffCount { get; set; }
        public List<CAFF_Info> Caff_Infos { get; set; } = new();
        public List<CAFF> CAFFs { get; set; } = new();
        public List<VREF> VREFs { get; set; } = new();
    }
}