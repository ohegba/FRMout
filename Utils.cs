using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FalloutFRMReader
{
    public class OrderedPair<T1, T2>
    {
        T1 x;
        T2 y;

        public OrderedPair(T1 a, T2 b)
        {
            x = a; y = b;
        }

        public Object this[int number]
        {
            get
            {
                if (number == 1)
                    return y;
                else
                    return x;
            }
            set
            {
                if (number == 1)
                    y = (T2)value;
                else
                    x = (T1)value;
            }
        }
    }

    public static class Utils
    {

        public enum BrightnessMultiplier { NIGHT, DAWN, AFTERNOON, HIGH_NOON };

        public static byte[] revMe(this byte[] barr)
        {
            for (int i = 0; i < barr.Length / 2; i++)
            {
                byte sto = barr[i];
                barr[i] = barr[barr.Length - i - 1];
                barr[barr.Length - i - 1] = sto;
            }
            return barr;
        }

        public static Int16 ReadBEInt16(this BinaryReader br)
        {
            return BitConverter.ToInt16((br.ReadBytes(2)).revMe(), 0);
        }

        public static Int32 ReadBEInt32(this BinaryReader br)
        {
            return BitConverter.ToInt32((br.ReadBytes(4)).revMe(), 0);
        }

        public static UInt16 ReadBEUInt16(this BinaryReader br)
        {
            return BitConverter.ToUInt16((br.ReadBytes(2)).revMe(), 0);
        }

        public static UInt32 ReadBEUInt32(this BinaryReader br)
        {
            return BitConverter.ToUInt32((br.ReadBytes(4)).revMe(), 0);
        }

    }
}
