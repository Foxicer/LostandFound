using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ReturnPoint
{
    public class FormClaimantInfo : Form
    {
        public FormClaimantInfo(string infoFilePath, string photoFilePath)
        {
            this.Text = "Claimant Information";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(400, 500);

            // Claimant photo
            PictureBox claimantPic = new PictureBox
            {
                Size = new Size(350, 350),
                Top = 10,
                Left = 20,
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = File.Exists(photoFilePath) ? Image.FromFile(photoFilePath) : null
            };

            // Claimant info text
            TextBox infoBox = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Top = 370,
                Left = 20,
                Width = 350,
                Height = 100,
                ScrollBars = ScrollBars.Vertical,
                Text = File.Exists(infoFilePath) ? File.ReadAllText(infoFilePath) : "No info available"
            };

            this.Controls.Add(claimantPic);
            this.Controls.Add(infoBox);
        }
    }
}
