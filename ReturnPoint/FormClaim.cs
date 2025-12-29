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
            this.Text = "Claim Item - ReturnPoint";
            this.Size = new Size(500, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Theme.GetBackgroundTeal();

            photoPath = photoFilePath;
            saveFolder = folderPath;
            claimantPhotoPath = facePhotoPath;

            var titleFont = new Font("Segoe UI", 18F, FontStyle.Bold);
            var labelFont = new Font("Segoe UI", 11F);
            var inputFont = new Font("Segoe UI", 12F);

            Label titleLabel = new Label
            {
                Text = "Claim This Item",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = titleFont,
                ForeColor = Theme.NearBlack
            };

            Label nameLabel = new Label
            {
                Text = "Your Full Name:",
                Location = new Point(20, 60),
                AutoSize = true,
                Font = labelFont,
                ForeColor = Theme.NearBlack
            };
            nameTextBox = new TextBox
            {
                Location = new Point(20, 85),
                Width = 460,
                Height = 38,
                Font = inputFont,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label contactLabel = new Label
            {
                Text = "Contact Information:",
                Location = new Point(20, 130),
                AutoSize = true,
                Font = labelFont,
                ForeColor = Theme.NearBlack
            };
            contactTextBox = new TextBox
            {
                Location = new Point(20, 155),
                Width = 460,
                Height = 38,
                Font = inputFont,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            saveButton = new Button
            {
                Text = "Confirm Claim",
                Location = new Point(20, 210),
                Width = 220,
                Height = 44,
                BackColor = Theme.TealGreen,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += SaveButton_Click;

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(260, 210),
                Width = 220,
                Height = 44,
                BackColor = Theme.DarkGray,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            cancelButton.FlatAppearance.BorderSize = 0;
            cancelButton.Click += (s, e) => this.Close();

            this.Controls.Add(titleLabel);
            this.Controls.Add(nameLabel);
            this.Controls.Add(nameTextBox);
            this.Controls.Add(contactLabel);
            this.Controls.Add(contactTextBox);
            this.Controls.Add(saveButton);
            this.Controls.Add(cancelButton);
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
