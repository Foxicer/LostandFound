using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Linq;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Drawing;
using System.Drawing.Drawing2D;
namespace ReturnPoint
{
    public partial class FormGallery : Form
    {
        private Panel outerPanel;
        private TableLayoutPanel galleryTable;
        private Button openCameraButton;
        private Button btnLogout;
        private string saveFolder;
        private Panel searchPanel;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnClearSearch;
        private ListBox lstTags;
        private TextBox txtNewTag;
        private Button btnAddTag;
        private Panel selectedCard;
        private Panel panelMain;
        private PictureBox? logoPictureBox;
        private Bitmap? backgroundBitmap;
        private const int COLUMNS = 5;
        private const int IMAGE_SIZE = 220;
        public class RoundedPanel : Panel
        {
            public int CornerRadius { get; set; } = 20;
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                using (Graphics g = e.Graphics)
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    using (Brush brush = new SolidBrush(this.BackColor))
                    {
                        var rect = new Rectangle(0, 0, this.Width, this.Height);
                        var path = GetRoundedRect(rect, CornerRadius);
                        g.FillPath(brush, path);
                    }
                }
            }
            private GraphicsPath GetRoundedRect(Rectangle rect, int radius)
            {
                int d = radius * 2;
                GraphicsPath path = new GraphicsPath();
                path.AddArc(rect.X, rect.Y, d, d, 180, 90);
                path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
                path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
                path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
                path.CloseFigure();
                return path;
            }
        }
        public FormGallery()
        {
            if (string.IsNullOrWhiteSpace(saveFolder))
            {
                saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CapturedImages");
            }
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
            this.Text = "Gallery - ReturnPoint";
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Theme.GetBackgroundTeal();
            this.BackgroundImage = Theme.CreateGradientBitmap(1920, 1080, vertical: true);
            this.BackgroundImageLayout = ImageLayout.Stretch;
            outerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Theme.GetBackgroundTeal(),
                BackgroundImage = Theme.CreateGradientBitmap(1920, 1080, vertical: true),
                BackgroundImageLayout = ImageLayout.Stretch
            };
            
            galleryTable = new TableLayoutPanel
            {
                ColumnCount = COLUMNS,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Padding = new Padding(30, 20, 30, 20),
                BackColor = Theme.GetBackgroundTeal(),
                Dock = DockStyle.Top
            };
            
            // Set column styles
            for (int i = 0; i < COLUMNS; i++)
            {
                galleryTable.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            }
            
            outerPanel.Controls.Add(galleryTable);
            searchPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340,
                BackColor = Theme.MediumTeal,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(20),
                Visible = true,
                AutoScroll = true
            };
            
            // Create Profile Card
            Panel profileCard = new Panel
            {
                Width = 300,
                Height = 140,
                BackColor = Theme.DarkTeal,
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(20, 20),
                Padding = new Padding(10)
            };
            
            // Profile Picture
            PictureBox profilePic = new PictureBox
            {
                Width = 50,
                Height = 50,
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(10, 10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            
            // Try to load profile picture or use default logo
            try
            {
                string profilePicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logo.png");
                if (File.Exists(profilePicPath))
                {
                    profilePic.Image = Image.FromFile(profilePicPath);
                }
                else
                {
                    // Create a simple placeholder
                    Bitmap placeholder = new Bitmap(50, 50);
                    using (Graphics g = Graphics.FromImage(placeholder))
                    {
                        g.Clear(Theme.LightGray);
                        g.DrawString("👤", new Font("Segoe UI", 24), Brushes.DarkGray, new PointF(5, 5));
                    }
                    profilePic.Image = placeholder;
                }
            }
            catch
            {
                Bitmap placeholder = new Bitmap(50, 50);
                using (Graphics g = Graphics.FromImage(placeholder))
                {
                    g.Clear(Theme.LightGray);
                    g.DrawString("👤", new Font("Segoe UI", 24), Brushes.DarkGray, new PointF(5, 5));
                }
                profilePic.Image = placeholder;
            }
            
            // Get user info
            UploaderInfo currentUser = GetLoggedInUser();
            
            // Construct full display name
            string displayName = currentUser.Name ?? "User";
            if (!string.IsNullOrWhiteSpace(currentUser.FirstName) || !string.IsNullOrWhiteSpace(currentUser.LastName))
            {
                // If we have parsed name parts, use those
                var nameParts = new[] { currentUser.FirstName, currentUser.MiddleName, currentUser.LastName }
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .ToList();
                if (nameParts.Count > 0)
                {
                    displayName = string.Join(" ", nameParts);
                }
            }
            
            // User Name Label
            Label lblUserName = new Label
            {
                Text = displayName,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Location = new Point(70, 10),
                Width = 220,
                Height = 25
            };
            
            // Grade Section Label - ensure it's not "N/A"
            string gradeDisplay = currentUser.GradeSection ?? "N/A";
            if (string.IsNullOrWhiteSpace(currentUser.GradeSection) || currentUser.GradeSection == "N/A")
            {
                System.Diagnostics.Debug.WriteLine($"[Profile Card] Grade section is null/N/A. Full user info: Name={currentUser.Name}, Grade={currentUser.GradeSection}");
            }
            
            Label lblGradeSection = new Label
            {
                Text = gradeDisplay,
                Font = new Font("Segoe UI", 9),
                ForeColor = Theme.LightGray,
                AutoSize = false,
                Location = new Point(70, 35),
                Width = 220,
                Height = 40
            };
            
            // Edit Profile Button
            Button btnEditProfile = new Button
            {
                Text = "Edit Profile",
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                BackColor = Theme.AccentBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(10, 105),
                Width = 280,
                Height = 25,
                Cursor = Cursors.Hand
            };
            btnEditProfile.FlatAppearance.BorderSize = 0;
            btnEditProfile.Click += (s, e) => MessageBox.Show("Profile editing feature coming soon!", "Edit Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            profileCard.Controls.Add(profilePic);
            profileCard.Controls.Add(lblUserName);
            profileCard.Controls.Add(lblGradeSection);
            profileCard.Controls.Add(btnEditProfile);
            
            searchPanel.Controls.Add(profileCard);
            
            Label lblSearch = new Label 
            { 
                Text = "🔍 Search", 
                AutoSize = true, 
                Top = 180, 
                Left = 20, 
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            txtSearch = new TextBox 
            { 
                Top = 210, 
                Left = 20, 
                Width = 290, 
                Height = 38,
                Font = new Font("Segoe UI", 10),
                BackColor = Theme.LightGray,
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Theme.NearBlack
            };
            btnSearch = new Button 
            { 
                Text = "Search", 
                Top = 255, 
                Left = 20, 
                Width = 135, 
                Height = 36, 
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Theme.TealGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnClearSearch = new Button 
            { 
                Text = "Clear", 
                Top = 255, 
                Left = 175, 
                Width = 135, 
                Height = 36, 
                Font = new Font("Segoe UI", 10),
                BackColor = Theme.DarkGray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnClearSearch.FlatAppearance.BorderSize = 0;
            Label lblTags = new Label 
            { 
                Text = "📌 Tags", 
                AutoSize = true, 
                Top = 305, 
                Left = 20, 
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            lstTags = new ListBox 
            { 
                Top = 335, 
                Left = 20, 
                Width = 290, 
                Height = 140, 
                Font = new Font("Segoe UI", 10),
                BackColor = Theme.LightGray,
                ForeColor = Theme.NearBlack,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtNewTag = new TextBox 
            { 
                Top = 485, 
                Left = 20, 
                Width = 200, 
                Height = 36,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ForeColor = Theme.NearBlack
            };
            btnAddTag = new Button 
            { 
                Text = "Add", 
                Top = 485, 
                Left = 230, 
                Width = 80, 
                Height = 36, 
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Theme.AccentBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAddTag.FlatAppearance.BorderSize = 0;
            searchPanel.Controls.Add(lblSearch);
            searchPanel.Controls.Add(txtSearch);
            searchPanel.Controls.Add(btnSearch);
            searchPanel.Controls.Add(btnClearSearch);
            searchPanel.Controls.Add(lblTags);
            searchPanel.Controls.Add(lstTags);
            searchPanel.Controls.Add(txtNewTag);
            searchPanel.Controls.Add(btnAddTag);
            this.Controls.Add(searchPanel);
            openCameraButton = new Button
            {
                Text = "+",
                Width = 60,
                Height = 60,
                BackColor = Theme.TealGreen,
                ForeColor = Theme.SoftWhite,
                Font = new Font("Arial", 20, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            openCameraButton.FlatAppearance.BorderSize = 0;
            openCameraButton.FlatAppearance.MouseDownBackColor = Theme.DarkTeal;
            openCameraButton.FlatAppearance.MouseOverBackColor = Theme.MediumTeal;
            openCameraButton.Click += OpenCameraButton_Click;
            this.Controls.Add(outerPanel);
            
            this.Controls.Add(openCameraButton);
            openCameraButton.BringToFront();
            openCameraButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            openCameraButton.Location = new Point(this.ClientSize.Width - 90, this.ClientSize.Height - 90);
            this.Resize += (s, e) =>
            {
                openCameraButton.Location = new Point(this.ClientSize.Width - 90, this.ClientSize.Height - 90);
                outerPanel.PerformLayout();
            };
            btnSearch.Click += (s, e) => PerformSearch(txtSearch.Text.Trim());
            btnClearSearch.Click += (s, e) =>
            {
                txtSearch.Text = "";
                LoadSavedImages();
            };
            btnAddTag.Click += (s, e) =>
            {
                if (selectedCard == null) { MessageBox.Show("Select an image first."); return; }
                var tag = txtNewTag.Text.Trim();
                if (string.IsNullOrEmpty(tag)) return;
                SaveTagForCard((string)selectedCard.Tag, tag);
                LoadTagsForCard(selectedCard);
                txtNewTag.Text = "";
            };
            btnLogout = new Button
            {
                Text = "Logout",
                Width = 100,
                Height = 36,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Theme.DeepRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Logout and return to login?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Application.Restart();
                    Application.ExitThread();
                }
            };
            this.Controls.Add(btnLogout);
            this.Load += (s, e) => PositionLogout();
            this.Resize += (s, e) => PositionLogout();
            void PositionLogout()
            {
                int rightMargin = 20;
                int x = searchPanel.Left - btnLogout.Width - 10;
                btnLogout.Location = new System.Drawing.Point(Math.Max(20, x), 16);
                btnLogout.BringToFront();
            }
            galleryTable.BackColor = Theme.GetBackgroundTeal();
            outerPanel.BackColor = Theme.GetBackgroundTeal();
            LoadSavedImages();
            Theme.Apply(this);
            AddLogoCopyright();
            SetLogoTransparentBackground();
        }
        private void OpenCameraButton_Click(object sender, EventArgs e)
        {
            var uploader = GetLoggedInUser();
            
            string? selectedDevice = null;
            try
            {
                var videoDevices = new AForge.Video.DirectShow.FilterInfoCollection(AForge.Video.DirectShow.FilterCategory.VideoInputDevice);
                
                if (videoDevices.Count == 0)
                {
                    MessageBox.Show("No camera devices found!", "Camera Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
                if (videoDevices.Count > 1)
                {
                    using (var selectForm = new FormSelectCamera(videoDevices))
                    {
                        if (selectForm.ShowDialog(this) == DialogResult.OK)
                        {
                            selectedDevice = selectForm.SelectedDeviceMoniker;
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else
                {
                    selectedDevice = videoDevices[0].MonikerString;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error detecting cameras: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            FormCamera camForm = new FormCamera(saveFolder, selectedDevice);
            camForm.PhotoSaved += (filePath) =>
            {
                try
                {
                    var meta = PromptForImageMetadata(uploader);
                    if (meta == null)
                    {
                        LoadSavedImages();
                        LogImageToGoogleSheets(filePath, "", uploader);
                        return;
                    }
                    string infoPath = Path.Combine(Path.GetDirectoryName(filePath),
                        Path.GetFileNameWithoutExtension(filePath) + "_info.txt");
                    string uploaderName = "Unknown";
                    if (uploader != null)
                    {
                        var fullNameParts = new[] { uploader.FirstName, uploader.MiddleName, uploader.LastName }
                            .Where(s => !string.IsNullOrWhiteSpace(s))
                            .ToList();
                        uploaderName = fullNameParts.Count > 0 ? string.Join(" ", fullNameParts) : uploader.Name ?? Environment.UserName;
                    }
                    string gradeSection = !string.IsNullOrWhiteSpace(uploader?.GradeSection) ? uploader.GradeSection : "N/A";
                    var lines = new[]
                    {
                        $"Uploader: {uploaderName}",
                        $"GradeSection: {gradeSection}",
                        $"Location: {meta.Value.Location}",
                        $"Date: {meta.Value.Date:yyyy-MM-dd HH:mm}"
                    };
                    File.WriteAllLines(infoPath, lines);
                    
                    // Log tags to Google Sheets
                    string tagsPath = Path.Combine(Path.GetDirectoryName(filePath),
                        Path.GetFileNameWithoutExtension(filePath) + "_tags.txt");
                    string tagsStr = File.Exists(tagsPath) ? string.Join(", ", File.ReadAllLines(tagsPath)) : "";
                    if (uploader != null)
                    {
                        LogImageToGoogleSheets(filePath, tagsStr, uploader);
                    }
                }
                catch {  }
                LoadSavedImages();
            };
            camForm.ShowDialog();
        }
        private void LoadSavedImages()
        {
            galleryTable.Controls.Clear();
            galleryTable.RowCount = 0;
            
            var list = Directory.GetFiles(saveFolder, "*.jpg")
                .Select(f =>
                {
                    string fileName = Path.GetFileNameWithoutExtension(f);
                    DateTime dt = DateTime.MinValue;
                    if (fileName.StartsWith("photo_") && fileName.Length >= 20)
                    {
                        string dateStr = fileName.Substring(6, 15);
                        if (DateTime.TryParseExact(dateStr, "yyyyMMdd_HHmmss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt))
                        {
                            // parsed successfully
                        }
                    }
                    if (dt == DateTime.MinValue)
                    {
                        dt = File.GetCreationTimeUtc(f);
                        if (dt == DateTime.MinValue) dt = File.GetLastWriteTimeUtc(f);
                    }
                    return new { File = f, Date = dt };
                })
                .OrderByDescending(x => x.Date)
                .ToList();
            
            int columnIndex = 0;
            int rowIndex = 0;
            
            foreach (var item in list)
            {
                if (columnIndex == 0)
                {
                    galleryTable.RowCount++;
                }
                
                try
                {
                    string filePath = item.File;
                    Image img = Image.FromFile(filePath);
                    
                    var card = new Panel
                    {
                        Width = IMAGE_SIZE,
                        Height = IMAGE_SIZE + 80,
                        BackColor = Theme.MediumTeal,
                        Margin = new Padding(10),
                        Padding = new Padding(5),
                        BorderStyle = BorderStyle.FixedSingle,
                        Tag = filePath,
                        Cursor = Cursors.Hand
                    };
                    
                    PictureBox pic = new PictureBox
                    {
                        Image = img,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Width = IMAGE_SIZE - 10,
                        Height = IMAGE_SIZE - 50,
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
                    
                    string filePath_copy = filePath;
                    btnInfo.Click += (s, e) => ShowItemInfo(filePath_copy);
                    pic.Click += (s, e) => SelectCard(card);
                    card.Click += (s, e) => SelectCard(card);
                    
                    galleryTable.Controls.Add(card, columnIndex, rowIndex);
                    
                    columnIndex++;
                    if (columnIndex >= COLUMNS)
                    {
                        columnIndex = 0;
                        rowIndex++;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading image: {ex.Message}");
                }
            }
        }
        private void SelectCard(Panel card)
        {
            if (selectedCard != null)
            {
                selectedCard.BorderStyle = BorderStyle.None;
            }
            selectedCard = card;
            selectedCard.BorderStyle = BorderStyle.FixedSingle;
            LoadTagsForCard(card);
        }
        private void ShowItemInfo(string filePath)
        {
            string infoPath = Path.Combine(Path.GetDirectoryName(filePath) ?? saveFolder,
                Path.GetFileNameWithoutExtension(filePath) + "_info.txt");
            if (!File.Exists(infoPath))
            {
                MessageBox.Show("No information available for this item.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            var lines = File.ReadAllLines(infoPath);
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var ln in lines)
            {
                var idx = ln.IndexOf(':');
                if (idx > -1)
                {
                    var k = ln.Substring(0, idx).Trim();
                    var v = ln.Substring(idx + 1).Trim();
                    map[k] = v;
                }
            }
            string uploaderName = map.TryGetValue("Uploader", out var u) ? u : "N/A";
            string gradeSection = map.TryGetValue("GradeSection", out var g) ? g : "N/A";
            string location = map.TryGetValue("Location", out var l) ? l : "N/A";
            string dateFound = map.TryGetValue("Date", out var d) ? d : (map.TryGetValue("DateFound", out var d2) ? d2 : "N/A");
            Form infoForm = new Form
            {
                Text = "Item Information",
                StartPosition = FormStartPosition.CenterParent,
                Size = new Size(420, 260)
            };
            Label uploaderLabel = new Label
            {
                Text = $"Uploader: {uploaderName}",
                Top = 16,
                Left = 20,
                AutoSize = true,
                Font = new Font("Arial", 11, FontStyle.Bold),
            };
            Label gradeLabel = new Label
            {
                Text = $"Grade/Section: {gradeSection}",
                Top = 48,
                Left = 20,
                AutoSize = true,
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            Label locationLabel = new Label
            {
                Text = $"Location: {location}",
                Top = 80,
                Left = 20,
                AutoSize = true,
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            Label dateLabel = new Label
            {
                Text = $"Date Found: {dateFound}",
                Top = 112,
                Left = 20,
                AutoSize = true,
                Font = new Font("Arial", 11, FontStyle.Bold)
            };
            infoForm.Controls.Add(uploaderLabel);
            infoForm.Controls.Add(gradeLabel);
            infoForm.Controls.Add(locationLabel);
            infoForm.Controls.Add(dateLabel);
            infoForm.ShowDialog();
        }
        private void stringToFileAppend(string path, string text)
        {
            File.AppendAllText(path, text + Environment.NewLine);
        }
        private void SaveTagForCard(string filePath, string tag)
        {
            try
            {
                string tagPath = Path.Combine(Path.GetDirectoryName(filePath),
                    Path.GetFileNameWithoutExtension(filePath) + "_tags.txt");
                var existing = File.Exists(tagPath) ? File.ReadAllLines(tagPath) : Array.Empty<string>();
                if (!existing.Any(t => string.Equals(t.Trim(), tag, StringComparison.OrdinalIgnoreCase)))
                {
                    File.AppendAllLines(tagPath, new[] { tag });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not save tag: " + ex.Message);
            }
        }
        private void LoadTagsForCard(Panel card)
        {
            lstTags.Items.Clear();
            if (card?.Tag is string filePath)
            {
                string tagPath = Path.Combine(Path.GetDirectoryName(filePath),
                    Path.GetFileNameWithoutExtension(filePath) + "_tags.txt");
                if (File.Exists(tagPath))
                {
                    foreach (var t in File.ReadAllLines(tagPath).Where(x => !string.IsNullOrWhiteSpace(x)))
                        lstTags.Items.Add(t.Trim());
                }
            }
        }
        private void PerformSearch(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                // Show all images
                LoadSavedImages();
                return;
            }
            query = query.ToLowerInvariant();
            // Filter logic would go here - for now we reload all
            LoadSavedImages();
        }
        private class UploaderInfo { 
            public string Name; 
            public string FirstName;
            public string MiddleName;
            public string LastName;
            public string GradeSection; 
        }
        private void ParseFullName(string fullName, UploaderInfo info)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return;
            var parts = fullName.Trim().Split(new[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 1)
                info.FirstName = parts[0];
            if (parts.Length >= 2)
                info.MiddleName = parts[1];
            if (parts.Length >= 3)
                info.LastName = string.Join(" ", parts.Skip(2)); 
        }
        private UploaderInfo PromptForUploaderInfo()
        {
            using (Form f = new Form())
            {
                f.Text = "Uploader Information";
                f.StartPosition = FormStartPosition.CenterParent;
                f.Size = new Size(360, 200);
                Label lblName = new Label { Text = "Name:", Top = 16, Left = 12, AutoSize = true };
                TextBox txtName = new TextBox { Top = 36, Left = 12, Width = 320 };
                Label lblGrade = new Label { Text = "Grade / Section:", Top = 72, Left = 12, AutoSize = true };
                TextBox txtGrade = new TextBox { Top = 92, Left = 12, Width = 320 };
                Button ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Top = 130, Left = 170, Width = 75 };
                Button cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Top = 130, Left = 255, Width = 75 };
                f.Controls.Add(lblName); f.Controls.Add(txtName);
                f.Controls.Add(lblGrade); f.Controls.Add(txtGrade);
                f.Controls.Add(ok); f.Controls.Add(cancel);
                f.AcceptButton = ok; f.CancelButton = cancel;
                if (f.ShowDialog() == DialogResult.OK)
                {
                    return new UploaderInfo { Name = txtName.Text.Trim(), GradeSection = txtGrade.Text.Trim() };
                }
                return null;
            }
        }
        private (string Location, DateTime Date)? PromptForImageMetadata(UploaderInfo uploader)
        {
            using (Form f = new Form())
            {
                f.Text = "Item details";
                f.StartPosition = FormStartPosition.CenterParent;
                f.Size = new Size(380, 240);
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MaximizeBox = false;
                f.MinimizeBox = false;
                Label lblUploader = new Label { Text = "Uploader:", Top = 12, Left = 12, AutoSize = true };
                Label lblUploaderVal = new Label { Text = (uploader != null && !string.IsNullOrWhiteSpace(uploader.FirstName)) ? $"{uploader.FirstName} {uploader.MiddleName ?? ""} {uploader.LastName ?? ""}".Trim() : Environment.UserName, Top = 12, Left = 110, AutoSize = true, Font = new Font("Arial", 9, FontStyle.Bold) };
                Label lblGrade = new Label { Text = "Grade / Section:", Top = 40, Left = 12, AutoSize = true };
                Label lblGradeVal = new Label { Text = !string.IsNullOrWhiteSpace(uploader?.GradeSection) ? uploader.GradeSection : "N/A", Top = 40, Left = 110, AutoSize = true, Font = new Font("Arial", 9, FontStyle.Bold) };
                Label lblLocation = new Label { Text = "Location:", Top = 72, Left = 12, AutoSize = true };
                TextBox txtLocation = new TextBox { Top = 92, Left = 12, Width = 340 };
                Label lblDate = new Label { Text = "Date / Time:", Top = 124, Left = 12, AutoSize = true };
                DateTimePicker dtp = new DateTimePicker { Top = 144, Left = 12, Width = 220, Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd HH:mm", Value = DateTime.Now };
                Button ok = new Button { Text = "OK", DialogResult = DialogResult.OK, Top = 180, Left = 190, Width = 75 };
                Button cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Top = 180, Left = 275, Width = 75 };
                f.Controls.Add(lblUploader); f.Controls.Add(lblUploaderVal);
                f.Controls.Add(lblGrade); f.Controls.Add(lblGradeVal);
                f.Controls.Add(lblLocation); f.Controls.Add(txtLocation);
                f.Controls.Add(lblDate); f.Controls.Add(dtp);
                f.Controls.Add(ok); f.Controls.Add(cancel);
                f.AcceptButton = ok; f.CancelButton = cancel;
                if (f.ShowDialog() == DialogResult.OK)
                {
                    var loc = txtLocation.Text.Trim();
                    if (string.IsNullOrWhiteSpace(loc)) loc = "N/A";
                    return (loc, dtp.Value);
                }
                return null;
            }
        }
        private UploaderInfo GetLoggedInUser()
        {
            string currentUserEmail = null;
            
            // Try to read the current user email from file - multiple possible locations
            string[] emailFilePaths = new[]
            {
                // First check project root (where Flask and C# login save it)
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\current_user_email.txt"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "current_user_email.txt"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\current_user_email.txt"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ReturnPoint", "current_user_email.txt")
            };
            
            foreach (var emailFilePath in emailFilePaths)
            {
                try
                {
                    string fullPath = Path.GetFullPath(emailFilePath);
                    System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Checking: {fullPath}");
                    if (File.Exists(fullPath))
                    {
                        currentUserEmail = File.ReadAllText(fullPath).Trim();
                        System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] ✓ Found current user email: {currentUserEmail}");
                        break;
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Error checking {emailFilePath}: {ex.Message}");
                }
            }
            
            if (string.IsNullOrWhiteSpace(currentUserEmail))
            {
                System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] ✗ Could not find current_user_email.txt in any location");
            }
            
            try
            {
                // Try to get user info from Flask backend
                using (HttpClient client = new HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    string url = "http://localhost:5000/api/user";
                    if (!string.IsNullOrWhiteSpace(currentUserEmail))
                    {
                        url += $"?email={Uri.EscapeDataString(currentUserEmail)}";
                    }
                    System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Calling Flask: {url}");
                    
                    HttpResponseMessage response = client.GetAsync(url).Result;
                    System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Flask response: {response.StatusCode}");
                    if (response.IsSuccessStatusCode)
                    {
                        string json = response.Content.ReadAsStringAsync().Result;
                        System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Response JSON: {json}");
                        using (var doc = JsonDocument.Parse(json))
                        {
                            var root = doc.RootElement;
                            string name = root.TryGetProperty("name", out var n) ? n.GetString() : null;
                            string firstName = root.TryGetProperty("first_name", out var fn) ? fn.GetString() : null;
                            string middleName = root.TryGetProperty("middle_name", out var mn) ? mn.GetString() : null;
                            string lastName = root.TryGetProperty("last_name", out var ln) ? ln.GetString() : null;
                            string gradeSection = root.TryGetProperty("grade_section", out var gs) ? gs.GetString() : null;
                            
                            System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] ✓ Flask returned: Name={name}, FirstName={firstName}, Grade={gradeSection}");
                            
                            var result = new UploaderInfo
                            {
                                Name = !string.IsNullOrWhiteSpace(name) ? name : Environment.UserName,
                                FirstName = firstName ?? "",
                                MiddleName = middleName ?? "",
                                LastName = lastName ?? "",
                                GradeSection = gradeSection ?? "N/A"
                            };
                            
                            if (string.IsNullOrWhiteSpace(result.FirstName) && !string.IsNullOrWhiteSpace(result.Name))
                            {
                                ParseFullName(result.Name, result);
                            }
                            System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] SUCCESS (Flask): {result.FirstName} {result.LastName} ({result.GradeSection})");
                            return result;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] ✗ Flask failed: {response.StatusCode} - {response.Content.ReadAsStringAsync().Result}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] ✗ Error calling Flask: {ex.Message}");
            }
            
            // Fallback: Try reading from Program.CurrentUser
            try
            {
                var progType = Type.GetType("ReturnPoint.Program");
                if (progType != null)
                {
                    var prop = progType.GetProperty("CurrentUser", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    if (prop != null)
                    {
                        var cur = prop.GetValue(null);
                        if (cur != null)
                        {
                            var nameProp = cur.GetType().GetProperty("Name");
                            var firstNameProp = cur.GetType().GetProperty("FirstName");
                            var middleNameProp = cur.GetType().GetProperty("MiddleName");
                            var lastNameProp = cur.GetType().GetProperty("LastName");
                            var gradeProp = cur.GetType().GetProperty("GradeSection") ?? cur.GetType().GetProperty("grade_section");
                            string name = nameProp?.GetValue(cur)?.ToString();
                            string firstName = firstNameProp?.GetValue(cur)?.ToString();
                            string middleName = middleNameProp?.GetValue(cur)?.ToString();
                            string lastName = lastNameProp?.GetValue(cur)?.ToString();
                            string grade = gradeProp?.GetValue(cur)?.ToString();
                            var result = new UploaderInfo 
                            { 
                                Name = name ?? Environment.UserName, 
                                FirstName = firstName,
                                MiddleName = middleName,
                                LastName = lastName,
                                GradeSection = string.IsNullOrWhiteSpace(grade) ? "N/A" : grade 
                            };
                            if (string.IsNullOrWhiteSpace(result.FirstName) && !string.IsNullOrWhiteSpace(result.Name))
                            {
                                ParseFullName(result.Name, result);
                            }
                            System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] SUCCESS (Program.CurrentUser): {result.FirstName} {result.LastName} ({result.GradeSection})");
                            return result;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] ✗ Error reading Program.CurrentUser: {ex.Message}");
            }
            
            // Fallback: Try reading from users.json with current email
            try
            {
                if (!string.IsNullOrWhiteSpace(currentUserEmail))
                {
                    string[] usersJsonPaths = new[]
                    {
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\users.json"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json"),
                        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\users.json")
                    };
                    
                    foreach (var usersJsonPath in usersJsonPaths)
                    {
                        string fullPath = Path.GetFullPath(usersJsonPath);
                        System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Checking users.json: {fullPath}");
                        
                        if (File.Exists(fullPath))
                        {
                            string json = File.ReadAllText(fullPath);
                            var opts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            var users = JsonSerializer.Deserialize<List<JsonElement>>(json, opts);
                            
                            System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Loaded {users?.Count ?? 0} users from {fullPath}");
                            
                            if (users != null)
                            {
                                foreach (var user in users)
                                {
                                    string email = user.TryGetProperty("email", out var emailProp) ? emailProp.GetString() : "";
                                    if (string.Equals(email, currentUserEmail, StringComparison.OrdinalIgnoreCase))
                                    {
                                        string name = user.TryGetProperty("name", out var nameProp) ? nameProp.GetString() : null;
                                        string firstName = user.TryGetProperty("first_name", out var fnProp) ? fnProp.GetString() : null;
                                        string middleName = user.TryGetProperty("middle_name", out var mnProp) ? mnProp.GetString() : null;
                                        string lastName = user.TryGetProperty("last_name", out var lnProp) ? lnProp.GetString() : null;
                                        string gradeSection = user.TryGetProperty("grade_section", out var gsProp) ? gsProp.GetString() : null;
                                        
                                        System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] ✓ Found user in JSON: Name={name}, Grade={gradeSection}");
                                        
                                        var result = new UploaderInfo
                                        {
                                            Name = !string.IsNullOrWhiteSpace(name) ? name : Environment.UserName,
                                            FirstName = firstName ?? "",
                                            MiddleName = middleName ?? "",
                                            LastName = lastName ?? "",
                                            GradeSection = gradeSection ?? "N/A"
                                        };
                                        
                                        if (string.IsNullOrWhiteSpace(result.FirstName) && !string.IsNullOrWhiteSpace(result.Name))
                                        {
                                            ParseFullName(result.Name, result);
                                            System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Parsed full name: FirstName={result.FirstName}, MiddleName={result.MiddleName}, LastName={result.LastName}");
                                        }
                                        System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] SUCCESS (users.json): {result.FirstName} {result.MiddleName} {result.LastName} | Grade={result.GradeSection}");
                                        return result;
                                    }
                                }
                                System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] ✗ Email not found in users.json: {currentUserEmail}");
                            }
                            break; // Found the file, stop searching other paths
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] ✗ Error reading users.json: {ex.Message}");
            }
            
            // Last fallback: Return default user
            System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] ✗ All methods failed, returning default user");
            return new UploaderInfo 
            { 
                Name = Environment.UserName, 
                FirstName = null,
                MiddleName = null,
                LastName = null,
                GradeSection = "N/A" 
            };
        }

        private async void LogImageToGoogleSheets(string filePath, string tags, UploaderInfo uploader)
        {
            try
            {
                string filename = Path.GetFileName(filePath);
                string uploaderName = "Unknown";
                if (uploader != null)
                {
                    var parts = new[] { uploader.FirstName, uploader.MiddleName, uploader.LastName }
                        .Where(s => !string.IsNullOrWhiteSpace(s))
                        .ToList();
                    uploaderName = parts.Count > 0 ? string.Join(" ", parts) : uploader.Name ?? Environment.UserName;
                }
                
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                
                using (var client = new HttpClient())
                {
                    var logData = new
                    {
                        filename = filename,
                        user_name = uploaderName,
                        tags = tags,
                        file_path = filePath,
                        timestamp = timestamp
                    };
                    
                    var json = JsonSerializer.Serialize(logData);
                    var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                    
                    try
                    {
                        var response = await client.PostAsync("http://localhost:5000/api/log-captured-image", content);
                        if (response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"✓ Image logged to Google Sheets: {filename}");
                        }
                        else
                        {
                            Console.WriteLine($"⚠ Failed to log image: {filename}");
                        }
                    }
                    catch (HttpRequestException)
                    {
                        // Flask not available, that's okay - image is still stored locally
                        Console.WriteLine($"ℹ Flask unavailable, image stored locally: {filename}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error logging image to Google Sheets: {ex.Message}");
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
    }
}

