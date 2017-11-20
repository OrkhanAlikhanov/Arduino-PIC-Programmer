using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Address = System.UInt16;
using Word = System.UInt16;
using Byte = System.Byte;
using ArduinoPicProgrammer.Core.ProgrammerHelper;

namespace ArduinoPicProgrammer.Core
{
    public partial class Programmer
    {
        public readonly SerialPort port;
        private List<MemoryBlock> blocks;

        public Programmer(string portName, List<MemoryBlock> blocks)
        {
            this.blocks = blocks;
            port = new SerialPort(portName);
            port.BaudRate = BaduRate;
            port.ReadTimeout = 1000000;
            port.Open();
        }

        private Address currentPC = 0;

        public void BurnBlocks()
        {
            //blocks.Sort(); //in any case
            Send(Command.Reset);
            Send(Command.BulkEraseProgramMemory);
            Word rdead = ReadDataFromProgramMemory();

            foreach (var block in blocks)
            {
                SetPC(block.StartAddress);
                for (int i = 0; i < block.Data.Count; i++)
                {
                    Word data = (Word)(block.Data[i] & 0x3fff);
                    SendWithData(Command.PreformProgramMemory, data);
                    Word read = ReadDataFromProgramMemory();
                    if (read != data)
                    {
                        throw new Exception("Not written");
                    }
                    if (i != block.Data.Count - 1)
                    {
                        Send(Command.IncrementAddress);
                        currentPC++;
                    }
                }
            }
        }

        private void SetPC(Address address)
        {
            if (currentPC == address)
                return;
            if (currentPC > address)
                throw new Exception($"couldn't place pc to address 0x{address:X}");

            if (currentPC < ConfigrationMemory.Start && address >= ConfigrationMemory.Start && address <= ConfigrationMemory.End)
            {
                Send(Command.LoadConfiguration);
                currentPC = ConfigrationMemory.Start;
                SetPC(address);
                return;
            }
            SendWithData(Command.IncrementAddressCount, (Address)(address - currentPC));
            currentPC = address;
        }

        public Word ReadDataFromProgramMemory()
        {
            Send(Command.ReadDataFromProgramMemory, false);

            int high = port.ReadByte();
            int low = port.ReadByte();

            return (Word)((high << 8) | low);
        }

        public void SendWithData(Command command, Word data, bool isData = false)
        {
            int mask = (1 << (isData ? DataBits : ProgramBits)) - 1;
            data &= (Word)mask; //mask to eleminate excess data

            Byte high = (Byte)(data >> 8);
            Byte low = (Byte)(data & 0xFF);

            port.Write(new Byte[] { (byte)command, high, low }, 0, 3);
            WaitOperation();
        }

        public void Send(Command command, bool wait = true)
        {
            port.Write(new byte[] { (Byte)command }, 0, 1);
            if (wait) WaitOperation();
        }

        private void WaitOperation()
        {
            int a = port.ReadByte(); // blocks and waits for byte to come

            if (a != (int)Command.OperationFinished)
            {
                throw new Exception("Was wating something else responding.");
            }
        }
    }
}
