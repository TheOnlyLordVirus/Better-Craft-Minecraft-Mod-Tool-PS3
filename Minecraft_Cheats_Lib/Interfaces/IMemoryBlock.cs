using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Cheats_Lib
{
    public interface IMemoryBlock
    {
        ushort Length { get; }
        ushort Index { get; }
        byte[] Memory { get; }
    }
}
