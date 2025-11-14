using System;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private Button btnLogout;

        private string saveFolder;
        private string deletedFolder;
        private Panel selectedCard;

        public FormGalleryAdmin()
        {
            Text = "Gallery - Admin";
            WindowState = FormWindowState.Maximized;
            BackColor = Color.SeaGreen;

            saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CapturedImages");
            deletedFolder = Path.Combine(saveFolder, "Deleted");
            Directory.CreateDirectory(saveFolder);
            Directory.CreateDirectory(deletedFolder);

            // outer scrollable area
            outerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.SeaGreen
            };

            galleryPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                BackColor = Color.SeaGreen,
                Padding = new Padding(20, 10, 20, 10),
                Location = new Point(10, 10)
            };

            // limit width so 3 columns are enforced (cards use 220 width + margins)
            int cardWidth = 220;
            int cardHorizontalMargin = 20; // card.Margin.Left+Right
            int columns = 3;
            int totalColumnWidth = columns * (cardWidth + cardHorizontalMargin);
            galleryPanel.MaximumSize = new Size(totalColumnWidth + galleryPanel.Padding.Left + galleryPanel.Padding.Right, 0);

            outerPanel.Controls.Add(galleryPanel);

            // right admin control panel (stationary)
            rightPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 320,
                BackColor = Color.FromArgb(230, 240, 240),
                Padding = new Padding(10)
            };

            Label lblMode = new Label { Text = "View", AutoSize = true, Top = 6, Left = 10, Font = new Font("Arial", 10, FontStyle.Bold) };
            cbViewMode = new ComboBox { Top = 30, Left = 10, Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            cbViewMode.Items.AddRange(new[] { "Active", "Deleted" });
            cbViewMode.SelectedIndex = 0;

            lblSelectedFile = new Label { Text = "Selected: (none)", AutoSize = false, Top = 70, Left = 10, Width = 280, Height = 40, BorderStyle = BorderStyle.FixedSingle };
            lblDateAdded = new Label { Text = "Date Added: N/A", AutoSize = true, Top = 120, Left = 10 };

            btnDelete = new Button { Text = "Move to Deleted", Top = 150, Left = 10, Width = 140 };
            btnRestore = new Button { Text = "Restore", Top = 150, Left = 160, Width = 130, Enabled = false };
            btnPermanentlyDelete = new Button { Text = "Delete Permanently", Top = 190, Left = 10, Width = 280, BackColor = Color.IndianRed, ForeColor = Color.White };
            btnRefresh = new Button { Text = "Refresh", Top = 230, Left = 10, Width = 280 };

            rightPanel.Controls.Add(lblMode);
            rightPanel.Controls.Add(cbViewMode);
            rightPanel.Controls.Add(lblSelectedFile);
            rightPanel.Controls.Add(lblDateAdded);
            rightPanel.Controls.Add(btnDelete);
            rightPanel.Controls.Add(btnRestore);
            rightPanel.Controls.Add(btnPermanentlyDelete);
            rightPanel.Controls.Add(btnRefresh);

            Controls.Add(rightPanel);
            Controls.Add(outerPanel);

            // logout button placed inside rightPanel so it is always visible and themed with the panel
            btnLogout = new Button
            {
                Text = "Logout",
                Width = 100,
                Height = 34,
                Top = 900,
                Left = Math.Max(10, rightPanel.Width - 110),
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
            // add into rightPanel so docking doesn't obscure it
            rightPanel.Controls.Add(btnLogout);
            // reposition when rightPanel is resized
            rightPanel.SizeChanged += (s, e) =>
            {
                btnLogout.Left = Math.Max(10, rightPanel.ClientSize.Width - btnLogout.Width - 10);
            };

            cbViewMode.SelectedIndexChanged += (s, e) => LoadImages(cbViewMode.SelectedIndex == 1);
            btnRefresh.Click += (s, e) => LoadImages(cbViewMode.SelectedIndex == 1);
            btnDelete.Click += (s, e) => DeleteSelected();
            btnRestore.Click += (s, e) => RestoreSelected();
            btnPermanentlyDelete.Click += (s, e) => PermanentlyDeleteSelected();

            // apply app theme to admin gallery
            Theme.Apply(this);
            // ensure logout stands out (override if necessary) and visible on top
            btnLogout.BackColor = OuterRingIsAvailable();
            btnLogout.ForeColor = Theme.SoftWhite;
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.BringToFront();

            Color OuterRingIsAvailable()
            {
                // prefer OuterRing but fall back to StrongAqua
                return Theme.OuterRing;
            }

            LoadImages(false);
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
                BackColor = Color.LightGray,
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

                // move related files: _tags.txt, _info.txt, _claim.txt, Claimants folder (if exists)
                MoveIfExists(Path.ChangeExtension(filePath, null) + "_tags.txt", Path.Combine(deletedFolder, Path.GetFileNameWithoutExtension(filePath) + "_tags.txt"));
                MoveIfExists(Path.ChangeExtension(filePath, null) + "_info.txt", Path.Combine(deletedFolder, Path.GetFileNameWithoutExtension(filePath) + "_info.txt"));
                MoveIfExists(Path.Combine(Path.GetDirectoryName(filePath), Path.GetFileNameWithoutExtension(filePath) + "_claim.txt"),
                             Path.Combine(deletedFolder, Path.GetFileNameWithoutExtension(filePath) + "_claim.txt"));

                string claimantFolder = Path.Combine(Path.GetDirectoryName(filePath), "Claimants");
                if (Directory.Exists(claimantFolder))
                {
                    var claimantDestFolder = Path.Combine(deletedFolder, Path.GetFileNameWithoutExtension(filePath) + "_Claimants");
                    Directory.CreateDirectory(claimantDestFolder);
                    foreach (var cf in Directory.GetFiles(claimantFolder, $"claimant_{Path.GetFileNameWithoutExtension(filePath)}*"))
                        MoveIfExists(cf, Path.Combine(claimantDestFolder, Path.GetFileName(cf)));
                }

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

                // restore related files if present in deleted folder
                string baseName = Path.GetFileNameWithoutExtension(filePath);
                MoveIfExists(Path.Combine(deletedFolder, baseName + "_tags.txt"), Path.Combine(saveFolder, baseName + "_tags.txt"));
                MoveIfExists(Path.Combine(deletedFolder, baseName + "_info.txt"), Path.Combine(saveFolder, baseName + "_info.txt"));
                MoveIfExists(Path.Combine(deletedFolder, baseName + "_claim.txt"), Path.Combine(saveFolder, baseName + "_claim.txt"));

                // claimant subfolder variant
                string claimantDeleted = Path.Combine(deletedFolder, baseName + "_Claimants");
                if (Directory.Exists(claimantDeleted))
                {
                    var claimantRestore = Path.Combine(Path.Combine(saveFolder, "Claimants"));
                    Directory.CreateDirectory(claimantRestore);
                    foreach (var f in Directory.GetFiles(claimantDeleted))
                    {
                        MoveIfExists(f, Path.Combine(claimantRestore, Path.GetFileName(f)));
                    }
                    Directory.Delete(claimantDeleted, true);
                }

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
                    Path.Combine(Path.GetDirectoryName(filePath), baseName + "_info.txt"),
                    Path.Combine(Path.GetDirectoryName(filePath), baseName + "_claim.txt")
                };
                foreach (var r in related) if (File.Exists(r)) File.Delete(r);

                // if there is a claimant folder variant in deleted folder remove it too
                var claimantFolder = Path.Combine(Path.GetDirectoryName(filePath), baseName + "_Claimants");
                if (Directory.Exists(claimantFolder)) Directory.Delete(claimantFolder, true);

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
            catch { /* ignore individual failures */ }
        }
    }
}