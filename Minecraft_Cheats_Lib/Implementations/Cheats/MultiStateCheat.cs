using Minecraft_Cheats_Lib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Cheats_Lib.Implementations.Cheats
{
    public sealed class MultiStateCheat : IMultiStateCheat
    {
        public List<IMinecraftCheatParameter> CheatStates => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public int ByteCount => throw new NotImplementedException();

        public bool IsActive => throw new NotImplementedException();

        public void NextState()
        {
            throw new NotImplementedException();
        }

        public void PreviousState()
        {
            throw new NotImplementedException();
        }

        public void SetState(IMinecraftCheatParameter parameter)
        {
            throw new NotImplementedException();
        }
    }
}
