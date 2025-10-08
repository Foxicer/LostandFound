using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ReturnPoint
{
    public class FormGallery : Form
    {
        private Panel outerPanel;
        private FlowLayoutPanel galleryPanel;
        private Button openCameraButton;
        private string saveFolder;

        public FormGallery()
        {
            this.Text = "Gallery";
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.SeaGreen; // 🌿 Main background green

            saveFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CapturedImages");
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            // Outer panel (scroll area)
            outerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.SeaGreen // same as form background
            };

            // Inner panel (center gallery)
            galleryPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                BackColor = Color.SeaGreen
            };

            outerPanel.Controls.Add(galleryPanel);
            outerPanel.Resize += (s, e) =>
            {
                galleryPanel.Left = (outerPanel.ClientSize.Width - galleryPanel.Width) / 2;
            };

            // Floating "+" button
            openCameraButton = new Button
            {
                Text = "+",
                Width = 60,
                Height = 60,
                BackColor = Color.Aqua, // ✅ aqua by default
                ForeColor = Color.White,
                Font = new Font("Arial", 18, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            openCameraButton.FlatAppearance.BorderSize = 0;

            // hover effect → lighter aqua
            openCameraButton.MouseEnter += (s, e) =>
            {
                openCameraButton.BackColor = Color.MediumAquamarine;
            };
            openCameraButton.MouseLeave += (s, e) =>
            {
                openCameraButton.BackColor = Color.Aqua;
            };

            openCameraButton.Click += OpenCameraButton_Click;

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

            LoadSavedImages();
        }

        private void OpenCameraButton_Click(object sender, EventArgs e)
        {
            FormCamera camForm = new FormCamera(saveFolder);
            camForm.PhotoSaved += (filePath) =>
            {
                AddImageToGallery(filePath, false);
            };
            camForm.ShowDialog();
        }

        private void LoadSavedImages()
        {
            galleryPanel.Controls.Clear();

            string[] files = Directory.GetFiles(saveFolder, "*.jpg");

            foreach (string file in files)
            {
                bool claimed = File.Exists(Path.Combine(Path.GetDirectoryName(file),
                    Path.GetFileNameWithoutExtension(file) + "_claim.txt"));

                AddImageToGallery(file, claimed);
            }
        }

        private void AddImageToGallery(string filePath, bool alreadyClaimed = false)
        {
            if (!File.Exists(filePath)) return;

            Image img = Image.FromFile(filePath);

            int displayWidth = 320;
            int displayHeight = (int)((double)img.Height / img.Width * displayWidth);

            Panel card = new Panel
            {
                Width = displayWidth + 20,
                Height = displayHeight + 60,
                BackColor = this.BackColor,
                Margin = new Padding(10),
                Padding = new Padding(0),
                BorderStyle = BorderStyle.None
            };

            PictureBox pic = new PictureBox
            {
                Image = img,
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = displayWidth,
                Height = displayHeight,
                Cursor = Cursors.Hand
            };

            // Extract the info-display logic
            void ShowItemInfo()
            {
                string infoPath = Path.Combine(Path.GetDirectoryName(filePath),
                    Path.GetFileNameWithoutExtension(filePath) + "_info.txt");

                if (File.Exists(infoPath))
                {
                    string[] lines = File.ReadAllLines(infoPath);

                    Form infoForm = new Form
                    {
                        Text = "Item Information",
                        StartPosition = FormStartPosition.CenterParent,
                        Size = new Size(400, 250)
                    };

                    Label locationLabel = new Label
                    {
                        Text = lines.Length > 1 ? lines[1] : "Location: N/A",
                        Top = 20,
                        Left = 20,
                        AutoSize = true,
                        Font = new Font("Arial", 12, FontStyle.Bold)
                    };

                    Label dateLabel = new Label
                    {
                        Text = lines.Length > 2 ? lines[2] : "Date Found: N/A",
                        Top = 60,
                        Left = 20,
                        AutoSize = true,
                        Font = new Font("Arial", 12, FontStyle.Bold)
                    };

                    infoForm.Controls.Add(locationLabel);
                    infoForm.Controls.Add(dateLabel);
                    infoForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("No information available for this item.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
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

            Button claimBtn = new Button
            {
                Text = alreadyClaimed ? "Claimed" : "Claim",
                Top = displayHeight + 5,
                Width = displayWidth / 2 - 5,
                Height = 40,
                BackColor = Color.Aqua,
                ForeColor = Color.White,
                Enabled = !alreadyClaimed
            };

            Button infoBtn = new Button
            {
                Text = "Info",
                Top = displayHeight + 5,
                Left = displayWidth / 2 + 5,
                Width = displayWidth / 2 - 5,
                Height = 40,
                BackColor = Color.LightGray,
                ForeColor = Color.Black
            };

            infoBtn.Click += (s, e) => ShowItemInfo();

            claimBtn.Click += (s, e) =>
            {
                if (!alreadyClaimed)
                {
                    using (ClaimantInfoForm infoForm = new ClaimantInfoForm())
                    {
                        if (infoForm.ShowDialog() == DialogResult.OK)
                        {
                            string claimInfoPath = Path.Combine(Path.GetDirectoryName(filePath),
                                Path.GetFileNameWithoutExtension(filePath) + "_claim.txt");

                            File.WriteAllText(claimInfoPath,
                                $"Name: {infoForm.ClaimantName}\nContact: {infoForm.Contact}\n" +
                                $"Role: {infoForm.Role}\nGrade/Section: {infoForm.GradeSection}\nWhen Found: {infoForm.WhenFound}");

                            // Open claim camera and save claimant photo in same folder
                            FormClaimCamera claimCam = new FormClaimCamera(Path.GetDirectoryName(filePath));
                            claimCam.FaceCaptured += (photoPath) =>
                            {
                                claimBtn.Text = "Claimed";
                                claimBtn.Enabled = false;

                                MessageBox.Show($"Claimant photo saved as:\n{photoPath}", 
                                    "Photo Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            };
                            claimCam.ShowDialog();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("This item has already been claimed.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };


            card.Controls.Add(pic);
            card.Controls.Add(claimBtn);
            card.Controls.Add(infoBtn);
            galleryPanel.Controls.Add(card);

            galleryPanel.Left = (outerPanel.ClientSize.Width - galleryPanel.Width) / 2;
        }



    }
}
