using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ReturnPoint
{
    public class FormGalleryAdmin : Form
    {
        private Panel outerPanel;
        private FlowLayoutPanel galleryPanel;
        private Panel rightPanel;
        private ComboBox cbViewMode;
        private Label lblSelectedFile;
        private Label lblDateAdded;
        private Button btnDelete;
        private Button btnRestore;
        private Button btnPermanentlyDelete;
        private Button btnRefresh;
        private Button btnImageDetails;
        private Button btnTagManager;
        private Button btnLogout;
        private string saveFolder;
        private string deletedFolder;
        private Panel selectedCard;
        private PictureBox? logoPictureBox;
        private Bitmap? backgroundBitmap;
        public FormGalleryAdmin()
        {
            Text = "Gallery Admin - ReturnPoint";
            WindowState = FormWindowState.Maximized;
            BackColor = Theme.GetBackgroundTeal();
            this.BackgroundImage = Theme.CreateGradientBitmap(1920, 1080, vertical: true);
            this.BackgroundImageLayout = ImageLayout.Stretch;
            saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CapturedImages");
            deletedFolder = Path.Combine(saveFolder, "Deleted");
            Directory.CreateDirectory(saveFolder);
            Directory.CreateDirectory(deletedFolder);
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
                BackColor = Theme.GetBackgroundTeal(),
                Padding = new Padding(30, 20, 30, 20),
                Location = new Point(0, 0)
            };
            int cardWidth = 220;
            int cardHorizontalMargin = 30;
            int columns = 4;
            int totalColumnWidth = columns * (cardWidth + cardHorizontalMargin);
            galleryPanel.MaximumSize = new Size(totalColumnWidth + galleryPanel.Padding.Left + galleryPanel.Padding.Right, 0);
            outerPanel.Controls.Add(galleryPanel);
            rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 320,
                BackColor = Color.White,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(15),
                AutoScroll = true
            };
            Label lblSearchTitle = new Label
            {
                Text = "🔍 Search",
                AutoSize = true,
                Top = 15,
                Left = 15,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            TextBox txtSearch = new TextBox
            {
                Top = 45,
                Left = 15,
                Width = 270,
                Height = 35,
                Font = new Font("Segoe UI", 11),
                BackColor = Theme.SoftWhite,
                ForeColor = Theme.NearBlack
            };
            Label lblSelected = new Label
            {
                Text = "Selected File",
                AutoSize = true,
                Top = 95,
                Left = 15,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Theme.NearBlack
            };
            lblSelectedFile = new Label
            {
                Text = "(none)",
                AutoSize = false,
                Top = 125,
                Left = 15,
                Width = 270,
                Height = 60,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Theme.GetBackgroundTeal(),
                ForeColor = Theme.NearBlack,
                Font = new Font("Segoe UI", 10),
                Padding = new Padding(8)
            };
            lblDateAdded = new Label
            {
                Text = "📅 Added: N/A",
                AutoSize = false,
                Top = 195,
                Left = 15,
                Width = 270,
                Height = 50,
                Font = new Font("Segoe UI", 9),
                ForeColor = Theme.NearBlack,
                BackColor = Color.Transparent
            };
            btnDelete = new Button
            {
                Text = "🗑️ Delete",
                Top = 260,
                Left = 15,
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
                Top = 260,
                Left = 155,
                Width = 130,
                Height = 40,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Theme.Success,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Enabled = false
            };
            btnRestore.FlatAppearance.BorderSize = 0;
            btnPermanentlyDelete = new Button
            {
                Text = "❌ Delete Permanently",
                Top = 310,
                Left = 15,
                Width = 270,
                Height = 40,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                BackColor = Theme.DeepRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPermanentlyDelete.FlatAppearance.BorderSize = 0;
            btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Top = 360,
                Left = 15,
                Width = 270,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Theme.AccentBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnImageDetails = new Button
            {
                Text = "📋 Image Details",
                Top = 410,
                Left = 15,
                Width = 270,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Theme.MediumTeal,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnImageDetails.FlatAppearance.BorderSize = 0;
            btnTagManager = new Button
            {
                Text = "🏷️ Manage Tags",
                Top = 460,
                Left = 15,
                Width = 270,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Theme.TealGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnTagManager.FlatAppearance.BorderSize = 0;
            cbViewMode = new ComboBox
            {
                Top = 510,
                Left = 15,
                Width = 270,
                Height = 36,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10),
                BackColor = Theme.GetBackgroundTeal()
            };
            cbViewMode.Items.AddRange(new[] { "Active Items", "Deleted Items" });
            cbViewMode.SelectedIndex = 0;
            btnLogout = new Button
            {
                Text = "🚪 Logout",
                Top = 560,
                Left = 15,
                Width = 270,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Theme.DeepRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
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
            rightPanel.Controls.Add(lblSearchTitle);
            rightPanel.Controls.Add(txtSearch);
            rightPanel.Controls.Add(lblSelected);
            rightPanel.Controls.Add(lblSelectedFile);
            rightPanel.Controls.Add(lblDateAdded);
            rightPanel.Controls.Add(btnDelete);
            rightPanel.Controls.Add(btnRestore);
            rightPanel.Controls.Add(btnPermanentlyDelete);
            rightPanel.Controls.Add(btnRefresh);
            rightPanel.Controls.Add(btnImageDetails);
            rightPanel.Controls.Add(btnTagManager);
            rightPanel.Controls.Add(cbViewMode);
            rightPanel.Controls.Add(btnLogout);
            Controls.Add(rightPanel);
            Controls.Add(outerPanel);
            cbViewMode.SelectedIndexChanged += (s, e) => LoadImages(cbViewMode.SelectedIndex == 1);
            btnRefresh.Click += (s, e) => LoadImages(cbViewMode.SelectedIndex == 1);
            btnDelete.Click += (s, e) => DeleteSelected();
            btnRestore.Click += (s, e) => RestoreSelected();
            btnPermanentlyDelete.Click += (s, e) => PermanentlyDeleteSelected();
            btnImageDetails.Click += (s, e) => OpenImageDetailsForm();
            btnTagManager.Click += (s, e) => OpenTagManager();
            LoadImages(false);
            AddLogoCopyright();
            SetLogoTransparentBackground();
        }
        private void LoadImages(bool showDeleted)
        {
            galleryPanel.Controls.Clear();
            selectedCard = null;
            lblSelectedFile.Text = "Selected: (none)";
            lblDateAdded.Text = "Date Added: N/A";
            btnRestore.Enabled = false;
            btnDelete.Enabled = true;
            btnPermanentlyDelete.Enabled = false;
            string folder = showDeleted ? deletedFolder : saveFolder;
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string[] files = Directory.GetFiles(folder, "*.jpg").OrderByDescending(f => File.GetCreationTime(f)).ToArray();
            foreach (var f in files) AddImageToGalleryAdmin(f, showDeleted);
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
            galleryPanel.Controls.Add(card);
        }
        private void SelectAdminCard(Panel card)
        {
            if (selectedCard != null) selectedCard.BorderStyle = BorderStyle.None;
            selectedCard = card;
            selectedCard.BorderStyle = BorderStyle.FixedSingle;
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
                DeleteImageFromGoogleSheets(Path.GetFileName(filePath));
                LoadImages(cbViewMode.SelectedIndex == 1);
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
                LoadImages(cbViewMode.SelectedIndex == 1);
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
                LoadImages(cbViewMode.SelectedIndex == 1);
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
            Form tagForm = new Form
            {
                Text = "Tag Manager - Create and Manage Tags",
                Width = 600,
                Height = 500,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Theme.LightGray,
                Font = new Font("Segoe UI", 11),
                Owner = this
            };
            Label lblAvailableTags = new Label
            {
                Text = "Available Tags:",
                Top = 10,
                Left = 10,
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            ListBox lbTags = new ListBox
            {
                Top = 35,
                Left = 10,
                Width = 570,
                Height = 200,
                BackColor = Theme.SoftWhite,
                ForeColor = Theme.NearBlack
            };
            Label lblNewTag = new Label
            {
                Text = "Create New Tag:",
                Top = 245,
                Left = 10,
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            TextBox txtNewTag = new TextBox
            {
                Top = 270,
                Left = 10,
                Width = 400,
                Height = 30,
                Font = new Font("Segoe UI", 11),
                BackColor = Theme.SoftWhite,
                ForeColor = Theme.NearBlack
            };
            Button btnCreateTag = new Button
            {
                Text = "➕ Create Tag",
                Top = 270,
                Left = 420,
                Width = 160,
                Height = 30,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Theme.MediumTeal,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCreateTag.FlatAppearance.BorderSize = 0;
            Button btnDeleteTag = new Button
            {
                Text = "🗑️ Delete Selected",
                Top = 310,
                Left = 10,
                Width = 160,
                Height = 30,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Theme.DeepRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnDeleteTag.FlatAppearance.BorderSize = 0;
            Button btnAssignTag = new Button
            {
                Text = "🏷️ Assign to Selected Image",
                Top = 310,
                Left = 180,
                Width = 220,
                Height = 30,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Theme.AccentBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAssignTag.FlatAppearance.BorderSize = 0;
            Label lblInfo = new Label
            {
                Text = "The first two tags on an image will display as the Item name",
                Top = 350,
                Left = 10,
                Width = 560,
                Height = 40,
                AutoSize = false,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Theme.MediumTeal,
                BackColor = Color.Transparent
            };
            Button btnClose = new Button
            {
                Text = "Close",
                Top = 400,
                Left = 450,
                Width = 120,
                Height = 40,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Theme.MediumTeal,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => tagForm.Close();
            LoadAllTagsIntoListBox(lbTags);
            btnCreateTag.Click += (s, e) =>
            {
                string tagName = txtNewTag.Text.Trim();
                if (!string.IsNullOrWhiteSpace(tagName))
                {
                    string tagsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tags.txt");
                    var tags = File.Exists(tagsFile) ? File.ReadAllLines(tagsFile).ToList() : new List<string>();
                    if (!tags.Contains(tagName))
                    {
                        tags.Add(tagName);
                        File.WriteAllLines(tagsFile, tags.OrderBy(t => t));
                        LoadAllTagsIntoListBox(lbTags);
                        txtNewTag.Clear();
                        MessageBox.Show($"Tag '{tagName}' created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Tag '{tagName}' already exists!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Please enter a tag name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            btnDeleteTag.Click += (s, e) =>
            {
                if (lbTags.SelectedItem != null)
                {
                    string selectedTag = lbTags.SelectedItem.ToString();
                    if (MessageBox.Show($"Delete tag '{selectedTag}'?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        string tagsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tags.txt");
                        var tags = File.Exists(tagsFile) ? File.ReadAllLines(tagsFile).ToList() : new List<string>();
                        tags.Remove(selectedTag);
                        File.WriteAllLines(tagsFile, tags);
                        LoadAllTagsIntoListBox(lbTags);
                    }
                }
                else
                {
                    MessageBox.Show("Please select a tag to delete.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            btnAssignTag.Click += (s, e) =>
            {
                if (selectedCard == null)
                {
                    MessageBox.Show("Please select an image first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (lbTags.SelectedItem == null)
                {
                    MessageBox.Show("Please select a tag to assign.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                string filePath = (string)selectedCard.Tag;
                string selectedTag = lbTags.SelectedItem.ToString();
                AddTagToImage(filePath, selectedTag);
                MessageBox.Show($"Tag '{selectedTag}' added to image!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            tagForm.Controls.Add(lblAvailableTags);
            tagForm.Controls.Add(lbTags);
            tagForm.Controls.Add(lblNewTag);
            tagForm.Controls.Add(txtNewTag);
            tagForm.Controls.Add(btnCreateTag);
            tagForm.Controls.Add(btnDeleteTag);
            tagForm.Controls.Add(btnAssignTag);
            tagForm.Controls.Add(lblInfo);
            tagForm.Controls.Add(btnClose);
            tagForm.ShowDialog(this);
        }
        private void LoadAllTagsIntoListBox(ListBox listBox)
        {
            listBox.Items.Clear();
            string tagsFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tags.txt");
            if (File.Exists(tagsFile))
            {
                string[] tags = File.ReadAllLines(tagsFile);
                foreach (var tag in tags.OrderBy(t => t))
                {
                    listBox.Items.Add(tag);
                }
            }
        }
        private void AddTagToImage(string filePath, string tagName)
        {
            try
            {
                string baseName = Path.GetFileNameWithoutExtension(filePath);
                string folderPath = Path.GetDirectoryName(filePath);
                string tagsPath = Path.Combine(folderPath, baseName + "_tags.txt");
                var tags = File.Exists(tagsPath) ? File.ReadAllLines(tagsPath).ToList() : new List<string>();
                if (!tags.Contains(tagName))
                {
                    tags.Add(tagName);
                    File.WriteAllLines(tagsPath, tags);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding tag: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                    System.Diagnostics.Debug.WriteLine($"[FormGalleryAdmin] Delete image from Sheets: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FormGalleryAdmin] Flask unavailable for delete: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FormGalleryAdmin] Error deleting image from Sheets: {ex.Message}");
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
                    System.Diagnostics.Debug.WriteLine($"[FormGalleryAdmin] Restore image in Sheets: {response.StatusCode}");
                }
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FormGalleryAdmin] Flask unavailable for restore: {ex.Message}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[FormGalleryAdmin] Error restoring image in Sheets: {ex.Message}");
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
