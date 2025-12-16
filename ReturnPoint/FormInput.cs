using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ReturnPoint
{
    public class FormInput : Form
    {
        private TextBox locationTextBox;
        private DateTimePicker datePicker;
        private Button saveButton;
        private string photoPath;
        private string saveFolder;

        public FormInput(string photoFilePath, string folderPath)
        {
            this.Text = "Item Information";
            this.Size = new Size(400, 300);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            photoPath = photoFilePath;
            saveFolder = folderPath;

            Label locationLabel = new Label
            {
                Text = "Where was the item last found?",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold),
            };
            locationTextBox = new TextBox
            {
                Location = new Point(20, 50),
                Width = 340,
                Font = new Font("Arial", 12)
            };

            Label dateLabel = new Label
            {
                Text = "When was the item last found?",
                Location = new Point(20, 90),
                AutoSize = true,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            datePicker = new DateTimePicker
            {
                Location = new Point(20, 120),
                Width = 200,
                Font = new Font("Arial", 12),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "yyyy-MM-dd HH:mm"
            };

            saveButton = new Button
            {
                Text = "Save",
                Location = new Point(20, 180),
                Width = 100,
                Height = 40,
                BackColor = Color.SeaGreen,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold)
            };
            saveButton.Click += SaveButton_Click;

            this.Controls.Add(locationLabel);
            this.Controls.Add(locationTextBox);
            this.Controls.Add(dateLabel);
            this.Controls.Add(datePicker);
            this.Controls.Add(saveButton);
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            string location = locationTextBox.Text.Trim();
            string dateFound = datePicker.Value.ToString("yyyy-MM-dd HH:mm");

            if (string.IsNullOrEmpty(location))
            {
                MessageBox.Show("Please enter the location where the item was found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string infoFileName = Path.GetFileNameWithoutExtension(photoPath) + "_info.txt";
            string infoFilePath = Path.Combine(saveFolder, infoFileName);

            try
            {
                File.WriteAllText(infoFilePath, $"Photo: {Path.GetFileName(photoPath)}\r\nLocation: {location}\r\nDate Found: {dateFound}");
                MessageBox.Show("Item information saved successfully!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving item information: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
