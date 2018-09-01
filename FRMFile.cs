using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace FalloutFRMReader
{
   public class FRMFile
    {
        UInt32 versionNumber, frameArea;
        public UInt16 FPS, actionFrame, framesPerDir;
        Int16[] pixelShiftsX = new Int16[6], pixelShiftsY = new Int16[6];
        UInt32[] directionsOffsets = new UInt32[6];
        public List<FRMFrame> frames = new List<FRMFrame>();
        public List<FRMFrame>[] listArray = new List<FRMFrame>[6];

        public override string ToString()
        {
            return "FRM VERS " + versionNumber + " wide " + FPS + " FPS " +
                actionFrame + " AF " + framesPerDir + " FPD";
        }

        public FRMFile(String fileName)
        {
            BinaryReader br = new BinaryReader(new FileStream(fileName, FileMode.Open));
            
            versionNumber = br.ReadBEUInt32();
            FPS = br.ReadBEUInt16();
            actionFrame = br.ReadBEUInt16();
            framesPerDir = br.ReadBEUInt16();
            
            for (int i = 0; i < 6; i++)
                pixelShiftsX[i] = br.ReadBEInt16();
            for (int i = 0; i < 6; i++)
                pixelShiftsY[i] = br.ReadBEInt16();
            for (int i = 0; i < 6; i++)
                directionsOffsets[i] = br.ReadBEUInt32();
            for (int i = 0; i < 6; i++)
                listArray[i] = new List<FRMFrame>();

            frameArea = br.ReadBEUInt32();

           
            //if (actionFrame == 0)
            //{
            //    FRMFrame frm = new FRMFrame(br);
            //    frames.Add(frm);
            //    listArray[0].Add(frm);
            //}
            //else
            {
                int infer = -1;
                try
                {
                    infer = (int)UInt16.Parse(fileName[fileName.Length - 1] + "");
                }
                catch (Exception eee) { }
                

                try
                {
                    //for (int i = 0; i < ((actionFrame) * framesPerDir); i++)
                    //{

                    //    int frameAssign = i / framesPerDir;
                    //    // MessageBox.Show(i + "-" + frameAssign);
                    //    FRMFrame newFrame = new FRMFrame(br);
                    //    frames.Add(newFrame);
                    //    listArray[frameAssign].Add(newFrame);
                    //    //  MessageBox.Show(br.BaseStream.Position + "");


                    //}

                    int cntI = 0;
                    while (br.BaseStream.Position != br.BaseStream.Length)
                    {
                        int frameAssign = (infer >= 0)?infer:(cntI / framesPerDir); cntI++;
                        // MessageBox.Show(i + "-" + frameAssign);
                        FRMFrame newFrame = new FRMFrame(br);
                        frames.Add(newFrame);
                        listArray[frameAssign].Add(newFrame);
                    }

                }
                catch (Exception eee)
                { MessageBox.Show("The FRM importer expected more frame data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
           
           
            br.Close();
        }

        
    }
}
