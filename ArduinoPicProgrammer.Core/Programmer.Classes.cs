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
    partial class  Programmer
    {

        //Actually 9600 is default
        //https://referencesource.microsoft.com/#System/sys/system/io/ports/SerialPort.cs,49
        public const int BaduRate = 9600;

        public Range ProgramMemory = new Range(0x0000, 0x07FF);
        public Range ConfigrationMemory = new Range(0x2000, 0x2007);
        public Range DataMemory = new Range(0x2007, 0x2008);

        public int ProgramBits { get; } = 14;
        public int DataBits { get; } = 8;

        //ex. OSCCAL preservation
        public Range ProtectedMemory = new Range(0, 0);
        public enum Command
        {
            PreformProgramMemory = 0x1,
            IncrementAddress,
            IncrementAddressCount,
            LoadConfiguration,
            BulkEraseProgramMemory,
            ReadDataFromProgramMemory,
            Reset,
            OperationFinished
        }
    }
}
