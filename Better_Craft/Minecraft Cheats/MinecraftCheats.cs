/*
 * Minecraft Cheats for BetterCraft by: LordVirus 8/26/2022
 * 
 * Thanks to all of the scene members that did the extensive Minecraft ps3 reverse engineering that has made this possible! - LordVirus
 * 
 */

namespace Minecraft_Cheats
{
    using Better_Craft.Models;
    using Minecraft_Cheats_Lib;
    using Minecraft_Cheats_Lib.Implementations;
    using Minecraft_Cheats_Lib.Implementations.Cheats;
    using Minecraft_Cheats_Lib.Interfaces;
    using PS3Lib;
    using PS3ManagerAPI;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using MessageBox = System.Windows.MessageBox;

    public static class Minecraft_Cheats
    {
        #region Helpers

        public static readonly Dictionary<string, Func<PS3API, IMinecraftCheat>> Cheats =
            new Dictionary<string, Func<PS3API, IMinecraftCheat>>(StringComparer.OrdinalIgnoreCase)
            {
                { 
                    "GOD_MODE",
                    (PS3API) =>
                    { 
                        var ps3mem = new PS3Memory(PS3API); 
                        return new ToggleCheat(name: "God Mode", offset: 0x004B2021, onState: new byte[] { 0x80 }, offState: new byte[] { 0x20 }, reader: ps3mem, writer: ps3mem);
                    } 
                },
                {
                    "SUPER_SPEED",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Super Speed", offset: 0x003ABD49, onState: new byte[] { 0xFF, 0xFF, 0xFF }, offState: new byte[] { 0x26, 0xAD, 0x89 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "MULTI_JUMP",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Multi Jump", offset: 0x0022790B, onState: new byte[] { 0x14 }, offState: new byte[] { 0x18 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "INSTANT_MINE",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Instant Mine", offset: 0x00AEB090, onState: new byte[] { 0xBF }, offState: new byte[] { 0x3F }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "INSTANT_KILL",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Instant Kill", offset: 0x001AC412, onState: new byte[] { 0x28 }, offState: new byte[] { 0x08 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "INSTANT_HIT",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Instant Hit", offset: 0x003A3FF0, onState: new byte[] { 0x40, 0x80 }, offState: new byte[] { 0x3F, 0x00 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "INFINITE_CRAFT",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Infinite Craft", offset: 0x0098871F, onState: new byte[] { 0x01 }, offState: new byte[] { 0x00 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "FAST_BOW",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Fast Bow", offset: 0x000FB4C6, onState: new byte[] { 0x18 }, offState: new byte[] { 0x08 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "FAR_KNOCKBACK",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Far Knockback", offset: 0x003A4018, onState: new byte[] { 0x40, 0x80 }, offState: new byte[] { 0x3E, 0xCC }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "CAVE_XRAY",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Cave Xray", offset: 0x00A99155, onState: new byte[] { 0x80 }, offState: new byte[] { 0x60 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "ANTI_KNOCKBACK",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Anti Knockback", offset: 0x003A4018, onState: new byte[] { 0x00, 0x00 }, offState: new byte[] { 0x3E, 0xCC }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "REMOVE_JUMP",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Remove Jump", offset: 0x003ABDC9, onState: new byte[] { 0xF4 }, offState: new byte[] { 0xB4 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "DISABLE_SWIM",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Disable Swim", offset: 0x003ABD40, onState: new byte[] { 0xBF }, offState: new byte[] { 0x3F }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "AUTO_MINE",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Auto Mine", offset: 0x00AEC42C, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "AUTO_HIT",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Auto Hit", offset: 0x00AEC34C, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "CHANGE_MOVEMENT_SWIM",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Change Movement To Swim", offset: 0x003ABD44, onState: new byte[] { 0xBC }, offState: new byte[] { 0x3C }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "BLUE_CLOUDS",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Blue Clouds", offset: 0x0038B964, onState: new byte[] { 0xFF }, offState: new byte[] { 0x3D }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "SPEED_CLOUDS",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Speed Clouds", offset: 0x00B230AD, onState: new byte[] { 0x70 }, offState: new byte[] { 0x80 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "BURN_IN_WATER",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Burn In Water", offset: 0x00225EA8, onState: new byte[] { 0x41 }, offState: new byte[] { 0x40 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "MAX_PICKUP_ITEMS",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Max Item Pickups", offset: 0x00310AD4, onState: new byte[] { 0x41 }, offState: new byte[] { 0x40 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "FULL_BRIGHT",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Full Bright", offset: 0x00A9A6C8, onState: new byte[] { 0x7F }, offState: new byte[] { 0x3F }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "KILL_AURA",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Kill Aura", offset: 0x00233290, onState: new byte[] { 0xFF }, offState: new byte[] { 0x00 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "FAST_BUILD",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Fast Build", offset: 0x00AECE70, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "CAN_FLY",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Fly Mode", offset: 0x00B02378, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "REMOVE_STARS_IN_SKY",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Remove Stars In The Sky", offset: 0x0038C658, onState: new byte[] { 0xFF }, offState: new byte[] { 0x3F }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "NO_DAMAGE_HIT",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "No Damage Hit", offset: 0x003A3FF0, onState: new byte[] { 0xFF, 0xFF }, offState: new byte[] { 0x3F, 0x00 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "INFINITE_BLOCK",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Infinite Blocks", offset: 0x0010673F, onState: new byte[] { 0x00 }, offState: new byte[] { 0x01 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "CRITICAL_HIT",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Critical Hit", offset: 0x003ABDD1, onState: new byte[] { 0xAF }, offState: new byte[] { 0xEF }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "CANT_GRAB_ITEMS",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Disable Item Pickup", offset: 0x00310B0C, onState: new byte[] { 0x41 }, offState: new byte[] { 0x40 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "DISABLE_CHANGING_WEATHER",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Disabled Weather Changing", offset: 0x00393E84, onState: new byte[] { 0x41 }, offState: new byte[] { 0x40 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "ROBLOX_WALK",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Roblox Walk", offset: 0x00A857D1, onState: new byte[] { 0x00 }, offState: new byte[] { 0x80 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "CREEPER_INSTANT_EXPLODE",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Instant Creeper Explosions", offset: 0x001CCC2C, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "TNT_INSTANT_EXPLODE",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Instant TNT Explosions", offset: 0x0051E6A0, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "REMOVE_FALL_DAMAGE",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Remove Fall Damage", offset: 0x003A409C, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "ALL_PLAYERS_FAST_MINE",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Fast Mine For All Players", offset: 0x0010E0C6, onState: new byte[] { 0x18 }, offState: new byte[] { 0x08 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "WALL_HACK",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Wall Hack", offset: 0x00A98F50, onState: new byte[] { 0x3F }, offState: new byte[] { 0x3D }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "PLAYERS_SLIDE",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Players Slide On Blocks", offset: 0x003AAA98, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "REVERSE_KNOCKBACK",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Reverse Knockback", offset: 0x003A4018, onState: new byte[] { 0xBF, 0x80 }, offState: new byte[] { 0x3E, 0xCC }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "ESP_CHESTS",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Chest ESP", offset: 0x00A9C2B4, onState: new byte[] { 0x3E, 0xFF }, offState: new byte[] { 0x3F, 0x80 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "TNT_CANT_EXPLODE_BLOCKS",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "TNT Can't Destroy Blocks", offset: 0x00245DF0, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "DEMI_GOD",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Demi God", offset: 0x003A4066, onState: new byte[] { 0x88 }, offState: new byte[] { 0x08 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "GRAVITY_MOON",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Moon Gravity", offset: 0x003ABF88, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "FLAT_BLOCKS",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Flat Blocks", offset: 0x000924FF, onState: new byte[] { 0x01 }, offState: new byte[] { 0x00 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "AUTO_SPRINT",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Auto Sprint", offset: 0x00B01EEF, onState: new byte[] { 0x00 }, offState: new byte[] { 0x01 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "FORCE_SNOW",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Force Snow", offset: 0x00A9B986, onState: new byte[] { 0x58 }, offState: new byte[] { 0x08 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "FORCE_RAIN",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Force Rain", offset: 0x00A9B23E, onState: new byte[] { 0x48 }, offState: new byte[] { 0x08 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "SKY_TO_NETHER",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Nether Skybox", offset: 0x00B22050, onState: new byte[] { 0x41 }, offState: new byte[] { 0x40 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "SMOKE_LOBBY",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Smokey Overworld", offset: 0x00B24177, onState: new byte[] { 0x01 }, offState: new byte[] { 0x00 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "REMOVE_HANDS",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Remove Hands", offset: 0x00AF10AB, onState: new byte[] { 0x01 }, offState: new byte[] { 0x00 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "SHOW_ARMOR",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Show Armor", offset: 0x0090B5F3, onState: new byte[] { 0x01 }, offState: new byte[] { 0x00 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "REMOVE_RUN_ANIMATION",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Remove Run Animation", offset: 0x00227BDC, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "ALL_PLAYERS_TAKE_DAMAGE",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Damage All Players", offset: 0x0039E2D4, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "DISABLE_HUD_TEXT",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Disable Hud Text", offset: 0x008FC4B4, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "MINE_IN_ADVENTURE",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Mine In Adventure Mod", offset: 0x002F0273, onState: new byte[] { 0x00 }, offState: new byte[] { 0x01 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "BOAT_STOP_WORKING",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Boats Stop Working", offset: 0x000E0F90, onState: new byte[] { 0x41 }, offState: new byte[] { 0x40 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "GAMMA_TO_MAX",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Super Gamma", offset: 0x00A9C2B5, onState: new byte[] { 0xFF }, offState: new byte[] { 0x80 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "TRIDENT_RIPTIDE_TO_MAX",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Super Trident Riptide", offset: 0x00217DCF, onState: new byte[] { 0x08 }, offState: new byte[] { 0x00 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "NO_BLOCK_COLISSION",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Disabled Block Colission", offset: 0x000108AC, onState: new byte[] { 0x41 }, offState: new byte[] { 0x40 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "FROST_WALKER",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Force Frost Walker", offset: 0x00218A4F, onState: new byte[] { 0x01 }, offState: new byte[] { 0x00 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "NO_WEB_HAX",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Disabled Web Slowdown", offset: 0x00234F9F, onState: new byte[] { 0x00 }, offState: new byte[] { 0x01 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "ENTITY_GOD_MODE",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Entity God Mode", offset: 0x003A3F6C, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "DISABLE_RESPAWN",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Disable Respawns", offset: 0x00AF1EE0, onState: new byte[] { 0x4E, 0x80, 0x00, 0x20 }, offState: new byte[] { 0xF8, 0x21, 0xFD, 0x21 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "SPECTRAL_ARROWS_WITH_BOW",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Force Spectral Arrows From Bow", offset: 0x014C90D5, onState: new byte[] { 0x20, 0x8D }, offState: new byte[] { 0x1E, 0xAD }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "CHANGE_AIR_TO_WATER",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Change The Air To Water", offset: 0x001D7FCC, onState: new byte[] { 0x40 }, offState: new byte[] { 0x41 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "ALL_PLAYERS_LEFT_HAND",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Left Handed Mode", offset: 0x0151F2F3, onState: new byte[] { 0xF0 }, offState: new byte[] { 0xF8 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "DERP_WALK",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Derp Walk", offset: 0x002341D0, onState: new byte[] { 0xC3 }, offState: new byte[] { 0xC0 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "INFINITE_OXYGEN_IN_WATER",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Infinite Water Breathing", offset: 0x0039DE28, onState: new byte[] { 0x41 }, offState: new byte[] { 0x40 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "PLAYERS_TO_BABY",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Play As A Baby", offset: 0x0039F52F, onState: new byte[] { 0x01 }, offState: new byte[] { 0x00 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "DISABLE_KILLED_OUT_OF_WORLD",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Disable Void Death", offset: 0x003A9350, onState: new byte[] { 0x4E, 0x80, 0x00, 0x20 }, offState: new byte[] { 0xF8, 0x21, 0xFF, 0x91 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "FREEZE_ALL_ENTITY",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Freeze All Entitys", offset: 0x003A9FE8, onState: new byte[] { 0x4E, 0x80, 0x00, 0x20 }, offState: new byte[] { 0xF8, 0x21, 0xFF, 0x81 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "DISABLE_PORTALS",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Disable Portals", offset: 0x002379E7, onState: new byte[] { 0x00 }, offState: new byte[] { 0x01 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "ALL_PLAYERS_SUFFOCATE",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Suffocate All Players", offset: 0x0022FDC8, onState: new byte[] { 0x4E, 0x80, 0x00, 0x20 }, offState: new byte[] { 0xF8, 0x21, 0xFF, 0x11 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "ELYTRA_CAPES",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Force Show Elytra Cape", offset: 0x014C93D9, onState: new byte[] { 0x1C, 0x0A, 0x60 }, offState: new byte[] { 0x20, 0x94, 0x50 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "CREATIVE_INVENTORY",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Creative Inventory", offset: 0x00AACEDC, onState: new byte[] { 0x4E, 0x80, 0x00, 0x20 }, offState: new byte[] { 0xF8, 0x21, 0xFF, 0x71 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "STOP_CHUNK_LOADING",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Disable Chunk Loading", offset: 0x00B2437C, onState: new byte[] { 0x4E, 0x80, 0x00, 0x20 }, offState: new byte[] { 0xF8, 0x21, 0xFF, 0x71 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "OPTIMIZE_CHUNKS_LOAD",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Optimize Chunk Loading", offset: 0x00B21C61, onState: new byte[] { 0xD7 }, offState: new byte[] { 0x30 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "NETHER_PORTAL_WITH_DIRT",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Build Dirt Nether Portals", offset: 0x014C89FE, onState: new byte[] { 0x14, 0x70 }, offState: new byte[] { 0x5E, 0x70 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "NETHER_PORTAL_WITH_STONE",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Build Stone Nether Portals", offset: 0x014C89FE, onState: new byte[] { 0x11, 0xC0 }, offState: new byte[] { 0x5E, 0x70 }, reader: ps3mem, writer: ps3mem);
                    }
                },
                {
                    "DEATH_SCREEN_VISION",
                    (PS3API) =>
                    {
                        var ps3mem = new PS3Memory(PS3API);
                        return new ToggleCheat(name: "Death Screen Vision", offset: 0x003A7654, onState: new byte[] { 0x41 }, offState: new byte[] { 0x40 }, reader: ps3mem, writer: ps3mem);
                    }
                },
            };


        /// <summary>
        /// Current API Instance.
        /// </summary>
        private static PS3API PS3;
        //private static PS3Memory ps3Memory = new PS3Memory(PS3);

        /// <summary>
        /// Are we connected?
        /// </summary>
        private static bool Connected = false;

        /// <summary>
        /// The time needed to wait between async looping opperations.
        /// This really is used to lighten the load on PS3MAPI becuase it is painfully slow.
        /// </summary>
        private static int WaitTime = 300;

        /// <summary>
        /// Helper class file.
        /// </summary>
        public static class HelperFunctions
        {
            /// <summary>
            /// Set the API.
            /// </summary>
            public static PS3API CurrentPS3Api
            {
                get { return PS3; }
            }

            /// <summary>
            /// Are we connected?
            /// </summary>
            public static bool isConnected
            {
                get { return Connected; }
            }

            /// <summary>
            /// Connect to the damn ps3.
            /// </summary>
            public static void Connect(SelectAPI Api)
            {
                try
                {
                    Connected = false;

                    Minecraft_Cheats.PS3 = new PS3API();
                    Minecraft_Cheats.PS3.ChangeAPI(Api);

                    if (PS3.ConnectTarget())
                    {
                        // Connected and attached.
                        if (PS3.AttachProcess())
                        {
                            if (Api.Equals(SelectAPI.ControlConsole))
                            {
                                PS3.CCAPI.Notify(CCAPI.NotifyIcon.TROPHY2, "Successfully connected and attached cheat tool to Minecraft!");
                                PS3.CCAPI.RingBuzzer(CCAPI.BuzzerMode.Double);
                            }

                            else if(Api.Equals(SelectAPI.PS3Manager))
                            {
                                PS3.PS3MAPI.Notify("Successfully connected and attached cheat tool to Minecraft!");
                                PS3.PS3MAPI.RingBuzzer(PS3MAPI.PS3_CMD.BuzzerMode.Double);
                                WaitTime = 1500;
                            }

                            if(!Api.Equals(SelectAPI.PS3Manager))
                            {
                                WaitTime = 300;
                            }

                            MessageBox.Show($"Connected and Attached with \"{Api} API\"", "Status", MessageBoxButton.OK, MessageBoxImage.Information);
                            Connected = true;
                        }

                        // Failed to attach.
                        else
                        {
                            if (Api.Equals(SelectAPI.ControlConsole))
                            {
                                PS3.CCAPI.Notify(CCAPI.NotifyIcon.WRONGWAY, "Failed to attach to Minecraft...");
                                PS3.CCAPI.RingBuzzer(CCAPI.BuzzerMode.Single);
                                MessageBox.Show($"Connected, but failed to attach with: \"{Api} API\"", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            else if (Api.Equals(SelectAPI.PS3Manager))
                            {
                                PS3.PS3MAPI.Notify("Failed to attach to Minecraft...");
                                PS3.PS3MAPI.RingBuzzer(PS3MAPI.PS3_CMD.BuzzerMode.Single);
                                MessageBox.Show($"Connected, but failed to attach with: \"{Api} API\"", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                            }

                            else if (Api.Equals(SelectAPI.TargetManager))
                                MessageBox.Show($"Connected, but failed to attach with: \"{Api} API\" Are you using a debug eboot.bin?", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);

                            PS3 = null;
                        }
                    }

                    // Failed to connect.
                    else
                    {
                        MessageBox.Show($"Failed to connect & attach with: \"{Api}\" API", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        PS3 = null;
                    }
                }

                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);

                    //string tmapiError = "Unable to load DLL 'PS3TMAPI.dll': The specified module could not be found. (Exception from HRESULT: 0x8007007E)";
                    if (e.Message.Contains("PS3TMAPI.dll"))
                    {
                        MessageBox.Show("This probably means you need to install the ProDG tmapi software.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                    }

                    PS3 = null;
                }
            }

            /// <summary>
            /// Disconnect.
            /// </summary>
            public static void Disconnect()
            {
                if (Minecraft_Cheats.HelperFunctions.isConnected)
                {
                    MessageBoxResult YesNo = MessageBox.Show("Would you like to disable all cheats before disconnecting?", "Disconnection", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (YesNo.Equals(MessageBoxResult.Yes))
                    {
                        Minecraft_Cheats.HelperFunctions.Reset_All_Mods();

                        //if (LSD_TRIP)
                        //    LSD_TRIP = false;
                        //if (RAINBOW_HUD)
                        //    RAINBOW_HUD = false;
                        //if (RAINBOW_SKY)
                        //    RAINBOW_SKY = false;
                        //if (RAINBOW_VISION)
                        //    RAINBOW_VISION = false;
                        //if (MOVE_WITH_INVENTORY_OPENED)
                        //    MOVE_WITH_INVENTORY_OPENED = false;
                    }

                    if (CurrentPS3Api.GetCurrentAPI().Equals(SelectAPI.ControlConsole))
                    {
                        PS3.CCAPI.RingBuzzer(CCAPI.BuzzerMode.Single);
                        PS3.CCAPI.Notify(CCAPI.NotifyIcon.WRONGWAY, "Disconnected cheat tool from Minecraft!");
                    }

                    else if (CurrentPS3Api.GetCurrentAPI().Equals(SelectAPI.PS3Manager))
                    {
                        PS3.PS3MAPI.RingBuzzer(PS3MAPI.PS3_CMD.BuzzerMode.Single);
                        PS3.PS3MAPI.Notify("Disconnected cheat tool from Minecraft!");
                    }
                    
                    PS3.DisconnectTarget();

                    Connected = false;

                    MessageBox.Show("Disconnected from your Playstation 3", "Status", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                else
                {
                    MessageBox.Show("You must be connected to your PlayStation 3 to disconnect from it.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            /// <summary>
            /// Toggles the cheat specified by the name 
            /// </summary>
            /// <param name="name">The name of the cheat to toggle</param>
            /// <exception cref="ArgumentException">Throws if invalid name</exception>
            public static void ToggleCheat(string name)
            {
                if (!Cheats.ContainsKey(name))
                    throw new ArgumentException("The passed cheat name was not found in the collection", nameof(name));


                //TODO: The cheat system shouldn't have to instantiate everytime, should be using a better ps3 api state
                var cheatResult = Cheats[name](PS3);

                if (!(cheatResult is IToggleCheat toggleCheat))
                    throw new ArgumentException("The cheat specified is not a toggle cheat", nameof(name));

                toggleCheat.Toggle();
            }


            ///// <summary>
            ///// Toggles a mod.
            ///// </summary>
            ///// <param name="ModOption">The static function for a Minecraft_Cheats mod.</param>
            //public static dynamic ToggleOption<T>(Expression<Func<T>> ModOption)
            //{
            //    try
            //    {
            //        // Get propertyInfo and make sure it exists.
            //        PropertyInfo propertyInfo = ((MemberExpression)ModOption.Body).Member as PropertyInfo;

            //        if (propertyInfo is null)
            //        {
            //            throw new ArgumentException("The lambda expression 'ModOption' should point to a valid mod Property.");
            //        }

            //        // Get the name of the property.
            //        string propertyName = propertyInfo.Name;

            //        var property = typeof(Minecraft_Cheats).GetProperty(propertyName, BindingFlags.Static | BindingFlags.Public);

            //        if (property is null)
            //        {
            //            throw new ArgumentException("The lambda expression 'ModOption' should be a property of 'Minecraft_Cheats'");
            //        }

            //        // Get the value of the property.
            //        dynamic value = propertyInfo.GetValue(null/*Static class*/);

            //        // If this toggle has multible toggle states
            //        if (value is int)
            //        {
            //            if (Attribute.IsDefined(propertyInfo, typeof(ToggleState)).Equals(false))
            //            {
            //                throw new ArgumentException("Your int based toggle must contain the 'ToggleState' attribute!");
            //            }

            //            ToggleState toggleStateAttribute = (ToggleState)propertyInfo.GetCustomAttribute(typeof(ToggleState));
            //            int minSize = toggleStateAttribute.MinValue;
            //            int maxSize = toggleStateAttribute.MaxValue;

            //            // Reset to 0 from max size
            //            if (value.Equals(maxSize) && minSize.Equals(0))
            //            {
            //                propertyInfo.SetValue(null/*Static class*/, 0);
            //            }

            //            // Reset to 0 from min size.
            //            else if (value.Equals(minSize) && minSize < 0)
            //            {
            //                propertyInfo.SetValue(null/*Static class*/, 0);
            //            }

            //            // We are at max size, and min value is less than 0, we then start at -1.
            //            else if(value.Equals(maxSize) && minSize < 0)
            //            {
            //                propertyInfo.SetValue(null/*Static class*/, -1);
            //            }

            //            // We are at less than 0, deincrement until min size
            //            else if (value < 0)
            //            {
            //                propertyInfo.SetValue(null/*Static class*/, --value);
            //            }

            //            // Increase toggle by 1.
            //            else
            //            {
            //                propertyInfo.SetValue(null/*Static class*/, ++value);
            //            }

            //            return propertyInfo.GetValue(null/*Static class*/);
            //        }

            //        // Toggle current state.
            //        else if (value is bool)
            //        {
            //            propertyInfo.SetValue(null/*Static class*/, !value);
            //            return propertyInfo.GetValue(null/*Static class*/);
            //        }

            //        else
            //        {
            //            throw new ArgumentException("ModOption should be a property of the 'Minecraft_Cheats' class that is either an int or bool!");
            //        }
            //    }

            //    catch (Exception Ex)
            //    {
            //        MessageBox.Show(Ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            //        return null;
            //    }
            //}

            /// <summary>
            /// Resets all of the mods.
            /// </summary>
            public static void Reset_All_Mods()
            {
                foreach(var kvp in Cheats)
                {
                    //TODO: The cheat system shouldn't have to instantiate everytime, should be using a better ps3 api state
                    var cheatResult = kvp.Value(PS3);
                    if (cheatResult is IToggleCheat toggleCheat)
                    {
                        if (toggleCheat.IsOn)
                            toggleCheat.Toggle();
                        continue;
                    }

                    if (cheatResult is IMultiStateCheat multiStateCheat)
                    {

                    }

                }
                //PropertyInfo[] cheats = typeof(Minecraft_Cheats).GetProperties();

                //foreach(PropertyInfo cheat in cheats)
                //{
                //    dynamic cheatValue = cheat.GetValue(null);

                //    if(cheatValue is int)
                //    {
                //        cheat.SetValue(null, 0);
                //    }

                //    else if (cheatValue is bool)
                //    {
                //        cheat.SetValue(null, false);
                //    }
                //}
            }
        }

        ///// <summary>
        ///// Custom toggle states being tracked as attributes.
        ///// </summary>
        //[AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
        //private class ToggleState : Attribute
        //{
        //    private int maxValue;
        //    private int minValue;

        //    public ToggleState(int maxValue, int minValue = 0)
        //    {
        //        try
        //        {
        //            if (minValue > 0)
        //                throw new ArgumentException("MinValue for the 'ToggleState' attribute must be less than 1!");

        //            this.maxValue = maxValue;
        //            this.minValue = minValue;
        //        }

        //        catch (Exception Ex)
        //        {
        //            MessageBox.Show(Ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
        //            Process.GetCurrentProcess().Kill();
        //        }
        //    }

        //    /// <summary>
        //    /// The max value of this toggle state.
        //    /// </summary>
        //    public int MaxValue
        //    {
        //        get { return maxValue; }
        //    }

        //    /// <summary>
        //    /// The min value for this toggle state
        //    /// (Must be less than 1!)
        //    /// </summary>
        //    public int MinValue
        //    {
        //        get { return minValue; }
        //    }


        //    /// <summary>
        //    /// Throw a error for out of bounds value passed to a mod property of the type 'int'.
        //    /// </summary>
        //    public static void ErrorToggleState(ToggleState toggleState)
        //    {
        //        MessageBox.Show($"ToggleState out of bounds!\nThe max value for this toggle is: {toggleState.MaxValue}\nThe min value for this toggle state is: {toggleState.MinValue}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        //    }
        //}

        #endregion

        //#region "Int Toggles"


        //[ToggleState(2)]
        //public static int SUPER_JUMP
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        byte[] buffer = new byte[4];
        //        PS3.GetMemory(0x003AA77C, buffer);

        //        if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3E, 0xD7, 0x0A, 0x3D }))
        //            return 0;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x47, 0x7F, 0x42 }))
        //            return 1;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0xD7, 0x0A, 0x3D }))
        //            return 2;
        //        else
        //        {
        //            PS3.SetMemory(0x004B2021, new byte[] { 0x3E, 0xD7, 0x0A, 0x3D });
        //            return 0;
        //        }

        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        uint offset = 0x003AA77C;

        //        if (value.Equals(0))
        //        {
        //            Minecraft_Cheats.REMOVE_FALL_DAMAGE = false;
        //            PS3.SetMemory(offset, new byte[] { 0x3E, 0xD7, 0x0A, 0x3D }); ////SET to default
        //        }

        //        else if(value.Equals(1))
        //        {
        //            Minecraft_Cheats.REMOVE_FALL_DAMAGE = true;
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x47, 0x7F, 0x42 });
        //        }

        //        else if(value.Equals(2))
        //        {
        //            Minecraft_Cheats.REMOVE_FALL_DAMAGE = true;
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0xD7, 0x0A, 0x3D });
        //        }

        //        else
        //            ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //    }
        //}

        //[ToggleState(5)]
        //public static int TNT_EXPLOSION_SIZE
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        byte[] buffer = new byte[2];
        //        PS3.GetMemory(0x0051E0D0, buffer);

        //        if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40, 0x80 }))
        //            return 0;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00, 0x00 }))
        //            return 1;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41, 0x30 }))
        //            return 2;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41, 0x99 }))
        //            return 3;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x42, 0x00 }))
        //            return 4;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x43, 0x00 }))
        //            return 5;
        //        else
        //        {
        //            PS3.SetMemory(0x0051E0D0, new byte[] { 0x40, 0x80 });
        //            return 0;
        //        }
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        uint offset = 0x0051E0D0;

        //        if (value.Equals(0))
        //            PS3.SetMemory(offset, new byte[] { 0x40, 0x80 }); ////SET to default

        //        else if (value.Equals(1))
        //            PS3.SetMemory(offset, new byte[] { 0x00, 0x00 }); //No explosion

        //        else if (value.Equals(2))
        //            PS3.SetMemory(offset, new byte[] { 0x41, 0x30 }); //Small increase

        //        else if (value.Equals(3))
        //            PS3.SetMemory(offset, new byte[] { 0x41, 0x99 }); //Medium increase

        //        else if (value.Equals(4))
        //            PS3.SetMemory(offset, new byte[] { 0x42, 0x00 }); //Extreme increase

        //        else if (value.Equals(5))
        //        {
        //            MessageBox.Show("This selection will seriously lag your ps3.", "Notice!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //            PS3.SetMemory(offset, new byte[] { 0x43, 0x00 }); //Nuclear Explosion
        //        }

        //        else
        //            ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //    }
        //}

        //[ToggleState(4)]
        //public static int CREEPER_EXPLOSION_SIZE
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        byte[] buffer = new byte[2];
        //        PS3.GetMemory(0x001CC7E0, buffer);

        //        if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80 }))
        //            return 0;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00, 0x00 }))
        //            return 1;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41, 0x30 }))
        //            return 2;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41, 0x99 }))
        //            return 3;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x42, 0x80 }))
        //            return 4;
        //        else
        //        {
        //            PS3.SetMemory(0x001CC7E0, new byte[] { 0x3F, 0x80 });
        //            return 0;
        //        }
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        uint offset = 0x001CC7E0;

        //        if (value.Equals(0))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x80 }); ////SET to default

        //        else if (value.Equals(1))
        //            PS3.SetMemory(offset, new byte[] { 0x00, 0x00 }); //No explosion

        //        else if (value.Equals(2))
        //            PS3.SetMemory(offset, new byte[] { 0x41, 0x30 }); //Small Explosion

        //        else if (value.Equals(3))
        //            PS3.SetMemory(offset, new byte[] { 0x41, 0x99 }); //Medium Explosion

        //        else if (value.Equals(4))
        //        {
        //            MessageBox.Show("This selection will seriously lag your ps3.", "Notice!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        //            PS3.SetMemory(offset, new byte[] { 0x42, 0x80 }); //Large Explosion
        //        }

        //        else
        //            ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //    }
        //}

        //[ToggleState(11)]
        //public static int FOV_VALUE
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        byte[] buffer = new byte[3];
        //        PS3.GetMemory(0x014C670C, buffer);

        //        if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00 }))
        //            return 0;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x70, 0x00 }))
        //            return 1;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x60, 0x00 }))
        //            return 2;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x50, 0x00 }))
        //            return 3;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x40, 0x00 }))
        //            return 4;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x30, 0x00 }))
        //            return 5;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x25, 0x00 }))
        //            return 6;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x20, 0x00 }))
        //            return 7;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x15, 0x00 }))
        //            return 8;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x10, 0x00 }))
        //            return 9;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x1F, 0x80, 0x00 }))
        //            return 10;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0xFF, 0xFF }))
        //            return 11;
        //        else
        //        {
        //            PS3.SetMemory(0x014C670C, new byte[] { 0x3F, 0x80, 0x00 });
        //            return 0;
        //        }
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        uint offset = 0x014C670C;

        //        if (value.Equals(0))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00 }); //RESET

        //        else if (value.Equals(1))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x70 }); //X1

        //        else if (value.Equals(2))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x60 });//X2

        //        else if (value.Equals(3))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x50 }); //X3

        //        else if (value.Equals(4))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x40 });//X4

        //        else if (value.Equals(5))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x30 }); //X5

        //        else if (value.Equals(6))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x25 }); //X6

        //        else if (value.Equals(7))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x20 }); //X7

        //        else if (value.Equals(8))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x15 }); //X8 

        //        else if (value.Equals(9))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x10 }); //X9

        //        else if (value.Equals(10))
        //            PS3.SetMemory(offset, new byte[] { 0x1F, 0x80 }); //Updside Down

        //        else if (value.Equals(11))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0xFF, 0xFF }); //ZOOM

        //        else
        //            ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //    }
        //}

        //[ToggleState(7)]
        //public static int SKY_COLORS
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        if (!bRAINBOW_SKY)
        //        {
        //            byte[] buffer1 = new byte[2];
        //            byte[] buffer2 = new byte[2];
        //            PS3.GetMemory(0x00410734, buffer1);
        //            PS3.GetMemory(0x00410738, buffer2);

        //            if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x40, 0xC0 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0x3F, 0x80 }))
        //                return 0;
        //            else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x40, 0x50 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0x3F, 0x80 }))
        //                return 1;
        //            else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x40, 0x50 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0xBF, 0x80 }))
        //                return 2;
        //            else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x49, 0xC0 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0xBF, 0x80 }))
        //                return 3;
        //            else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x49, 0xC0 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0x42, 0xC0 }))
        //                return 4;
        //            else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x43, 0xC0 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0x42, 0xC0 }))
        //                return 5;
        //            else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x43, 0xC0 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0xF0, 0xC0 }))
        //                return 6;
        //            else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x40, 0xC0 }) && Enumerable.SequenceEqual(buffer2, new byte[] { 0x3F, 0xF0 }))
        //                return 7;
        //            else
        //            {
        //                PS3.SetMemory(0x00410734, new byte[] { 0x40, 0xC0 });
        //                PS3.SetMemory(0x00410738, new byte[] { 0x3F, 0x80 });
        //                return 0;
        //            }
        //        }

        //        else
        //            return 0;
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        if(!bRAINBOW_SKY)
        //        {
        //            uint offset1 = 0x00410734;
        //            uint offset2 = 0x00410738;

        //            if (value.Equals(0))
        //            {
        //                // Reset
        //                PS3.SetMemory(offset1, new byte[] { 0x40, 0xC0 });
        //                PS3.SetMemory(offset2, new byte[] { 0x3F, 0x80 });
        //            }

        //            else if (value.Equals(1))
        //            {
        //                //GREEN SKY COLORS
        //                PS3.SetMemory(offset1, new byte[] { 0x40, 0x50 });
        //            }

        //            else if (value.Equals(2))
        //            {
        //                //BLUE SKY COLORS
        //                PS3.SetMemory(offset2, new byte[] { 0xBF, 0x80 });
        //            }

        //            else if (value.Equals(3))
        //            {
        //                //Purple Sky Colors
        //                PS3.SetMemory(offset1, new byte[] { 0x49, 0xC0 });
        //            }

        //            else if (value.Equals(4))
        //            {
        //                //Pink Sky Colors
        //                PS3.SetMemory(offset2, new byte[] { 0x42, 0xC0 });
        //            }

        //            else if (value.Equals(5))
        //            {
        //                //Orange Sky Colors
        //                PS3.SetMemory(offset1, new byte[] { 0x43, 0xC0 });
        //            }

        //            else if (value.Equals(6))
        //            {
        //                //Black Sky Colors
        //                PS3.SetMemory(offset2, new byte[] { 0xF0, 0xC0 });
        //            }

        //            else if (value.Equals(7))
        //            {
        //                //White Sky Colors
        //                PS3.SetMemory(offset1, new byte[] { 0x40, 0xC0 });
        //                PS3.SetMemory(offset2, new byte[] { 0x3F, 0xF0 });
        //            }

        //            else
        //                ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //        }
        //    }
        //}

        //[ToggleState(6)]
        //public static int HUD_COLORS
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        if (!bRAINBOW_HUD)
        //        {
        //            byte[] buffer = new byte[16];
        //            PS3.GetMemory(0x30DBAD64, buffer);

        //            if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }))
        //                return 0;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }))
        //                return 1;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x1F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }))
        //                return 2;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0xFF, 0x00, 0x00, 0x1F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }))
        //                return 3;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0X5F, 0x80, 0x00, 0x00, 0x5F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }))
        //                return 4;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0X8F, 0x80, 0x00, 0x00, 0x8F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }))
        //                return 5;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0X00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }))
        //                return 6;
        //            else
        //            {
        //                PS3.SetMemory(0x30DBAD64, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 });
        //                return 0;
        //            }
        //        }

        //        else
        //            return 0;
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        if(!bRAINBOW_HUD)
        //        {
        //            uint offset = 0x30DBAD64;
        //            if (value.Equals(0))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }); //RESET

        //            else if (value.Equals(1))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }); //GREEN

        //            else if (value.Equals(2))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x1F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }); //PURPLE

        //            else if (value.Equals(3))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0xFF, 0x00, 0x00, 0x1F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }); //RED

        //            else if (value.Equals(4))
        //                PS3.SetMemory(offset, new byte[] { 0X5F, 0x80, 0x00, 0x00, 0x5F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }); //YELLOW

        //            else if (value.Equals(5))
        //                PS3.SetMemory(offset, new byte[] { 0X8F, 0x80, 0x00, 0x00, 0x8F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00 }); //BLUE

        //            else if (value.Equals(6))
        //                PS3.SetMemory(offset, new byte[] { 0X00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }); //INVISIBLE

        //            else
        //                ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //        }
        //    }
        //}

        //[ToggleState(2)]
        //public static int TIME_CYCLE
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        byte[] buffer = new byte[1];
        //        PS3.GetMemory(0x001DA1D4, buffer);

        //        if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
        //            return 0;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x43 }))
        //            return 1;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x44 }))
        //            return 2;
        //        else
        //        {
        //            PS3.SetMemory(0x001DA1D4, new byte[] { 0x40 });
        //            return 0;
        //        }

        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        uint offset = 0x001DA1D4;

        //        if (value.Equals(0))
        //            PS3.SetMemory(offset, new byte[] { 0x40 }); ////SET to default

        //        else if (value.Equals(1))
        //            PS3.SetMemory(offset, new byte[] { 0x43 });

        //        else if (value.Equals(2))
        //            PS3.SetMemory(offset, new byte[] { 0x44 });

        //        else
        //            ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //    }
        //}

        //[ToggleState(5, -5)]
        //public static int TIME_SCALE
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        byte[] buffer = new byte[1];
        //        PS3.GetMemory(0x00C202C9, buffer);

        //        if (Enumerable.SequenceEqual(buffer, new byte[] { 0x50 }))
        //            return 0;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
        //            return -1;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x30 }))
        //            return -2;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x20 }))
        //            return -3;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x10 }))
        //            return -4;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
        //            return -5;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x60 }))
        //            return 1;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x70 }))
        //            return 2;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x80 }))
        //            return 3;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x90 }))
        //            return 4;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF0 }))
        //            return 5;
        //        else
        //        {
        //            PS3.SetMemory(0x00C202C9, new byte[] { 0x50 });
        //            return 0;
        //        }
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        uint offset = 0x00C202C9;

        //        if (value.Equals(0))
        //            PS3.SetMemory(offset, new byte[] { 0x50 }); //SET TO DEFAULT

        //        else if (value.Equals(-1))
        //            PS3.SetMemory(offset, new byte[] { 0x40 }); //Speed Time -1

        //        else if (value.Equals(-2))
        //            PS3.SetMemory(offset, new byte[] { 0x30 }); //Speed Time -2

        //        else if (value.Equals(-3))
        //            PS3.SetMemory(offset, new byte[] { 0x20 }); //Speed Time -3

        //        else if (value.Equals(-4))
        //            PS3.SetMemory(offset, new byte[] { 0x10 }); //Speed Time -4

        //        else if (value.Equals(-5))
        //            PS3.SetMemory(offset, new byte[] { 0x00 }); //Speed Time -5

        //        else if (value.Equals(1))
        //            PS3.SetMemory(offset, new byte[] { 0x60 }); //Speed Time X1

        //        else if (value.Equals(2))
        //            PS3.SetMemory(offset, new byte[] { 0x70 }); //Speed Time X2

        //        else if (value.Equals(3))
        //            PS3.SetMemory(offset, new byte[] { 0x80 }); //Speed Time X3

        //        else if (value.Equals(4))
        //            PS3.SetMemory(offset, new byte[] { 0x90 }); //Speed Time X4

        //        else if (value.Equals(5))
        //            PS3.SetMemory(offset, new byte[] { 0xF0 }); //Speed Time X5

        //        else
        //            ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //    }
        //}

        //[ToggleState(7)]
        //public static int ENTITY_RENDER_HEIGHT
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        byte[] buffer = new byte[2];
        //        PS3.GetMemory(0x00AD5EC8, buffer);

        //        if (Enumerable.SequenceEqual(buffer, new byte[] { 0xBF, 0x80 }))
        //            return 0;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xBF, 0x00 }))
        //            return 1;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xBF, 0xAA }))
        //            return 2;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xBF, 0xFF }))
        //            return 3;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xC0, 0x50 }))
        //            return 4;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xC0, 0x99 }))
        //            return 5;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xC0, 0xFF }))
        //            return 6;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0XC1, 0x80 }))
        //            return 7;
        //        else
        //        {
        //            PS3.SetMemory(0x00AD5EC8, new byte[] { 0xBF, 0x80 });
        //            return 0;
        //        }
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        uint offset = 0x00AD5EC8;

        //        if (value.Equals(0))
        //            PS3.SetMemory(offset, new byte[] { 0xBF, 0x80 });

        //        else if (value.Equals(1))
        //            PS3.SetMemory(offset, new byte[] { 0xBF, 0x00 });

        //        else if (value.Equals(2))
        //            PS3.SetMemory(offset, new byte[] { 0xBF, 0xAA });

        //        else if (value.Equals(3))
        //            PS3.SetMemory(offset, new byte[] { 0xBF, 0xFF });

        //        else if (value.Equals(4))
        //            PS3.SetMemory(offset, new byte[] { 0xC0, 0x50 });

        //        else if (value.Equals(5))
        //            PS3.SetMemory(offset, new byte[] { 0xC0, 0x99 });

        //        else if (value.Equals(6))
        //            PS3.SetMemory(offset, new byte[] { 0xC0, 0xFF });

        //        else if (value.Equals(7))
        //            PS3.SetMemory(offset, new byte[] { 0XC1, 0x80 });

        //        else
        //            ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //    }
        //}

        //[ToggleState(6)]
        //public static int ENTITY_RENDER_WIDTH
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        byte[] buffer = new byte[2];
        //        PS3.GetMemory(0x00AD5ECC, buffer);

        //        if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80 }))
        //            return 0;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x2F, 0x80 }))
        //            return 1;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0xFF }))
        //            return 2;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40, 0x80 }))
        //            return 3;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40, 0xFF }))
        //            return 4;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41, 0xFF }))
        //            return 5;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x43, 0x80 }))
        //            return 6;
        //        else
        //        {
        //            PS3.SetMemory(0x00AD5ECC, new byte[] { 0x3F, 0x80 });
        //            return 0;
        //        }
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        uint offset = 0x00AD5ECC;

        //        if (value.Equals(0))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x80 });

        //        else if (value.Equals(1))
        //            PS3.SetMemory(offset, new byte[] { 0x2F, 0x80 });

        //        else if (value.Equals(2))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0xFF });

        //        else if (value.Equals(3))
        //            PS3.SetMemory(offset, new byte[] { 0x40, 0x80 });

        //        else if (value.Equals(4))
        //            PS3.SetMemory(offset, new byte[] { 0x40, 0xFF });

        //        else if (value.Equals(5))
        //            PS3.SetMemory(offset, new byte[] { 0x41, 0xFF });

        //        else if (value.Equals(6))
        //            PS3.SetMemory(offset, new byte[] { 0x43, 0x80 });

        //        else
        //            ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //    }
        //}

        //[ToggleState(15)]
        //public static int FPS_VALUES
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        byte[] buffer = new byte[1];
        //        PS3.GetMemory(0x00AF0443, buffer);

        //        if (Enumerable.SequenceEqual(buffer, new byte[] { 0x00 }))
        //            return 0;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x10 }))
        //            return 1;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x20 }))
        //            return 2;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x30 }))
        //            return 3;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
        //            return 4;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x50 }))
        //            return 5;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x60 }))
        //            return 6;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x70 }))
        //            return 7;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x80 }))
        //            return 8;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x90 }))
        //            return 9;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xA0 }))
        //            return 10;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xB0 }))
        //            return 11;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xC0 }))
        //            return 12;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xD0 }))
        //            return 13;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xE0 }))
        //            return 14;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0xF0 }))
        //            return 15;
        //        else
        //        {
        //            PS3.SetMemory(0x00AF0443, new byte[] { 0x00 });
        //            return 0;
        //        }
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        uint offset = 0x00AF0443;

        //        if (value.Equals(0))
        //            PS3.SetMemory(offset, new byte[] { 0x00 });

        //        else if (value.Equals(1))
        //            PS3.SetMemory(offset, new byte[] { 0x10 });

        //        else if (value.Equals(2))
        //            PS3.SetMemory(offset, new byte[] { 0x20 });

        //        else if (value.Equals(3))
        //            PS3.SetMemory(offset, new byte[] { 0x30 });

        //        else if (value.Equals(4))
        //            PS3.SetMemory(offset, new byte[] { 0x40 });

        //        else if (value.Equals(5))
        //            PS3.SetMemory(offset, new byte[] { 0x50 });

        //        else if (value.Equals(6))
        //            PS3.SetMemory(offset, new byte[] { 0x60 });

        //        else if (value.Equals(7))
        //            PS3.SetMemory(offset, new byte[] { 0x70 });

        //        else if (value.Equals(8))
        //            PS3.SetMemory(offset, new byte[] { 0x80 });

        //        else if (value.Equals(9))
        //            PS3.SetMemory(offset, new byte[] { 0x90 });

        //        else if (value.Equals(10))
        //            PS3.SetMemory(offset, new byte[] { 0xA0 });

        //        else if (value.Equals(11))
        //            PS3.SetMemory(offset, new byte[] { 0xB0 });

        //        else if (value.Equals(12))
        //            PS3.SetMemory(offset, new byte[] { 0xC0 });

        //        else if (value.Equals(13))
        //            PS3.SetMemory(offset, new byte[] { 0xD0 });

        //        else if (value.Equals(14))
        //            PS3.SetMemory(offset, new byte[] { 0xE0 });

        //        else if (value.Equals(15))
        //            PS3.SetMemory(offset, new byte[] { 0xF0 });

        //        else
        //            ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //    }
        //}

        //[ToggleState(12)]
        //public static int GAMEPLAY_COLORS
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        if (!bRAINBOW_VISION)
        //        {
        //            byte[] buffer = new byte[10];
        //            PS3.GetMemory(0x3000AAF8, buffer);

        //            if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 }))
        //                return 0;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0xFF, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 }))
        //                return 1;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 }))
        //                return 2;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 }))
        //                return 3;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x80 }))
        //                return 4;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0xFF, 0x00, 0x00, 0x3F, 0x80 }))
        //                return 5;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80 }))
        //                return 6;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00 }))
        //                return 7;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0xFF }))
        //                return 8;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80 }))
        //                return 9;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80 }))
        //                return 10;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80 }))
        //                return 11;
        //            else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80 }))
        //                return 12;
        //            else
        //            {
        //                PS3.SetMemory(0x3000AAF8, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });
        //                return 0;
        //            }
        //        }

        //        else
        //            return 0;
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        if(!bRAINBOW_VISION)
        //        {
        //            uint offset = 0x3000AAF8;

        //            if (value.Equals(0))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });//DEFAULT

        //            else if (value.Equals(1))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0xFF, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });//RED

        //            else if (value.Equals(2))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });//CYAN

        //            else if (value.Equals(3))
        //                PS3.SetMemory(offset, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80 });//RED BLUE

        //            else if (value.Equals(4))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00, 0x00, 0x00, 0x3F, 0x80 });//Purple

        //            else if (value.Equals(5))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0xFF, 0x00, 0x00, 0x3F, 0x80 });//Green

        //            else if (value.Equals(6))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80 });//Purple Green

        //            else if (value.Equals(7))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x00 });//Yellow

        //            else if (value.Equals(8))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x3F, 0xFF });//Blue

        //            else if (value.Equals(9))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80 });//Yellow Blue

        //            else if (value.Equals(10))
        //                PS3.SetMemory(offset, new byte[] { 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80 });//Cyan Red

        //            else if (value.Equals(11))
        //                PS3.SetMemory(offset, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80, 0x00, 0x00, 0x4F, 0x80 });//Black and white

        //            else if (value.Equals(12))
        //                PS3.SetMemory(offset, new byte[] { 0x4F, 0x80, 0x00, 0x00, 0x3F, 0x80, 0x00, 0x00, 0x4F, 0x80 });//Pink Red

        //            else
        //                ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //        }
        //    }
        //}

        //[ToggleState(5)]
        //public static int SELECTED_BLOCK_LINE_COLOR
        //{
        //    // Get current state from reading memory
        //    get
        //    {

        //        byte[] buffer1 = new byte[2];
        //        byte[] buffer2 = new byte[1];
        //        byte[] buffer3 = new byte[1];
        //        PS3.GetMemory(0x00B25990, buffer1);
        //        PS3.GetMemory(0x00B25A59, buffer2);
        //        PS3.GetMemory(0x00B25A5E, buffer3);

        //        if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x00, 0x00 }) &&
        //            Enumerable.SequenceEqual(buffer2, new byte[] { 0x40 }) &&
        //            Enumerable.SequenceEqual(buffer3, new byte[] { 0x08 }))
        //            return 0;
        //        else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x3F, 0xFF }) &&
        //                 Enumerable.SequenceEqual(buffer2, new byte[] { 0x40 }) &&
        //                 Enumerable.SequenceEqual(buffer3, new byte[] { 0x08 }))
        //            return 1;
        //        else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x00, 0x00 }) &&
        //                 Enumerable.SequenceEqual(buffer2, new byte[] { 0x7E }) &&
        //                 Enumerable.SequenceEqual(buffer3, new byte[] { 0x08 }))
        //            return 2;
        //        else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x00, 0x00 }) &&
        //                 Enumerable.SequenceEqual(buffer2, new byte[] { 0x7E }) &&
        //                 Enumerable.SequenceEqual(buffer3, new byte[] { 0x48 }))
        //            return 3;
        //        else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x00, 0x00 }) &&
        //                 Enumerable.SequenceEqual(buffer2, new byte[] { 0x7E }) &&
        //                 Enumerable.SequenceEqual(buffer3, new byte[] { 0x40 }))
        //            return 4;
        //        else if (Enumerable.SequenceEqual(buffer1, new byte[] { 0x3F, 0xFF }) &&
        //                 Enumerable.SequenceEqual(buffer2, new byte[] { 0x7E }) &&
        //                 Enumerable.SequenceEqual(buffer3, new byte[] { 0x40 }))
        //            return 5;
        //        else
        //        {
        //            PS3.SetMemory(0x00B25990, new byte[] { 0x00, 0x00 });
        //            PS3.SetMemory(0x00B25A59, new byte[] { 0x40 });
        //            PS3.SetMemory(0x00B25A5E, new byte[] { 0x08 });
        //            return 0;
        //        }
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        uint offset1 = 0x00B25990;
        //        uint offset2 = 0x00B25A59;
        //        uint offset3 = 0x00B25A5E;

        //        if (value.Equals(0))
        //        {
        //            PS3.SetMemory(offset1, new byte[] { 0x00, 0x00 });
        //            PS3.SetMemory(offset2, new byte[] { 0x40 });
        //            PS3.SetMemory(offset3, new byte[] { 0x08 });
        //        }

        //        // White
        //        else if (value.Equals(1))
        //        {
        //            PS3.SetMemory(offset1, new byte[] { 0x3F, 0xFF });
        //            PS3.SetMemory(offset2, new byte[] { 0x40 });
        //            PS3.SetMemory(offset3, new byte[] { 0x08 });
        //        }

        //        // Green
        //        else if (value.Equals(2))
        //        {
        //            PS3.SetMemory(offset1, new byte[] { 0x00, 0x00 });
        //            PS3.SetMemory(offset2, new byte[] { 0x7E });
        //            PS3.SetMemory(offset3, new byte[] { 0x08 });
        //        }

        //        // Cyan
        //        else if (value.Equals(3))
        //        {
        //            PS3.SetMemory(offset1, new byte[] { 0x00, 0x00 });
        //            PS3.SetMemory(offset2, new byte[] { 0x7E });
        //            PS3.SetMemory(offset3, new byte[] { 0x48 });
        //        }

        //        // Blue
        //        else if (value.Equals(4))
        //        {
        //            PS3.SetMemory(offset1, new byte[] { 0x00, 0x00 });
        //            PS3.SetMemory(offset2, new byte[] { 0x7E });
        //            PS3.SetMemory(offset2, new byte[] { 0x40 });
        //        }

        //        // Yellow
        //        else if (value.Equals(5))
        //        {
        //            PS3.SetMemory(offset1, new byte[] { 0x3F, 0xFF });
        //            PS3.SetMemory(offset2, new byte[] { 0x7E });
        //            PS3.SetMemory(offset3, new byte[] { 0x40 });
        //        }

        //        else
        //            ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //    }
        //}

        //[ToggleState(3)]
        //public static int SELECTED_BLOCK_LINE_SIZE
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        byte[] buffer = new byte[1];
        //        PS3.GetMemory(0x00B25998, buffer);

        //        if (Enumerable.SequenceEqual(buffer, new byte[] { 0x40 }))
        //            return 0;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x41 }))
        //            return 1;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x42 }))
        //            return 2;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x60 }))
        //            return 3;
        //        else
        //        {
        //            PS3.SetMemory(0x00B25998, new byte[] { 0x40 });
        //            return 0;
        //        }
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        uint offset = 0x00B25998;

        //        if (value.Equals(0))
        //            PS3.SetMemory(offset, new byte[] { 0x40 });

        //        else if (value.Equals(1))
        //            PS3.SetMemory(offset, new byte[] { 0x41 });

        //        else if (value.Equals(2))
        //            PS3.SetMemory(offset, new byte[] { 0x42 });

        //        else if (value.Equals(3))
        //            PS3.SetMemory(offset, new byte[] { 0x60 });

        //        else
        //            ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //    }
        //}

        //[ToggleState(4)]
        //public static int WEIRD_SUN_MOON_STATES
        //{
        //    // Get current state from reading memory
        //    get
        //    {
        //        byte[] buffer = new byte[2];
        //        PS3.GetMemory(0x00B21F1C, buffer);

        //        if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0x80 })) //DEFAULT
        //            return 0;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x2F, 0x80 })) //REMOVE SUN / MOON
        //            return 1;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x3F, 0xFF })) //4 SUN + Light Moon Better
        //            return 2;
        //        else if (Enumerable.SequenceEqual(buffer, new byte[] { 0x4F, 0xFF })) //Light Moon Max
        //            return 3;
        //        else
        //        {
        //            PS3.SetMemory(0x00B21F1C, new byte[] { 0x3F, 0x80 });
        //            return 0;
        //        }
        //    }

        //    // Set memory and use a number to represent current toggle state.
        //    set
        //    {
        //        uint offset = 0x00B21F1C;

        //        if (value.Equals(0))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0x80 });

        //        else if (value.Equals(1))
        //            PS3.SetMemory(offset, new byte[] { 0x2F, 0x80 });

        //        else if (value.Equals(2))
        //            PS3.SetMemory(offset, new byte[] { 0x3F, 0xFF });

        //        else if (value.Equals(3))
        //            PS3.SetMemory(offset, new byte[] { 0x4F, 0xFF });

        //        else
        //            ToggleState.ErrorToggleState((ToggleState)MethodBase.GetCurrentMethod().GetCustomAttribute(typeof(ToggleState)));
        //    }

        //}

        //#endregion



    }
}
