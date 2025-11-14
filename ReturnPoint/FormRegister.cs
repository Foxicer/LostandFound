using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace ReturnPoint
{
    public class FormRegister : Form
    {
        private TextBox txtName;
        private TextBox txtEmail;
        private TextBox txtGradeSection;
        private TextBox txtPassword;
        private TextBox txtConfirm;
        private ComboBox cbRole;
        private Button btnRegister;
        private Button btnCancel;
        private Label lblMsg;

        // Expose the registered email so login form can prefill
        public string RegisteredEmail { get; private set; }

        public FormRegister()
        {
            // Fullscreen, centered layout (matches other auth forms)
            Text = "Register Account";
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;

            var labelFont = new System.Drawing.Font("Segoe UI", 12F);
            var inputFont = new System.Drawing.Font("Segoe UI", 12F);

            txtName = new TextBox { Width = 520, Font = inputFont };
            txtEmail = new TextBox { Width = 520, Font = inputFont };
            txtGradeSection = new TextBox { Width = 520, Font = inputFont };
            txtPassword = new TextBox { Width = 520, UseSystemPasswordChar = true, Font = inputFont };
            txtConfirm = new TextBox { Width = 520, UseSystemPasswordChar = true, Font = inputFont };

            cbRole = new ComboBox { Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
            cbRole.Items.AddRange(new[] { "user", "admin" });
            cbRole.SelectedIndex = 0;

            btnRegister = new Button { Text = "Create Account", Width = 260, Height = 44, Font = labelFont };
            btnCancel = new Button { Text = "Cancel", Width = 260, Height = 44, Font = labelFont };
            lblMsg = new Label { AutoSize = false, TextAlign = System.Drawing.ContentAlignment.MiddleCenter, Height = 32, ForeColor = System.Drawing.Color.Red, Font = inputFont };

            var lblName = new Label { Text = "Full Name", AutoSize = true, Font = labelFont };
            var lblEmail = new Label { Text = "Email (Gmail recommended)", AutoSize = true, Font = labelFont };
            var lblGrade = new Label { Text = "Grade and Section", AutoSize = true, Font = labelFont };
            var lblP = new Label { Text = "Password", AutoSize = true, Font = labelFont };
            var lblC = new Label { Text = "Confirm Password", AutoSize = true, Font = labelFont };
            var lblR = new Label { Text = "Role", AutoSize = true, Font = labelFont };

            var main = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 3 };
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 33));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 34));
            main.RowStyles.Add(new RowStyle(SizeType.Percent, 33));

            var center = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Width = 560,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                Padding = new Padding(12)
            };

            center.Controls.Add(lblName);
            center.Controls.Add(txtName);
            center.Controls.Add(lblEmail);
            center.Controls.Add(txtEmail);
            center.Controls.Add(lblGrade);
            center.Controls.Add(txtGradeSection);
            center.Controls.Add(lblP);
            center.Controls.Add(txtPassword);
            center.Controls.Add(lblC);
            center.Controls.Add(txtConfirm);
            center.Controls.Add(lblR);
            center.Controls.Add(cbRole);
            center.Controls.Add(lblMsg);

            var btnRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true };
            btnRow.Controls.Add(btnRegister);
            btnRow.Controls.Add(btnCancel);
            center.Controls.Add(btnRow);

            main.Controls.Add(center, 1, 1);
            Controls.Add(main);

            btnRegister.Click += (s, e) => DoRegister();
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };

            AcceptButton = btnRegister;
            CancelButton = btnCancel;

            Theme.Apply(this);
        }

        private void DoRegister()
        {
            lblMsg.Text = "";
            var name = txtName.Text?.Trim();
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
                    ["grade_section"] = grade,
                    ["password"] = pass,
                    ["profile_picture"] = null,
                    ["role"] = role
                };

                users.Add(newUser);
                var writeOpts = new JsonSerializerOptions { WriteIndented = true };
                var outJson = JsonSerializer.Serialize(users, writeOpts);
                File.WriteAllText(path, outJson);

                RegisteredEmail = email;
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