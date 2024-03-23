using Minecraft_Cheats_Lib.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minecraft_Cheats_Lib.Implementations.Cheats
{
    public sealed class ToggleCheat : IToggleCheat
    {
        private readonly IMemoryReader _reader;
        private readonly IMemoryWriter _writer;
        private readonly PS3MemoryBlock _onState;
        private readonly PS3MemoryBlock _offState;
        private readonly int _length;


        public string Name { get; }
        public bool IsOn 
        {
            get
            {
                var memoryBlock = _reader.Read(Offset, _length);
                return memoryBlock.Memory.SequenceEqual(_onState.Memory);                   
            }
        }
        public uint Offset { get; }


        public ToggleCheat(
            string name, 
            uint offset, 
            byte[] onState, byte[] offState,
            IMemoryReader reader, IMemoryWriter writer)
        {
            _reader = reader;
            _writer = writer;

            if (onState.Length != offState.Length)
                throw new ArgumentException("The on state byte count must be equal to the off state byte count!");

            Name = name;
            Offset = offset;
            _onState = new PS3MemoryBlock(in onState);
            _offState = new PS3MemoryBlock(in offState);
            _length = offState.Length;
        }

        public void Toggle()
        {
            if (IsOn)
            {
                _writer.Write(Offset, _offState);
                return;
            }
            _writer.Write(Offset, _onState);

        }
    }
}
