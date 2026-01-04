using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Windows.Forms;
using ReturnPoint.Models;
namespace ReturnPoint
{
    public partial class FormRegister : Form
    {
        private TextBox txtFirst;
        private TextBox txtMiddle;
        private TextBox txtLast;
        private TextBox txtEmail;
        private TextBox txtGradeSection;
        private TextBox txtPassword;
        private TextBox txtConfirm;
        private ComboBox cbRole;
        private Button btnRegister;
        private Button btnCancel;
        private Label lblMsg;
        public string RegisteredEmail { get; private set; } = "";
        public FormRegister()
        {
            Text = "Create Account - ReturnPoint";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            this.BackgroundImage = Theme.CreateGradientBitmap(1920, 1080, vertical: true);
            this.BackgroundImageLayout = ImageLayout.Stretch;
            var titleFont = new System.Drawing.Font("Segoe UI", 32F, System.Drawing.FontStyle.Bold);
            var subtitleFont = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Regular);
            var labelFont = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular);
            var inputFont = new System.Drawing.Font("Segoe UI", 12F);
            txtFirst = new TextBox { Width = 120, Height = 40, Font = inputFont, PlaceholderText = "First name", BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtMiddle = new TextBox { Width = 120, Height = 40, Font = inputFont, PlaceholderText = "Middle", BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtLast = new TextBox { Width = 120, Height = 40, Font = inputFont, PlaceholderText = "Last name", BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtEmail = new TextBox { Width = 370, Height = 40, Font = inputFont, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtGradeSection = new TextBox { Width = 370, Height = 40, Font = inputFont, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtPassword = new TextBox { Width = 370, Height = 40, UseSystemPasswordChar = true, Font = inputFont, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            txtConfirm = new TextBox { Width = 370, Height = 40, UseSystemPasswordChar = true, Font = inputFont, BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            cbRole = new ComboBox { Width = 370, Height = 40, DropDownStyle = ComboBoxStyle.DropDownList, Font = inputFont, BackColor = Color.White };
            cbRole.Items.AddRange(new[] { "Student", "Admin" });
            cbRole.SelectedIndex = 0;
            btnRegister = new Button 
            { 
                Text = "Create Account", 
                Width = 180, 
                Height = 44, 
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold),
                BackColor = Theme.TealGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnCancel = new Button 
            { 
                Text = "Back", 
                Width = 180, 
                Height = 44, 
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular),
                BackColor = Theme.DarkGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            lblMsg = new Label { AutoSize = false, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Height = 32, ForeColor = Theme.DeepRed, Font = inputFont, Visible = false };
            var lblTitle = new Label { Text = "Create Your Account", AutoSize = true, Font = titleFont, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, ForeColor = Theme.NearBlack };
            var lblSubtitle = new Label { Text = "Join ReturnPoint today", AutoSize = true, Font = subtitleFont, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, ForeColor = Theme.DarkGray };
            var lblFirst = new Label { Text = "Full Name", AutoSize = true, Font = labelFont, ForeColor = Theme.NearBlack };
            var lblEmail = new Label { Text = "Email Address", AutoSize = true, Font = labelFont, ForeColor = Theme.NearBlack };
            var lblGrade = new Label { Text = "Grade and Section", AutoSize = true, Font = labelFont, ForeColor = Theme.NearBlack };
            var lblP = new Label { Text = "Password", AutoSize = true, Font = labelFont, ForeColor = Theme.NearBlack };
            var lblC = new Label { Text = "Confirm Password", AutoSize = true, Font = labelFont, ForeColor = Theme.NearBlack };
            var lblR = new Label { Text = "Account Type", AutoSize = true, Font = labelFont, ForeColor = Theme.NearBlack };
            var main = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 3 };
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 20));
            main.BackColor = Theme.GetBackgroundTeal();
            var center = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Width = 400,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                Padding = new Padding(30),
                BackColor = Color.White
            };
            center.Controls.Add(lblTitle);
            center.Controls.Add(lblSubtitle);
            center.Controls.Add(new Label { Height = 28 }); 
            center.Controls.Add(lblFirst);
            center.Controls.Add(new Label { Height = 6 });
            var nameRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Width = 370 };
            nameRow.Controls.Add(txtFirst);
            nameRow.Controls.Add(new Label { Width = 8 });
            nameRow.Controls.Add(txtMiddle);
            nameRow.Controls.Add(new Label { Width = 8 });
            nameRow.Controls.Add(txtLast);
            center.Controls.Add(nameRow);
            center.Controls.Add(new Label { Height = 14 }); 
            center.Controls.Add(lblEmail);
            center.Controls.Add(new Label { Height = 6 });
            center.Controls.Add(txtEmail);
            center.Controls.Add(new Label { Height = 14 }); 
            center.Controls.Add(lblGrade);
            center.Controls.Add(new Label { Height = 6 });
            center.Controls.Add(txtGradeSection);
            center.Controls.Add(new Label { Height = 14 }); 
            center.Controls.Add(lblP);
            center.Controls.Add(new Label { Height = 6 });
            center.Controls.Add(txtPassword);
            center.Controls.Add(new Label { Height = 14 }); 
            center.Controls.Add(lblC);
            center.Controls.Add(new Label { Height = 6 });
            center.Controls.Add(txtConfirm);
            center.Controls.Add(new Label { Height = 14 }); 
            center.Controls.Add(lblR);
            center.Controls.Add(new Label { Height = 6 });
            center.Controls.Add(cbRole);
            center.Controls.Add(new Label { Height = 14 }); 
            center.Controls.Add(lblMsg);
            center.Controls.Add(new Label { Height = 20 }); 
            var btnRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Width = 370 };
            btnRow.Controls.Add(btnRegister);
            btnRow.Controls.Add(new Label { Width = 20 }); 
            btnRow.Controls.Add(btnCancel);
            center.Controls.Add(btnRow);
            main.Controls.Add(center, 1, 1);
            Controls.Add(main);
            btnRegister.Click += (s, e) => DoRegister();
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            AcceptButton = btnRegister;
            CancelButton = btnCancel;
            Theme.Apply(this);
            BackColor = Theme.GetBackgroundTeal();
            main.BackColor = Theme.GetBackgroundTeal();
            btnRow.BackColor = Theme.GetBackgroundTeal();
            center.BackColor = Theme.GetBackgroundTeal();
        }
        private void DoRegister()
        {
            lblMsg.Text = "";
            var first = txtFirst.Text?.Trim() ?? "";
            var middle = txtMiddle.Text?.Trim() ?? "";
            var last = txtLast.Text?.Trim() ?? "";
            var name = string.Join(" ", new[] { first, middle, last }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
            var email = txtEmail.Text?.Trim();
            var grade = txtGradeSection.Text?.Trim();
            var pass = txtPassword.Text ?? "";
            var confirm = txtConfirm.Text ?? "";
            var role = cbRole.SelectedItem?.ToString() ?? "user";
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                lblMsg.Text = "Enter name, email and password.";
                return;
            }
            if (!pass.Equals(confirm))
            {
                lblMsg.Text = "Passwords do not match.";
                return;
            }
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                List<Dictionary<string, object>> users = new();
                if (File.Exists(path))
                {
                    var json = File.ReadAllText(path);
                    var des = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json, opts);
                    if (des != null) users = des;
                }
                foreach (var u in users)
                {
                    if (u.TryGetValue("email", out var eVal) && eVal?.ToString().Equals(email, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        lblMsg.Text = "Email already registered.";
                        return;
                    }
                }
                var newUser = new Dictionary<string, object>
                {
                    ["name"] = name,
                    ["email"] = email,
                    ["grade_section"] = string.IsNullOrWhiteSpace(grade) ? "N/A" : grade,
                    ["password"] = pass,
                    ["profile_picture"] = null,
                    ["role"] = role
                };
                users.Add(newUser);
                var writeOpts = new JsonSerializerOptions { WriteIndented = true };
                var outJson = JsonSerializer.Serialize(users, writeOpts);
                File.WriteAllText(path, outJson);
                RegisteredEmail = email ?? "";
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Registration failed: " + ex.Message;
            }
        }
    }
}
