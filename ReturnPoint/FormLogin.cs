using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
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
        private PictureBox? logoPictureBox;
        private Bitmap? backgroundBitmap;
        public User? AuthenticatedUser { get; private set; }
        public FormLogin()
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
            PictureBox logoLeft = new PictureBox
            {
            SizeMode = PictureBoxSizeMode.Zoom,
            Width = 60,
            Height = 80,
            Margin = new Padding(0, 0, 12, 0),
            BackColor = Color.Transparent
           };
           
           // Load logo image if it exists
           try
           {
               string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../logo.png");
               if (File.Exists(logoPath))
               {
                   logoLeft.Image = Image.FromFile(logoPath);
               }
           }
           catch { /* Logo not found, continue without it */ }
           logoLeft.Size = new Size(150,100 );
           logoLeft.MinimumSize = logoLeft.Size;
var titlePanel = new FlowLayoutPanel
{
    FlowDirection = FlowDirection.LeftToRight,
    AutoSize = true,
    Width = 400,
    Height = 100,   
    WrapContents = false,
    BackColor = Color.Transparent,
    Margin = new Padding(0, 0, 0, 10)
};
var textPanel = new FlowLayoutPanel
{
     FlowDirection = FlowDirection.TopDown,
    AutoSize = true,
    WrapContents = false,
    BackColor = Color.Transparent,
    Margin = new Padding(0, 10, 0, 0)
};

textPanel.Controls.Add(lblTitle);
textPanel.Controls.Add(lblSubtitle);

titlePanel.Controls.Add(logoLeft);
titlePanel.Controls.Add(textPanel);
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
            
            var centerPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Width = 420,
                AutoSize = true,
                Padding = new Padding(30),
                BackColor = Theme.GetBackgroundTeal()
            };
            
            centerPanel.Controls.Add(titlePanel);
            centerPanel.Controls.Add(new Label { Height = 30 });
            centerPanel.Controls.Add(lblEmail);
            centerPanel.Controls.Add(new Label { Height = 6 });
            centerPanel.Controls.Add(txtEmail);
            centerPanel.Controls.Add(new Label { Height = 16 });
            centerPanel.Controls.Add(lblP);
            centerPanel.Controls.Add(new Label { Height = 6 });
            centerPanel.Controls.Add(txtPass);
            centerPanel.Controls.Add(new Label { Height = 16 });
            centerPanel.Controls.Add(lblMsg);
            centerPanel.Controls.Add(new Label { Height = 20 });
            
            // Loading indicator
            var lblLoading = new Label
            {
                Text = "Signing in...",
                AutoSize = false,
                Height = 20,
                Width = 380,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
                ForeColor = Theme.AccentBlue,
                Font = new System.Drawing.Font("Segoe UI", 10F),
                Visible = false
            };
            centerPanel.Controls.Add(lblLoading);
            centerPanel.Controls.Add(new Label { Height = 10 });
            
            var btnRow = new FlowLayoutPanel 
            { 
                FlowDirection = FlowDirection.TopDown, 
                AutoSize = true, 
                WrapContents = false,
                Width = 380
            };
            btnRow.Controls.Add(btnLogin);
            btnRow.Controls.Add(new Label { Height = 10 });
            var secondRowBtns = new FlowLayoutPanel 
            { 
                FlowDirection = FlowDirection.LeftToRight, 
                AutoSize = true, 
                WrapContents = false
            };
            secondRowBtns.Controls.Add(btnRegister);
            secondRowBtns.Controls.Add(new Label { Width = 20 });
            secondRowBtns.Controls.Add(btnCancel);
            btnRow.Controls.Add(secondRowBtns);
            centerPanel.Controls.Add(btnRow);
            
            // Add panels in hierarchy
            borderPanel.Controls.Add(centerPanel);
            containerPanel.Controls.Add(borderPanel);
            panelMain.Controls.Add(containerPanel);
            
            // Center the container when resizing
            this.Resize += (s, e) => CenterContainer(containerPanel);
            this.Load += (s, e) => CenterContainer(containerPanel);
            
            btnLogin.Click += (s, e) => TryLogin();
            btnRegister.Click += (s, e) =>
            {
                using var reg = new FormRegister();
                if (reg.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(reg.RegisteredEmail))
                {
                    txtEmail.Text = reg.RegisteredEmail;
                    lblMsg.Text = "Account created. You can now login.";
                    lblMsg.Visible = true;
                }
            };
            btnCancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            AcceptButton = btnLogin;
            CancelButton = btnCancel;
            Theme.Apply(this);
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
        
        private void ResetLoadingState()
        {
            Invoke((MethodInvoker)delegate
            {
                btnLogin.Enabled = true;
                btnRegister.Enabled = true;
                txtEmail.Enabled = true;
                txtPass.Enabled = true;
                
                // Find and hide the loading label in the center panel
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
                                            if (fc is Label lbl && lbl.Text == "Signing in...")
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

        private void TryLogin()
        {
            lblMsg.Text = "";
            lblMsg.Visible = false;
            var email = txtEmail.Text?.Trim();
            var pass = txtPass.Text ?? "";
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                lblMsg.Text = "Enter email and password.";
                lblMsg.Visible = true;
                return;
            }
            if (!email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                lblMsg.Text = "Please sign in with a Gmail address.";
                lblMsg.Visible = true;
                return;
            }
            
            // Disable inputs and show loading
            btnLogin.Enabled = false;
            btnRegister.Enabled = false;
            txtEmail.Enabled = false;
            txtPass.Enabled = false;
            
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
                                        if (fc is Label lbl && lbl.Text == "Signing in...")
                                            lbl.Visible = true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            // Try to login via Flask API first (Google Sheets)
            Task.Run(async () => await LoginViaAPI(email, pass));
        }

        private async Task LoginViaAPI(string email, string password)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    
                    // Prepare the request
                    var loginData = new { email = email, password = password };
                    var json = JsonSerializer.Serialize(loginData);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    // Call Flask API
                    var response = await client.PostAsync("http://localhost:5000/api/login", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Login successful - get user data with email parameter (since C# HttpClient doesn't share session cookies)
                        var userResponse = await client.GetAsync($"http://localhost:5000/api/user?email={Uri.EscapeDataString(email)}");
                        if (userResponse.IsSuccessStatusCode)
                        {
                            var userJson = await userResponse.Content.ReadAsStringAsync();
                            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var userData = JsonSerializer.Deserialize<JsonElement>(userJson, opts);

                            var user = new User
                            {
                                Name = userData.TryGetProperty("name", out var nameProp) ? nameProp.GetString() ?? email : email,
                                FirstName = userData.TryGetProperty("first_name", out var fnProp) ? fnProp.GetString() ?? "" : "",
                                MiddleName = userData.TryGetProperty("middle_name", out var mnProp) ? mnProp.GetString() ?? "" : "",
                                LastName = userData.TryGetProperty("last_name", out var lnProp) ? lnProp.GetString() ?? "" : "",
                                Email = userData.TryGetProperty("email", out var emailProp) ? emailProp.GetString() ?? email : email,
                                GradeSection = userData.TryGetProperty("grade_section", out var gradeProp) ? gradeProp.GetString() ?? "" : "",
                                Password = password,
                                Role = userData.TryGetProperty("role", out var roleProp) ? roleProp.GetString() ?? "user" : "user"
                            };

                            AuthenticatedUser = user;
                            
                            // Save email to project root so Gallery can read it (do this before closing dialog)
                            try
                            {
                                string projectRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\");
                                projectRoot = Path.GetFullPath(projectRoot);
                                string emailFilePath = Path.Combine(projectRoot, "current_user_email.txt");
                                System.Diagnostics.Debug.WriteLine($"[FormLogin] Writing email to: {emailFilePath}");
                                File.WriteAllText(emailFilePath, email);
                                System.Diagnostics.Debug.WriteLine($"[FormLogin] ✓ Saved email successfully");
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine($"[FormLogin] ✗ Error saving email: {ex.Message}");
                            }
                            
                            Invoke((MethodInvoker)delegate
                            {
                                DialogResult = DialogResult.OK;
                                Close();
                            });
                        }
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        // Wrong credentials
                        Invoke((MethodInvoker)delegate
                        {
                            lblMsg.Text = "Incorrect email or password.";
                            lblMsg.Visible = true;
                            ResetLoadingState();
                        });
                    }
                    else
                    {
                        try
                        {
                            var errorJson = await response.Content.ReadAsStringAsync();
                            var errorData = JsonSerializer.Deserialize<JsonElement>(errorJson);
                            var message = errorData.TryGetProperty("message", out var msgProp) ? msgProp.GetString() : "Incorrect email or password.";

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
                                lblMsg.Text = "Incorrect email or password.";
                                lblMsg.Visible = true;
                                ResetLoadingState();
                            });
                        }
                    }
                }
            }
            catch (HttpRequestException)
            {
                // Flask API not available - show specific error message
                Invoke((MethodInvoker)delegate
                {
                    lblMsg.Text = "Flask server is not running. Please start the Flask API.";
                    lblMsg.ForeColor = Color.Orange;
                    lblMsg.Visible = true;
                    ResetLoadingState();
                });
                
                // Also try fallback to local JSON
                LoginViaLocalJSON(email, password);
            }
            catch (TaskCanceledException)
            {
                // Timeout - Flask not responding
                Invoke((MethodInvoker)delegate
                {
                    lblMsg.Text = "Flask server is not responding. Please check if it's running.";
                    lblMsg.ForeColor = Color.Orange;
                    lblMsg.Visible = true;
                    ResetLoadingState();
                });
                
                // Try fallback to local JSON
                LoginViaLocalJSON(email, password);
            }
            catch (Exception ex)
            {
                Invoke((MethodInvoker)delegate
                {
                    lblMsg.Text = "Connection error: " + ex.Message;
                    lblMsg.Visible = true;
                    ResetLoadingState();
                });
            }
        }

        private void LoginViaLocalJSON(string email, string password)
        {
            try
            {
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\users.json");
                path = Path.GetFullPath(path);
                if (!File.Exists(path))
                {
                    Invoke((MethodInvoker)delegate
                    {
                        lblMsg.Text = "No users registered locally.";
                        lblMsg.Visible = true;
                        ResetLoadingState();
                    });
                    return;
                }
                var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var json = File.ReadAllText(path);
                var users = JsonSerializer.Deserialize<List<User>>(json, opts) ?? new List<User>();
                
                // Check if email exists
                var userWithEmail = users.FirstOrDefault(u =>
                    string.Equals(u.Email ?? "", email, StringComparison.OrdinalIgnoreCase));
                
                if (userWithEmail == null)
                {
                    Invoke((MethodInvoker)delegate
                    {
                        lblMsg.Text = "Incorrect email or password.";
                        lblMsg.ForeColor = Theme.DeepRed;
                        lblMsg.Visible = true;
                        ResetLoadingState();
                    });
                    return;
                }
                
                // Email exists, check password
                if (!string.Equals(userWithEmail.Password ?? "", password, StringComparison.Ordinal))
                {
                    Invoke((MethodInvoker)delegate
                    {
                        lblMsg.Text = "Incorrect email or password.";
                        lblMsg.ForeColor = Theme.DeepRed;
                        lblMsg.Visible = true;
                        ResetLoadingState();
                    });
                    return;
                }
                
                var found = userWithEmail;
                
                // Parse full name into parts if needed
                if (string.IsNullOrWhiteSpace(found.FirstName) && !string.IsNullOrWhiteSpace(found.Name))
                {
                    var parts = found.Name.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 1)
                        found.FirstName = parts[0];
                    if (parts.Length >= 2)
                        found.MiddleName = parts[1];
                    if (parts.Length >= 3)
                        found.LastName = string.Join(" ", parts.Skip(2));
                }
                
                AuthenticatedUser = found;
                
                // Save email to project root so Gallery can read it (do this before closing dialog)
                try
                {
                    string projectRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\");
                    projectRoot = Path.GetFullPath(projectRoot);
                    string emailFilePath = Path.Combine(projectRoot, "current_user_email.txt");
                    System.Diagnostics.Debug.WriteLine($"[FormLogin] Writing email to: {emailFilePath}");
                    File.WriteAllText(emailFilePath, email);
                    System.Diagnostics.Debug.WriteLine($"[FormLogin] ✓ Saved email successfully");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[FormLogin] ✗ Error saving email: {ex.Message}");
                }
                
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
                    lblMsg.Text = "Local login error: " + ex.Message;
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