using Minecraft_Cheats_Lib;
using Minecraft_Cheats_Lib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Better_Craft.Models
{
    internal readonly struct MinecraftCheatInfo
    {
        public IMinecraftCheat MinecraftCheat { get; }
        public bool CanToggle { get; }
        
        internal MinecraftCheatInfo(IMinecraftCheat minecraftCheat, bool canToggle)
        {
            CanToggle = canToggle;
            MinecraftCheat = minecraftCheat;
        }
    }
}
