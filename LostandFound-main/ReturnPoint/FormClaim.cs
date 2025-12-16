using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ReturnPoint
{
    public class FormClaim : Form
    {
        private TextBox nameTextBox;
        private TextBox contactTextBox;
        private Button saveButton;
        private string photoPath;
        private string saveFolder;
        private string claimantPhotoPath;

        public FormClaim(string photoFilePath, string folderPath, string facePhotoPath)
        {
            this.Text = "Claim Item";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            photoPath = photoFilePath;
            saveFolder = folderPath;
            claimantPhotoPath = facePhotoPath;

            Label nameLabel = new Label
            {
                Text = "Claimant Name:",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            nameTextBox = new TextBox
            {
                Location = new Point(20, 50),
                Width = 340,
                Font = new Font("Arial", 12)
            };

            Label contactLabel = new Label
            {
                Text = "Contact Info:",
                Location = new Point(20, 90),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            contactTextBox = new TextBox
            {
                Location = new Point(20, 120),
                Width = 340,
                Font = new Font("Arial", 12)
            };

            saveButton = new Button
            {
                Text = "Confirm Claim",
                Location = new Point(20, 180),
                Width = 150,
                Height = 40,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            saveButton.Click += SaveButton_Click;

            this.Controls.Add(nameLabel);
            this.Controls.Add(nameTextBox);
            this.Controls.Add(contactLabel);
            this.Controls.Add(contactTextBox);
            this.Controls.Add(saveButton);
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            string name = nameTextBox.Text.Trim();
            string contact = contactTextBox.Text.Trim();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(contact))
            {
                MessageBox.Show("Please enter both name and contact information.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string claimFileName = Path.GetFileNameWithoutExtension(photoPath) + "_claimed.txt";
            string claimFilePath = Path.Combine(saveFolder, claimFileName);

            try
            {
                File.WriteAllText(claimFilePath,
                $"Photo: {Path.GetFileName(photoPath)}\r\n" +
                $"Claimant Name: {name}\r\n" +
                $"Contact Info: {contact}\r\n" +
                $"Face Photo: {Path.GetFileName(claimantPhotoPath)}\r\n" +
                $"Claimed On: {DateTime.Now:yyyy-MM-dd HH:mm}");

                MessageBox.Show("Item successfully claimed!", "Claimed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving claim information: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
