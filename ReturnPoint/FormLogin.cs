using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using ReturnPoint.Models;

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
                BackColor = Theme.LightGray;
                BackgroundImage = Theme.CreateGradientBitmap(1920, 1080, vertical: true);
                BackgroundImageLayout = ImageLayout.Stretch;
            }
            Controls.Add(panelMain);
            // Fullscreen, centered layout
            Text = "Login - ReturnPoint";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            this.BackgroundImage = Theme.CreateGradientBitmap(1920, 1080, vertical: true);
            this.BackgroundImageLayout = ImageLayout.Stretch;

            var titleFont = new System.Drawing.Font("Segoe UI", 32F, System.Drawing.FontStyle.Bold);
            var subtitleFont = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular);
            var labelFont = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            var inputFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular);

            txtEmail = new TextBox 
            { 
                Width = 380, 
                Height = 44,
                Font = inputFont, 
                Padding = new Padding(10),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtPass = new TextBox 
            { 
                Width = 380,
                Height = 44,
                UseSystemPasswordChar = true, 
                Font = inputFont, 
                Padding = new Padding(10),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            btnLogin = new Button 
            { 
                Text = "Sign In", 
                Width = 180, 
                Height = 44, 
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold),
                BackColor = Theme.TealGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            
            btnRegister = new Button 
            { 
                Text = "Create Account", 
                Width = 180, 
                Height = 44, 
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular),
                BackColor = Theme.AccentBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            
            btnCancel = new Button 
            { 
                Text = "Exit", 
                Width = 180, 
                Height = 44, 
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular),
                BackColor = Theme.DarkGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            
            lblMsg = new Label 
            { 
                AutoSize = false, 
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter, 
                Height = 32, 
                ForeColor = Theme.DeepRed, 
                Font = labelFont,
                Visible = false
            };

            var lblTitle = new Label 
            { 
                Text = "Welcome Back", 
                AutoSize = true, 
                Font = titleFont, 
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ForeColor = Theme.NearBlack
            };
            
            var lblSubtitle = new Label 
            { 
                Text = "Sign in to your account", 
                AutoSize = true, 
                Font = subtitleFont, 
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ForeColor = Theme.DarkGray
            };
            
            var lblEmail = new Label 
            { 
                Text = "Email Address", 
                AutoSize = true, 
                Font = labelFont,
                ForeColor = Theme.NearBlack
            };
            var lblP = new Label 
            { 
                Text = "Password", 
                AutoSize = true, 
                Font = labelFont,
                ForeColor = Theme.NearBlack
            };

            var main = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 3 };
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 25));
            main.BackColor = Theme.LightGray; 
            panelMain.Controls.Add(main);


            var centerPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Width = 420,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                Padding = new Padding(30),
                BackColor = Color.White
            };

            centerPanel.Controls.Add(lblTitle);
            centerPanel.Controls.Add(lblSubtitle);
            centerPanel.Controls.Add(new Label { Height = 30 }); // Spacer
            centerPanel.Controls.Add(lblEmail);
            centerPanel.Controls.Add(new Label { Height = 6 }); 
            centerPanel.Controls.Add(txtEmail);
            centerPanel.Controls.Add(new Label { Height = 16 }); // Spacer
            centerPanel.Controls.Add(lblP);
            centerPanel.Controls.Add(new Label { Height = 6 }); 
            centerPanel.Controls.Add(txtPass);
            centerPanel.Controls.Add(new Label { Height = 16 }); // Spacer
            centerPanel.Controls.Add(lblMsg);
            centerPanel.Controls.Add(new Label { Height = 20 }); // Spacer

            var btnRow = new FlowLayoutPanel 
            { 
                FlowDirection = FlowDirection.TopDown, 
                AutoSize = true, 
                WrapContents = false,
                Width = 380
            };
            btnRow.Controls.Add(btnLogin);
            btnRow.Controls.Add(new Label { Height = 10 }); // Spacer
            
            var secondRowBtns = new FlowLayoutPanel 
            { 
                FlowDirection = FlowDirection.LeftToRight, 
                AutoSize = true, 
                WrapContents = false
            };
            secondRowBtns.Controls.Add(btnRegister);
            secondRowBtns.Controls.Add(new Label { Width = 20 }); // Spacer
            secondRowBtns.Controls.Add(btnCancel);
            btnRow.Controls.Add(secondRowBtns);
            
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
            panelMain.BackColor = Theme.GetBackgroundTeal();
            main.BackColor = Theme.GetBackgroundTeal();
            centerPanel.BackColor = Theme.GetBackgroundTeal();
            btnRow.BackColor = Theme.GetBackgroundTeal();
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
}