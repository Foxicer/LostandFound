using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Linq;
using System.Text.Json;
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

        // ... new fields for search / tagging ...
        private Panel searchPanel;
        private TextBox txtSearch;
        private Button btnSearch;
        private Button btnClearSearch;
        private ListBox lstTags;
        private TextBox txtNewTag;
        private Button btnAddTag;
        private Panel selectedCard;

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
            // initialize saveFolder to a sensible default before using it
            if (string.IsNullOrWhiteSpace(saveFolder))
            {
                saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CapturedImages");
            }
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }

            this.Text = "Gallery";
            this.WindowState = FormWindowState.Maximized;
            // this.BackColor = Color.SeaGreen; // 🌿 Main background green
            // apply app theme to gallery form
            // Theme.Apply(this);

            // Outer panel (scroll area)
            outerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                // BackColor = Color.SeaGreen // same as form background
                // background will be set by Theme.Apply later
            };

            // Inner panel (left-aligned gallery)
            galleryPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                Padding = new Padding(20, 10, 20, 10),
                Location = new Point(10, 10) // align to left
            };

            

            // calculate width so exactly 3 columns fit:
            int cornerRadius = 20;
            var path = new GraphicsPath();
            int cardWidth = 220;            // must match card.Width in AddImageToGallery
            int cardHorizontalMargin = 10 + 10; // card.Margin.Left + card.Margin.Right
            int columns = 3;
            int totalColumnWidth = columns * (cardWidth + cardHorizontalMargin);
            galleryPanel.MaximumSize = new Size(totalColumnWidth + galleryPanel.Padding.Left + galleryPanel.Padding.Right, 0);

            outerPanel.Controls.Add(galleryPanel);

            // Right floating search / tag panel (stationary)
            searchPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 300,
                // BackColor = Color.FromArgb(230, 240, 240),
                // start neutral; we'll force white below after theming
                Padding = new Padding(10)
            };

            Label lblSearch = new Label { Text = "Search", AutoSize = true, Top = 6, Left = 10, Font = new Font("Arial", 10, FontStyle.Bold) };
            txtSearch = new TextBox { Top = 30, Left = 10, Width = 260 };
            btnSearch = new Button { Text = "Search", Top = 60, Left = 10, Width = 125 };
            btnClearSearch = new Button { Text = "Clear", Top = 60, Left = 145, Width = 125 };

            Label lblTags = new Label { Text = "Tags (selected)", AutoSize = true, Top = 100, Left = 10, Font = new Font("Arial", 10, FontStyle.Bold) };
            lstTags = new ListBox { Top = 125, Left = 10, Width = 260, Height = 160 };
            txtNewTag = new TextBox { Top = 295, Left = 10, Width = 180 };
            btnAddTag = new Button { Text = "Add Tag", Top = 293, Left = 195, Width = 75 };

            searchPanel.Controls.Add(lblSearch);
            searchPanel.Controls.Add(txtSearch);
            searchPanel.Controls.Add(btnSearch);
            searchPanel.Controls.Add(btnClearSearch);
            searchPanel.Controls.Add(lblTags);
            searchPanel.Controls.Add(lstTags);
            searchPanel.Controls.Add(txtNewTag);
            searchPanel.Controls.Add(btnAddTag);

            // add the search panel first so outerPanel (Dock=Fill) fills remaining area
            this.Controls.Add(searchPanel);

            // Floating "+" button
            openCameraButton = new Button
            {
                Text = "+",
                Width = 60,
                Height = 60,
                // BackColor = Color.Aqua, // ✅ aqua by default
                // ForeColor = Color.White,
                BackColor = Theme.StrongAqua,
                ForeColor = Theme.SoftWhite,
                Font = new Font("Arial", 18, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            openCameraButton.FlatAppearance.BorderSize = 0;

            // hover effect → lighter aqua
            openCameraButton.MouseEnter += (s, e) =>
            {
                // openCameraButton.BackColor = Color.MediumAquamarine;
                openCameraButton.BackColor = Theme.MediumAqua;
            };
            openCameraButton.MouseLeave += (s, e) =>
            {
                // openCameraButton.BackColor = Color.Aqua;
                openCameraButton.BackColor = Theme.StrongAqua;
            };

            openCameraButton.Click += OpenCameraButton_Click;

            // now add the scrollable gallery area (fills remaining area)
            this.Controls.Add(outerPanel);
            this.Controls.Add(openCameraButton);

            openCameraButton.BringToFront();
            openCameraButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            openCameraButton.Location = new Point(this.ClientSize.Width - 80, this.ClientSize.Height - 80);

            this.Resize += (s, e) =>
            {
                openCameraButton.Location = new Point(this.ClientSize.Width - 80, this.ClientSize.Height - 80);
                outerPanel.PerformLayout();
            };

            // wire up search / tag events
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

            // logout button - place in top-right of the form but not overlapped by search panel
            btnLogout = new Button
            {
                Text = "Logout",
                Width = 100,
                Height = 34,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Logout and return to login?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Application.Restart();
                    Application.ExitThread();
                }
            };
            // add to form and position relative to client area and searchPanel width
            this.Controls.Add(btnLogout);
            this.Load += (s, e) => PositionLogout();
            this.Resize += (s, e) => PositionLogout();

            void PositionLogout()
            {
                int rightMargin = 16;
                // if searchPanel exists and is docked right, place logout slightly left of its left edge
                int x = this.ClientSize.Width - btnLogout.Width - rightMargin;
                if (searchPanel != null && searchPanel.Dock == DockStyle.Right)
                {
                    x = searchPanel.Left - btnLogout.Width - 8;
                }
                btnLogout.Location = new System.Drawing.Point(Math.Max(8, x), 8);
                btnLogout.BringToFront();
            }

            // Apply theme after all controls exist so Theme.Apply can walk everything
            Theme.Apply(this);

            // Ensure the search-area is white with black text (override theme)
            if (searchPanel != null)
            {
                searchPanel.BackColor = Color.White;
                searchPanel.BorderStyle = BorderStyle.FixedSingle;
                // force children to black-on-white where appropriate
                txtSearch.BackColor = Color.White; txtSearch.ForeColor = Color.Black;
                btnSearch.BackColor = Color.White; btnSearch.ForeColor = Color.Black; btnSearch.FlatStyle = FlatStyle.Flat;
                btnClearSearch.BackColor = Color.White; btnClearSearch.ForeColor = Color.Black; btnClearSearch.FlatStyle = FlatStyle.Flat;
                lstTags.BackColor = Color.White; lstTags.ForeColor = Color.Black;
                txtNewTag.BackColor = Color.White; txtNewTag.ForeColor = Color.Black;
                btnAddTag.BackColor = Color.White; btnAddTag.ForeColor = Color.Black; btnAddTag.FlatStyle = FlatStyle.Flat;
            }

            // Make sure logout button is visible and themed
            btnLogout.BackColor = Theme.OuterRing;
            btnLogout.ForeColor = Theme.SoftWhite;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.BringToFront();

            LoadSavedImages();
        }

        private void OpenCameraButton_Click(object sender, EventArgs e)
        {
            // automatically determine uploader from app/session (falls back to Windows user)
            var uploader = GetLoggedInUser();

            FormCamera camForm = new FormCamera(saveFolder);
            camForm.PhotoSaved += (filePath) =>
            {
                try
                {
                    // prompt for location and date (uploader and grade filled automatically)
                    var meta = PromptForImageMetadata(uploader);
                    // if user cancelled the metadata dialog, still add the image but do not write metadata
                    if (meta == null)
                    {
                        AddImageToGallery(filePath, false);
                        return;
                    }

                    // create or overwrite info file for this image with uploader metadata
                    string infoPath = Path.Combine(Path.GetDirectoryName(filePath),
                        Path.GetFileNameWithoutExtension(filePath) + "_info.txt");

                    string uploaderName = !string.IsNullOrWhiteSpace(uploader?.Name) ? uploader.Name : Environment.UserName;
                    string gradeSection = !string.IsNullOrWhiteSpace(uploader?.GradeSection) ? uploader.GradeSection : "N/A";

                    var lines = new[]
                    {
                        $"Uploader: {uploaderName}",
                        $"GradeSection: {gradeSection}",
                        $"Location: {meta.Value.Location}",
                        $"Date: {meta.Value.Date:yyyy-MM-dd HH:mm}"
                    };

                    File.WriteAllLines(infoPath, lines);
                }
                catch { /* non-fatal */ }

                AddImageToGallery(filePath, false);
            };
            camForm.ShowDialog();
        }

        private void LoadSavedImages()
        {
            galleryPanel.Controls.Clear();

            // gather files with a best-effort Date (prefer _info.txt "Date" / "DateFound", fallback to file time)
            var list = Directory.GetFiles(saveFolder, "*.jpg")
                .Select(f =>
                {
                    DateTime dt;
                    if (!TryGetDateFromInfo(f, out dt))
                    {
                        // fallback: file creation or last write time
                        dt = File.GetCreationTimeUtc(f);
                        if (dt == DateTime.MinValue) dt = File.GetLastWriteTimeUtc(f);
                    }
                    return new { File = f, Date = dt };
                })
                // newest first
                .OrderByDescending(x => x.Date)
                .ToList();

            foreach (var item in list)
            {
                bool claimed = File.Exists(Path.Combine(Path.GetDirectoryName(item.File),
                    Path.GetFileNameWithoutExtension(item.File) + "_claim.txt"));
                AddImageToGallery(item.File, claimed);
            }
        }

        // try to parse Date/DateFound in the image info file (returns UTC)
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

                    // try common formats first
                    string[] fmts = { "yyyy-MM-dd HH:mm", "yyyy-MM-dd H:mm", "yyyy-MM-ddTHH:mm:ss", "o" };
                    if (DateTime.TryParseExact(val, fmts, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out var parsed))
                    {
                        when = parsed.ToUniversalTime();
                        return true;
                    }
                    // last resort: generic parse
                    if (DateTime.TryParse(val, out parsed))
                    {
                        when = parsed.ToUniversalTime();
                        return true;
                    }
                }
            }
            catch { /* ignore parsing issues */ }
            return false;
        }

        private void AddImageToGallery(string filePath, bool alreadyClaimed = false)
        {
            if (!File.Exists(filePath)) return;

            Image img = Image.FromFile(filePath);

            int displayWidth = 200; // Base image width
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

            // store the filepath on the card for selection/search/tagging
            card.Tag = filePath;
            // selection on click (card or picture)
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

            // Extract the info-display logic
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
 
                string uploader = map.TryGetValue("Uploader", out var u) ? u : "N/A";
                string gradeSection = map.TryGetValue("GradeSection", out var g) ? g : "N/A";
                string location = map.TryGetValue("Location", out var l) ? l : "N/A";
                // prefer "Date" key (new files). fall back to legacy "DateFound"
                string dateFound = map.TryGetValue("Date", out var d) ? d : (map.TryGetValue("DateFound", out var d2) ? d2 : "N/A");
 
                Form infoForm = new Form
                {
                    Text = "Item Information",
                    StartPosition = FormStartPosition.CenterParent,
                    Size = new Size(420, 260)
                };
 
                Label uploaderLabel = new Label
                {
                    Text = $"Uploader: {uploader}",
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
                        Size = new Size(700, 500) // bigger to fit photo
                    };

                    // --- Text Labels ---
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

                    // --- Claimant Photo ---
                    PictureBox picClaimant = new PictureBox
                    {
                        Top = 20,
                        Left = 350,
                        Width = 300,
                        Height = 300,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        BorderStyle = BorderStyle.FixedSingle
                    };

                    // look in .../Claimant subfolder
                    string claimantFolder = Path.Combine(folder, "Claimants");
                    if (Directory.Exists(claimantFolder))
                    {
                        string[] claimantPhotos = Directory.GetFiles(claimantFolder, "claimant_*.jp*"); // handles .jpg/.jpeg

                        if (claimantPhotos.Length > 0)
                        {
                            try
                            {
                                string latestPhoto = claimantPhotos.Last();

                                // load into memory (avoids file locking)
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

                    // Add controls
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


            pic.Click += (s, e) => ShowClaimantInfo();

            // Single Info button (claim functionality removed)
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

        // ---- selection + tag helpers ----
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
                // append if not already present
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
                    // match filename
                    if (Path.GetFileName(filePath).ToLowerInvariant().Contains(query)) match = true;
                    // match tags
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

        // small helper type + prompt for uploader info
        private class UploaderInfo { public string Name; public string GradeSection; }

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

        // prompt user for Location and Date when adding an image (uses uploader info for display)
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
                Label lblUploaderVal = new Label { Text = !string.IsNullOrWhiteSpace(uploader?.Name) ? uploader.Name : Environment.UserName, Top = 12, Left = 110, AutoSize = true, Font = new Font("Arial", 9, FontStyle.Bold) };

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
            // 1) try program-level CurrentUser (if your app sets it)
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
                            var gradeProp = cur.GetType().GetProperty("GradeSection") ?? cur.GetType().GetProperty("grade_section");
                            string name = nameProp?.GetValue(cur)?.ToString();
                            string grade = gradeProp?.GetValue(cur)?.ToString();
                            return new UploaderInfo { Name = name ?? Environment.UserName, GradeSection = string.IsNullOrWhiteSpace(grade) ? "N/A" : grade };
                        }
                    }
                }
            }
            catch { /* ignore reflection failures */ }

            // 2) try current_user.txt that contains the logged-in email
            try
            {
                var cuPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "current_user.txt");
                if (File.Exists(cuPath))
                {
                    var email = File.ReadAllText(cuPath).Trim();
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        var usersPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
                        if (File.Exists(usersPath))
                        {
                            var json = File.ReadAllText(usersPath);
                            using (var doc = JsonDocument.Parse(json))
                            {
                                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                                {
                                    foreach (var el in doc.RootElement.EnumerateArray())
                                    {
                                        if (el.TryGetProperty("email", out var em) && string.Equals(em.GetString(), email, StringComparison.OrdinalIgnoreCase))
                                        {
                                            string name = null;
                                            if (el.TryGetProperty("name", out var n)) name = n.GetString();
                                            else if (el.TryGetProperty("username", out var un)) name = un.GetString();

                                            string grade = null;
                                            if (el.TryGetProperty("grade_section", out var g)) grade = g.GetString();
                                            else if (el.TryGetProperty("gradeSection", out var g2)) grade = g2.GetString();
                                            else if (el.TryGetProperty("grade", out var g3)) grade = g3.GetString();

                                            return new UploaderInfo
                                            {
                                                Name = !string.IsNullOrWhiteSpace(name) ? name : Environment.UserName,
                                                GradeSection = !string.IsNullOrWhiteSpace(grade) ? grade : "N/A"
                                            };
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { /* ignore json/read errors */ }

            // fallback
            return new UploaderInfo { Name = Environment.UserName, GradeSection = "N/A" };
        }
    }
}
