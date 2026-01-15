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
        private string? selectedDeviceMoniker;

        public delegate void PhotoSavedHandler(string filePath);
        public event PhotoSavedHandler? PhotoSaved;

        public FormCamera(string folderPath, string? deviceMoniker = null)
        {
            selectedDeviceMoniker = deviceMoniker;
            this.Text = "Capture Photo - ReturnPoint";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(900, 700);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Theme.GetBackgroundTeal();

            saveFolder = string.IsNullOrWhiteSpace(folderPath)
                ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CapturedImages")
                : folderPath;

            if (!Directory.Exists(saveFolder))
                Directory.CreateDirectory(saveFolder);

            livePreview = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Theme.NearBlack
            };

            countdownLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 36, FontStyle.Bold),
                ForeColor = Theme.DeepRed,
                BackColor = Theme.GetBackgroundTeal(),
                Text = "Initializing camera..."
            };

            this.Controls.Add(livePreview);
            this.Controls.Add(countdownLabel);

            this.Load += FormCamera_Load;
            this.FormClosing += FormCamera_FormClosing;
        }

        private void FormCamera_Load(object? sender, EventArgs e)
        {
            try
            {
                videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                
                string deviceInfo = $"Found {videoDevices.Count} camera device(s):\n";
                for (int i = 0; i < videoDevices.Count; i++)
                {
                    deviceInfo += $"  {i + 1}. {videoDevices[i].Name}\n";
                }
                System.Diagnostics.Debug.WriteLine(deviceInfo);
                
                if (videoDevices.Count == 0)
                {
                    MessageBox.Show(
                        "No camera device found!\n\n" +
                        "Please check:\n" +
                        "• Camera is connected to the computer\n" +
                        "• Camera drivers are installed\n" +
                        "• This app has camera permissions in Windows settings\n\n" +
                        deviceInfo,
                        "Camera Error", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                int deviceIndex = 0;
                if (!string.IsNullOrEmpty(selectedDeviceMoniker))
                {
                    for (int i = 0; i < videoDevices.Count; i++)
                    {
                        if (videoDevices[i].MonikerString == selectedDeviceMoniker)
                        {
                            deviceIndex = i;
                            break;
                        }
                    }
                }

                videoSource = new VideoCaptureDevice(videoDevices[deviceIndex].MonikerString);
                videoSource.NewFrame += Video_NewFrame;
                
                try
                {
                    videoSource.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Failed to start camera: {ex.Message}\n\n" +
                        $"Device: {videoDevices[deviceIndex].Name}\n\n" +
                        "Please check:\n" +
                        "• Camera is not in use by another application\n" +
                        "• Camera permissions are granted in Windows settings\n" +
                        "• Camera drivers are properly installed",
                        "Camera Start Error", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Error);
                    this.Close();
                    return;
                }

                countdownValue = 5;
                countdownLabel.Text = countdownValue.ToString();

                countdownTimer = new System.Windows.Forms.Timer();
                countdownTimer.Interval = 1000;
                countdownTimer.Tick += CountdownTimer_Tick;
                countdownTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error initializing camera: {ex.Message}\n\n{ex.StackTrace}", 
                    "Initialization Error", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);
                this.Close();
            }
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

                System.Threading.Thread.Sleep(100);
                CapturePhoto();
            }
        }

        private void Video_NewFrame(object? sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                if (eventArgs?.Frame == null)
                {
                    System.Diagnostics.Debug.WriteLine("Warning: eventArgs.Frame is null");
                    return;
                }

                Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
                if (frame == null)
                {
                    System.Diagnostics.Debug.WriteLine("Warning: frame clone is null");
                    return;
                }

                frame.RotateFlip(RotateFlipType.RotateNoneFlipX);

                lock (this)
                {
                    lastFrame?.Dispose();
                    lastFrame = (Bitmap)frame.Clone();
                }

                if (livePreview?.InvokeRequired == true)
                {
                    livePreview.Invoke(new Action(() =>
                    {
                        try
                        {
                            livePreview.Image?.Dispose();
                            livePreview.Image = frame;
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error updating UI with frame: {ex.Message}");
                        }
                    }));
                }
                else if (livePreview != null)
                {
                    try
                    {
                        livePreview.Image?.Dispose();
                        livePreview.Image = frame;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error setting frame directly: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Video_NewFrame: {ex.Message}");
            }
        }

        private void CapturePhoto()
        {
            Bitmap? preview = null;
            lock (this)
            {
                if (lastFrame == null) return;
                try
                {
                    preview = (Bitmap)lastFrame.Clone();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error cloning lastFrame: {ex.Message}");
                    MessageBox.Show($"Error capturing photo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            
            if (preview == null) return;
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
                previewForm.CancelButton = btnRetake; 

                var dr = previewForm.ShowDialog(this);
                savePhoto = dr == DialogResult.OK;
            }

            if (!savePhoto)
            {
                if (videoSource != null && !videoSource.IsRunning)
                {
                    videoSource.NewFrame -= Video_NewFrame;
                    videoSource.NewFrame += Video_NewFrame;
                }
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

            string fileName = $"photo_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            string folder = saveFolder;
            string filePath = Path.Combine(folder, fileName);

            try
            {
                SavePortraitPhoto(preview, filePath);
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
                ForeColor = Theme.DeepRed
            };

            this.Controls.Add(livePreview);
            this.Controls.Add(countdownLabel);

            this.Load += FormCamera_Load;
            this.FormClosing += FormCamera_FormClosing;
        }
    }
}

