using System;
using System.Windows.Forms;
using AForge.Video.DirectShow;

namespace ReturnPoint
{
    public class FormSelectCamera : Form
    {
        private ListBox deviceListBox;
        private Button btnSelect;
        private Button btnCancel;
        private Label lblTitle;
        public string? SelectedDeviceMoniker { get; private set; }

        public FormSelectCamera(FilterInfoCollection devices)
        {
            this.Text = "Select Camera Device";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(400, 300);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Theme.GetBackgroundTeal();

            lblTitle = new Label
            {
                Text = "Available Camera Devices:",
                AutoSize = true,
                Location = new System.Drawing.Point(20, 20),
                Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold),
                ForeColor = Theme.SoftWhite
            };

            deviceListBox = new ListBox
            {
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(360, 180),
                Font = new System.Drawing.Font("Segoe UI", 10),
                BackColor = Theme.LightGray,
                ForeColor = Theme.NearBlack
            };

            for (int i = 0; i < devices.Count; i++)
            {
                deviceListBox.Items.Add($"{i + 1}. {devices[i].Name}");
            }

            if (deviceListBox.Items.Count > 0)
                deviceListBox.SelectedIndex = 0;

            btnSelect = new Button
            {
                Text = "Select",
                Location = new System.Drawing.Point(140, 240),
                Size = new System.Drawing.Size(100, 40),
                BackColor = Theme.TealGreen,
                ForeColor = Theme.SoftWhite,
                Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            btnSelect.FlatAppearance.BorderSize = 0;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new System.Drawing.Point(250, 240),
                Size = new System.Drawing.Size(100, 40),
                BackColor = Theme.DarkGray,
                ForeColor = Theme.SoftWhite,
                Font = new System.Drawing.Font("Segoe UI", 10),
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            btnSelect.Click += (s, e) =>
            {
                if (deviceListBox.SelectedIndex >= 0)
                {
                    SelectedDeviceMoniker = devices[deviceListBox.SelectedIndex].MonikerString;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Please select a camera device.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            btnCancel.Click += (s, e) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(deviceListBox);
            this.Controls.Add(btnSelect);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSelect;
            this.CancelButton = btnCancel;
        }
    }
}
