using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
        private Button btnRegister;
        private Button btnCancel;
        private Label lblMsg;
        private Panel panelMain;
        private PictureBox? logoPictureBox;
        private Bitmap? backgroundBitmap;
        public string RegisteredEmail { get; private set; } = "";
        public FormRegister()
        {
            panelMain = new Panel();
            panelMain.Dock = DockStyle.Fill;
            panelMain.BackColor = Theme.LightGray;
            panelMain.BackgroundImage = Theme.CreateGradientBitmap(1920, 1080, vertical: true);
            panelMain.BackgroundImageLayout = ImageLayout.Stretch;
            panelMain.AutoScroll = true;
            
            Controls.Add(panelMain);
            
            // Add logo copyright
            AddLogoCopyright();
            SetLogoTransparentBackground();
            
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
            
            // Container panel for centering
            var containerPanel = new Panel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(20)
            };
            
            // Create a border panel
            var borderPanel = new Panel
            {
                Padding = new Padding(3),
                BackColor = Color.White,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            
            var center = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Width = 400,
                AutoSize = true,
                Padding = new Padding(30),
                BackColor = Theme.GetBackgroundTeal()
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
            center.Controls.Add(lblMsg);
            center.Controls.Add(new Label { Height = 20 });
            
            // Loading indicator
            var lblLoading = new Label
            {
                Text = "Creating account...",
                AutoSize = false,
                Height = 20,
                Width = 370,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ForeColor = Theme.AccentBlue,
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Visible = false
            };
            center.Controls.Add(lblLoading);
            center.Controls.Add(new Label { Height = 10 });
            
            var btnRow = new FlowLayoutPanel { FlowDirection = FlowDirection.LeftToRight, AutoSize = true, Width = 370 };
            btnRow.Controls.Add(btnRegister);
            btnRow.Controls.Add(new Label { Width = 20 }); 
            btnRow.Controls.Add(btnCancel);
            center.Controls.Add(btnRow);
            
            // Add panels in hierarchy
            borderPanel.Controls.Add(center);
            containerPanel.Controls.Add(borderPanel);
            panelMain.Controls.Add(containerPanel);
            
            // Center the container when resizing
            this.Resize += (s, e) => CenterContainer(containerPanel);
            this.Load += (s, e) => CenterContainer(containerPanel);
            
            btnRegister.Click += (s, e) => DoRegister();
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            AcceptButton = btnRegister;
            CancelButton = btnCancel;
            Theme.Apply(this);
            BackColor = Theme.GetBackgroundTeal();
            panelMain.BackColor = Theme.GetBackgroundTeal();
            containerPanel.BackColor = Theme.GetBackgroundTeal();
            btnRow.BackColor = Theme.GetBackgroundTeal();
        }
        
        private void CenterContainer(Panel container)
        {
            if (panelMain.ClientSize.Width > 0 && panelMain.ClientSize.Height > 0)
            {
                int x = Math.Max(0, (panelMain.ClientSize.Width - container.Width) / 2);
                int y = Math.Max(20, (panelMain.ClientSize.Height - container.Height) / 2);
                container.Location = new Point(x, y);
            }
        }
        
        private void DoRegister()
        {
            lblMsg.Text = "";
            lblMsg.Visible = false;
            var first = txtFirst.Text?.Trim() ?? "";
            var middle = txtMiddle.Text?.Trim() ?? "";
            var last = txtLast.Text?.Trim() ?? "";
            var email = txtEmail.Text?.Trim();
            var grade = txtGradeSection.Text?.Trim() ?? "";
            var pass = txtPassword.Text ?? "";
            var confirm = txtConfirm.Text ?? "";
            
            if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(last) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                lblMsg.Text = "Enter first name, last name, email and password.";
                lblMsg.Visible = true;
                return;
            }
            if (!pass.Equals(confirm))
            {
                lblMsg.Text = "Password and confirm password do not match.";
                lblMsg.Visible = true;
                return;
            }
            
            // Disable inputs and show loading
            btnRegister.Enabled = false;
            btnCancel.Enabled = false;
            txtFirst.Enabled = false;
            txtMiddle.Enabled = false;
            txtLast.Enabled = false;
            txtEmail.Enabled = false;
            txtGradeSection.Enabled = false;
            txtPassword.Enabled = false;
            txtConfirm.Enabled = false;
            
            // Find and show the loading label
            foreach (Control c in panelMain.Controls)
            {
                if (c is Panel containerPanel)
                {
                    foreach (Control bc in containerPanel.Controls)
                    {
                        if (bc is Panel borderPanel)
                        {
                            foreach (Control cp in borderPanel.Controls)
                            {
                                if (cp is FlowLayoutPanel centerPanel)
                                {
                                    foreach (Control fc in centerPanel.Controls)
                                    {
                                        if (fc is Label lbl && lbl.Text == "Creating account...")
                                            lbl.Visible = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            // Try to register via Flask API first (all users default to "user" role)
            Task.Run(async () => await RegisterViaAPI(first, middle, last, email, grade, pass));
        }

        private void ResetLoadingState()
        {
            Invoke((MethodInvoker)delegate
            {
                btnRegister.Enabled = true;
                btnCancel.Enabled = true;
                txtFirst.Enabled = true;
                txtMiddle.Enabled = true;
                txtLast.Enabled = true;
                txtEmail.Enabled = true;
                txtGradeSection.Enabled = true;
                txtPassword.Enabled = true;
                txtConfirm.Enabled = true;
                
                // Find and hide the loading label
                foreach (Control c in panelMain.Controls)
                {
                    if (c is Panel containerPanel)
                    {
                        foreach (Control bc in containerPanel.Controls)
                        {
                            if (bc is Panel borderPanel)
                            {
                                foreach (Control cp in borderPanel.Controls)
                                {
                                    if (cp is FlowLayoutPanel centerPanel)
                                    {
                                        foreach (Control fc in centerPanel.Controls)
                                        {
                                            if (fc is Label lbl && lbl.Text == "Creating account...")
                                                lbl.Visible = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            });
        }

        private async Task RegisterViaAPI(string firstName, string middleName, string lastName, string email, string gradeSection, string password)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    // Prepare the request
                    var registerData = new
                    {
                        first_name = firstName,
                        middle_name = middleName,
                        last_name = lastName,
                        email = email,
                        grade_section = gradeSection,
                        password = password,
                        confirm_password = password
                    };
                    var json = JsonSerializer.Serialize(registerData);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    // Call Flask API
                    var response = await client.PostAsync("http://localhost:5000/api/register", content);

                    if (response.IsSuccessStatusCode)
                    {
                        RegisteredEmail = email;
                        Invoke((MethodInvoker)delegate
                        {
                            DialogResult = DialogResult.OK;
                            Close();
                        });
                    }
                    else
                    {
                        try
                        {
                            var errorJson = await response.Content.ReadAsStringAsync();
                            var errorData = JsonSerializer.Deserialize<JsonElement>(errorJson);
                            var message = errorData.TryGetProperty("message", out var msgProp) ? msgProp.GetString() : "Registration failed";

                            Invoke((MethodInvoker)delegate
                            {
                                lblMsg.Text = message;
                                lblMsg.Visible = true;
                                ResetLoadingState();
                            });
                        }
                        catch
                        {
                            Invoke((MethodInvoker)delegate
                            {
                                lblMsg.Text = "Registration failed. Please try again.";
                                lblMsg.Visible = true;
                                ResetLoadingState();
                            });
                        }
                    }
                }
            }
            catch (HttpRequestException)
            {
                // Flask API not available, fallback to local JSON
                RegisterViaLocalJSON(firstName, middleName, lastName, email, gradeSection, password);
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    lblMsg.Text = "Registration error: " + ex.Message;
                    lblMsg.Visible = true;
                    ResetLoadingState();
                });
            }
        }

        private void RegisterViaLocalJSON(string firstName, string middleName, string lastName, string email, string gradeSection, string password)
        {
            try
            {
                var name = string.Join(" ", new[] { firstName, middleName, lastName }.Where(s => !string.IsNullOrWhiteSpace(s))).Trim();
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\users.json");
                path = Path.GetFullPath(path);
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
                    if (u.TryGetValue("email", out var eVal) && eVal?.ToString()?.Equals(email, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        Invoke((MethodInvoker)delegate
                        {
                            lblMsg.Text = "Email already registered.";
                            lblMsg.Visible = true;
                            ResetLoadingState();
                        });
                        return;
                    }
                }
                var newUser = new Dictionary<string, object>
                {
                    ["name"] = name,
                    ["email"] = email,
                    ["grade_section"] = string.IsNullOrWhiteSpace(gradeSection) ? "N/A" : gradeSection,
                    ["password"] = password,
                    ["profile_picture"] = (object?)null ?? "",
                    ["role"] = "user"
                };
                users.Add(newUser);
                var writeOpts = new JsonSerializerOptions { WriteIndented = true };
                var outJson = JsonSerializer.Serialize(users, writeOpts);
                File.WriteAllText(path, outJson);
                RegisteredEmail = email ?? "";
                Invoke((MethodInvoker)delegate
                {
                    DialogResult = DialogResult.OK;
                    Close();
                });
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    lblMsg.Text = "Registration failed: " + ex.Message;
                    lblMsg.Visible = true;
                    ResetLoadingState();
                });
            }
        }
        
        private void AddLogoCopyright()
        {
            try
            {
                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../logo.png");
                if (File.Exists(logoPath))
                {
                    logoPictureBox = new PictureBox
                    {
                        Image = Image.FromFile(logoPath),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Width = 40,
                        Height = 40,
                        BackColor = Color.Transparent,
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                    };
                    logoPictureBox.Location = new System.Drawing.Point(this.ClientSize.Width - 60, this.ClientSize.Height - 60);
                    
                    var copyrightLabel = new Label
                    {
                        Text = "© ReturnPoint 2026",
                        AutoSize = true,
                        BackColor = Color.Transparent,
                        ForeColor = Theme.DarkGray,
                        Font = new System.Drawing.Font("Segoe UI", 8F),
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                    };
                    copyrightLabel.Location = new System.Drawing.Point(this.ClientSize.Width - 140, this.ClientSize.Height - 30);
                    
                    this.Controls.Add(logoPictureBox);
                    this.Controls.Add(copyrightLabel);
                }
            }
            catch { /* Logo not found, continue without it */ }
        }

        private void SetLogoTransparentBackground()
        {
            try
            {
                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../logo.png");
                if (File.Exists(logoPath))
                {
                    Bitmap originalImage = new Bitmap(logoPath);
                    Bitmap transparentBitmap = new Bitmap(originalImage.Width, originalImage.Height);
                    transparentBitmap.MakeTransparent();
                    
                    for (int y = 0; y < originalImage.Height; y++)
                    {
                        for (int x = 0; x < originalImage.Width; x++)
                        {
                            Color originalColor = originalImage.GetPixel(x, y);
                            int newAlpha = (int)(originalColor.A * 0.35);
                            Color transparentColor = Color.FromArgb(newAlpha, originalColor.R, originalColor.G, originalColor.B);
                            transparentBitmap.SetPixel(x, y, transparentColor);
                        }
                    }
                    
                    backgroundBitmap = transparentBitmap;
                    this.BackgroundImage = backgroundBitmap;
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                    originalImage.Dispose();
                }
            }
            catch (Exception ex) 
            { 
                System.Diagnostics.Debug.WriteLine($"Error loading logo background: {ex.Message}");
            }
        }
    }
}