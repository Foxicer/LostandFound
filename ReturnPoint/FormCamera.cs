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

            saveFolder = folderPath;
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

            Bitmap capturedFrame = (Bitmap)lastFrame.Clone();

            this.Invoke((Action)(() =>
            {
                livePreview.Image?.Dispose();
                livePreview.Image = (Bitmap)capturedFrame.Clone();

                DialogResult result = MessageBox.Show("Is this photo correct?", "Confirm Photo",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string fileName = $"capture_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
                    string filePath = Path.Combine(saveFolder, fileName);

                    try
                    {
                        SavePortraitPhoto(capturedFrame, filePath);
                        PhotoSaved?.Invoke(filePath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error saving photo: {ex.Message}");
                    }

                    // Show input form after photo is saved
                    using (FormInput inputForm = new FormInput(filePath, saveFolder))
                    {
                        inputForm.ShowDialog();
                    }

                    this.Close();
                }
                else
                {
                    if (videoSource != null && !videoSource.IsRunning)
                    {
                        videoSource.Start();
                    }
                    if (videoSource != null) videoSource.NewFrame += Video_NewFrame;

                    countdownValue = 5;
                    countdownLabel.Text = countdownValue.ToString();
                    countdownTimer?.Start();
                }
            }));
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
