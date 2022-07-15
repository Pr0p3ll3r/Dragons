using Microsoft.Win32;
using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnityEngine;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
    private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    [DllImport("user32.dll")]
    static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

    private struct MARGINS
    {
        public int cxLeftWidth;
        public int cxRightWidth;
        public int cyTopHeight;
        public int cyBottomHeight;
    }

    [DllImport("Dwmapi.dll")]
    private static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);
    
    const int GWL_EXSTYLE = -20;
    const int HWND_TOPMOST = -1;
    const uint WS_EX_LAYERED = 0x00080000;
    const uint WS_EX_TRANSPARENT = 0x00000020;
    const uint LWA_COLORKEY = 0x00000001;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern Int32 SystemParametersInfo(UInt32 action, int uParam, String vParam, UInt32 winIni);

    private string currentdesktopPath;
    private const UInt32 SPI_GETDESKWALLPAPER = 0x73;
    private const int MAX_PATH = 260;

    private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
    private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
    private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [SerializeField] private Shader UIDefaultNew;

    public enum CmdShow { Hide = 0, ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3, ShowNormalDontActivate = 4, Show = 5, Minimize = 6, ShowMinimizedDontActivate = 7, ShowDontActivate = 8, Restore = 9, ShowDefault = 10, ForceMinimize = 11 }

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern bool IsZoomed(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    private IntPtr hWnd = IntPtr.Zero;

    public bool IsMaximized()
    {
#if !UNITY_EDITOR
        GetHandle();
        return IsZoomed(hWnd);
#else
        return false;
#endif
    }

    public bool IsMinimized()
    {
#if !UNITY_EDITOR
        GetHandle();
        return IsIconic(hWnd);
#else
        return false;
#endif
    }

    public void Maximize()
    {
        ShowAsync(CmdShow.ShowMaximized);
    }

    public void Minimize()
    {
        ShowAsync(CmdShow.Minimize);
    }

    public void ShowAsync(CmdShow cmd)
    {
#if !UNITY_EDITOR
        GetHandle();
        ShowWindowAsync(hWnd, (int)cmd);
#endif
    }

    private void GetHandle()
    {
#if !UNITY_EDITOR
        if (hWnd != IntPtr.Zero) return;
        hWnd = GetActiveWindow();
#endif
    }

    Tray tray;
    private void Awake()
    {
#if !UNITY_EDITOR
        DontDestroyOnLoad(gameObject);
        tray = new Tray();
        tray.InitTray();
        tray.HideTray();
#endif
    }

    void Start()
    {
        Canvas.GetDefaultCanvasMaterial().shader = UIDefaultNew;
        currentdesktopPath = GetCurrentDesktopWallpaperPath();
#if !UNITY_EDITOR
        hWnd = GetActiveWindow();

        MARGINS margins = new MARGINS() { cxLeftWidth = -1 }; 
        DwmExtendFrameIntoClientArea(hWnd, ref margins);
        
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        SetLayeredWindowAttributes(hWnd, 0, 0, LWA_COLORKEY);

        SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);

        string imgWallpaper = UnityEngine.Application.dataPath + "/StreamingAssets/Background.jpg";
        SetWallpaper(imgWallpaper);
#endif
        UnityEngine.Application.runInBackground = true;
    }

    private void SetWallpaper(string path)
    {
        RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true);
        key.SetValue(@"WallpaperStyle", 2.ToString()); // 2 is stretched
        key.SetValue(@"TileWallpaper", 0.ToString());

        SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
    }

    private string GetCurrentDesktopWallpaperPath()
    {
        string path = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop").GetValue("WallPaper") as string;
        return path;
    }

    private void OnApplicationQuit()
    {
#if !UNITY_EDITOR
        SetWallpaper(currentdesktopPath);
        tray?.Dispose();
        tray = null;
#endif
    }

    public void MinimalizeToTray()
    {
#if !UNITY_EDITOR
        ShowAsync(CmdShow.Hide);
        tray.ShowTray();
#endif
    }

}
