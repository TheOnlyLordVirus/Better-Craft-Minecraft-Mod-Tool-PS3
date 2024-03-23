using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Cheats_Lib.Interfaces
{
    public interface IMultiStateCheat : IMinecraftCheat
    {
        //List<IMinecraftCheatParameter> CheatStates { get; }
        //void SetState(IMinecraftCheatParameter parameter);

        /// <summary>
        /// Goes to the next state of the cheat, if there is no next state the cheat is deactivated
        /// </summary>
        void NextState();

        //// <summary>
        /// Goes to the previous state of the cheat, if there is no previous state the cheat is deactivated
        /// </summary>
        void PreviousState();
    }
}
