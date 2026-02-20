using System;
using System.Windows.Forms;
using System.Drawing;

namespace ReturnPoint
{
    public class FormRoleSelection : Form
    {
        private Button btnStudent;
        private Button btnAdmin;
        private PictureBox logoPictureBox;

        public FormRoleSelection()
        {
            Text = "Choose Your Role - ReturnPoint";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            BackColor = Theme.GetBackgroundTeal();
            BackgroundImage = Theme.CreateGradientBitmap(1366, 768, vertical: true);
            BackgroundImageLayout = ImageLayout.Stretch;

            // ===== TOP HEADER WITH TITLE =====
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };

            Label lblTitle = new Label
            {
                Text = "ReturnPoint: SHA's Lost and Found",
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            headerPanel.Controls.Add(lblTitle);

            Controls.Add(headerPanel);

            // ===== MAIN CONTAINER - 50/50 SPLIT =====
            Panel mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            // ===== LEFT SIDE - STUDENT =====
            Panel studentPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 683,
                BackColor = Color.FromArgb(41, 128, 128),
                Cursor = Cursors.Hand
            };

            // Create a centered container for student content
            TableLayoutPanel studentLayout = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 5,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };
            // Center columns and rows
            studentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            studentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            studentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            studentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            studentLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            studentLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            studentLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            studentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));

            Label lblStudentIcon = new Label
            {
                Text = "👨‍🎓",
                Font = new Font("Segoe UI", 80),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            studentLayout.Controls.Add(lblStudentIcon, 1, 1);

            Label lblStudentTitle = new Label
            {
                Text = "STUDENT",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            studentLayout.Controls.Add(lblStudentTitle, 1, 2);

            Label lblStudentDesc = new Label
            {
                Text = "Report lost or found items\nManage your claims\nTrack item status",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(220, 220, 220),
                AutoSize = true,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            studentLayout.Controls.Add(lblStudentDesc, 1, 3);

            btnStudent = new Button
            {
                Text = "→ Enter as Student",
                Width = 300,
                Height = 60,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(26, 200, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnStudent.FlatAppearance.BorderSize = 0;
            btnStudent.Click += BtnStudent_Click;
            btnStudent.MouseEnter += (s, e) => 
            { 
                btnStudent.BackColor = Color.FromArgb(0, 220, 220);
                btnStudent.Font = new Font("Segoe UI", 15, FontStyle.Bold);
                studentPanel.BackColor = Color.FromArgb(51, 148, 148);
            };
            btnStudent.MouseLeave += (s, e) => 
            { 
                btnStudent.BackColor = Color.FromArgb(26, 200, 200);
                btnStudent.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                studentPanel.BackColor = Color.FromArgb(41, 128, 128);
            };
            studentLayout.Controls.Add(btnStudent, 1, 4);
            studentLayout.SetCellPosition(btnStudent, new TableLayoutPanelCellPosition(1, 4));

            studentPanel.Controls.Add(studentLayout);
            mainContainer.Controls.Add(studentPanel);

            // ===== RIGHT SIDE - ADMIN =====
            Panel adminPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 683,
                BackColor = Color.FromArgb(100, 80, 120),
                Cursor = Cursors.Hand
            };

            // Create a centered container for admin content
            TableLayoutPanel adminLayout = new TableLayoutPanel
            {
                ColumnCount = 3,
                RowCount = 5,
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(0)
            };
            // Center columns and rows
            adminLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            adminLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            adminLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            adminLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));
            adminLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            adminLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            adminLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            adminLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30F));

            Label lblAdminIcon = new Label
            {
                Text = "🔐",
                Font = new Font("Segoe UI", 80),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            adminLayout.Controls.Add(lblAdminIcon, 1, 1);

            Label lblAdminTitle = new Label
            {
                Text = "ADMIN",
                Font = new Font("Segoe UI", 32, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            adminLayout.Controls.Add(lblAdminTitle, 1, 2);

            Label lblAdminDesc = new Label
            {
                Text = "Manage lost and found items\nReview student claims\nAdminister system",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.FromArgb(220, 220, 220),
                AutoSize = true,
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };
            adminLayout.Controls.Add(lblAdminDesc, 1, 3);

            btnAdmin = new Button
            {
                Text = "→ Enter as Admin",
                Width = 300,
                Height = 60,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                BackColor = Color.FromArgb(180, 100, 200),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnAdmin.FlatAppearance.BorderSize = 0;
            btnAdmin.Click += BtnAdmin_Click;
            btnAdmin.MouseEnter += (s, e) => 
            { 
                btnAdmin.BackColor = Color.FromArgb(200, 120, 220);
                btnAdmin.Font = new Font("Segoe UI", 15, FontStyle.Bold);
                adminPanel.BackColor = Color.FromArgb(120, 100, 140);
            };
            btnAdmin.MouseLeave += (s, e) => 
            { 
                btnAdmin.BackColor = Color.FromArgb(180, 100, 200);
                btnAdmin.Font = new Font("Segoe UI", 14, FontStyle.Bold);
                adminPanel.BackColor = Color.FromArgb(100, 80, 120);
            };
            adminLayout.Controls.Add(btnAdmin, 1, 4);

            adminPanel.Controls.Add(adminLayout);
            mainContainer.Controls.Add(adminPanel);

            Controls.Add(mainContainer);

            // ===== LOGO IN TOP-LEFT =====
            AddLogoTopLeft();
        }

        private void AddLogoTopLeft()
        {
            try
            {
                string logoPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../logo.png");
                if (System.IO.File.Exists(logoPath))
                {
                    logoPictureBox = new PictureBox
                    {
                        Image = Image.FromFile(logoPath),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Width = 60,
                        Height = 60,
                        BackColor = Color.Transparent,
                        Top = 20,
                        Left = 20
                    };
                    Controls.Add(logoPictureBox);
                    logoPictureBox.BringToFront();
                }
            }
            catch { /* Logo not found, continue without it */ }
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
