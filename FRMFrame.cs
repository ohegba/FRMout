using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FalloutFRMReader;
using System.Drawing;
using System.Windows.Forms;

namespace FalloutFRMReader
{
   public class FRMFrame
    {
        public int width, height, size, offX, offY;
        public byte[] imageData;

        public override string ToString()
        {
            return "FRAME " + width + " x " + height + " Offset:" +
                offX + "," + offY; 
        }
        
        public FRMFrame(BinaryReader br)
        {
          //  MessageBox.Show("FRAME AT " + br.BaseStream.Position);
            width = br.ReadBEInt16();
            height = br.ReadBEInt16();
            size = br.ReadBEInt32();
            offX = br.ReadBEInt16();
            offY = br.ReadBEInt16();
            imageData = br.ReadBytes(size);
        }

        public Image toBitmap(FRMPalette palette)
        {
            
            Bitmap bm = new Bitmap(width, height);
            int cnt = 0;
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                {
                    Color c = palette.entry[imageData[cnt]];
                    cnt++;
                  //  MessageBox.Show(c.R+" "+c.G+" "+c.B);
                    bm.SetPixel(j,i, c);

                }
            return bm;
        }

    }
}
