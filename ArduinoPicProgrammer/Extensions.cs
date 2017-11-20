using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArduinoPicProgrammer
{
    public static class Extensions
    {
        public static void AppendLine(this RichTextBox rt)
        {
            rt.AppendText("\n");
        }
        public static void AppendLine(this RichTextBox rt, string line)
        {
            rt.AppendText(line);
            rt.AppendText("\n");
        }
    }
}
