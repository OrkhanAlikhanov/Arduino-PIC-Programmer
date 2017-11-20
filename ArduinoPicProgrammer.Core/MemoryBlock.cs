using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Address = System.UInt16;
using Word = System.UInt16;
using Byte = System.Byte;

namespace ArduinoPicProgrammer.Core
{
    public class MemoryBlock : IComparable<MemoryBlock>
    {
        public Address StartAddress { get; private set; }
        public List<Word> Data { get; private set; }
        public Address EndAddress
        {
            get
            {
                return (Address)(StartAddress + Data.Count - 1);
            }
        }

        public MemoryBlock(Address start, List<Word> data)
        {
            StartAddress = start;
            Data = data;
        }

        int IComparable<MemoryBlock>.CompareTo(MemoryBlock other)
        {
            return this.StartAddress.CompareTo(other.StartAddress);
        }
    }
}
