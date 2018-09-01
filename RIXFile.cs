using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;

namespace FalloutFRMReader
{
    class RIXFile
    {
        UInt16 width, height;
        Color[] palette = new Color[256];
        byte[] imgData;
        delegate int BinaryIntTrans(BinaryReader i);
        delegate int byteIntTrans(byte i);
        BinaryIntTrans modB = x => Math.Max(Math.Min(x.ReadByte() * (byte)Utils.BrightnessMultiplier.HIGH_NOON, 255), 0);
        byteIntTrans unmodB = b => Math.Max(Math.Min(b / (byte)Utils.BrightnessMultiplier.HIGH_NOON, 255), 0);

        public RIXFile(String fname)
        {
            using (BinaryReader br = new BinaryReader(new FileStream(fname, FileMode.Open)))
            {
                if (!(new String(br.ReadChars(4))).Equals("RIX3"))
                    throw new InvalidDataException("Attempted to open file as ColorRIX VGA, but it does not identify as such.");

                width = br.ReadUInt16();
                height = br.ReadUInt16();

                br.ReadBytes(2);

                for (int i = 0; i < 256; i++)
                {
                    palette[i] = Color.FromArgb(modB(br), modB(br), modB(br));
                }

                imgData = br.ReadBytes(width * height);
            }
        }

        public RIXFile(Bitmap bmp, FRMPalette pal)
        {
            imgData = FastBMP.FixRGBImageToPalette(bmp, pal);
            width = (UInt16) bmp.Width;
            height = (UInt16)bmp.Height;
            palette = pal.entry;

        }

        public void WriteRIX(String fname)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(fname, FileMode.Create)))
            {
                bw.Write(ASCIIEncoding.ASCII.GetBytes("RIX3"));
                bw.Write(width);
                bw.Write(height);
                bw.Write((UInt16)175);
                foreach (Color c in palette)
                {
                    bw.Write((byte)unmodB(c.R));
                    bw.Write((byte)unmodB(c.G));
                    bw.Write((byte)unmodB(c.B));
                }
                for (int i = 0; i < imgData.Length; i++)
                {
                    bw.Write(imgData[i]);
                }
            }
        }

        public Image toBitmap()
        {
            Bitmap bm = new Bitmap(width, height);
            int cnt = 0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    Color c = palette[imgData[cnt]];
                    cnt++;
                    //  MessageBox.Show(c.R+" "+c.G+" "+c.B);
                    bm.SetPixel(j, i, c);

                }
            return bm;
        }
    }
}
