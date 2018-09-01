using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FalloutFRMReader
{
    public partial class Form1 : Form
    {
        FRMPalette commonColors;
        FRMFile currentFile;
        

        public Form1()
        {
            InitializeComponent();
            
        }

        public void SetupTreeView(FRMFile file)
        {
            treeView1.Nodes.Clear();
            //string[] directions = { "Northeast", "East", "Southeast", "South", "Southwest", "West", "Northeast" };
            TreeNode[] rootChildren = new TreeNode[6];
            for (int i = 0; i < 6; i++)
            {
                TreeNode tn =new TreeNode("Orientation " + i);
                tn.Tag = i;
                rootChildren[i] = tn;
            }

            for(int i = 0; i < 6; i++)
                for (int j = 0; j < file.listArray[i].Count; j++)
                {
                    TreeNode frmNode = new TreeNode("FRAME " + j + "");
                    frmNode.Tag = j;
                    rootChildren[i].Nodes.Add(frmNode);
                    
                }


            treeView1.Nodes.Add(new TreeNode("Frame Collection - " + file.actionFrame + ", " + file.framesPerDir, rootChildren));
        }

        private void loadPaletteFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = ofdPaletteLoad.ShowDialog();
            if (dr != DialogResult.OK)
                return;

            if (ofdPaletteLoad.FileName.EndsWith(".txt"))
            {
                try
                {
                    commonColors = FRMPalette.paletteFromGIMPFile(ofdPaletteLoad.FileName);
                    toolStripStatusLabel1.Text = "Palette Successfully Loaded.";
                    pictureBox1.Image = null;
                }
                catch (Exception eee)
                {
                    MessageBox.Show(eee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (ofdPaletteLoad.FileName.EndsWith(".PAL"))
            {
                try
                {
                    commonColors = FRMPalette.paletteFromPALFile(ofdPaletteLoad.FileName);
                    toolStripStatusLabel1.Text = "Palette Successfully Loaded.";
                    pictureBox1.Image = null;
                }
                catch (Exception eee)
                {
                    MessageBox.Show(eee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void loadFRMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdFRMLoad.ShowDialog();
                try
                {
                    currentFile = new FRMFile(ofdFRMLoad.FileName);
                    toolStripStatusLabel1.Text = "Frame File Successfully Loaded.";
               }
               catch (Exception eee)
               {
                  MessageBox.Show(eee.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
               }

                if (currentFile != null & commonColors != null)
                {
                    //  MessageBox.Show(commonColors.entry[0].R + " red " + commonColors.entry[0].G + " green " + commonColors.entry[0].B + " blue ");
                    // MessageBox.Show(currentFile + " FRAMES" + currentFile.frames[0]);
                    pictureBox1.Image = currentFile.frames[0].toBitmap(commonColors);
                    SetupTreeView(currentFile);
                    // MessageBox.Show("DUn");
                }
          
        }

        private void RenderButton_Click(object sender, EventArgs e)
        {
            
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            string fnom = saveFileDialog1.FileName.ToUpper();
            if (fnom.EndsWith(".BMP"))
                pictureBox1.Image.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Bmp);
            else if (fnom.EndsWith(".PNG"))
                pictureBox1.Image.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Png);
            else if (fnom.EndsWith(".TIFF"))
                pictureBox1.Image.Save(saveFileDialog1.FileName, System.Drawing.Imaging.ImageFormat.Tiff);
            else if (fnom.EndsWith(".RIX"))
            {
                RIXFile intermediateRIX = new RIXFile((Bitmap)pictureBox1.Image, commonColors);
                intermediateRIX.WriteRIX(saveFileDialog1.FileName);
            }

        }

        private void saveCurrentFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.FileName = ofdFRMLoad.FileName.Replace(".FRM","");
            saveFileDialog1.ShowDialog();
        }

        private void firstFRM2PNGInDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (commonColors == null) return;
            DialogResult s = folderBrowserDialog1.ShowDialog();
            if (s == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryInfo di = new DirectoryInfo(folderBrowserDialog1.SelectedPath);
                int c = 0;
                foreach (FileInfo f in di.GetFiles())
                {
                    if (f.Name.EndsWith(".FRM"))
                    {
                        string name = folderBrowserDialog1.SelectedPath + "\\" + f.Name;
                        currentFile = new FRMFile(name);
                        currentFile.frames[0].toBitmap(commonColors).Save(name + ".PNG", System.Drawing.Imaging.ImageFormat.Png);
                        c++;
                    }
                }

                MessageBox.Show(c + " file(s) processed.", "Batch Job Terminated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {
            

        }

        private void folderBrowserDialog1_HelpRequest_1(object sender, EventArgs e)
        {

        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

            TreeNode node = treeView1.SelectedNode;
            if (node.Text.StartsWith("FRAME"))
            {
                FRMFrame inQuestion = currentFile.listArray[int.Parse(node.Parent.Tag.ToString())][int.Parse(node.Tag.ToString())];
                toolStripStatusLabel1.Text = Path.GetFileName(ofdFRMLoad.FileName)+" "+inQuestion.ToString();
                pictureBox1.Image = inQuestion.toBitmap(commonColors);
            }
        }

        private void treeView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void ofdPaletteLoad_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void displayRIXToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdRIXVGA.ShowDialog();
        }

        private void ofdRIXVGA_FileOk(object sender, CancelEventArgs e)
        {
            RIXFile splashTexture = new RIXFile(ofdRIXVGA.FileName);
            toolStripStatusLabel1.Text = "ColorRIX VGA File Loaded.";

            if (splashTexture!=null)
            {
                pictureBox1.Image = splashTexture.toBitmap();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("FRMOut is a fan-made program for viewing the static art assets of Fallout 1 & 2.\r\n(FRM oriented sprite sequence files, RIX splash screen texture files)\r\n\r\nFallout 1 & 2 were published by Interplay, and the ColorRIX format\r\nwas historically used by the geographically-nearby RIX Softworks.\r\nFallout and associated trademarks, etc. belong to ZeniMax Media.\r\n\r\nCopyright (c) 14 June 2015. Some rights reserved;\r\n\tThis software should be considered licensed to you \r\n\tunder the terms of the Apache 2.0 License.\r\nUse at your own risk.", "About FRMOut", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdIMPORT.ShowDialog();
        }

        private void ofdIMPORT_FileOk(object sender, CancelEventArgs e)
        {
            Bitmap ic = (Bitmap) Image.FromFile(ofdIMPORT.FileName);
           
           
            Bitmap bm = new Bitmap(ic.Width, ic.Height);
            Graphics g = Graphics.FromImage(bm);
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            g.DrawImage(ic, new RectangleF(0f, 0f, ic.Width, ic.Height), new RectangleF(0f, 0f, ic.Width, ic.Height), GraphicsUnit.Pixel);

            RIXFile rixNew = new RIXFile(bm,commonColors);
            pictureBox1.Image = rixNew.toBitmap();
        }
    }
}
