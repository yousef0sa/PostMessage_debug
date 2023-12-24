using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace PostMessage_debug.Mouse
{
    public class MouseMovement
    {
        private IntPtr selectedWindowHandle;
        private int originalWindowWidth;
        private int originalWindowHeight;
        private PictureBox pictureBox1;

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        const uint WM_MOUSEMOVE = 0x0200;

        // Constructor
        public MouseMovement(IntPtr SelectedWindowHandle, int OriginalWindowWidth, int OriginalWindowHeight, PictureBox pictureBox)
        {
            this.selectedWindowHandle = SelectedWindowHandle;
            this.originalWindowWidth = OriginalWindowWidth;
            this.originalWindowHeight = OriginalWindowHeight;
            this.pictureBox1 = pictureBox;
        }

        /// <summary>
        /// Event handler for the mouse move event.
        /// Adjusts the mouse coordinates based on the scale factors and sends the WM_MOUSEMOVE message to the selected window.
        /// </summary>
        /// <param name="sender">The object that triggered the event.</param>
        /// <param name="e">The MouseEventArgs containing information about the mouse move event.</param>
        public void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (selectedWindowHandle == IntPtr.Zero || originalWindowWidth == 0 || originalWindowHeight == 0)
                return;

            // Calculate scale factors
            float scaleX = (float)originalWindowWidth / pictureBox1.Width;
            float scaleY = (float)originalWindowHeight / pictureBox1.Height;

            // Adjust coordinates
            int adjustedX = (int)(e.X * scaleX);
            int adjustedY = (int)(e.Y * scaleY);

            // Create the lParam with the adjusted coordinates for PostMessage
            IntPtr lParam = (IntPtr)((adjustedY << 16) | adjustedX);

            // Send the WM_MOUSEMOVE message to the window
            PostMessage(selectedWindowHandle, WM_MOUSEMOVE, IntPtr.Zero, lParam);
        }

    }

}
