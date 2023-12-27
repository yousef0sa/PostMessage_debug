using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

public class MouseClickSender
{

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
    private static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);


    // Windows Messages for mouse events
    private const uint WM_LBUTTONDOWN = 0x0201;
    private const uint WM_LBUTTONUP = 0x0202;
    private const uint WM_RBUTTONDOWN = 0x0204;
    private const uint WM_RBUTTONUP = 0x0205;

    private IntPtr lParam;
    private IntPtr windowHandle;



    // Constructor
    public MouseClickSender(IntPtr WindowHandle,  IntPtr MouseMoveLParam)
    {
        this.windowHandle = WindowHandle;
        this.lParam = MouseMoveLParam;
    }

    // Method to send left click down
    public void SendLeftClickDown()
    {
        PostMessage(windowHandle, WM_LBUTTONDOWN, new IntPtr(0x0001), lParam);
    }

    // Method to send left click up
    public void SendLeftClickUp()
    {
        PostMessage(windowHandle, WM_LBUTTONUP, new IntPtr(0x0000), lParam);
    }

    // Method to send right click down
    public void SendRightClickDown()
    {
        PostMessage(windowHandle, WM_RBUTTONDOWN, new IntPtr(0x0002), lParam);
    }

    // Method to send right click up
    public void SendRightClickUp()
    {
        PostMessage(windowHandle, WM_RBUTTONUP, new IntPtr(0x0000), lParam);
    }
}


