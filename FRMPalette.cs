using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FalloutFRMReader
{
    public class FRMPalette
    {
        public Color[] entry = new Color[256];
        delegate int BinaryIntTrans(BinaryReader i);

        public static FRMPalette paletteFromPALFile(String fname)
        {
            FRMPalette pal = new FRMPalette();
            int bMod = (int) Utils.BrightnessMultiplier.HIGH_NOON;
            BinaryIntTrans modB = x => Math.Max(Math.Min(x.ReadByte() * bMod, 255), 0);

            using (BinaryReader br = new BinaryReader(new FileStream(fname, FileMode.Open)))
                for (int i = 0; i < 256; i++)
                    pal.entry[i] = Color.FromArgb(modB(br),modB(br),modB(br));

            return pal;
        }

        public static FRMPalette paletteFromGIMPFile(String fname)
        {
            FRMPalette pal = new FRMPalette();

            String line;
            int cnt = 0;
            Regex rgx = new Regex(@"\s*(\d+)\s*(\d+)\s*(\d+)", RegexOptions.IgnoreCase);
            bool beginPalette = false;
            using (StreamReader sr = new StreamReader(new FileStream(fname, FileMode.Open)))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    //MessageBox.Show(line);
                    if (!beginPalette && line.StartsWith("#"))
                    {
                        beginPalette = true;
                      //  MessageBox.Show("Found hash.");
                        continue;
                    }

                    if (beginPalette && line.Trim().Length > 1)
                    {
                        MatchCollection matches = rgx.Matches(line);
                        if (matches.Count > 0 && matches[0].Groups.Count > 0)
                        {
                            
                            pal.entry[cnt] = Color.FromArgb(int.Parse(matches[0].Groups[1].Value), int.Parse(matches[0].Groups[2].Value), int.Parse(matches[0].Groups[3].Value));
                            cnt++;
                        }
                    }

                  

                }
                if (cnt != 256)
                    throw new InvalidDataException("Trouble constructing the indexed colors palette, not enough colors specified in GIMP-style Palette File!");
            }

            return pal;
           
        }
    }
}
