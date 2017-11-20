using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;

using Address = System.UInt16;
using Word = System.UInt16;
using ArduinoPicProgrammer.Core.HexFileHelper;

namespace ArduinoPicProgrammer.Core
{
    public partial class HexFile
    {
        public List<MemoryBlock> blocks = new List<MemoryBlock>();

        public HexFile(string path)
        {
            this.Load(path);
        }

        private bool merged; //for checking endine
        public void Load(string path)
        {
            string fileContent;
            using (var streamReader = new StreamReader(path))
                fileContent = streamReader.ReadToEnd();

            string[] hexLines = fileContent.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in hexLines)
            {
                ProcessLine(line);
            }
            if (!merged)
                throw new Exception("No end of file found!");
        }

        private void ProcessLine(string line)
        {
            HexLine hexLine = HexLine.Parse(line);

            
            Address baseAddress = 0x000000; //always 0, not used currently 
            switch (hexLine.RecordType)
            {
                case RecordType.Data:
                    List<Word> words = hexLine.Data.ToLittleEndians(); //Reading little endian data
                    Address byteAddress = (Address)(baseAddress + hexLine.Address); //not used. correcting address if it is Extended Address
                    Address wordAddress = (Address)(byteAddress >> 1); //Byte to Word address
                    blocks.Add(new MemoryBlock(wordAddress, words));
                    break;
                case RecordType.EndOfFile:
                    MergeBlocks();
                    break;
                case RecordType.ExtendedSegmentAddress:
                //baseAddress = ((Address)hexLine.Data.ReadBigEndianWord(0)) << 4;
                //break;
                case RecordType.ExtendedLinearAddress:
                //baseAddress = ((Address)hexLine.Data.ReadBigEndianWord(0)) << 16;
                //break;
                case RecordType.StartSegmentAddress:
                case RecordType.StartLinearAddress:
                    throw new NotSupportedException();
                default:
                    throw new Exception($"Unknown {nameof(RecordType)} {hexLine.RecordType:X}");
            }

        }

        private void MergeBlocks()
        {
            //sort blocks by start address
            blocks.Sort();
            for (int i = 0; i < blocks.Count - 1; i++)
            {
                var cur = blocks[i];
                var next = blocks[i + 1];

                if (cur.EndAddress >= next.StartAddress)
                    throw new Exception("Memory overlap happend!");

                if (cur.EndAddress + 1 == next.StartAddress)
                {
                    cur.Data.AddRange(next.Data);
                    blocks.RemoveAt(i + 1);
                    i = -1; // start again
                }

                //else keep blocks separate
            }
            merged = true;
        }
    }
}
