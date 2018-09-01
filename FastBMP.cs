using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;


namespace FalloutFRMReader
{
    public class FastBMP
    {
             public static byte[] FixRGBImageToPalette(Bitmap bmIn, FRMPalette pal)
        {

            byte[] indexedImage = new byte[(bmIn.Width*bmIn.Height)];
            long idx = 0; 
            unsafe
            {
                BitmapData rawData = bmIn.LockBits(new Rectangle(0, 0, bmIn.Width, bmIn.Height), ImageLockMode.ReadWrite, bmIn.PixelFormat);
                int bytesPP = System.Drawing.Bitmap.GetPixelFormatSize(bmIn.PixelFormat) / 8;
                int bmHeight = rawData.Height;
                int bmWidth = rawData.Width * bytesPP;
                byte* pointerBeginImageData = (byte*)rawData.Scan0;

                Dictionary<String, OrderedPair<Color, byte>> lookup = new Dictionary<string, OrderedPair<Color, byte>>();
                for (int y = 0; y < bmHeight; y++)
                {
                    byte* rgbBlock = pointerBeginImageData + (y * rawData.Stride);
                    for (int x = 0; x < bmWidth; x = x + bytesPP)
                    {
                        int curB = rgbBlock[x];
                        int curG = rgbBlock[x + 1];
                        int curR = rgbBlock[x + 2];

                        
                        Color champion = pal.entry[0];
                        double distBest = Double.MaxValue;
                        String colorKey = "#"+curR+curG+curB;
                        if (!lookup.ContainsKey(colorKey))
                        {
                            byte pdx = 0; byte championPal = 0;
                            foreach (Color refColor in pal.entry)
                            {

                                double euclid = Math.Sqrt(Math.Pow(refColor.R - curR, 2) + Math.Pow(refColor.G - curG, 2) + Math.Pow(refColor.B - curB, 2));
                                if (euclid < distBest)
                                {
                                    champion = refColor; distBest = euclid; championPal = pdx;
                                }
                                pdx++;
                            }
                            lookup[colorKey] = new OrderedPair<Color,byte>(champion, championPal);
                        }
                        else
                        {
                            champion = (Color)(lookup[colorKey])[0];
                        }
                       /* rgbBlock[x] = (byte)        champion.B;
                        rgbBlock[x + 1] = (byte)    champion.G;
                        rgbBlock[x + 2] = (byte)    champion.R;*/
                        indexedImage[idx] = (byte)(lookup[colorKey])[1];
                        idx++;

                    }
                }
                bmIn.UnlockBits(rawData);
            }

                 return indexedImage;
        }
    }
}
