using ArduinoPicProgrammer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Word = System.UInt16;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var prog = new Programmer("COM6", null);
            Word data;
            while (true)
            {
                var line = Console.ReadLine().ToLowerInvariant();
                switch (line)
                {
                    case "d":
                        Console.WriteLine(prog.port.BytesToWrite);
                        break;
                    case "read":
                        data = prog.ReadDataFromProgramMemory();
                        PrintWord(data);
                        break;
                    case "inc":
                        prog.Send(Programmer.Command.IncrementAddress);
                        break;
                    case "inc_c":
                        Console.Write("how much: ");
                        data = Convert.ToUInt16(Console.ReadLine());
                        prog.SendWithData(Programmer.Command.IncrementAddressCount, data);
                        break;
                    case "write":
                        Console.Write("what: ");
                        data = Convert.ToUInt16(Console.ReadLine());
                        prog.SendWithData(Programmer.Command.PreformProgramMemory, data);
                        break;
                    case "reset":

                        prog.Send(Programmer.Command.Reset);
                        break;
                    default:
                        break;
                }
            }
        }

        static void PrintWord(Word data)
        {
            var bin = Convert.ToString(data, 2);
            Console.WriteLine($"DEC: {data}");
            Console.WriteLine($"HEX: {data:X}");
            Console.WriteLine($"BIN: {bin.PadLeft(16, '0')}");
        }
    }
}
