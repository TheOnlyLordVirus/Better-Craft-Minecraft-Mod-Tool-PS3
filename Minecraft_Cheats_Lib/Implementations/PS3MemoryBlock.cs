using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Cheats_Lib.Implementations
{
    public sealed class PS3MemoryBlock : IMemoryBlock
    {
        private readonly byte[] _data;

        public ushort Length => (ushort)_data.Length;

        public ushort Index => 0;

        public byte[] Memory => _data;

        public PS3MemoryBlock(in byte[] data)
        {
            _data = data;
        }
    }
}
