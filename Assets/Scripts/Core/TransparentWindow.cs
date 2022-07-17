using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class TransparentWindow : MonoBehaviour
{
    [DllImport("user32.dll")]
    private static extern IntPtr GetActiveWindow();

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowPos", SetLastError = true)]
    private static extern int SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

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

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern Int32 SystemParametersInfo(UInt32 action, int uParam, String vParam, UInt32 winIni);

    private static readonly UInt32 SPI_SETDESKWALLPAPER = 0x14;
    private static readonly UInt32 SPIF_UPDATEINIFILE = 0x01;
    private static readonly UInt32 SPIF_SENDWININICHANGE = 0x02;

    public enum CmdShow { Hide = 0, ShowNormal = 1, ShowMinimized = 2, ShowMaximized = 3, ShowNormalDontActivate = 4, Show = 5, Minimize = 6, ShowMinimizedDontActivate = 7, ShowDontActivate = 8, Restore = 9, ShowDefault = 10, ForceMinimize = 11 }

    [DllImport("user32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

    private Tray tray;
    private string currentdesktopPath;
    private IntPtr hWnd;

    public static TransparentWindow Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {      
        currentdesktopPath = GetCurrentDesktopWallpaperPath();

#if !UNITY_EDITOR
        hWnd = GetActiveWindow();
        
        tray = new Tray();                 
        tray.InitTray(hWnd);
        tray.HideTray();
        //Transparent window
        MARGINS margins = new MARGINS() { cxLeftWidth = -1 }; 
        DwmExtendFrameIntoClientArea(hWnd, ref margins);
        
        //Clickthrough window, everything with a color of black and 0 alpha
        SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);

        //Window on top
        //SetWindowPos(hWnd, HWND_TOPMOST, 0, 0, 0, 0, 0);

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

    private void Update()
    {
        SetClickthrough(Physics2D.OverlapPoint(GetMouseWorldPosition()) == null && IsPointerOverUI() == false);
    }

    public void MinimizeToTray()
    {
#if !UNITY_EDITOR
        ShowWindowAsync(hWnd, (int)CmdShow.Hide);
        tray.ShowTray();
#endif
    }

    private void SetClickthrough(bool clickthrough)
    {
        if (clickthrough)
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
        else
            SetWindowLong(hWnd, GWL_EXSTYLE, WS_EX_LAYERED);
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        vec.z = 0f;
        return vec;
    }

    public static bool IsPointerOverUI()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return true;
        }
        else
        {
            PointerEventData pe = new PointerEventData(EventSystem.current);
            pe.position = Input.mousePosition;
            List<RaycastResult> hits = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pe, hits);
            return hits.Count > 0;
        }
    }
}
