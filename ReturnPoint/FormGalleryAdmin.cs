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
    public class FormGalleryAdmin : Form
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
        private Button btnLogout;
        private FlowLayoutPanel pnlTags;
        private TextBox txtNewTag;
        private string saveFolder;
        private string deletedFolder;
        private Panel selectedCard;
        private PictureBox? logoPictureBox;
        private Bitmap? backgroundBitmap;
        private const int COLUMNS = 5;
        private const int IMAGE_SIZE = 220;
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
            txtSearch = new TextBox
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
            cbViewMode.SelectedIndexChanged += (s, e) => LoadImages(cbViewMode.SelectedIndex == 1, txtSearch.Text);
            btnRefresh.Click += (s, e) => LoadImages(cbViewMode.SelectedIndex == 1, txtSearch.Text);
            btnDelete.Click += (s, e) => DeleteSelected();
            btnRestore.Click += (s, e) => RestoreSelected();
            btnPermanentlyDelete.Click += (s, e) => PermanentlyDeleteSelected();
            btnImageDetails.Click += (s, e) => OpenImageDetailsForm();
            btnTagManager.Click += (s, e) => OpenTagManager();
            txtSearch.TextChanged += (s, e) => LoadImages(cbViewMode.SelectedIndex == 1, txtSearch.Text);
            LoadImages(false);
            AddLogoCopyright();
            SetLogoTransparentBackground();
        }
        private void LoadImages(bool showDeleted, string searchQuery = "")
        {
            galleryTable.Controls.Clear();
            galleryTable.RowCount = 0;
            selectedCard = null;
            lblSelectedFile.Text = "Selected: (none)";
            lblDateAdded.Text = "Date Added: N/A";
            btnRestore.Enabled = false;
            btnDelete.Enabled = true;
            btnPermanentlyDelete.Enabled = false;
            
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
                if (columnIndex == 0)
                {
                    galleryTable.RowCount++;
                }
                
                try
                {
                    string filePath = item.File;
                    Image img;
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    using (MemoryStream ms = new MemoryStream())
                    {
                        fs.CopyTo(ms);
                        img = Image.FromStream(ms);
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
                    
                    // 4:3 aspect ratio: width = 210 (220 - 10 padding), height = 157.5
                    int picWidth = IMAGE_SIZE - 10;
                    int picHeight = (int)(picWidth * 3 / 4.0); // 157px for 4:3 ratio
                    
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
                    card.Click += (s, e) => SelectAdminCard(card);
                    
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
