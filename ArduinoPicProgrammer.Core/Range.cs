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
    public class Range
    {
        public Address Start { get; set; }
        public Address End { get; set; }

        public Range(Address start, Address end)
        {
            if (start > end)
                throw new ArgumentException("end address must greater");

            Start = start;
            End = end;
        }
    }
}
