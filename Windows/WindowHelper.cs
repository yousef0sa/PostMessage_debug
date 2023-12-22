using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// Represents information about a window.
/// </summary>
public class WindowInfo
{
    public IntPtr Handle { get; set; }
    public string Title { get; set; }
    public Icon Icon { get; set; }
}

public class WindowHelper
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool IsWindowVisible(IntPtr hWnd);

    private const int GCLP_HICONSM = -34;
    private const int GCLP_HICON = -14;
    private const int WM_GETICON = 0x7F;

    private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);


    /// <summary>
    /// Retrieves a list of open windows.
    /// </summary>
    /// <returns>A list of WindowInfo objects representing the open windows.</returns>
    public static List<WindowInfo> GetOpenWindows()
    {
        List<WindowInfo> windows = new List<WindowInfo>();

        EnumWindows((hWnd, lParam) =>
        {
            if (IsWindowVisible(hWnd))
            {
                StringBuilder windowText = new StringBuilder(255);
                GetWindowText(hWnd, windowText, windowText.Capacity);
                string windowName = windowText.ToString();

                if (!string.IsNullOrEmpty(windowName))
                {
                    Icon icon = GetWindowIcon(hWnd);
                    windows.Add(new WindowInfo { Handle = hWnd, Title = windowName, Icon = icon });
                }
            }

            return true; // Continue enumeration
        }, IntPtr.Zero);

        return windows;
    }

    /// <summary>
    /// Represents an icon, which is a small graphical representation of a program or file.
    /// </summary>
    /// <remarks>
    /// The Icon class provides methods for creating, manipulating, and displaying icons.
    /// It is typically used to retrieve the icon associated with a window or a file.
    /// </remarks>
    private static Icon GetWindowIcon(IntPtr hWnd)
    {
        IntPtr iconHandle = SendMessage(hWnd, WM_GETICON, new IntPtr(GCLP_HICON), IntPtr.Zero);
        if (iconHandle == IntPtr.Zero)
            iconHandle = GetClassLongPtr(hWnd, GCLP_HICON);
        if (iconHandle == IntPtr.Zero)
            iconHandle = SendMessage(hWnd, WM_GETICON, new IntPtr(GCLP_HICONSM), IntPtr.Zero);
        if (iconHandle == IntPtr.Zero)
            iconHandle = GetClassLongPtr(hWnd, GCLP_HICONSM);

        if (iconHandle != IntPtr.Zero)
            return Icon.FromHandle(iconHandle);

        return null;
    }

#if WIN64
    [DllImport("user32.dll", EntryPoint = "GetClassLongPtr")]
    private static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);
#else
    [DllImport("user32.dll", EntryPoint = "GetClassLong")]
    private static extern IntPtr GetClassLongPtr(IntPtr hWnd, int nIndex);
#endif
}
