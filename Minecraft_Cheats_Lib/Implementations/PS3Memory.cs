using PS3Lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Cheats_Lib.Implementations
{
    public sealed class PS3Memory : IMemoryReader, IMemoryWriter
    {
        private readonly PS3API _api;

        public PS3Memory(PS3API api)
        {
            _api = api;
        }

        public IMemoryBlock Read(uint offset, int length)
        {
            return new PS3MemoryBlock (_api.GetBytes(offset, length));
        }

        public void Write(uint offset, IMemoryBlock memoryBlock)
        {
            _api.SetMemory(offset, memoryBlock.Memory);
        }
    }
}
