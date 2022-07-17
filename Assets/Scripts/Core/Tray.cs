using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using UnityEngine;

public class Win32API
{
	#region Win32Api

	/// <summary>
	/// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getforegroundwindow
	/// </summary>
	[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
	public static extern IntPtr GetForegroundWindow();

	private const int
		 SW_HIDE = 0, // hide window and taskbar
		 SW_SHOW = 5; // Display the current size and position

	/// <summary>
	/// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-showwindow
	/// </summary>
	[DllImport("user32.dll")]
	public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

	private static readonly IntPtr
		 HWND_TOP = new IntPtr(0); // put on the top
	private const uint
		 SWP_NOSIZE = 0x0001, // Keep the current size (ignore the cx and cy parameters).
		 SWP_NOZORDER = 0x0004; // Keep the current Z order (ignore the hWndInsertAfter parameter).

	/// <summary>
	/// Set the window position
	/// https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setwindowpos
	/// </summary>
	[DllImport("user32.dll")]
	public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
	#endregion

	[DllImport("user32.dll")]
	public static extern IntPtr GetActiveWindow();

	#region Public

	#region ShowWindow
	//public static void Hide(IntPtr hwnd) => ShowWindow(hwnd, SW_HIDE);
	public static void Show(IntPtr hwnd) => ShowWindow(hwnd, SW_SHOW);
	#endregion

	#region SetWindowPos
	public static void SetWindowPosOnDisplay2(IntPtr hWnd, int left, int top)
	{
		SetWindowPos(hWnd, HWND_TOP, left, top, 0, 0, SWP_NOSIZE | SWP_NOZORDER);
	}
	#endregion
	#endregion
}

public class Tray : IDisposable
{
	private const int WIDTH = 40;
	private const int HEIGHT = 40;
	private IntPtr hwnd;

	private NotifyIcon notifyIcon; // tray icon
	private ContextMenuStrip contextMenu; // Context menu
	private ToolStripMenuItem menuItem_ShowWindow; // Show window
	private ToolStripMenuItem menuItem_Display1; // Display all on the main screen
	private ToolStripMenuItem menuItem_Display2; // Full screen display on screen 2
	private ToolStripMenuItem menuItem_Quit; // Exit the program

	public void InitTray(IntPtr _hwnd)
	{
		int displayLength = Display.displays.Length;
		hwnd = _hwnd;
		notifyIcon = new NotifyIcon();
		contextMenu = new ContextMenuStrip();
		menuItem_ShowWindow = new ToolStripMenuItem();
		if (displayLength > 1)
		{
			this.menuItem_Display1 = new ToolStripMenuItem();
			this.menuItem_Display2 = new ToolStripMenuItem();
		}
		menuItem_Quit = new ToolStripMenuItem();
		menuItem_ShowWindow.Font = new System.Drawing.Font("Segoe UI", 9);
		menuItem_Quit.Font = new System.Drawing.Font("Segoe UI", 9);
		contextMenu.Font = new System.Drawing.Font("Segoe UI", 9);
		contextMenu.SuspendLayout();
		//
		// notifyIcon
		//
		notifyIcon.ContextMenuStrip = contextMenu;
        string iconPath = UnityEngine.Application.dataPath + "/StreamingAssets/Icon.png";  
        notifyIcon.Icon = this.CustomTrayIcon(iconPath, WIDTH, HEIGHT);
        notifyIcon.Text = "Smoki";
		notifyIcon.MouseClick += NotifyIcon_MouseClick;
		notifyIcon.Visible = true;
		//
		// contextMenu
		//
		List<ToolStripMenuItem> menuItems = new List<ToolStripMenuItem>
			{
				menuItem_ShowWindow,
				menuItem_Quit
			};
		if (displayLength > 1)
		{
			menuItems.Insert(2, this.menuItem_Display1);
			menuItems.Insert(3, this.menuItem_Display2);
		}
		contextMenu.Items.AddRange(menuItems.ToArray());
		contextMenu.Size = new Size(181, (menuItems.Count * 22) + 20);
		// 
		// menuItem_MainWindow
		// 
		menuItem_ShowWindow.Size = new Size(180, 22);
		menuItem_ShowWindow.Text = "Show Window";//Show Window
		menuItem_ShowWindow.Click += (sender, e) =>
		{
			Win32API.Show(hwnd);
			HideTray();
		};

		if (displayLength > 1)
		{
			// 
			// menuItem_Maximize
			// 
			menuItem_Display1.Size = new Size(180, 22);
			menuItem_Display1.Text = "Screen 1 full screen display";
			menuItem_Display1.Click += (sender, e) =>
			{
				Win32API.SetWindowPosOnDisplay2(hwnd, 0, 0);
				UnityEngine.Screen.SetResolution(Display.displays[0].systemWidth, Display.displays[0].systemHeight, true);
				HideTray();
			};
			// 
			// menuItem_Maximize
			// 
			menuItem_Display2.Size = new Size(180, 22);
			menuItem_Display2.Text = "Screen 2 full screen display";
			menuItem_Display2.Click += (sender, e) =>
			{
				Win32API.SetWindowPosOnDisplay2(hwnd, Display.displays[0].systemWidth, 0);
				UnityEngine.Screen.SetResolution(Display.displays[1].systemWidth, Display.displays[1].systemHeight, true);
				HideTray();
			};
		}
		// 
		// menuItem_Quit
		// 
		menuItem_Quit.Size = new Size(180, 22);
		menuItem_Quit.Text = "Quit";
		menuItem_Quit.Click += (sender, e) =>
		{
			UnityEngine.Application.Quit();
		};

		contextMenu.ResumeLayout(false);
	}

	private Icon CustomTrayIcon(string iconPath, int width, int height)
	{
		Bitmap bt = new Bitmap(iconPath);
		Bitmap fitSizeBt = new Bitmap(bt, width, height);
		return Icon.FromHandle(fitSizeBt.GetHicon());
	}

	private void NotifyIcon_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left)
		{
			Win32API.Show(hwnd);
			HideTray();
		}
	}

	private void DestroyClick() => notifyIcon.MouseDoubleClick -= NotifyIcon_MouseClick;

	public void ShowTray() => notifyIcon.Visible = true; //Whether the tray button is visible

	public void HideTray() => notifyIcon.Visible = false;//Whether the tray button is visible

	public void Dispose()
	{
		DestroyClick();
		notifyIcon?.Dispose();
		contextMenu?.Dispose();
		menuItem_ShowWindow?.Dispose();
		menuItem_Display1?.Dispose();
		menuItem_Display2?.Dispose();
		menuItem_Quit?.Dispose();
		hwnd = IntPtr.Zero;
	}
}