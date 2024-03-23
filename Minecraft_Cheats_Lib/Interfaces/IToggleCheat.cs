using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Cheats_Lib.Interfaces
{
    public interface IToggleCheat : IMinecraftCheat
    {
        /// <summary>
        /// Gets if the cheat is toggled on or off
        /// </summary>
        bool IsOn { get; }

        /// <summary>
        /// Toggles the state of the cheat from off to on, and on to off
        /// </summary>
        void Toggle();
    }
}
