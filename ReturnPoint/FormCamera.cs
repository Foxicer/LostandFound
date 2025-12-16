using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ReturnPoint
{
    public class FormCamera : Form
    {
        private PictureBox livePreview;
        private Label countdownLabel;
        private VideoCaptureDevice? videoSource;
        private FilterInfoCollection? videoDevices;
        private Bitmap? lastFrame;
        private string saveFolder;
        private System.Windows.Forms.Timer? countdownTimer;
        private int countdownValue;

        public delegate void PhotoSavedHandler(string filePath);
        public event PhotoSavedHandler? PhotoSaved;

        public FormCamera(string folderPath)
        {
            this.Text = "Camera";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(800, 800);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            // ensure saveFolder is initialized from the provided argument (or default)
            saveFolder = string.IsNullOrWhiteSpace(folderPath)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CapturedImages")
                : folderPath;

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            livePreview = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            countdownLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 28, FontStyle.Bold),
                ForeColor = Color.Red
            };

            this.Controls.Add(livePreview);
            this.Controls.Add(countdownLabel);

            this.Load += FormCamera_Load;
            this.FormClosing += FormCamera_FormClosing;
        }

        private void FormCamera_Load(object? sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No camera found!");
                this.Close();
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);
            videoSource.NewFrame += Video_NewFrame;
            videoSource.Start();

            countdownValue = 5;
            countdownLabel.Text = countdownValue.ToString();

            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            countdownValue--;
            if (countdownValue > 0)
            {
                countdownLabel.Text = countdownValue.ToString();
            }
            else
            {
                countdownTimer?.Stop();
                countdownLabel.Text = "";

                if (videoSource != null && videoSource.IsRunning)
                {
                    videoSource.NewFrame -= Video_NewFrame;
                }

                CapturePhoto();
            }
        }

        private void Video_NewFrame(object? sender, NewFrameEventArgs eventArgs)
        {
            Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
            frame.RotateFlip(RotateFlipType.RotateNoneFlipX);

            lastFrame?.Dispose();
            lastFrame = (Bitmap)frame.Clone();

            livePreview.Image?.Dispose();
            livePreview.Image = frame;
        }

        private void CapturePhoto()
        {
            if (lastFrame == null) return;

            // pause live feed (NewFrame was already detached in the timer tick)
            // create a cloned preview for dialog (so UI thread owns it)
            Bitmap preview = (Bitmap)lastFrame.Clone();

            // show preview dialog: Save or Retake
            bool savePhoto = false;
            using (Form previewForm = new Form())
            {
                previewForm.Text = "Is this photo correct?";
                previewForm.StartPosition = FormStartPosition.CenterParent;
                previewForm.Size = new Size(600, 800);
                previewForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                previewForm.MaximizeBox = false;
                previewForm.MinimizeBox = false;

                PictureBox pic = new PictureBox
                {
                    Image = preview,
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Dock = DockStyle.Top,
                    Height = 680
                };

                Button btnSave = new Button { Text = "Save", Width = 120, Height = 36, Left = 260, Top = 690, DialogResult = DialogResult.OK };
                Button btnRetake = new Button { Text = "Retake", Width = 120, Height = 36, Left = 400, Top = 690, DialogResult = DialogResult.Retry };

                previewForm.Controls.Add(pic);
                previewForm.Controls.Add(btnSave);
                previewForm.Controls.Add(btnRetake);
                previewForm.AcceptButton = btnSave;
                previewForm.CancelButton = btnRetake; // Escape acts like Retake

                var dr = previewForm.ShowDialog(this);
                savePhoto = dr == DialogResult.OK;
            }

            if (!savePhoto)
            {
                // user chose retake: reattach frame handler and restart countdown for another capture
                if (videoSource != null && !videoSource.IsRunning)
                {
                    // video source may still be running; ensure NewFrame handler attached
                    videoSource.NewFrame -= Video_NewFrame;
                    videoSource.NewFrame += Video_NewFrame;
                }
                // reset countdown to allow re-capture
                countdownValue = 5;
                countdownLabel.Text = countdownValue.ToString();
                if (countdownTimer == null)
                {
                    countdownTimer = new System.Windows.Forms.Timer();
                    countdownTimer.Interval = 1000;
                    countdownTimer.Tick += CountdownTimer_Tick;
                }
                countdownTimer?.Start();
                preview.Dispose();
                return;
            }

            // proceed to save the confirmed photo
            string fileName = $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            string folder = saveFolder;
            string filePath = Path.Combine(folder, fileName);

            try
            {
                // save the captured frame
                SavePortraitPhoto(preview, filePath);

                // raise event for subscribers
                PhotoSaved?.Invoke(filePath);

                MessageBox.Show($"Photo saved:\n{filePath}", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save photo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                preview.Dispose();
            }
        }

        private void SavePortraitPhoto(Bitmap original, string filePath)
        {
            int targetWidth = 1080;
            int targetHeight = 1920;
            double targetRatio = (double)targetWidth / targetHeight;
            double originalRatio = (double)original.Width / original.Height;

            Rectangle cropRect;
            if (originalRatio > targetRatio)
            {
                int newWidth = (int)(original.Height * targetRatio);
                int x = (original.Width - newWidth) / 2;
                cropRect = new Rectangle(x, 0, newWidth, original.Height);
            }
            else
            {
                int newHeight = (int)(original.Width / targetRatio);
                int y = (original.Height - newHeight) / 2;
                cropRect = new Rectangle(0, y, original.Width, newHeight);
            }

            using (Bitmap cropped = original.Clone(cropRect, original.PixelFormat))
            using (Bitmap resized = new Bitmap(cropped, new Size(targetWidth, targetHeight)))
            {
                resized.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
            }

            original.Dispose();
        }

        private void FormCamera_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }

            lastFrame?.Dispose();
        }

        // Constructor overload for default save folder
        public FormCamera()
        {
            this.Text = "Camera";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(800, 800);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            saveFolder = Path.Combine(Application.StartupPath, "CapturedImages");
            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            livePreview = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            countdownLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 28, FontStyle.Bold),
                ForeColor = Color.Red
            };

            this.Controls.Add(livePreview);
            this.Controls.Add(countdownLabel);

            this.Load += FormCamera_Load;
            this.FormClosing += FormCamera_FormClosing;
        }
    }
}
