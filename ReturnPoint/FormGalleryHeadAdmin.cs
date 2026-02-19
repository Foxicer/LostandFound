using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ReturnPoint
{
    public class FormGalleryHeadAdmin : Form
    {
        private Panel outerPanel;
        private TableLayoutPanel galleryTable;
        private Panel rightPanel;
        private ComboBox cbViewMode;
        private TextBox txtSearch;
        private Label lblSelectedFile;
        private Label lblDateAdded;
        private Button btnDelete;
        private Button btnRestore;
        private Button btnPermanentlyDelete;
        private Button btnRefresh;
        private Button btnImageDetails;
        private Button btnTagManager;
        private Button btnCreateAccount;
        private Button btnInbox;
        private Button btnLogout;
        private FlowLayoutPanel pnlTags;
        private TextBox txtNewTag;
        private string saveFolder;
        private string deletedFolder;
        private Panel selectedCard;
        private PictureBox? logoPictureBox;
        private Bitmap? backgroundBitmap;
        private int COLUMNS = 5;
        private const int IMAGE_SIZE = 220;
        private Form? loadingForm;
        private bool isLoading = false;
        
        private int CalculateColumnsForWidth(int availableWidth)
        {
            // Minimum space per column: IMAGE_SIZE + margins + padding
            int minSpacePerColumn = IMAGE_SIZE + 30; // 220 + 10 margins on each side + some padding
            int cols = Math.Max(1, availableWidth / minSpacePerColumn);
            return Math.Min(cols, 8); // Cap at 8 columns maximum
        }
        
        public FormGalleryHeadAdmin()
        {
            Text = "Gallery HeadAdmin - ReturnPoint";
            WindowState = FormWindowState.Maximized;
            BackColor = Theme.GetBackgroundTeal();
            // Update background dynamically based on current size
            UpdateBackgroundImage();
            this.BackgroundImageLayout = ImageLayout.Stretch;
            this.Resize += (s, e) => UpdateBackgroundImage();
            saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CapturedImages");
            deletedFolder = Path.Combine(saveFolder, "Deleted");
            Directory.CreateDirectory(saveFolder);
            Directory.CreateDirectory(deletedFolder);
            outerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Theme.GetBackgroundTeal(),
                BackgroundImageLayout = ImageLayout.Stretch,
                AutoScroll = false,
                Padding = new Padding(0),
                BorderStyle = BorderStyle.FixedSingle
            };
            // Update outer panel background dynamically
            this.Resize += (s, e) => {
                if (outerPanel.Width > 0 && outerPanel.Height > 0)
                    outerPanel.BackgroundImage = Theme.CreateGradientBitmap(outerPanel.Width, outerPanel.Height, vertical: true);
            };
            
            // Create a scrollable container for the gallery table
            Panel scrollableContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Theme.GetBackgroundTeal(),
                BackgroundImageLayout = ImageLayout.Stretch,
                Padding = new Padding(0)
            };
            
            galleryTable = new TableLayoutPanel
            {
                ColumnCount = COLUMNS,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(30, 20, 30, 20),
                BackColor = Theme.GetBackgroundTeal(),
                Dock = DockStyle.None,
                Location = new Point(0, 0)
            };
            
            // Set column styles
            for (int i = 0; i < COLUMNS; i++)
            {
                galleryTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }
            
            scrollableContainer.Controls.Add(galleryTable);
            outerPanel.Controls.Add(scrollableContainer);
            
            // Create responsive right panel using FlowLayoutPanel
            FlowLayoutPanel rightPanelFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                Width = Math.Max(300, (int)(Screen.PrimaryScreen.Bounds.Width * 0.15)),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(15),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            rightPanel = rightPanelFlow;
            
            // Helper function to create responsive label
            Label CreateLabel(string text, bool bold = false) => new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 11, bold ? FontStyle.Bold : FontStyle.Regular),
                ForeColor = Theme.NearBlack,
                MaximumSize = new Size(rightPanelFlow.Width - 40, 0),
                Margin = new Padding(0, 10, 0, 5)
            };
            
            // Helper for responsive textbox
            TextBox CreateTextBox(int height = 35) => new TextBox
            {
                Dock = DockStyle.Top,
                Height = height,
                Font = new Font("Segoe UI", 11),
                BackColor = Theme.SoftWhite,
                ForeColor = Theme.NearBlack,
                Margin = new Padding(0, 5, 0, 10)
            };
            
            // Helper for responsive button
            Button CreateButton(string text, Color bgColor) => new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = bgColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 5, 0, 5)
            };
            
            Label lblSearchTitle = CreateLabel("🔍 Search", true);
            txtSearch = CreateTextBox(35);
            
            Label lblSelected = CreateLabel("Selected File", true);
            lblSelectedFile = new Label
            {
                Text = "(none)",
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 60,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Theme.GetBackgroundTeal(),
                ForeColor = Theme.NearBlack,
                Font = new Font("Segoe UI", 10),
                Padding = new Padding(8),
                Margin = new Padding(0, 5, 0, 10)
            };
            
            lblDateAdded = new Label
            {
                Text = "📅 Added: N/A",
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 9),
                ForeColor = Theme.NearBlack,
                BackColor = Color.Transparent,
                Margin = new Padding(0, 5, 0, 10)
            };
            
            // Delete and Restore buttons in a row
            FlowLayoutPanel deleteRestorePanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.LeftToRight,
                Dock = DockStyle.Top,
                Height = 50,
                AutoSize = false,
                Margin = new Padding(0, 5, 0, 5)
            };
            
            btnDelete = new Button
            {
                Text = "🗑️ Delete",
                Width = 130,
                Height = 40,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Theme.WarmGold,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDelete.FlatAppearance.BorderSize = 0;
            
            btnRestore = new Button
            {
                Text = "✓ Restore",
                Width = 130,
                Height = 40,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Theme.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false,
                Margin = new Padding(5, 0, 0, 0)
            };
            btnRestore.FlatAppearance.BorderSize = 0;
            
            deleteRestorePanel.Controls.Add(btnDelete);
            deleteRestorePanel.Controls.Add(btnRestore);
            
            btnPermanentlyDelete = CreateButton("❌ Delete Permanently", Theme.DeepRed);
            btnRefresh = CreateButton("🔄 Refresh", Theme.AccentBlue);
            btnImageDetails = CreateButton("📋 Image Details", Theme.MediumTeal);
            btnTagManager = CreateButton("🏷️ Manage Tags", Theme.TealGreen);
            btnCreateAccount = CreateButton("➕ Create Account", Theme.Success);
            btnInbox = CreateButton("📬 Inbox", Theme.WarmGold);
            
            Label lblPendingCount = new Label
            {
                Text = "",
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Theme.DeepRed,
                BackColor = Color.Transparent,
                Margin = new Padding(0, -5, 0, 10)
            };
            
            cbViewMode = new ComboBox
            {
                Dock = DockStyle.Top,
                Height = 36,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                BackColor = Theme.GetBackgroundTeal(),
                Margin = new Padding(0, 5, 0, 10)
            };
            cbViewMode.Items.AddRange(new[] { "Active Items", "Deleted Items" });
            cbViewMode.SelectedIndex = 0;
            
            btnLogout = CreateButton("🚪 Logout", Theme.DeepRed);
            
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Logout and return to login?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Application.Restart();
                    Application.ExitThread();
                }
            };
            
            btnCreateAccount.Click += (s, e) =>
            {
                OpenCreateAccountForm();
            };
            
            // Add all controls to the responsive panel
            rightPanelFlow.Controls.Add(lblSearchTitle);
            rightPanelFlow.Controls.Add(txtSearch);
            rightPanelFlow.Controls.Add(lblSelected);
            rightPanelFlow.Controls.Add(lblSelectedFile);
            rightPanelFlow.Controls.Add(lblDateAdded);
            rightPanelFlow.Controls.Add(deleteRestorePanel);
            rightPanelFlow.Controls.Add(btnPermanentlyDelete);
            rightPanelFlow.Controls.Add(btnRefresh);
            rightPanelFlow.Controls.Add(btnImageDetails);
            rightPanelFlow.Controls.Add(btnTagManager);
            rightPanelFlow.Controls.Add(btnCreateAccount);
            rightPanelFlow.Controls.Add(btnInbox);
            rightPanelFlow.Controls.Add(lblPendingCount);
            rightPanelFlow.Controls.Add(cbViewMode);
            rightPanelFlow.Controls.Add(btnLogout);
            
            Controls.Add(rightPanel);
            Controls.Add(outerPanel);
            cbViewMode.SelectedIndexChanged += (s, e) => LoadImages(cbViewMode.SelectedIndex == 1, txtSearch.Text);
            btnRefresh.Click += (s, e) => LoadImages(cbViewMode.SelectedIndex == 1, txtSearch.Text);
            btnDelete.Click += (s, e) => DeleteSelected();
            btnRestore.Click += (s, e) => RestoreSelected();
            btnPermanentlyDelete.Click += (s, e) => PermanentlyDeleteSelected();
            btnImageDetails.Click += (s, e) => OpenImageDetailsForm();
            btnTagManager.Click += (s, e) => OpenTagManager();
            btnInbox.Click += (s, e) =>
            {
                int pending = LoadClaimsData().Count(c => c.Status == "pending");
                lblPendingCount.Text = pending > 0 ? $"({pending} pending)" : "";
                OpenInbox();
            };
            txtSearch.TextChanged += (s, e) => LoadImages(cbViewMode.SelectedIndex == 1, txtSearch.Text);
            LoadImages(false);
            AddLogoCopyright();
            SetLogoTransparentBackground();
        }
        
        private void OpenCreateAccountForm()
        {
            Form createAccountForm = new Form
            {
                Text = "Create Account - ReturnPoint",
                Width = 700,
                Height = 800,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Theme.SoftWhite,
                Font = new Font("Segoe UI", 10),
                Owner = this,
                AutoScroll = true
            };

            int yPos = 20;
            int leftPos = 30;
            int fieldWidth = 640;
            int fieldHeight = 36;

            // Role Selection
            Label lblRole = new Label
            {
                Text = "Role:",
                AutoSize = true,
                Top = yPos,
                Left = leftPos,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            yPos += 25;

            ComboBox cbRole = new ComboBox
            {
                Top = yPos,
                Left = leftPos,
                Width = fieldWidth,
                Height = fieldHeight,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack
            };
            cbRole.Items.AddRange(new[] { "user", "admin", "headadmin" });
            cbRole.SelectedIndex = 0;
            yPos += 60;

            // First Name
            Label lblFirst = new Label
            {
                Text = "First Name:",
                AutoSize = true,
                Top = yPos,
                Left = leftPos,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            yPos += 25;

            TextBox txtFirst = new TextBox
            {
                Top = yPos,
                Left = leftPos,
                Width = fieldWidth,
                Height = fieldHeight,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack
            };
            yPos += 50;

            // Middle Name
            Label lblMiddle = new Label
            {
                Text = "Middle Name:",
                AutoSize = true,
                Top = yPos,
                Left = leftPos,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            yPos += 25;

            TextBox txtMiddle = new TextBox
            {
                Top = yPos,
                Left = leftPos,
                Width = fieldWidth,
                Height = fieldHeight,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack
            };
            yPos += 50;

            // Last Name
            Label lblLast = new Label
            {
                Text = "Last Name:",
                AutoSize = true,
                Top = yPos,
                Left = leftPos,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            yPos += 25;

            TextBox txtLast = new TextBox
            {
                Top = yPos,
                Left = leftPos,
                Width = fieldWidth,
                Height = fieldHeight,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack
            };
            yPos += 50;

            // Email
            Label lblEmail = new Label
            {
                Text = "Email:",
                AutoSize = true,
                Top = yPos,
                Left = leftPos,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            yPos += 25;

            TextBox txtEmail = new TextBox
            {
                Top = yPos,
                Left = leftPos,
                Width = fieldWidth,
                Height = fieldHeight,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack
            };
            yPos += 50;

            // Grade Section
            Label lblGrade = new Label
            {
                Text = "Grade Section:",
                AutoSize = true,
                Top = yPos,
                Left = leftPos,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            yPos += 25;

            TextBox txtGrade = new TextBox
            {
                Top = yPos,
                Left = leftPos,
                Width = fieldWidth,
                Height = fieldHeight,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack
            };
            yPos += 50;

            // Password
            Label lblPassword = new Label
            {
                Text = "Password:",
                AutoSize = true,
                Top = yPos,
                Left = leftPos,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            yPos += 25;

            TextBox txtPassword = new TextBox
            {
                Top = yPos,
                Left = leftPos,
                Width = fieldWidth,
                Height = fieldHeight,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                UseSystemPasswordChar = true
            };
            yPos += 50;

            // Confirm Password
            Label lblConfirm = new Label
            {
                Text = "Confirm Password:",
                AutoSize = true,
                Top = yPos,
                Left = leftPos,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            yPos += 25;

            TextBox txtConfirm = new TextBox
            {
                Top = yPos,
                Left = leftPos,
                Width = fieldWidth,
                Height = fieldHeight,
                Font = new Font("Segoe UI", 11),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                UseSystemPasswordChar = true
            };
            yPos += 50;

            // Error message label
            Label lblMsg = new Label
            {
                Text = "",
                AutoSize = false,
                Top = yPos,
                Left = leftPos,
                Width = fieldWidth,
                Height = 40,
                Font = new Font("Segoe UI", 10),
                ForeColor = Theme.DeepRed,
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false
            };
            yPos += 60;

            // Create Button
            Button btnCreate = new Button
            {
                Text = "✓ Create Account",
                Top = yPos,
                Left = leftPos,
                Width = 300,
                Height = 40,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Theme.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCreate.FlatAppearance.BorderSize = 0;

            // Cancel Button
            Button btnCancel = new Button
            {
                Text = "Cancel",
                Top = yPos,
                Left = leftPos + 320,
                Width = 300,
                Height = 40,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Theme.DeepRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            btnCreate.Click += (s, e) =>
            {
                lblMsg.Text = "";
                lblMsg.Visible = false;

                // Validation
                if (cbRole.SelectedItem == null)
                {
                    lblMsg.Text = "Please select a role.";
                    lblMsg.Visible = true;
                    return;
                }

                string first = txtFirst.Text?.Trim() ?? "";
                string middle = txtMiddle.Text?.Trim() ?? "";
                string last = txtLast.Text?.Trim() ?? "";
                string email = txtEmail.Text?.Trim() ?? "";
                string grade = txtGrade.Text?.Trim() ?? "";
                string password = txtPassword.Text ?? "";
                string confirm = txtConfirm.Text ?? "";

                if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(last))
                {
                    lblMsg.Text = "First name and last name are required.";
                    lblMsg.Visible = true;
                    return;
                }

                if (string.IsNullOrEmpty(email))
                {
                    lblMsg.Text = "Email is required.";
                    lblMsg.Visible = true;
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    lblMsg.Text = "Password is required.";
                    lblMsg.Visible = true;
                    return;
                }

                if (!password.Equals(confirm))
                {
                    lblMsg.Text = "Passwords do not match.";
                    lblMsg.Visible = true;
                    return;
                }

                try
                {
                    string role = cbRole.SelectedItem.ToString();
                    SaveAccountToJSON(first, middle, last, email, grade, password, role);
                    MessageBox.Show("Account created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    createAccountForm.Close();
                }
                catch (Exception ex)
                {
                    lblMsg.Text = "Error: " + ex.Message;
                    lblMsg.Visible = true;
                }
            };

            btnCancel.Click += (s, e) => createAccountForm.Close();

            createAccountForm.Controls.Add(lblRole);
            createAccountForm.Controls.Add(cbRole);
            createAccountForm.Controls.Add(lblFirst);
            createAccountForm.Controls.Add(txtFirst);
            createAccountForm.Controls.Add(lblMiddle);
            createAccountForm.Controls.Add(txtMiddle);
            createAccountForm.Controls.Add(lblLast);
            createAccountForm.Controls.Add(txtLast);
            createAccountForm.Controls.Add(lblEmail);
            createAccountForm.Controls.Add(txtEmail);
            createAccountForm.Controls.Add(lblGrade);
            createAccountForm.Controls.Add(txtGrade);
            createAccountForm.Controls.Add(lblPassword);
            createAccountForm.Controls.Add(txtPassword);
            createAccountForm.Controls.Add(lblConfirm);
            createAccountForm.Controls.Add(txtConfirm);
            createAccountForm.Controls.Add(lblMsg);
            createAccountForm.Controls.Add(btnCreate);
            createAccountForm.Controls.Add(btnCancel);

            createAccountForm.ShowDialog(this);
        }

        private void SaveAccountToJSON(string firstName, string middleName, string lastName, string email, string gradeSection, string password, string role)
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
                    var des = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, object>>>(json, opts);
                    if (des != null) users = des;
                }

                // Check if email already exists
                foreach (var u in users)
                {
                    if (u.TryGetValue("email", out var eVal) && eVal?.ToString()?.Equals(email, StringComparison.OrdinalIgnoreCase) == true)
                    {
                        throw new Exception("Email already registered.");
                    }
                }

                var newUser = new Dictionary<string, object>
                {
                    ["name"] = name,
                    ["first_name"] = firstName,
                    ["middle_name"] = middleName,
                    ["last_name"] = lastName,
                    ["email"] = email,
                    ["grade_section"] = string.IsNullOrWhiteSpace(gradeSection) ? "N/A" : gradeSection,
                    ["password"] = password,
                    ["profile_picture"] = "",
                    ["role"] = role.ToLower()
                };

                users.Add(newUser);
                var writeOpts = new JsonSerializerOptions { WriteIndented = true };
                var outJson = System.Text.Json.JsonSerializer.Serialize(users, writeOpts);
                File.WriteAllText(path, outJson);

                System.Diagnostics.Debug.WriteLine($"[HeadAdmin] Account created: {email} with role: {role}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to save account: {ex.Message}");
            }
        }
        
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
        
        private void UpdateBackgroundImage()
        {
            try
            {
                if (this.Width > 0 && this.Height > 0)
                {
                    this.BackgroundImage?.Dispose();
                    this.BackgroundImage = Theme.CreateGradientBitmap(this.Width, this.Height, vertical: true);
                }
            }
            catch { /* Ignore errors updating background */ }
        }
        
        private void LoadImages(bool showDeleted, string searchQuery = "")
        {
            ShowLoadingScreen("Loading images...");
            
            System.ComponentModel.BackgroundWorker worker = new System.ComponentModel.BackgroundWorker();
            worker.DoWork += (s, e) =>
            {
                e.Result = new { ShowDeleted = showDeleted, SearchQuery = searchQuery };
            };
            worker.RunWorkerCompleted += (s, e) =>
            {
                try
                {
                    LoadImagesSync(showDeleted, searchQuery);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in LoadImagesSync: {ex.Message}");
                }
                finally
                {
                    HideLoadingScreen();
                }
            };
            worker.RunWorkerAsync();
        }
        
        private void LoadImagesSync(bool showDeleted, string searchQuery)
        {
            // Recalculate columns based on current width
            int availableWidth = outerPanel.Width - 60; // 60 for padding
            int newColumns = CalculateColumnsForWidth(availableWidth);
            if (newColumns != COLUMNS)
            {
                COLUMNS = newColumns;
                galleryTable.ColumnCount = COLUMNS;
                for (int i = 0; i < COLUMNS; i++)
                {
                    if (i < galleryTable.ColumnStyles.Count)
                        galleryTable.ColumnStyles[i] = new ColumnStyle(SizeType.AutoSize);
                    else
                        galleryTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
                }
            }
            
            Invoke((MethodInvoker)delegate
            {
                galleryTable.Controls.Clear();
                galleryTable.RowCount = 0;
                selectedCard = null;
                lblSelectedFile.Text = "Selected: (none)";
                lblDateAdded.Text = "Date Added: N/A";
                btnRestore.Enabled = false;
                btnDelete.Enabled = true;
                btnPermanentlyDelete.Enabled = false;
            });
            
            string folder = showDeleted ? deletedFolder : saveFolder;
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            
            string[] files = Directory.GetFiles(folder, "*.jpg").ToArray();
            if (files.Length == 0) return;
            
            var list = files.Select(f => new { File = f, Date = File.GetCreationTime(f) })
                .OrderByDescending(x => x.Date)
                .ToList();

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                list = list.Where(item =>
                {
                    // Search by filename
                    if (Path.GetFileName(item.File).ToLower().Contains(searchQuery))
                        return true;

                    // Search by tags
                    var tags = TagManager.GetImageTags(item.File);
                    foreach (var tagList in tags.Values)
                    {
                        if (tagList.Any(tag => tag.ToLower().Contains(searchQuery)))
                            return true;
                    }
                    return false;
                }).ToList();
            }
            
            int columnIndex = 0;
            int rowIndex = 0;
            
            foreach (var item in list)
            {
                try
                {
                    string filePath = item.File;
                    Image img;
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);
                        ms.Position = 0;
                        img = Image.FromStream(ms);
                    }
                    
                    Invoke((MethodInvoker)delegate
                    {
                        if (columnIndex == 0)
                        {
                            galleryTable.RowCount++;
                        }
                        
                        var card = new Panel
                        {
                            Width = IMAGE_SIZE,
                            Height = IMAGE_SIZE + 20,
                            BackColor = Theme.MediumTeal,
                            Margin = new Padding(10),
                            Padding = new Padding(5),
                            BorderStyle = BorderStyle.FixedSingle,
                            Tag = filePath,
                            Cursor = Cursors.Hand
                        };
                        
                        // 4:3 aspect ratio
                        int picWidth = IMAGE_SIZE - 10;
                        int picHeight = (int)(picWidth * 3 / 4.0);
                        
                        PictureBox pic = new PictureBox
                        {
                            Image = img,
                            SizeMode = PictureBoxSizeMode.Zoom,
                            Width = picWidth,
                            Height = picHeight,
                            Cursor = Cursors.Hand,
                            Dock = DockStyle.Top
                        };
                        
                        Label lblDate = new Label
                        {
                            Text = item.Date.ToString("MM/dd/yyyy"),
                            Dock = DockStyle.Top,
                            TextAlign = ContentAlignment.MiddleCenter,
                            Font = new Font("Segoe UI", 8),
                            ForeColor = Color.White,
                            BackColor = Theme.DarkTeal,
                            Height = 22
                        };
                        
                        Button btnInfo = new Button
                        {
                            Text = "Info",
                            Dock = DockStyle.Bottom,
                            BackColor = Theme.AccentBlue,
                            ForeColor = Color.White,
                            Font = new Font("Segoe UI", 9, FontStyle.Bold),
                            FlatStyle = FlatStyle.Flat,
                            Height = 28,
                            Cursor = Cursors.Hand
                        };
                        btnInfo.FlatAppearance.BorderSize = 0;
                        
                        card.Controls.Add(pic);
                        card.Controls.Add(lblDate);
                        card.Controls.Add(btnInfo);
                        
                        // Add tag badges
                        BuildTagBadges(card, filePath);
                        
                        string filePath_copy = filePath;
                        btnInfo.Click += (s, e) => ShowFileInfo(filePath_copy);
                        pic.Click += (s, e) => SelectAdminCard(card);
                        pic.DoubleClick += (s, e) => ShowImagePreview(filePath_copy);
                        card.Click += (s, e) => SelectAdminCard(card);
                        
                        galleryTable.Controls.Add(card, columnIndex, rowIndex);
                        
                        columnIndex++;
                        if (columnIndex >= COLUMNS)
                        {
                            columnIndex = 0;
                            rowIndex++;
                        }
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                }
            }
        }
        private void AddImageToGalleryAdmin(string filePath, bool isDeletedView)
        {
            if (!File.Exists(filePath)) return;
            Image img;
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (MemoryStream ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    img = Image.FromStream(ms);
                }
            }
            catch
            {
                return;
            }
            int displayWidth = 200;
            int displayHeight = (int)((double)img.Height / img.Width * displayWidth);
            Panel card = new Panel
            {
                Width = 220,
                Height = displayHeight + 70,
                BackColor = this.BackColor,
                Margin = new Padding(10),
                Padding = new Padding(10),
                BorderStyle = BorderStyle.None,
                Tag = filePath
            };
            PictureBox pic = new PictureBox
            {
                Image = img,
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = displayWidth,
                Height = displayHeight,
                Cursor = Cursors.Hand,
                Location = new Point(10, 0)
            };
            pic.Click += (s, e) => SelectAdminCard(card);
            card.Click += (s, e) => SelectAdminCard(card);
            Button btnInfo = new Button
            {
                Text = "Info",
                Top = displayHeight + 5,
                Width = 100,
                Height = 35,
                BackColor = Theme.GetBackgroundTeal(),
                ForeColor = Color.Black
            };
            btnInfo.Click += (s, e) => ShowFileInfo(filePath);
            card.Controls.Add(pic);
            card.Controls.Add(btnInfo);
        }
        private void SelectAdminCard(Panel card)
        {
            // Deselect previous card
            if (selectedCard != null)
            {
                selectedCard.BorderStyle = BorderStyle.None;
                selectedCard.BackColor = Theme.MediumTeal;
                
                // Remove checkmark indicator if it exists
                var checkmark = selectedCard.Controls.OfType<Label>().FirstOrDefault(l => l.Tag?.ToString() == "checkmark");
                if (checkmark != null)
                {
                    selectedCard.Controls.Remove(checkmark);
                    checkmark.Dispose();
                }
            }
            
            // Select new card
            selectedCard = card;
            selectedCard.BorderStyle = BorderStyle.Fixed3D;
            selectedCard.BackColor = Theme.AccentBlue;
            
            // Add checkmark indicator
            Label checkmarkLabel = new Label
            {
                Text = "✓",
                AutoSize = true,
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Tag = "checkmark",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            checkmarkLabel.Location = new Point(card.Width - 38, 5);
            selectedCard.Controls.Add(checkmarkLabel);
            selectedCard.BringToFront();
            
            string filePath = (string)card.Tag;
            lblSelectedFile.Text = $"Selected: {Path.GetFileName(filePath)}";
            lblDateAdded.Text = "Date Added: " + File.GetCreationTime(filePath).ToString("g");
            bool viewingDeleted = cbViewMode.SelectedIndex == 1;
            btnRestore.Enabled = viewingDeleted;
            btnDelete.Enabled = !viewingDeleted;
            btnPermanentlyDelete.Enabled = true;
        }
        private void ShowFileInfo(string filePath)
        {
            string infoPath = Path.Combine(Path.GetDirectoryName(filePath),
                Path.GetFileNameWithoutExtension(filePath) + "_info.txt");
            string infoText = File.Exists(infoPath) ? File.ReadAllText(infoPath) : "(no info file)";
            DateTime created = File.GetCreationTime(filePath);
            DateTime modified = File.GetLastWriteTime(filePath);
            string addedBy = null;
            if (!string.IsNullOrWhiteSpace(infoText))
            {
                var lines = infoText.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (trimmed.StartsWith("AddedBy:", StringComparison.OrdinalIgnoreCase))
                    {
                        addedBy = trimmed.Substring("AddedBy:".Length).Trim();
                        break;
                    }
                }
            }
            Form infoForm = new Form
            {
                Text = "File Info",
                Size = new Size(480, 360),
                StartPosition = FormStartPosition.CenterParent
            };
            TextBox tb = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                Dock = DockStyle.Fill,
                Text = $"File: {Path.GetFileName(filePath)}{Environment.NewLine}" +
                       $"Path: {filePath}{Environment.NewLine}" +
                       $"Created: {created:g}{Environment.NewLine}" +
                       $"Last Modified: {modified:g}{Environment.NewLine}{Environment.NewLine}" +
                       $"Added by: {(string.IsNullOrEmpty(addedBy) ? "(unknown)" : addedBy)}{Environment.NewLine}{Environment.NewLine}" +
                       $"Info file contents:{Environment.NewLine}{infoText}"
            };
            infoForm.Controls.Add(tb);
            infoForm.ShowDialog();
        }
        private void DeleteSelected()
        {
            if (selectedCard == null) { MessageBox.Show("Select an image first."); return; }
            var filePath = (string)selectedCard.Tag;
            if (!File.Exists(filePath)) return;
            if (MessageBox.Show($"Move '{Path.GetFileName(filePath)}' to Deleted folder?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                Directory.CreateDirectory(deletedFolder);
                string dest = Path.Combine(deletedFolder, Path.GetFileName(filePath));
                File.Move(filePath, dest);
                MoveIfExists(Path.ChangeExtension(filePath, null) + "_tags.txt", Path.Combine(deletedFolder, Path.GetFileNameWithoutExtension(filePath) + "_tags.txt"));
                MoveIfExists(Path.ChangeExtension(filePath, null) + "_info.txt", Path.Combine(deletedFolder, Path.GetFileNameWithoutExtension(filePath) + "_info.txt"));
                TagManager.DeleteImageTags(filePath);
                DeleteImageFromGoogleSheets(Path.GetFileName(filePath));
                LoadImages(cbViewMode.SelectedIndex == 1, txtSearch.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Delete failed: " + ex.Message);
            }
        }
        private void RestoreSelected()
        {
            if (selectedCard == null) { MessageBox.Show("Select an image first."); return; }
            var filePath = (string)selectedCard.Tag;
            if (!File.Exists(filePath)) return;
            if (MessageBox.Show($"Restore '{Path.GetFileName(filePath)}' back to active folder?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            try
            {
                string dest = Path.Combine(saveFolder, Path.GetFileName(filePath));
                File.Move(filePath, dest);
                string baseName = Path.GetFileNameWithoutExtension(filePath);
                MoveIfExists(Path.Combine(deletedFolder, baseName + "_tags.txt"), Path.Combine(saveFolder, baseName + "_tags.txt"));
                MoveIfExists(Path.Combine(deletedFolder, baseName + "_info.txt"), Path.Combine(saveFolder, baseName + "_info.txt"));
                RestoreImageInGoogleSheets(Path.GetFileName(filePath));
                LoadImages(cbViewMode.SelectedIndex == 1, txtSearch.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Restore failed: " + ex.Message);
            }
        }
        private void PermanentlyDeleteSelected()
        {
            if (selectedCard == null) { MessageBox.Show("Select an image first."); return; }
            var filePath = (string)selectedCard.Tag;
            if (!File.Exists(filePath)) return;
            if (MessageBox.Show($"Permanently delete '{Path.GetFileName(filePath)}'? This cannot be undone.", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            try
            {
                File.Delete(filePath);
                string baseName = Path.GetFileNameWithoutExtension(filePath);
                var related = new[]
                {
                    Path.Combine(Path.GetDirectoryName(filePath), baseName + "_tags.txt"),
                    Path.Combine(Path.GetDirectoryName(filePath), baseName + "_info.txt")
                };
                foreach (var r in related) if (File.Exists(r)) File.Delete(r);
                TagManager.DeleteImageTags(filePath);
                LoadImages(cbViewMode.SelectedIndex == 1, txtSearch.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Permanent delete failed: " + ex.Message);
            }
        }
        private void MoveIfExists(string src, string dst)
        {
            try
            {
                if (File.Exists(src))
                {
                    if (File.Exists(dst)) File.Delete(dst);
                    File.Move(src, dst);
                }
            }
            catch {  }
        }
        private void OpenImageDetailsForm()
        {
            Form detailsForm = new Form
            {
                Text = "Item Details - Lost & Found",
                Width = 1400,
                Height = 700,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Theme.LightGray,
                Font = new Font("Segoe UI", 11),
                Owner = this
            };
            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Theme.SoftWhite,
                ForeColor = Theme.NearBlack,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Theme.MediumTeal,
                    ForeColor = Theme.SoftWhite,
                    Font = new Font("Segoe UI", 11, FontStyle.Bold)
                }
            };
            dgv.Columns.AddRange(
                new DataGridViewColumn[]
                {
                    new DataGridViewTextBoxColumn { HeaderText = "Item", DataPropertyName = "Item", Width = 180, ReadOnly = true },
                    new DataGridViewTextBoxColumn { HeaderText = "Finder", DataPropertyName = "Finder", Width = 180, ReadOnly = true },
                    new DataGridViewTextBoxColumn { HeaderText = "Date Found", DataPropertyName = "DateFound", Width = 150, ReadOnly = true },
                    new DataGridViewTextBoxColumn { HeaderText = "Claimant", DataPropertyName = "Claimant", Width = 150, ReadOnly = false },
                    new DataGridViewTextBoxColumn { HeaderText = "Date Claimed", DataPropertyName = "DateClaimed", Width = 130, ReadOnly = false },
                    new DataGridViewTextBoxColumn { HeaderText = "Grade & Section", DataPropertyName = "GradeSection", Width = 130, ReadOnly = false }
                }
            );
            List<ImageDetailRow> rows = new List<ImageDetailRow>();
            LoadImageDetailsData(saveFolder, "Active", rows);
            LoadImageDetailsData(deletedFolder, "Deleted", rows);
            foreach (var row in rows.OrderByDescending(r => r.DateFoundObj))
            {
                dgv.Rows.Add(row.Item, row.Finder, row.DateFound, row.Claimant, row.DateClaimed, row.GradeSection);
            }
            dgv.CellValueChanged += (s, e) =>
            {
                if (e.ColumnIndex >= 3 && e.RowIndex >= 0)  
                {
                    SaveClaimantInfo(dgv, e.RowIndex, rows);
                }
            };
            detailsForm.Controls.Add(dgv);
            detailsForm.ShowDialog(this);
        }
        private void LoadImageDetailsData(string folder, string status, List<ImageDetailRow> rows)
        {
            if (!Directory.Exists(folder)) return;
            string[] files = Directory.GetFiles(folder, "*.jpg");
            foreach (var filePath in files)
            {
                try
                {
                    string baseName = Path.GetFileNameWithoutExtension(filePath);
                    string infoPath = Path.Combine(folder, baseName + "_info.txt");
                    string tagsPath = Path.Combine(folder, baseName + "_tags.txt");
                    string claimantPath = Path.Combine(folder, baseName + "_claimant.txt");
                    string finder = "Unknown";
                    string itemName = "Unknown Item";
                    DateTime dateFound = File.GetCreationTime(filePath);
                    string claimant = "";
                    string dateClaimed = "";
                    string claimantGradeSection = "";
                    if (File.Exists(infoPath))
                    {
                        string[] infoLines = File.ReadAllLines(infoPath);
                        foreach (var line in infoLines)
                        {
                            if (line.StartsWith("Uploader:"))
                                finder = line.Split(new[] { "Uploader:" }, StringSplitOptions.None)[1].Trim();
                            else if (line.StartsWith("Date:"))
                            {
                                string dateStr = line.Split(new[] { "Date:" }, StringSplitOptions.None)[1].Trim();
                                if (DateTime.TryParse(dateStr, out DateTime parsedDate))
                                    dateFound = parsedDate;
                            }
                        }
                    }
                    if (File.Exists(tagsPath))
                    {
                        string[] tags = File.ReadAllLines(tagsPath).Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();
                        if (tags.Length >= 2)
                            itemName = $"{tags[0]} - {tags[1]}";
                        else if (tags.Length == 1)
                            itemName = tags[0];
                    }
                    if (File.Exists(claimantPath))
                    {
                        string[] claimantLines = File.ReadAllLines(claimantPath);
                        foreach (var line in claimantLines)
                        {
                            if (line.StartsWith("Claimant:"))
                                claimant = line.Split(new[] { "Claimant:" }, StringSplitOptions.None)[1].Trim();
                            else if (line.StartsWith("DateClaimed:"))
                                dateClaimed = line.Split(new[] { "DateClaimed:" }, StringSplitOptions.None)[1].Trim();
                            else if (line.StartsWith("GradeSection:"))
                                claimantGradeSection = line.Split(new[] { "GradeSection:" }, StringSplitOptions.None)[1].Trim();
                        }
                    }
                    rows.Add(new ImageDetailRow
                    {
                        FilePath = filePath,
                        Item = itemName,
                        Finder = finder,
                        DateFound = dateFound.ToString("yyyy-MM-dd HH:mm"),
                        DateFoundObj = dateFound,
                        Claimant = claimant,
                        DateClaimed = dateClaimed,
                        GradeSection = claimantGradeSection,
                        Status = status
                    });
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error loading image details: " + ex.Message);
                }
            }
        }
        private void SaveClaimantInfo(DataGridView dgv, int rowIndex, List<ImageDetailRow> rows)
        {
            if (rowIndex >= 0 && rowIndex < rows.Count)
            {
                var row = rows[rowIndex];
                string claimant = dgv.Rows[rowIndex].Cells[3].Value?.ToString() ?? "";
                string dateClaimed = dgv.Rows[rowIndex].Cells[4].Value?.ToString() ?? "";
                string gradeSection = dgv.Rows[rowIndex].Cells[5].Value?.ToString() ?? "";
                row.Claimant = claimant;
                row.DateClaimed = dateClaimed;
                row.GradeSection = gradeSection;
                string baseName = Path.GetFileNameWithoutExtension(row.FilePath);
                string folderPath = Path.GetDirectoryName(row.FilePath);
                string claimantPath = Path.Combine(folderPath, baseName + "_claimant.txt");
                var claimantLines = new[]
                {
                    $"Claimant: {claimant}",
                    $"DateClaimed: {dateClaimed}",
                    $"GradeSection: {gradeSection}"
                };
                try
                {
                    File.WriteAllLines(claimantPath, claimantLines);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Error saving claimant info: " + ex.Message);
                    MessageBox.Show("Error saving claimant information: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private class ImageDetailRow
        {
            public string FilePath { get; set; }
            public string Item { get; set; }
            public string Finder { get; set; }
            public string DateFound { get; set; }
            public DateTime DateFoundObj { get; set; }
            public string Claimant { get; set; }
            public string DateClaimed { get; set; }
            public string GradeSection { get; set; }
            public string Status { get; set; }
        }
        private void OpenTagManager()
        {
            if (selectedCard == null)
            {
                MessageBox.Show("Please select an image first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string filePath = (string)selectedCard.Tag;
            Form tagForm = new Form
            {
                Text = $"Edit Tags - {Path.GetFileName(filePath)}",
                Width = 500,
                Height = 450,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Theme.SoftWhite,
                Font = new Font("Segoe UI", 10),
                Owner = this
            };

            Label lblCurrentTags = new Label
            {
                Text = "Current Tags:",
                AutoSize = true,
                Top = 15,
                Left = 15,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };

            FlowLayoutPanel pnlCurrentTags = new FlowLayoutPanel
            {
                Top = 40,
                Left = 15,
                Width = 460,
                Height = 80,
                BackColor = Theme.LightGray,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true,
                Padding = new Padding(5)
            };

            Label lblAvailableTags = new Label
            {
                Text = "Available Tags:",
                AutoSize = true,
                Top = 135,
                Left = 15,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };

            ListBox lbAvailableTags = new ListBox
            {
                Top = 160,
                Left = 15,
                Width = 460,
                Height = 120,
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                SelectionMode = SelectionMode.One
            };

            Label lblNewTag = new Label
            {
                Text = "Add New Tag:",
                AutoSize = true,
                Top = 290,
                Left = 15,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };

            TextBox txtAddTag = new TextBox
            {
                Top = 315,
                Left = 15,
                Width = 330,
                Height = 32,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = Theme.NearBlack,
                PlaceholderText = "Type tag name..."
            };

            Button btnAddTag = new Button
            {
                Text = "➕ Add",
                Top = 315,
                Left = 355,
                Width = 120,
                Height = 32,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Theme.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddTag.FlatAppearance.BorderSize = 0;

            Button btnApply = new Button
            {
                Text = "✓ Apply",
                Top = 360,
                Left = 15,
                Width = 220,
                Height = 35,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Theme.MediumTeal,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnApply.FlatAppearance.BorderSize = 0;

            Button btnCancel = new Button
            {
                Text = "Cancel",
                Top = 360,
                Left = 255,
                Width = 220,
                Height = 35,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Theme.LightGray,
                ForeColor = Theme.NearBlack,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            // Populate available tags
            var allTags = TagManager.GetAllTags();
            foreach (var tag in allTags)
            {
                lbAvailableTags.Items.Add(tag);
            }

            // Populate current tags
            RefreshCurrentTagsDisplay(pnlCurrentTags, filePath);

            tagForm.Controls.Add(lblCurrentTags);
            tagForm.Controls.Add(pnlCurrentTags);
            tagForm.Controls.Add(lblAvailableTags);
            tagForm.Controls.Add(lbAvailableTags);
            tagForm.Controls.Add(lblNewTag);
            tagForm.Controls.Add(txtAddTag);
            tagForm.Controls.Add(btnAddTag);
            tagForm.Controls.Add(btnApply);
            tagForm.Controls.Add(btnCancel);

            // Button handlers
            btnAddTag.Click += (s, e) =>
            {
                string tagName = txtAddTag.Text.Trim();
                if (!string.IsNullOrWhiteSpace(tagName))
                {
                    // Add to Uncategorized
                    TagManager.AddTagToCategory("Uncategorized", tagName);
                    
                    // Add to available list if not already there
                    if (!lbAvailableTags.Items.Contains(tagName))
                    {
                        lbAvailableTags.Items.Add(tagName);
                    }
                    
                    txtAddTag.Clear();
                }
            };

            lbAvailableTags.DoubleClick += (s, e) =>
            {
                if (lbAvailableTags.SelectedItem is string selectedTag)
                {
                    TagManager.AddTagToImage(filePath, "Uncategorized", selectedTag);
                    RefreshCurrentTagsDisplay(pnlCurrentTags, filePath);
                }
            };

            btnApply.Click += (s, e) =>
            {
                LoadImages(cbViewMode.SelectedIndex == 1, txtSearch.Text);
                tagForm.Close();
            };

            btnCancel.Click += (s, e) => tagForm.Close();

            tagForm.ShowDialog(this);
        }

        private void RefreshCurrentTagsDisplay(FlowLayoutPanel panel, string filePath)
        {
            panel.Controls.Clear();
            var imageTags = TagManager.GetImageTags(filePath);
            
            if (imageTags.Count == 0)
            {
                Label lblEmpty = new Label
                {
                    Text = "(no tags yet)",
                    AutoSize = true,
                    ForeColor = Theme.DarkGray,
                    Font = new Font("Segoe UI", 9, FontStyle.Italic)
                };
                panel.Controls.Add(lblEmpty);
                return;
            }

            foreach (var categoryTags in imageTags)
            {
                foreach (var tag in categoryTags.Value)
                {
                    Panel tagChip = new Panel
                    {
                        BackColor = ColorTranslator.FromHtml(TagManager.GetCategoryColor(categoryTags.Key)),
                        Height = 28,
                        Width = 120,
                        Margin = new Padding(3),
                        AutoSize = false
                    };

                    Label lblTag = new Label
                    {
                        Text = tag,
                        AutoSize = false,
                        Dock = DockStyle.Fill,
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        TextAlign = ContentAlignment.MiddleLeft,
                        Padding = new Padding(5, 0, 25, 0)
                    };

                    Button btnRemove = new Button
                    {
                        Text = "✕",
                        AutoSize = false,
                        Width = 20,
                        Height = 28,
                        Dock = DockStyle.Right,
                        BackColor = Color.Transparent,
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Font = new Font("Segoe UI", 8, FontStyle.Bold),
                        Cursor = Cursors.Hand
                    };
                    btnRemove.FlatAppearance.BorderSize = 0;

                    string tagName = tag;
                    string categoryName = categoryTags.Key;
                    btnRemove.Click += (s, e) =>
                    {
                        TagManager.RemoveTagFromImage(filePath, categoryName, tagName);
                        RefreshCurrentTagsDisplay(panel, filePath);
                    };

                    tagChip.Controls.Add(lblTag);
                    tagChip.Controls.Add(btnRemove);
                    panel.Controls.Add(tagChip);
                }
            }
        }

        private void BuildTagBadges(Panel card, string filePath)
        {
            // Remove existing tag panel if present
            var existingTagPanel = card.Controls.OfType<Panel>().FirstOrDefault(p => (string)p.Tag == "tag-panel");
            if (existingTagPanel != null)
                card.Controls.Remove(existingTagPanel);

            var imageTags = TagManager.GetImageTags(filePath);
            if (imageTags.Count == 0)
                return;

            Panel tagPanel = new Panel
            {
                Tag = "tag-panel",
                Height = 25,
                Dock = DockStyle.Bottom,
                BackColor = Theme.DarkTeal,
                AutoScroll = true
            };

            int xOffset = 5;
            foreach (var categoryTags in imageTags)
            {
                string categoryName = categoryTags.Key;
                var tags = categoryTags.Value;
                string categoryColor = TagManager.GetCategoryColor(categoryName);

                foreach (var tag in tags.Take(4)) // Show up to 4 tags per image
                {
                    Label tagBadge = new Label
                    {
                        Text = $"  {tag}  ",
                        AutoSize = true,
                        BackColor = ColorTranslator.FromHtml(categoryColor),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 7, FontStyle.Bold),
                        Location = new Point(xOffset, 3),
                        Padding = new Padding(2)
                    };

                    tagPanel.Controls.Add(tagBadge);
                    xOffset += tagBadge.Width + 2;

                    if (xOffset > 200)
                    {
                        xOffset = 5;
                        tagPanel.Height += 22;
                    }
                }
            }

            card.Controls.Add(tagPanel);
        }
        private async void DeleteImageFromGoogleSheets(string filename)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var data = new { filename = filename };
                    string json = JsonSerializer.Serialize(data);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("http://localhost:5000/api/delete-captured-image", content);
                    System.Diagnostics.Debug.WriteLine($"[FormGalleryHeadAdmin] Delete image from Sheets: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FormGalleryHeadAdmin] Flask unavailable for delete: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FormGalleryHeadAdmin] Error deleting image from Sheets: {ex.Message}");
            }
        }
        private async void RestoreImageInGoogleSheets(string filename)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var data = new { filename = filename, status = "active" };
                    string json = JsonSerializer.Serialize(data);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("http://localhost:5000/api/restore-captured-image", content);
                    System.Diagnostics.Debug.WriteLine($"[FormGalleryHeadAdmin] Restore image in Sheets: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FormGalleryHeadAdmin] Flask unavailable for restore: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FormGalleryHeadAdmin] Error restoring image in Sheets: {ex.Message}");
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
                    logoPictureBox.Location = new System.Drawing.Point(ClientSize.Width - 60, ClientSize.Height - 60);

                    var copyrightLabel = new Label
                    {
                        Text = "© ReturnPoint 2026",
                        AutoSize = true,
                        BackColor = Color.Transparent,
                        ForeColor = Theme.DarkGray,
                        Font = new System.Drawing.Font("Segoe UI", 8F),
                        Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                    };
                    copyrightLabel.Location = new System.Drawing.Point(ClientSize.Width - 140, ClientSize.Height - 30);

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

        private void OpenInbox()
        {
            Form inboxForm = new Form
            {
                Text = "Claim Inbox - ReturnPoint",
                Width = 1200,
                Height = 600,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Theme.SoftWhite,
                Font = new Font("Segoe UI", 10),
                Owner = this
            };

            // Load claims data
            List<ClaimData> claims = LoadClaimsData();

            // DataGridView for claims
            DataGridView dgvClaims = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Theme.SoftWhite,
                ForeColor = Theme.NearBlack,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Theme.DarkTeal,
                    ForeColor = Theme.SoftWhite,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                }
            };

            dgvClaims.Columns.AddRange(
                new DataGridViewColumn[]
                {
                    new DataGridViewTextBoxColumn { HeaderText = "Claimant", DataPropertyName = "ClaimantName", Width = 150, ReadOnly = true },
                    new DataGridViewTextBoxColumn { HeaderText = "Email", DataPropertyName = "ClaimantEmail", Width = 150, ReadOnly = true },
                    new DataGridViewTextBoxColumn { HeaderText = "Grade/Section", DataPropertyName = "ClaimantGradeSection", Width = 120, ReadOnly = true },
                    new DataGridViewTextBoxColumn { HeaderText = "Image", DataPropertyName = "ImageName", Width = 150, ReadOnly = true },
                    new DataGridViewTextBoxColumn { HeaderText = "Date Claimed", DataPropertyName = "DateClaimed", Width = 150, ReadOnly = true },
                    new DataGridViewTextBoxColumn { HeaderText = "Status", DataPropertyName = "Status", Width = 80, ReadOnly = true }
                }
            );

            foreach (var claim in claims.Where(c => c.Status == "pending"))
            {
                dgvClaims.Rows.Add(
                    claim.ClaimantName,
                    claim.ClaimantEmail,
                    claim.ClaimantGradeSection,
                    claim.ImageName,
                    claim.DateClaimed,
                    claim.Status
                );
            }

            Panel buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50,
                BackColor = Theme.LightGray,
                Padding = new Padding(10)
            };

            Button btnConfirm = new Button
            {
                Text = "✓ Confirm Claim",
                Width = 150,
                Height = 35,
                BackColor = Theme.Success,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Left = 10,
                Top = 10
            };
            btnConfirm.FlatAppearance.BorderSize = 0;

            Button btnClose = new Button
            {
                Text = "Close",
                Width = 150,
                Height = 35,
                BackColor = Theme.DarkGray,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Left = 170,
                Top = 10
            };
            btnClose.FlatAppearance.BorderSize = 0;

            btnConfirm.Click += (s, e) =>
            {
                if (dgvClaims.SelectedRows.Count > 0)
                {
                    int rowIndex = dgvClaims.SelectedRows[0].Index;
                    if (rowIndex < claims.Count)
                    {
                        var claim = claims[rowIndex];
                        ConfirmClaim(claim);
                        inboxForm.Close();
                        OpenInbox(); // Refresh the inbox
                    }
                }
                else
                {
                    MessageBox.Show("Please select a claim to confirm.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            btnClose.Click += (s, e) => inboxForm.Close();

            buttonPanel.Controls.Add(btnConfirm);
            buttonPanel.Controls.Add(btnClose);
            inboxForm.Controls.Add(dgvClaims);
            inboxForm.Controls.Add(buttonPanel);

            inboxForm.ShowDialog(this);
        }

        private List<ClaimData> LoadClaimsData()
        {
            List<ClaimData> claims = new List<ClaimData>();

            // Only search in saveFolder for pending claims
            if (!Directory.Exists(saveFolder))
                return claims;

            string[] claimFiles = Directory.GetFiles(saveFolder, "*_claim.txt");
            foreach (var claimFile in claimFiles)
            {
                try
                {
                    string[] lines = File.ReadAllLines(claimFile);
                    var claimData = new ClaimData { FilePath = claimFile };

                    foreach (var line in lines)
                    {
                        if (line.Contains("ClaimantEmail:"))
                        {
                            var parts = line.Split(new[] { "ClaimantEmail:" }, StringSplitOptions.None);
                            if (parts.Length > 1)
                                claimData.ClaimantEmail = parts[1].Trim();
                        }
                        else if (line.Contains("ClaimantName:"))
                        {
                            var parts = line.Split(new[] { "ClaimantName:" }, StringSplitOptions.None);
                            if (parts.Length > 1)
                                claimData.ClaimantName = parts[1].Trim();
                        }
                        else if (line.Contains("ClaimantGradeSection:"))
                        {
                            var parts = line.Split(new[] { "ClaimantGradeSection:" }, StringSplitOptions.None);
                            if (parts.Length > 1)
                                claimData.ClaimantGradeSection = parts[1].Trim();
                        }
                        else if (line.Contains("DateClaimed:"))
                        {
                            var parts = line.Split(new[] { "DateClaimed:" }, StringSplitOptions.None);
                            if (parts.Length > 1)
                                claimData.DateClaimed = parts[1].Trim();
                        }
                        else if (line.Contains("Status:"))
                        {
                            var parts = line.Split(new[] { "Status:" }, StringSplitOptions.None);
                            if (parts.Length > 1)
                                claimData.Status = parts[1].Trim();
                        }
                        else if (line.Contains("ConfirmedBy:"))
                        {
                            var parts = line.Split(new[] { "ConfirmedBy:" }, StringSplitOptions.None);
                            if (parts.Length > 1)
                                claimData.ConfirmedBy = parts[1].Trim();
                        }
                    }

                    // Get image name from claim file
                    string baseName = Path.GetFileNameWithoutExtension(claimFile).Replace("_claim", "");
                    claimData.ImageName = baseName + ".jpg";

                    claims.Add(claimData);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading claim: {ex.Message}");
                }
            }

            return claims;
        }

        private void ConfirmClaim(ClaimData claim)
        {
            try
            {
                string adminName = Program.CurrentUser?.Name ?? "Unknown Admin";
                string[] claimLines = File.ReadAllLines(claim.FilePath);
                var updatedLines = new List<string>();

                foreach (var line in claimLines)
                {
                    if (line.Contains("Status:"))
                        updatedLines.Add("Status: confirmed");
                    else if (line.Contains("ConfirmedBy:"))
                        updatedLines.Add($"ConfirmedBy: {adminName}");
                    else
                        updatedLines.Add(line);
                }

                // Add ConfirmedBy if not present
                if (!updatedLines.Any(l => l.Contains("ConfirmedBy:")))
                {
                    updatedLines.Add($"ConfirmedBy: {adminName}");
                }

                // Update claim file with confirmed status
                File.WriteAllLines(claim.FilePath, updatedLines);

                // Move image and all associated files to deleted folder (marked as claimed)
                MoveClaimedImageToDeleted(claim);

                MessageBox.Show("Claim confirmed successfully! Image moved to claimed items.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error confirming claim: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MoveClaimedImageToDeleted(ClaimData claim)
        {
            try
            {
                // Get the image file path
                string imagePath = Path.Combine(saveFolder, claim.ImageName);
                if (!File.Exists(imagePath))
                {
                    // Try to find the image file
                    var imageFiles = Directory.GetFiles(saveFolder, Path.GetFileNameWithoutExtension(claim.ImageName) + ".*");
                    if (imageFiles.Length > 0)
                        imagePath = imageFiles[0];
                    else
                        return; // Image not found
                }

                string baseName = Path.GetFileNameWithoutExtension(imagePath);
                string destinationPath = Path.Combine(deletedFolder, Path.GetFileName(imagePath));
                
                // Ensure deleted folder exists
                Directory.CreateDirectory(deletedFolder);

                // Move the image file
                if (File.Exists(imagePath))
                {
                    if (File.Exists(destinationPath))
                        File.Delete(destinationPath);
                    File.Move(imagePath, destinationPath);
                }

                // Move associated metadata files
                string[] associatedExtensions = { "_claim.txt", "_tags.txt", "_info.txt", "_claimant.txt" };
                foreach (var ext in associatedExtensions)
                {
                    string sourcePath = Path.Combine(saveFolder, baseName + ext);
                    string destPath = Path.Combine(deletedFolder, baseName + ext);
                    
                    if (File.Exists(sourcePath))
                    {
                        if (File.Exists(destPath))
                            File.Delete(destPath);
                        File.Move(sourcePath, destPath);
                    }
                }

                System.Diagnostics.Debug.WriteLine($"[ConfirmClaim] Moved image {claim.ImageName} to deleted folder");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ConfirmClaim] Error moving image: {ex.Message}");
                throw;
            }
        }

        private class ClaimData
        {
            public string FilePath { get; set; }
            public string ClaimantName { get; set; }
            public string ClaimantEmail { get; set; }
            public string ClaimantGradeSection { get; set; }
            public string DateClaimed { get; set; }
            public string ImageName { get; set; }
            public string Status { get; set; } = "pending";
            public string ConfirmedBy { get; set; } = "";
        }

        private void ShowImagePreview(string filePath)
        {
            try
            {
                Bitmap? img = null;
                try
                {
                    img = new Bitmap(filePath);
                }
                catch
                {
                    MessageBox.Show("Could not load image.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Form previewForm = new Form
                {
                    Text = $"Image Preview - {Path.GetFileName(filePath)}",
                    StartPosition = FormStartPosition.CenterParent,
                    WindowState = FormWindowState.Maximized,
                    BackColor = Color.Black,
                    FormBorderStyle = FormBorderStyle.Sizable,
                    MinimizeBox = false,
                    MaximizeBox = true
                };

                PictureBox previewPicturebox = new PictureBox
                {
                    Image = img,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock = DockStyle.Fill,
                    BackColor = Color.Black,
                    Cursor = Cursors.Hand
                };

                Label closeLabel = new Label
                {
                    Text = "(Double-click or press Escape to close)",
                    AutoSize = true,
                    BackColor = Color.Black,
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 10),
                    Padding = new Padding(10),
                    Dock = DockStyle.Bottom,
                    TextAlign = ContentAlignment.MiddleCenter,
                    Height = 40
                };

                previewForm.Controls.Add(previewPicturebox);
                previewForm.Controls.Add(closeLabel);

                previewPicturebox.DoubleClick += (s, e) => previewForm.Close();
                previewForm.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Escape)
                    {
                        previewForm.Close();
                        e.Handled = true;
                    }
                };

                previewForm.ShowDialog(this);
                img?.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error displaying image preview: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
