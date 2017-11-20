using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArduinoPicProgrammer.Core.HexFileHelper;

using Address = System.UInt16;
using Word = System.UInt16;
using Byte = System.Byte;

namespace ArduinoPicProgrammer.Core
{
    partial class HexFile
    {
        private enum RecordType
        {
            Data = 0x00,
            EndOfFile = 0x01,
            ExtendedSegmentAddress = 0x02, //Not Supported
            StartSegmentAddress = 0x03, //Not Supported
            ExtendedLinearAddress = 0x04, //Not Supported
            StartLinearAddress = 0x05 // Not Supported
        }

        private class HexLine
        {
            public RecordType RecordType { get; private set; }
            public Byte ByteCount { get; private set; }
            public Address Address { get; private set; }
            public List<Byte> Data { get; private set; }
            public Byte Checksum { get; private set; }

            public static HexLine Parse(string line)
            {
                if (line[0] != ':')
                    throw new FormatException("Must start with colon ':'");

                if ((line.Length - 1) % 2 != 0)
                    throw new FormatException("Line length must be even (excluding colon ':'");

                var bytes = ReadAllBytes(line);

                var hexLine = new HexLine();
                hexLine.Checksum = bytes.Last();
                Byte calculatedChecksum = (Byte)(~bytes.DropLast().Sum(_ => _) + 1);
                if (hexLine.Checksum != calculatedChecksum)
                    throw new ArgumentException("Checksum does not match");


                hexLine.RecordType = (RecordType)(bytes[3]);
                hexLine.Address = (Address)bytes.ReadBigEndianWord(1);
                hexLine.ByteCount = bytes[0];
                if (hexLine.ByteCount != bytes.Count - 5)
                {
                    throw new ArgumentException("Checksum is correct but has less/more data");
                }

                if (hexLine.ByteCount != 0)
                    hexLine.Data = bytes.GetRange(4, hexLine.ByteCount);

                return hexLine;
            }
            private static List<Byte> ReadAllBytes(string line)
            {
                var list = new List<Byte>();

                for (int i = 1; i < line.Length; i += 2)
                {
                    list.Add(line.ToByte(i));
                }

                return list;
            }

        }
    }
}
