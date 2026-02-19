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
        private string selectedRole = "user"; // user, admin, headadmin
        public string RegisteredEmail { get; private set; } = "";
        public FormRegister(string defaultRole = "user")
        {
            selectedRole = defaultRole;

            string roleDisplayText = defaultRole switch
            {
                "admin" => "🔐 Admin Registration",
                "headadmin" => "👑 HeadAdmin Registration",
                _ => "📝 User Registration"
            };

            Text = roleDisplayText + " - ReturnPoint";
            Width = 950;
            Height = 800;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Theme.GetBackgroundTeal();
            BackgroundImage = Theme.CreateGradientBitmap(950, 800, vertical: true);
            BackgroundImageLayout = ImageLayout.Stretch;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            // ===== MAIN CONTAINER =====
            Panel mainContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(40),
                AutoScroll = true
            };

            // ===== BORDERED REGISTRATION AREA =====
            Panel registerBox = new Panel
            {
                Dock = DockStyle.Top,
                BackColor = Theme.DarkTeal,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(30),
                AutoSize = true
            };

            FlowLayoutPanel formFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoSize = true,
                BackColor = Theme.DarkTeal
            };

            // ===== HEADER =====
            Label lblTitle = new Label
            {
                Text = roleDisplayText,
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 5)
            };
            formFlow.Controls.Add(lblTitle);

            Label lblSubtitle = new Label
            {
                Text = "Fill in your information below",
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 11),
                ForeColor = Theme.LightGray,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0, 0, 0, 20)
            };
            formFlow.Controls.Add(lblSubtitle);

            // ===== FULL NAME SECTION =====
            Label lblName = new Label
            {
                Text = "👤 Full Name",
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Margin = new Padding(0, 10, 0, 8)
            };
            formFlow.Controls.Add(lblName);

            // First name
            Label lblFirstName = new Label
            {
                Text = "First name",
                Dock = DockStyle.Top,
                Height = 16,
                Font = new Font("Segoe UI", 9),
                ForeColor = Theme.LightGray,
                Margin = new Padding(0, 0, 0, 3)
            };
            formFlow.Controls.Add(lblFirstName);

            txtFirst = new TextBox
            {
                Width = 350,
                Height = 40,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 12)
            };
            formFlow.Controls.Add(txtFirst);

            // Middle name
            Label lblMiddleName = new Label
            {
                Text = "Middle name",
                Dock = DockStyle.Top,
                Height = 16,
                Font = new Font("Segoe UI", 9),
                ForeColor = Theme.LightGray,
                Margin = new Padding(0, 0, 0, 3)
            };
            formFlow.Controls.Add(lblMiddleName);

            txtMiddle = new TextBox
            {
                Width = 350,
                Height = 40,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 12)
            };
            formFlow.Controls.Add(txtMiddle);

            // Last name
            Label lblLastName = new Label
            {
                Text = "Last name",
                Dock = DockStyle.Top,
                Height = 16,
                Font = new Font("Segoe UI", 9),
                ForeColor = Theme.LightGray,
                Margin = new Padding(0, 0, 0, 3)
            };
            formFlow.Controls.Add(lblLastName);

            txtLast = new TextBox
            {
                Width = 350,
                Height = 40,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 15)
            };
            formFlow.Controls.Add(txtLast);

            // ===== EMAIL SECTION =====
            Label lblEmail = new Label
            {
                Text = "📧 Email Address",
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Margin = new Padding(0, 10, 0, 8)
            };
            formFlow.Controls.Add(lblEmail);

            txtEmail = new TextBox
            {
                Width = 350,
                Height = 40,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 0, 0, 15)
            };
            formFlow.Controls.Add(txtEmail);

            // ===== GRADE/SECTION - Only for students =====
            if (defaultRole == "user")
            {
                Label lblGrade = new Label
                {
                    Text = "🏫 Grade and Section",
                    Dock = DockStyle.Top,
                    Height = 20,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    ForeColor = Color.White,
                    Margin = new Padding(0, 10, 0, 8)
                };
                formFlow.Controls.Add(lblGrade);

                txtGradeSection = new TextBox
                {
                    Width = 350,
                    Height = 40,
                    Font = new Font("Segoe UI", 10),
                    BackColor = Color.White,
                    ForeColor = Theme.NearBlack,
                    BorderStyle = BorderStyle.FixedSingle,
                    PlaceholderText = "e.g., Grade 10-A",
                    Margin = new Padding(0, 0, 0, 15)
                };
                formFlow.Controls.Add(txtGradeSection);
            }
            else
            {
                // Pre-fill for admin/headadmin
                txtGradeSection = new TextBox { Visible = false };
                txtGradeSection.Text = defaultRole;
            }

            // ===== PASSWORD SECTION =====
            Label lblPassword = new Label
            {
                Text = "🔒 Password",
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Margin = new Padding(0, 10, 0, 8)
            };
            formFlow.Controls.Add(lblPassword);

            Panel passwordPanel = new Panel
            {
                Width = 350,
                Height = 40,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 15)
            };

            txtPassword = new TextBox
            {
                UseSystemPasswordChar = true,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };

            Button btnTogglePassword = new Button
            {
                Text = "👁️",
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                FlatStyle = FlatStyle.Flat,
                Width = 40,
                Height = 40,
                Dock = DockStyle.Right,
                Margin = new Padding(0)
            };
            btnTogglePassword.FlatAppearance.BorderSize = 0;
            btnTogglePassword.Tag = false;
            btnTogglePassword.Click += (s, e) =>
            {
                bool isVisible = (bool)btnTogglePassword.Tag;
                txtPassword.UseSystemPasswordChar = isVisible;
                btnTogglePassword.Tag = !isVisible;
                btnTogglePassword.Text = isVisible ? "👁️" : "👁️‍🗨️";
            };

            passwordPanel.Controls.Add(txtPassword);
            passwordPanel.Controls.Add(btnTogglePassword);
            formFlow.Controls.Add(passwordPanel);

            // ===== CONFIRM PASSWORD SECTION =====
            Label lblConfirm = new Label
            {
                Text = "🔒 Confirm Password",
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Margin = new Padding(0, 10, 0, 8)
            };
            formFlow.Controls.Add(lblConfirm);

            Panel confirmPanel = new Panel
            {
                Width = 350,
                Height = 40,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 0, 0, 20)
            };

            txtConfirm = new TextBox
            {
                UseSystemPasswordChar = true,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };

            Button btnToggleConfirm = new Button
            {
                Text = "👁️",
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                FlatStyle = FlatStyle.Flat,
                Width = 40,
                Height = 40,
                Dock = DockStyle.Right,
                Margin = new Padding(0)
            };
            btnToggleConfirm.FlatAppearance.BorderSize = 0;
            btnToggleConfirm.Tag = false;
            btnToggleConfirm.Click += (s, e) =>
            {
                bool isVisible = (bool)btnToggleConfirm.Tag;
                txtConfirm.UseSystemPasswordChar = isVisible;
                btnToggleConfirm.Tag = !isVisible;
                btnToggleConfirm.Text = isVisible ? "👁️" : "👁️‍🗨️";
            };

            confirmPanel.Controls.Add(txtConfirm);
            confirmPanel.Controls.Add(btnToggleConfirm);
            formFlow.Controls.Add(confirmPanel);

            // ===== ERROR MESSAGE =====
            lblMsg = new Label
            {
                Width = 350,
                Height = 40,
                Font = new Font("Segoe UI", 10),
                ForeColor = Theme.DeepRed,
                BackColor = Theme.DarkTeal,
                TextAlign = ContentAlignment.TopLeft,
                AutoSize = false,
                Visible = false,
                Margin = new Padding(0, 10, 0, 20)
            };
            formFlow.Controls.Add(lblMsg);

            // ===== BUTTONS =====
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel
            {
                Width = 350,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Theme.DarkTeal
            };

            btnRegister = new Button
            {
                Text = "✓ Create Account",
                Width = 350,
                Height = 44,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Theme.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 10)
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            buttonPanel.Controls.Add(btnRegister);

            btnCancel = new Button
            {
                Text = "✕ Back",
                Width = 350,
                Height = 40,
                Font = new Font("Segoe UI", 10),
                BackColor = Theme.DarkGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            buttonPanel.Controls.Add(btnCancel);

            formFlow.Controls.Add(buttonPanel);
            registerBox.Controls.Add(formFlow);
            mainContainer.Controls.Add(registerBox);
            Controls.Add(mainContainer);

            // ===== EVENT HANDLERS =====
            btnRegister.Click += (s, e) => DoRegister();
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            AcceptButton = btnRegister;
            CancelButton = btnCancel;
            AddLogoCopyright();
            SetLogoTransparentBackground();
            Theme.Apply(this);
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
            
            ShowLoadingScreen("Creating account....");
            
            // Try to register via Flask API first (all users default to "user" role)
            Task.Run(async () => await RegisterViaAPI(first, middle, last, email, grade, pass));
        }

        private void ResetLoadingState()
        {
            HideLoadingScreen();
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
                        confirm_password = password,
                        role = selectedRole
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
                    ["role"] = selectedRole
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
        
        private Form? loadingForm;
        private bool isLoading = false;
        
        private void ShowLoadingScreen(string message = "Loading...")
        {
            if (isLoading) return;
            isLoading = true;
            
            loadingForm = new Form
            {
                Text = "Loading",
                FormBorderStyle = FormBorderStyle.None,
                Size = new Size(300, 150),
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Theme.GetBackgroundTeal(),
                TopMost = true,
                ControlBox = false
            };
            
            Panel contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.DarkTeal,
                Padding = new Padding(20)
            };
            
            Label messageLabel = new Label
            {
                Text = message,
                AutoSize = true,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 20)
            };
            
            Label spinnerLabel = new Label
            {
                Text = "⏳",
                AutoSize = true,
                Font = new Font("Segoe UI", 40),
                ForeColor = Theme.AccentBlue,
                Location = new Point(110, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Width = 80,
                Height = 80
            };
            
            contentPanel.Controls.Add(messageLabel);
            contentPanel.Controls.Add(spinnerLabel);
            loadingForm.Controls.Add(contentPanel);
            
            System.Windows.Forms.Timer animationTimer = new System.Windows.Forms.Timer();
            int spinnerIndex = 0;
            string[] spinners = new string[] { "⏳", "⌛" };
            
            animationTimer.Interval = 500;
            animationTimer.Tick += (s, e) =>
            {
                try
                {
                    if (spinnerLabel.InvokeRequired)
                    {
                        spinnerLabel.Invoke((MethodInvoker)delegate
                        {
                            spinnerIndex = (spinnerIndex + 1) % spinners.Length;
                            spinnerLabel.Text = spinners[spinnerIndex];
                        });
                    }
                    else
                    {
                        spinnerIndex = (spinnerIndex + 1) % spinners.Length;
                        spinnerLabel.Text = spinners[spinnerIndex];
                    }
                }
                catch { }
            };
            animationTimer.Start();
            
            loadingForm.FormClosed += (s, e) =>
            {
                animationTimer.Stop();
                animationTimer.Dispose();
            };
            
            loadingForm.Show(this);
        }
        
        private void HideLoadingScreen()
        {
            if (loadingForm != null && !loadingForm.IsDisposed)
            {
                try
                {
                    Invoke((MethodInvoker)delegate
                    {
                        loadingForm?.Close();
                        loadingForm?.Dispose();
                        loadingForm = null;
                        isLoading = false;
                    });
                }
                catch { }
            }
        }
    }
}