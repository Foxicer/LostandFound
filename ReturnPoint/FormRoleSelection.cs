using System;
using System.Windows.Forms;

namespace ReturnPoint
{
    public class FormRoleSelection : Form
    {
        private Button btnStudent;
        private Button btnAdmin;
        private Label lblTitle;
        private Label lblSubtitle;

        public FormRoleSelection()
        {
            Text = "Choose Your Role - ReturnPoint";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            BackColor = Theme.GetBackgroundTeal();
            BackgroundImage = Theme.CreateGradientBitmap(1920, 1080, vertical: true);
            BackgroundImageLayout = ImageLayout.Stretch;

            var titleFont = new System.Drawing.Font("Segoe UI", 48F, System.Drawing.FontStyle.Bold);
            var subtitleFont = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular);
            var buttonFont = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);

            // Title
            lblTitle = new Label
            {
                Text = "Welcome to ReturnPoint",
                Font = titleFont,
                ForeColor = System.Drawing.Color.White,
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Subtitle
            lblSubtitle = new Label
            {
                Text = "Please select your role to continue",
                Font = subtitleFont,
                ForeColor = System.Drawing.Color.White,
                AutoSize = true,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            // Student Button
            btnStudent = new Button
            {
                Text = "Student",
                Width = 250,
                Height = 80,
                Font = buttonFont,
                BackColor = Theme.TealGreen,
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            btnStudent.FlatAppearance.BorderSize = 0;
            btnStudent.Click += BtnStudent_Click;
            btnStudent.MouseEnter += (s, e) => { btnStudent.BackColor = Theme.MediumTeal; };
            btnStudent.MouseLeave += (s, e) => { btnStudent.BackColor = Theme.TealGreen; };

            // Admin Button
            btnAdmin = new Button
            {
                Text = "Admin",
                Width = 250,
                Height = 80,
                Font = buttonFont,
                BackColor = System.Drawing.Color.FromArgb(100, 150, 150),
                ForeColor = System.Drawing.Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            btnAdmin.FlatAppearance.BorderSize = 0;
            btnAdmin.Click += BtnAdmin_Click;
            btnAdmin.MouseEnter += (s, e) => { btnAdmin.BackColor = System.Drawing.Color.FromArgb(80, 130, 130); };
            btnAdmin.MouseLeave += (s, e) => { btnAdmin.BackColor = System.Drawing.Color.FromArgb(100, 150, 150); };

            // Center panel
            var centerPanel = new TableLayoutPanel
            {
                ColumnCount = 1,
                RowCount = 4,
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = System.Drawing.Color.Transparent
            };

            centerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));
            centerPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 25F));

            centerPanel.Controls.Add(lblTitle, 0, 0);
            centerPanel.Controls.Add(lblSubtitle, 0, 1);

            var buttonPanel = new TableLayoutPanel
            {
                ColumnCount = 2,
                RowCount = 1,
                Dock = DockStyle.None,
                Width = 550,
                Height = 100,
                BackColor = System.Drawing.Color.Transparent,
                Margin = new Padding(50)
            };
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            buttonPanel.Controls.Add(btnStudent, 0, 0);
            buttonPanel.Controls.Add(btnAdmin, 1, 0);

            centerPanel.Controls.Add(buttonPanel, 0, 2);

            Controls.Add(centerPanel);
        }

        private void BtnStudent_Click(object sender, EventArgs e)
        {
            using (var loginForm = new FormLogin(isAdminLogin: false))
            {
                if (loginForm.ShowDialog() == DialogResult.OK && loginForm.AuthenticatedUser != null)
                {
                    Program.CurrentUser = loginForm.AuthenticatedUser;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }

        private void BtnAdmin_Click(object sender, EventArgs e)
        {
            using (var loginForm = new FormLoginAdmin())
            {
                if (loginForm.ShowDialog() == DialogResult.OK && loginForm.AuthenticatedUser != null)
                {
                    Program.CurrentUser = loginForm.AuthenticatedUser;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }
}
