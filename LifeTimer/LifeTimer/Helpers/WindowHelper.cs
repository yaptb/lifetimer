using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using WinRT.Interop;

namespace LifeTimer.Helpers;



public static class WindowHelper
{

    [StructLayout(LayoutKind.Sequential)]
    public struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }


    private const int GWL_STYLE = -16;
    private const int GWL_EXSTYLE = -20;
    private const int WS_EX_LAYERED = 0x80000;
    private const int WS_EX_TRANSPARENT = 0x20;
    private const int WS_EX_NOACTIVATE = 0x08000000;
    private const int WS_THICKFRAME = 0x00040000;
    private const int WS_BORDER = 0x00800000;
    private const int WS_MAXIMIZEBOX = 0x00010000;
    private const int WS_MINIMIZEBOX = 0x00020000;

    private const int LWA_COLORKEY = 0x00000001;
    public const int WS_CAPTION = 0x00C00000;


    public const int WS_EX_DLGMODALFRAME = 0x00000001;
    public const int WS_EX_CLIENTEDGE = 0x00000200;
    public const int WS_EX_STATICEDGE = 0x00020000;


    private const uint LWA_ALPHA = 0x2;
    private const uint SWP_NOMOVE = 0x2;
    private const uint SWP_NOSIZE = 0x1;
    private const uint SWP_NOZORDER = 0x4;
    private const uint SWP_NOACTIVATE = 0x10;
    private const uint SWP_FRAMECHANGED = 0x0020;

    private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);
    private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
    private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);


    [Flags]
    public enum DWMWINDOWATTRIBUTE : uint
    {
        DWMWA_WINDOW_CORNER_PREFERENCE = 33,
        DWMWA_BORDER_COLOR,
        DWMWA_VISIBLE_FRAME_BORDER_THICKNESS
    }

    public enum DWM_WINDOW_CORNER_PREFERENCE
    {
        DWMWCP_DEFAULT = 0,
        DWMWCP_DONOTROUND = 1,
        DWMWCP_ROUND = 2,
        DWMWCP_ROUNDSMALL = 3
    }



    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    [DllImport("user32.dll")]
    private static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);


    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);


    [DllImport("dwmapi", CharSet = CharSet.Unicode, PreserveSig = false)]
    public static extern int DwmSetWindowAttribute(IntPtr hwnd, DWMWINDOWATTRIBUTE dwAttribute, ref uint pvAttribute, uint cbAttribute);

    [DllImport("dwmapi.dll")]
    public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMargins);



    public static void SetWindowTransparentColorKey(Window window)
    {
        var hWnd = WindowNative.GetWindowHandle(window);

        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        var appWindow = AppWindow.GetFromWindowId(windowId);
        appWindow.SetPresenter(AppWindowPresenterKind.Overlapped);
        appWindow.TitleBar.ExtendsContentIntoTitleBar = true;


        // Set window as layered
        var exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
        SetWindowLong(hWnd, GWL_EXSTYLE, exStyle | WS_EX_LAYERED );

        // Set transparency using a color key (e.g., magenta)
        //   SetLayeredWindowAttributes(hWnd, 0x00FF00FF, 0, LWA_COLORKEY);
        
        var result = SetLayeredWindowAttributes(hWnd, 0x00FF00FF, 128, LWA_COLORKEY);

    }



    public static void SetWindowTransparency(Window window, int opacity)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        
        // Set window as layered
        var exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
        SetWindowLong(hWnd, GWL_EXSTYLE, exStyle | WS_EX_LAYERED );
        
        // Set transparency
        byte alpha = (byte)Math.Max(1, Math.Min(255, opacity));
        SetLayeredWindowAttributes(hWnd, 0, alpha, LWA_ALPHA);
    }

    public static void SetClickThrough(Window window, bool clickThrough)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        var exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

        if (clickThrough)
        {
            exStyle |= WS_EX_TRANSPARENT | WS_EX_LAYERED;
        }
        else
        {
            exStyle &= ~WS_EX_TRANSPARENT;
        }

        SetWindowLong(hWnd, GWL_EXSTYLE, exStyle);
    }

    public static void SetNoFrame(Window window, bool noFrame)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        var style = GetWindowLong(hWnd, GWL_STYLE);

        if (noFrame)
        {
            style &= ~WS_BORDER; // Remove thick frame style
            SetWindowLong(hWnd, GWL_EXSTYLE, style);
        }
        else
        {
            style |= WS_BORDER; 
            SetWindowLong(hWnd, GWL_EXSTYLE, style);
         }
    }


    public static void SendToBack(Window window)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE | SWP_FRAMECHANGED);
    }



    public static void SetNoActivate(Window window, bool noActivate)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        var exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);

        if (noActivate)
        {
            exStyle |= WS_EX_NOACTIVATE;
        }
        else
        {
            exStyle &= ~WS_EX_NOACTIVATE;
        }

        SetWindowLong(hWnd, GWL_EXSTYLE, exStyle);
    }

    public static AppWindow GetAppWindow(Window window)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        return AppWindow.GetFromWindowId(windowId);
    }

    public static void SetWindowBounds(AppWindow appWindow, int x, int y, int width, int height)
    {
        appWindow?.MoveAndResize(new RectInt32(x, y, width, height));
    }

    public static void MaximizeToWorkingArea(Window window)
    {
        var appWindow = GetAppWindow(window);
        if (appWindow != null)
        {
            var displayArea = DisplayArea.GetFromWindowId(appWindow.Id, DisplayAreaFallback.Primary);
            SetWindowBounds(appWindow,
                displayArea.WorkArea.X,
                displayArea.WorkArea.Y,
                displayArea.WorkArea.Width,
                displayArea.WorkArea.Height);
        }
    }

    public static void BringToFront( Window window)
    {
        var hwnd = WindowNative.GetWindowHandle(window);
        SetForegroundWindow(hwnd);
    }


    public static void SetWindowCornerRadius(Window window, DWM_WINDOW_CORNER_PREFERENCE cornerPreference)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        var attribute = WindowHelper.DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
        var preference = (uint)cornerPreference;
        DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));
    }

    public static void RecalcWindowSize(Window window)
    {
        var hWnd = WindowNative.GetWindowHandle(window);
        SetWindowPos(hWnd, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOZORDER | SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
    }


    public static void SetWindowToBorderless(Window window)
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(window);

            // Remove window borders
            int style = GetWindowLong(hWnd, GWL_STYLE);
        style &= ~(WS_CAPTION | WS_THICKFRAME);

        SetWindowLong(hWnd, GWL_STYLE, style);

        int exStyle =GetWindowLong(hWnd,GWL_EXSTYLE);
        exStyle &= ~(WS_EX_DLGMODALFRAME | WS_EX_CLIENTEDGE | WS_EX_STATICEDGE);
        exStyle |= WS_EX_LAYERED;
        SetWindowLong(hWnd, GWL_EXSTYLE, exStyle);

        // Extend frame into client area
        var margins = new MARGINS() { cxLeftWidth = -1 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);
    }

    public static void RestoreWindowToDefault(Window window)
    {
        IntPtr hWnd = WindowNative.GetWindowHandle(window);

        // Restore standard window styles
        int style = GetWindowLong(hWnd, GWL_STYLE);
        style |= WS_CAPTION | WS_THICKFRAME;
        style &= ~WS_MAXIMIZEBOX;
        style &= ~WS_MINIMIZEBOX;
        SetWindowLong(hWnd, GWL_STYLE, style);

        int exStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
        exStyle |= WS_EX_DLGMODALFRAME |WS_EX_CLIENTEDGE | WS_EX_STATICEDGE ;
        SetWindowLong(hWnd,GWL_EXSTYLE, exStyle);

        // Remove extended frame
        var margins = new MARGINS() { cxLeftWidth = 0, cxRightWidth = 0, cyTopHeight = 0, cyBottomHeight = 0 };
        DwmExtendFrameIntoClientArea(hWnd, ref margins);
    }


    [DllImport("user32.dll")]
    public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    // Constants for dwFlags
    public const uint MONITOR_DEFAULTTONULL = 0x00000000;
    public const uint MONITOR_DEFAULTTOPRIMARY = 0x00000001;
    public const uint MONITOR_DEFAULTTONEAREST = 0x00000002;

    [DllImport("shcore.dll")]
    public static extern int GetDpiForMonitor(
    IntPtr hmonitor,
    MonitorDpiType dpiType,
    out uint dpiX,
    out uint dpiY
);

    // Enum for DPI type
    public enum MonitorDpiType
    {
        MDT_EFFECTIVE_DPI = 0,
        MDT_ANGULAR_DPI = 1,
        MDT_RAW_DPI = 2,
        MDT_DEFAULT = MDT_EFFECTIVE_DPI
    }




    public static Tuple<int, int> getDpiScaledPixelsCurrentMonitor(IntPtr hwnd, int widthDIP, int heightDIP)
    {
        var monitorHandle = MonitorFromWindow(hwnd, MONITOR_DEFAULTTONEAREST);
        uint dpiX, dpiY;

        GetDpiForMonitor(monitorHandle, MonitorDpiType.MDT_EFFECTIVE_DPI, out dpiX, out dpiY);

        double scaleFactor = dpiX / 96.0;

        // Convert to physical pixels
        int widthPx = (int)(widthDIP * scaleFactor);
        int heightPx = (int)(heightDIP * scaleFactor);

        return new Tuple<int, int>(widthPx, heightPx);
    }

    /*
    [DllImport("CoreMessaging.dll")]
    private static extern int CreateDispatcherQueueController(
    DispatcherQueueOptions options,
    out IntPtr dispatcherQueueController);

    [StructLayout(LayoutKind.Sequential)]
    struct DispatcherQueueOptions
    {
        public int dwSize;
        public int threadType;
        public int apartmentType;
    }

    public static void EnsureDispatcherQueue()
    {
        if (Windows.System.DispatcherQueue.GetForCurrentThread() == null)
        {
            DispatcherQueueOptions options;
            options.dwSize = Marshal.SizeOf(typeof(DispatcherQueueOptions));
            options.threadType = 2;      // DQTYPE_THREAD_CURRENT
            options.apartmentType = 2;   // DQTAT_COM_STA

            CreateDispatcherQueueController(options, out _);
        }
    }*/

    [DllImport("user32.dll")]
    static extern bool AdjustWindowRectEx(ref RECT lpRect, uint dwStyle, bool bMenu, uint dwExStyle);

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;
    }


    public const uint WS_OVERLAPPED = 0x00000000;
    public const uint WS_SYSMENU = 0x00080000;
    public const uint WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU |
                                            WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX;
   
    public const uint WS_EX_WINDOWEDGE = 0x00000100;
    
    public const uint WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE;


    public static RECT GetInteractiveWindowBoundsAdjustment(int clientWidth, int clientHeight)
    {
        RECT rect = new RECT
        {
            Left = 0,
            Top = 0,
            Right = clientWidth,
            Bottom = clientHeight
        };

        AdjustWindowRectEx(ref rect, WS_OVERLAPPEDWINDOW, false, WS_EX_OVERLAPPEDWINDOW);

        return rect;
    }




}