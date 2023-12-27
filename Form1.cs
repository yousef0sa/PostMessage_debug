using PostMessage_debug.Mouse;
using PostMessage_debug.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PostMessage_debug
{
    public partial class Form1 : Form
    {
        private IntPtr selectedWindowHandle;
        private WindowVideoCapture videoCapture;
        private MouseMovement mouseMovement;
        public int originalWindowWidth;
        public int originalWindowHeight;
        private IntPtr mouseMoveLParam;



        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Event handler for the DropDown event of cb_Window_Handle.
        /// Populates the cb_Window_Handle ComboBox with a list of open windows.
        /// </summary>
        private void cb_Window_Handle_DropDown(object sender, EventArgs e)
        {
            List<WindowInfo> windows = WindowHelper.GetOpenWindows();
            cb_Window_Handle.Items.Clear();

            foreach (var window in windows)
            {
                cb_Window_Handle.Items.Add(window);
            }
        }

        /// <summary>
        /// Event handler for drawing an item in the cb_Window_Handle ComboBox.
        /// </summary>
        private void cb_Window_Handle_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index >= 0)
            {
                WindowInfo windowInfo = (WindowInfo)cb_Window_Handle.Items[e.Index];

                // Define the size for the icon
                int iconSize = e.Bounds.Height - 2;  // Adjust the size as needed
                Rectangle iconRect = new Rectangle(e.Bounds.Left + 2, e.Bounds.Top + 1, iconSize, iconSize);

                // Draw the icon
                if (windowInfo.Icon != null)
                {
                    using (Icon resizedIcon = new Icon(windowInfo.Icon, new Size(iconSize, iconSize)))
                    {
                        e.Graphics.DrawIcon(resizedIcon, iconRect);
                    }
                }

                // Prepare the text to include the handle and the title
                string itemText = $"{windowInfo.Handle} - {windowInfo.Title}";

                // Calculate text bounds considering icon size
                Rectangle textRect = new Rectangle(e.Bounds.Left + iconSize + 4, e.Bounds.Top, e.Bounds.Width - iconSize - 4, e.Bounds.Height);

                // Draw the text
                using (Brush textBrush = new SolidBrush(e.ForeColor))
                {
                    e.Graphics.DrawString(itemText, e.Font, textBrush, textRect);
                }
            }
        }

        /// <summary>
        /// Event handler for the selection change in the cb_Window_Handle combo box.
        /// Updates the selectedWindowHandle variable with the handle of the selected window.
        /// </summary>
        private void cb_Window_Handle_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cb_Window_Handle.SelectedIndex != -1)
            {
                // Stop and dispose the previous video capture if it exists
                if (videoCapture != null)
                {
                    videoCapture.StopCapture();
                    if (pictureBox1.Image != null)
                    {
                        pictureBox1.Image.Dispose();
                        pictureBox1.Image = null;
                    }
                }

                WindowInfo selectedWindow = (WindowInfo)cb_Window_Handle.SelectedItem;
                selectedWindowHandle = selectedWindow.Handle;

                // Create a new video capture instance
                videoCapture = new WindowVideoCapture(selectedWindowHandle, pictureBox1, this);
                videoCapture.StartCapture();
            }

        }

        /// <summary>
        /// Event handler for the MouseMove event of the pictureBox1 control.
        /// </summary>
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (cb_Send_Mouse.Checked)
            {
                mouseMovement = new MouseMovement(selectedWindowHandle, originalWindowWidth, originalWindowHeight, pictureBox1);
                mouseMoveLParam = mouseMovement.OnMouseMove(sender, e);
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            // Create an instance of the MouseClickSender
            var mouseClickSender = new MouseClickSender(selectedWindowHandle, mouseMoveLParam);
            if (cb_Send_Mouse.Checked)
            {
                // Check which mouse button was pressed
                if (e.Button == MouseButtons.Left)
                {
                    // Send left click down
                    mouseClickSender.SendLeftClickDown();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    // Send right click down
                    mouseClickSender.SendRightClickDown();
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (cb_Send_Mouse.Checked)
            {
                // Create an instance of the MouseClickSender
                var mouseClickSender = new MouseClickSender(selectedWindowHandle, mouseMoveLParam);

                // Check which mouse button was pressed
                if (e.Button == MouseButtons.Left)
                {
                    // Send left click Up
                    mouseClickSender.SendLeftClickUp();
                }
                else if (e.Button == MouseButtons.Right)
                {
                    // Send right click Up
                    mouseClickSender.SendRightClickUp();
                }
            }
        }
    }
}
