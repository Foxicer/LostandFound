using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AForge.Video;
using AForge.Video.DirectShow;

namespace ReturnPoint
{
    public class FormClaimCamera : Form
    {
        private PictureBox livePreview;
        private Label countdownLabel;
        private Label blinkLabel;
        private VideoCaptureDevice videoSource;
        private FilterInfoCollection videoDevices;
        private Bitmap lastFrame;
        private string saveFolder;
        private System.Windows.Forms.Timer countdownTimer;
        private System.Windows.Forms.Timer blinkTimer;
        private int countdownValue;

        public delegate void PhotoCapturedHandler(string filePath);
        public event PhotoCapturedHandler FaceCaptured;

        public FormClaimCamera(string baseFolder)
        {
            this.Text = "Capture Photo";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(800, 800);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            saveFolder = Path.Combine(baseFolder, "Claimants");
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

            // Blinking "PLEASE STAY STILL" label
            blinkLabel = new Label
            {
                Text = "PLEASE STAY STILL",
                AutoSize = false,
                Height = 40,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 20, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            this.Controls.Add(livePreview);
            this.Controls.Add(countdownLabel);
            this.Controls.Add(blinkLabel);

            this.Load += FormClaimCamera_Load;
            this.FormClosing += FormClaimCamera_FormClosing;
        }

        private void FormClaimCamera_Load(object sender, EventArgs e)
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

            // Start countdown (5 seconds)
            countdownValue = 4;
            countdownLabel.Text = countdownValue.ToString();

            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000;
            countdownTimer.Tick += CountdownTimer_Tick;
            countdownTimer.Start();

            // Blinking label timer
            blinkTimer = new System.Windows.Forms.Timer();
            blinkTimer.Interval = 2000; // blink every 0.5 sec
            blinkTimer.Tick += (s, ev) =>
            {
                blinkLabel.Visible = !blinkLabel.Visible;
            };
            blinkTimer.Start();
        }

        private void CountdownTimer_Tick(object sender, EventArgs e)
        {
            countdownValue--;
            if (countdownValue > 0)
            {
                countdownLabel.Text = countdownValue.ToString();
            }
            else
            {
                countdownTimer.Stop();
                blinkTimer.Stop();
                blinkLabel.Visible = true;
                countdownLabel.Text = "";

                // Freeze camera feed
                if (videoSource != null && videoSource.IsRunning)
                    videoSource.NewFrame -= Video_NewFrame;

                CapturePhoto();
            }
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
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

            string fileName = $"claimant_{DateTime.Now:yyyyMMdd_HHmmss}.jpg";
            string filePath = Path.Combine(saveFolder, fileName);

            capturedFrame.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);

            FaceCaptured?.Invoke(filePath);
            capturedFrame.Dispose();
            this.Close();
        }

        private void FormClaimCamera_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
            }

            lastFrame?.Dispose();
        }
        private void SaveFacePhoto(Bitmap faceImage, string folderPath)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string fileName = Path.Combine(folderPath, $"claimant_{timestamp}.jpg");

            faceImage.Save(fileName, System.Drawing.Imaging.ImageFormat.Jpeg);

            // trigger event if you want gallery update
            FaceCaptured?.Invoke(fileName);
        }

    }
}
