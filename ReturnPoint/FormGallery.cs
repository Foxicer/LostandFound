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
        private FlowLayoutPanel galleryPanel;
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
            galleryPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(30, 20, 30, 20),
                Location = new Point(0, 0)
            };
            int cardWidth = 220;
            int cardHorizontalMargin = 15 + 15;
            int columns = 1;
            int totalColumnWidth = columns * (cardWidth + cardHorizontalMargin);
            galleryPanel.MaximumSize = new Size(totalColumnWidth + galleryPanel.Padding.Left + galleryPanel.Padding.Right, 0);
            outerPanel.Controls.Add(galleryPanel);
            searchPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 340,
                BackColor = Theme.MediumTeal,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(20),
                Visible = true
            };
            Label lblSearch = new Label 
            { 
                Text = "🔍 Search", 
                AutoSize = true, 
                Top = 20, 
                Left = 20, 
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            txtSearch = new TextBox 
            { 
                Top = 50, 
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
                Top = 95, 
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
                Top = 95, 
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
                Top = 145, 
                Left = 20, 
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            lstTags = new ListBox 
            { 
                Top = 175, 
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
                Top = 325, 
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
                Top = 325, 
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
                foreach (Control c in galleryPanel.Controls) c.Visible = true;
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
            galleryPanel.BackColor = Theme.GetBackgroundTeal();
            outerPanel.BackColor = Theme.GetBackgroundTeal();
            LoadSavedImages();
            Theme.Apply(this);
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
                        AddImageToGallery(filePath, false);
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
            galleryPanel.Controls.Clear();
            var list = Directory.GetFiles(saveFolder, "*.jpg")
                .Select(f =>
                {
                    string fileName = Path.GetFileNameWithoutExtension(f);
                    DateTime dt = DateTime.MinValue;
                    if (fileName.StartsWith("photo_") && fileName.Length >= 20)
                    {
                        string dateStr = fileName.Substring(6, 15); // YYYYMMDD_HHMMSS
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
            foreach (var item in list)
            {
                bool claimed = File.Exists(Path.Combine(Path.GetDirectoryName(item.File),
                    Path.GetFileNameWithoutExtension(item.File) + "_claim.txt"));
                AddImageToGallery(item.File, claimed);
            }
        }
        private bool TryGetDateFromInfo(string imageFile, out DateTime when)
        {
            when = DateTime.MinValue;
            try
            {
                var infoPath = Path.Combine(Path.GetDirectoryName(imageFile),
                    Path.GetFileNameWithoutExtension(imageFile) + "_info.txt");
                if (!File.Exists(infoPath)) return false;
                foreach (var ln in File.ReadAllLines(infoPath))
                {
                    var idx = ln.IndexOf(':' );
                    if (idx < 0) continue;
                    var key = ln.Substring(0, idx).Trim();
                    var val = ln.Substring(idx + 1).Trim();
                    if (!key.Equals("Date", StringComparison.OrdinalIgnoreCase) &&
                        !key.Equals("DateFound", StringComparison.OrdinalIgnoreCase)) continue;
                    string[] fmts = { "yyyy-MM-dd HH:mm", "yyyy-MM-dd H:mm", "yyyy-MM-ddTHH:mm:ss", "o" };
                    if (DateTime.TryParseExact(val, fmts, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out var parsed))
                    {
                        when = parsed.ToUniversalTime();
                        return true;
                    }
                    if (DateTime.TryParse(val, out parsed))
                    {
                        when = parsed.ToUniversalTime();
                        return true;
                    }
                }
            }
            catch {  }
            return false;
        }
        private void AddImageToGallery(string filePath, bool alreadyClaimed = false, UploaderInfo? uploader = null)
        {
            if (!File.Exists(filePath)) return;
            Image img = Image.FromFile(filePath);
            int displayWidth = 200; 
            int displayHeight = (int)((double)img.Height / img.Width * displayWidth);
            var card = new RoundedPanel
            {
                Width = 220,
                Height = displayHeight + 60,
                BackColor = this.BackColor,
                Margin = new Padding(10),
                Padding = new Padding(10, 0, 10, 0),
                CornerRadius = 20
            };
            card.Tag = filePath;
            card.Click += (s, e) => SelectCard(card);
            PictureBox pic = new PictureBox
            {
                Image = img,
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = displayWidth,
                Height = displayHeight,
                Cursor = Cursors.Hand,
                Location = new Point(10, 0)  
            };
            pic.Click += (s, e) => SelectCard(card);
            void ShowItemInfo()
            {
                string infoPath = Path.Combine(Path.GetDirectoryName(filePath),
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
                string gradeSection = (uploader?.GradeSection) ?? (map.TryGetValue("GradeSection", out var g) ? g : "N/A");
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
            void ShowClaimantInfo()
            {
                string folder = Path.GetDirectoryName(filePath);
                string infoPath = Path.Combine(folder, Path.GetFileNameWithoutExtension(filePath) + "_claim.txt");
                if (File.Exists(infoPath))
                {
                    string[] lines = File.ReadAllLines(infoPath);
                    Form claimForm = new Form
                    {
                        Text = "Claimant Information",
                        StartPosition = FormStartPosition.CenterParent,
                        Size = new Size(700, 500) 
                    };
                    Label lblName = new Label
                    {
                        Text = lines.Length > 0 ? lines[0] : "Name: N/A",
                        Top = 20,
                        Left = 20,
                        AutoSize = true,
                        Font = new Font("Arial", 12, FontStyle.Bold)
                    };
                    Label lblContact = new Label
                    {
                        Text = lines.Length > 1 ? lines[1] : "Contact: N/A",
                        Top = 60,
                        Left = 20,
                        AutoSize = true,
                        Font = new Font("Arial", 12, FontStyle.Bold)
                    };
                    Label lblRole = new Label
                    {
                        Text = lines.Length > 2 ? lines[2] : "Role: N/A",
                        Top = 100,
                        Left = 20,
                        AutoSize = true,
                        Font = new Font("Arial", 12, FontStyle.Bold)
                    };
                    Label lblGradeSection = new Label
                    {
                        Text = lines.Length > 3 ? lines[3] : "Grade/Section: N/A",
                        Top = 140,
                        Left = 20,
                        AutoSize = true,
                        Font = new Font("Arial", 12, FontStyle.Bold)
                    };
                    Label lblWhen = new Label
                    {
                        Text = lines.Length > 4 ? lines[4] : "When Found: N/A",
                        Top = 180,
                        Left = 20,
                        AutoSize = true,
                        Font = new Font("Arial", 12, FontStyle.Bold)
                    };
                    PictureBox picClaimant = new PictureBox
                    {
                        Top = 20,
                        Left = 350,
                        Width = 300,
                        Height = 300,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    string claimantFolder = Path.Combine(folder, "Claimants");
                    if (Directory.Exists(claimantFolder))
                    {
                        string[] claimantPhotos = Directory.GetFiles(claimantFolder, "claimant_*.jp*"); 
                        if (claimantPhotos.Length > 0)
                        {
                            try
                            {
                                string latestPhoto = claimantPhotos.Last();
                                using (FileStream fs = new FileStream(latestPhoto, FileMode.Open, FileAccess.Read))
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    fs.CopyTo(ms);
                                    picClaimant.Image = Image.FromStream(ms);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Could not load claimant photo: {ex.Message}", 
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                    claimForm.Controls.Add(lblName);
                    claimForm.Controls.Add(lblContact);
                    claimForm.Controls.Add(lblRole);
                    claimForm.Controls.Add(lblGradeSection);
                    claimForm.Controls.Add(lblWhen);
                    claimForm.Controls.Add(picClaimant);
                    claimForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No information available for this item.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            Button infoBtn = new Button
            {
                Text = "Info",
                Top = displayHeight + 5,
                Left = 10,
                Width = displayWidth,
                Height = 40,
                BackColor = Color.LightGray,
                ForeColor = Color.Black,
                Font = new Font("Verdana", 10, FontStyle.Bold)
            };
            infoBtn.Click += (s, e) => ShowItemInfo();
            card.Controls.Add(pic);
            card.Controls.Add(infoBtn);
             galleryPanel.Controls.Add(card);
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
                foreach (Control c in galleryPanel.Controls) c.Visible = true;
                return;
            }
            query = query.ToLowerInvariant();
            foreach (Control c in galleryPanel.Controls)
            {
                bool match = false;
                if (c is Panel card && card.Tag is string filePath)
                {
                    if (Path.GetFileName(filePath).ToLowerInvariant().Contains(query)) match = true;
                    string tagPath = Path.Combine(Path.GetDirectoryName(filePath),
                        Path.GetFileNameWithoutExtension(filePath) + "_tags.txt");
                    if (!match && File.Exists(tagPath))
                    {
                        foreach (var t in File.ReadAllLines(tagPath))
                        {
                            if (t != null && t.ToLowerInvariant().Contains(query)) { match = true; break; }
                        }
                    }
                }
                c.Visible = match;
            }
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
            
            // Try to read the current user email from file
            try
            {
                string emailFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\..\\current_user_email.txt");
                emailFilePath = Path.GetFullPath(emailFilePath);
                if (File.Exists(emailFilePath))
                {
                    currentUserEmail = File.ReadAllText(emailFilePath).Trim();
                    System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Read current user email from file: {currentUserEmail}");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Error reading email file: {ex.Message}");
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
                            
                            System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Parsed - Name: {name}, FirstName: {firstName}, LastName: {lastName}, Grade: {gradeSection}");
                            
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
                            System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] SUCCESS: {result.FirstName} {result.LastName} ({result.GradeSection})");
                            System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Raw gradeSection from JSON: '{gradeSection}'");
                            return result;
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Failed response: {response.StatusCode} - {response.Content.ReadAsStringAsync().Result}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[GetLoggedInUser] Error calling Flask: {ex.Message}");
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
                            return result;
                        }
                    }
                }
            }
            catch {  }
            
            // Last fallback: Return default user
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
    }
}

