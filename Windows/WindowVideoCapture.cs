using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PostMessage_debug.Windows
{
    public class WindowVideoCapture
    {
        [DllImport("user32.dll")]
        private static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rectangle lpRect);

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth,
            int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, uint dwRop);

        private IntPtr windowHandle;
        private Timer captureTimer;
        private PictureBox displayControl;

        public WindowVideoCapture(IntPtr WindowHandle, PictureBox pictureBox)
        {
            this.windowHandle = WindowHandle;
            this.displayControl = pictureBox;
            this.captureTimer = new Timer();
            this.captureTimer.Interval = 100; // Capture interval in milliseconds
            this.captureTimer.Tick += CaptureTimer_Tick;
        }

        private void CaptureTimer_Tick(object sender, EventArgs e)
        {
            CaptureAndDisplay();
        }

        /// <summary>
        /// Captures the content of a window and displays it in a PictureBox control.
        /// </summary>
        private void CaptureAndDisplay()
        {
            if (windowHandle == IntPtr.Zero) return;

            GetWindowRect(windowHandle, out Rectangle windowRect);
            int width = windowRect.Width - windowRect.X;
            int height = windowRect.Height - windowRect.Y;

            Bitmap bmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                IntPtr hdc = g.GetHdc();
                using (Graphics windowGraphics = Graphics.FromHwnd(windowHandle))
                {
                    IntPtr hdcSrc = windowGraphics.GetHdc();
                    if (!BitBlt(hdc, 0, 0, width, height, hdcSrc, 0, 0, 0x00CC0020 /* SRCCOPY */))
                    {
                        // If BitBlt fails, try using PrintWindow
                        PrintWindow(windowHandle, hdc, 0);
                    }
                    windowGraphics.ReleaseHdc(hdcSrc);
                }
                g.ReleaseHdc(hdc);
            }

            Bitmap resizedBmp = ResizeImageToPictureBox(bmp, displayControl);
            UpdatePictureBoxImage(displayControl, resizedBmp);
            bmp.Dispose();
        }


        private Bitmap ResizeImageToPictureBox(Bitmap original, PictureBox pictureBox)
        {
            float originalAspectRatio = (float)original.Width / original.Height;
            float pictureBoxAspectRatio = (float)pictureBox.Width / pictureBox.Height;

            int newWidth, newHeight;
            if (pictureBoxAspectRatio > originalAspectRatio)
            {
                newWidth = pictureBox.Width;
                newHeight = (int)(pictureBox.Width / originalAspectRatio);
            }
            else
            {
                newHeight = pictureBox.Height;
                newWidth = (int)(pictureBox.Height * originalAspectRatio);
            }

            Bitmap resized = new Bitmap(newWidth, newHeight);
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.DrawImage(original, 0, 0, newWidth, newHeight);
            }

            return resized;
        }




        /// <summary>
        /// Updates the image of a PictureBox control with a new Bitmap image.
        /// If the PictureBox control requires invoking, it will be done on the UI thread.
        /// </summary>
        /// <param name="pictureBox">The PictureBox control to update.</param>
        /// <param name="newImage">The new Bitmap image to set.</param>
        private void UpdatePictureBoxImage(PictureBox pictureBox, Bitmap newImage)
        {
            if (pictureBox.InvokeRequired)
            {
                pictureBox.Invoke(new MethodInvoker(() =>
                {
                    UpdateImage(pictureBox, newImage);
                }));
            }
            else
            {
                UpdateImage(pictureBox, newImage);
            }
        }

        /// <summary>
        /// Updates the image displayed in the specified PictureBox control.
        /// If the PictureBox already has an image, it disposes the old image before setting the new one.
        /// </summary>
        /// <param name="pictureBox">The PictureBox control to update.</param>
        /// <param name="newImage">The new image to display.</param>
        private void UpdateImage(PictureBox pictureBox, Bitmap newImage)
        {
            if (pictureBox.Image != null)
            {
                var oldImage = pictureBox.Image;
                pictureBox.Image = newImage;
                oldImage.Dispose();
            }
            else
            {
                pictureBox.Image = newImage;
            }
        }

        /// <summary>
        /// Starts the capture process.
        /// </summary>
        public void StartCapture()
        {
            captureTimer.Start();
        }

        /// <summary>
        /// Stops the video capture.
        /// </summary>
        public void StopCapture()
        {
            captureTimer.Stop();
            if (displayControl.Image != null)
            {
                displayControl.Image.Dispose();
                displayControl.Image = null;
            }
        }
    }
}
