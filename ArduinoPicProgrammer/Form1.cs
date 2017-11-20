using ArduinoPicProgrammer.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArduinoPicProgrammer
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string path = textBox1.Text;
            var hexfile = new HexFile(path);

            richTextBox1.Clear();
            richTextBox1.AppendLine(hexfile.blocks.Count.ToString());
            int num = 0;
            foreach (var b in hexfile.blocks)
            {
                num++;
                richTextBox1.AppendLine($"Block {num}\t{b.StartAddress} - {b.EndAddress}");

                for (int i = 0; i < b.Data.Count; i++)
                {
                    var bin = Convert.ToString(b.Data[i], 2);
                    richTextBox1.AppendLine($"{b.StartAddress + i}\t{bin.PadLeft(16, '0')}\t{b.Data[i]}\t{b.Data[i]:X}");
                }
                richTextBox1.AppendLine();
            }
            var prog = new Programmer("COM6", hexfile.blocks);
            prog.BurnBlocks();
        }
    }
}
