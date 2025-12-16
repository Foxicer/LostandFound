using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace ReturnPoint
{
    public class FormLogin : Form
    {
        private TextBox txtEmail;
        private TextBox txtPass;
        private Button btnLogin;
        private Button btnRegister;
        private Button btnCancel;
        private Label lblMsg;
        private Panel panelMain;

        public User? AuthenticatedUser { get; private set; }

        public FormLogin()
        {
            panelMain = new Panel();
            {
                Dock = DockStyle.Fill;
                BackColor = System.Drawing.Color.LightSeaGreen;
            }
            Controls.Add(panelMain);
            // Fullscreen, centered layout
            Text = "Login";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            var titleFont = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold);
            var labelFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);
            var inputFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);

            txtEmail = new TextBox { Width = 520, Font = inputFont, Padding = new Padding(8) };
            txtPass = new TextBox { Width = 520, UseSystemPasswordChar = true, Font = inputFont, Padding = new Padding(8) };
            btnLogin = new Button { Text = "Login", Width = 160, Height = 44, Font = labelFont };
            btnRegister = new Button { Text = "Register", Width = 160, Height = 44, Font = labelFont };
            btnCancel = new Button { Text = "Cancel", Width = 160, Height = 44, Font = labelFont };
            lblMsg = new Label { AutoSize = false, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Height = 32, ForeColor = Theme.DeepRed, Font = inputFont };

            var lblTitle = new Label { Text = "Welcome Back", AutoSize = true, Font = titleFont, TextAlign = System.Drawing.ContentAlignment.MiddleCenter };
            var lblEmail = new Label { Text = "Email (Gmail)", AutoSize = true, Font = labelFont };
            var lblP = new Label { Text = "Password", AutoSize = true, Font = labelFont };

            var main = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 3 };
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 40));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 30));
            main.BackColor = Color.LightSeaGreen; 
            panelMain.Controls.Add(main);


            var centerPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Width = 560,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                Padding = new Padding(20),
                BackColor = Color.LightSeaGreen // must also color this
            };

            centerPanel.Controls.Add(lblTitle);
            centerPanel.Controls.Add(new Label { Height = 20 }); // Spacer
            centerPanel.Controls.Add(lblEmail);
            centerPanel.Controls.Add(txtEmail);
            centerPanel.Controls.Add(new Label { Height = 10 }); // Spacer
            centerPanel.Controls.Add(lblP);
            centerPanel.Controls.Add(txtPass);
            centerPanel.Controls.Add(new Label { Height = 10 }); // Spacer
            centerPanel.Controls.Add(lblMsg);
            centerPanel.Controls.Add(new Label { Height = 10 }); // Spacer

            var btnRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, WrapContents = false };
            btnRow.Controls.Add(btnLogin);
            btnRow.Controls.Add(new Label { Width = 20 }); // Spacer
            btnRow.Controls.Add(btnRegister);
            btnRow.Controls.Add(new Label { Width = 20 }); // Spacer
            btnRow.Controls.Add(btnCancel);
            centerPanel.Controls.Add(btnRow);

            main.Controls.Add(centerPanel, 1, 1);
            Controls.Add(main);

            // events and behavior
            btnLogin.Click += (s, e) => TryLogin();
            btnRegister.Click += (s, e) =>
            {
                using var reg = new FormRegister();
                if (reg.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(reg.RegisteredEmail))
                {
                    txtEmail.Text = reg.RegisteredEmail;
                    lblMsg.Text = "Account created. You can now login.";
                }
            };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            AcceptButton = btnLogin;
            CancelButton = btnCancel;

            Theme.Apply(this);
            panelMain.BackColor = Color.LightSeaGreen;
            main.BackColor = Color.LightSeaGreen;
            centerPanel.BackColor = Color.LightSeaGreen;
            btnRow.BackColor = Color.LightSeaGreen;
        }

        private void TryLogin()
        {
            lblMsg.Text = "";
            var email = txtEmail.Text?.Trim();
            var pass = txtPass.Text ?? "";

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                lblMsg.Text = "Enter email and password.";
                return;
            }

            if (!email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                lblMsg.Text = "Please sign in with a Gmail address.";
                return;
            }

            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
                if (!File.Exists(path))
                {
                    lblMsg.Text = "No users registered.";
                    return;
                }

                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var json = File.ReadAllText(path);
                var users = JsonSerializer.Deserialize<List<User>>(json, opts) ?? new List<User>();

                // match against Email and Password properties (Json deserialization is case-insensitive)
                var found = users.FirstOrDefault(u =>
                    string.Equals(u.Email ?? "", email, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(u.Password ?? "", pass, StringComparison.Ordinal));

                if (found == null)
                {
                    lblMsg.Text = "Invalid email or password.";
                    return;
                }

                AuthenticatedUser = found;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Login error: " + ex.Message;
            }
        }
    }

    public class User
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? GradeSection { get; set; }
        public string? Password { get; set; }
        public string? ProfilePicture { get; set; }
        public string? Role { get; set; }
    }
}