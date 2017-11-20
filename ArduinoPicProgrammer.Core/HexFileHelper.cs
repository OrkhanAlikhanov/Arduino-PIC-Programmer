using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using Word = System.UInt16;
using Byte = System.Byte;

namespace ArduinoPicProgrammer.Core.HexFileHelper
{
    internal static class HexFileHelper
    {
        public static Word ReadLittleEndianWord(this List<Byte> bytes, int index)
        {
            return (Word)((bytes[index + 1] << 8) | bytes[index]);
        }

        public static List<Word> ToLittleEndians(this List<Byte> bytes)
        {

            if (bytes.Count % 2 != 0)
                throw new ArgumentException($"{nameof(bytes)} must has even data");

            var list = new List<Word>();

            for (int i = 0; i < bytes.Count; i += 2)
            {
                list.Add(bytes.ReadLittleEndianWord(i));
            }

            return list;
        }

        public static Word ReadBigEndianWord(this List<Byte> bytes, int index)
        {
            return (Word)((bytes[index] << 8) | bytes[index + 1]);
        }

        public static Byte ToByte(this string s)
        {
            if (s.Length != 2)
                throw new Exception("Length must be 2");

            return Byte.Parse(s, NumberStyles.HexNumber);
        }

        public static Byte ToByte(this string s, int startIndex)
        {
            return s.Substring(startIndex, 2).ToByte();
        }


        //http://stackoverflow.com/questions/1779129/how-to-take-all-but-the-last-element-in-a-sequence-using-linq
        public static IEnumerable<T> DropLast<T>(this IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var it = source.GetEnumerator();
            bool hasRemainingItems = false;
            bool isFirst = true;
            T item = default(T);

            do
            {
                hasRemainingItems = it.MoveNext();
                if (hasRemainingItems)
                {
                    if (!isFirst) yield return item;
                    item = it.Current;
                    isFirst = false;
                }
            } while (hasRemainingItems);
        }
    }
}
