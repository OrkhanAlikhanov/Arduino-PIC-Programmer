using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Address = System.UInt16;
using Word = System.UInt16;
using Byte = System.Byte;

namespace ArduinoPicProgrammer.Core.ProgrammerHelper
{
    internal static class ProgrammerHelper
    {
        public static bool IsInRange(this Address address, Range range)
        {
            return (address >= range.Start) || (address <= range.End);
        }
    }
}
