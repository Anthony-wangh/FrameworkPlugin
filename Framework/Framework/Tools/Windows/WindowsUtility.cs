using Framework.Common;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;
using Screen = System.Windows.Forms.Screen;
/*xbb
 * 系统方法类
 * */
public class WindowsUtility
{
    //设置当前窗口的显示状态
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

    //获取当前激活窗口
    [DllImport("user32.dll", EntryPoint = "GetForegroundWindow")]
    private static extern IntPtr GetForegroundWindow();

    //设置窗口位置，大小
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

    //窗口拖动
    [DllImport("user32.dll")]
    private static extern bool ReleaseCapture();
    [DllImport("user32.dll")]
    private static extern bool SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);


    [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
    static extern UInt16 GetUserDefaultUILanguage();

    //设置窗口边框
    [DllImport("user32.dll")]
    public static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);


    [DllImport("User32.dll")]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("User32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("User32.dll")]
    private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

   

    private const int SwShowminimized = 2;//最小化窗口
    private const int SwShownormal = 1;  //还原窗口
    private const int SwShowmaximized = 3; //最大化
    private const int WmSyscommand = 0x0112;
    private const int ScMove = 0xF010;
    private const int Htcaption = 0x0002;

    //系统语言id
    private const int ChineseSimplified = 2052;
    private const int ChineseTraditional = 1028;
    private const int EnglishUsa = 1033;
    private const int EnglishBritish = 2057;


    //最小化窗口 具体窗口参数看这     https://msdn.microsoft.com/en-us/library/windows/desktop/ms633548(v=vs.85).aspx

    //边框参数
    const uint SWP_SHOWWINDOW = 0x0040;
    const int GWL_STYLE = -16;
    const int WS_BORDER = 1;
    const int WS_POPUP = 0x800000;
    const int SW_SHOWMINIMIZED = 2;//(最小化窗口)

    const int HWND_TOPMOST = -1;//让窗口显示在上层
    const int SWP_NOMOVE = 2;//忽略位置设置
    const int SWP_NOSIZE = 1;//忽略大小设置
    private static Thread setTopThread;

    //const int SWP_SHOWWINDOW = 64;//显示窗口
    /// <summary>
    /// 窗口最小化
    /// </summary>
    public static void SetMinWindows()
    {
        ShowWindow(GetForegroundWindow(), SwShowminimized);
    }
    /// <summary>
    /// 窗口最大化
    /// </summary>
    public static void SetMaxWindows()
    {
        ShowWindow(GetForegroundWindow(), SwShowmaximized);
    }
    /// <summary>
    /// 窗口还原
    /// </summary>
    public static void SetWindowRestore()
    {
        ShowWindow(GetForegroundWindow(), SwShownormal);
    }

    ///设置无边框，并设置框体大小，位置
    public static void SetNoFrameWindow(Rect rect)
    {
        SetWindowLong(GetForegroundWindow(), GWL_STYLE, WS_POPUP);
        SetWindowPos(GetForegroundWindow(), 0, (int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height, SWP_SHOWWINDOW);
    }
    ///拖动窗口
    public static void DragWindow()
    {
        var window = GetForegroundWindow();
        ReleaseCapture();
        SendMessage(window, WmSyscommand, ScMove + Htcaption, 0);
    }
    /// <summary>
    /// 设置窗口位置
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public static void SetWindowPos(int x, int y)
    {
        SetWindowPos(GetForegroundWindow(), 0, x, y, 0, 0, 0x0040 | 0x0001);
    }
    /// <summary>
    /// 设置窗口大小
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public static void SetWindowSize(int w, int h)
    {
        SetWindowPos(GetForegroundWindow(), 0, 0, 0, w, h, 0x0040 | 0x0002);
    }
    /// <summary>
    /// 设置窗口位置和大小
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public static void SetWindowPosAndSize(int x, int y, int w, int h)
    {
        SetWindowPos(GetForegroundWindow(), 0, x, y, w, h, 0x0040);
    }

    /// <summary>
    /// 获取系统语言
    /// </summary>
    /// <returns></returns>
    public static SystemLanguage GetSystemLanguage()
    {
        var userLan = GetUserDefaultUILanguage();
        switch (userLan)
        {
            case ChineseSimplified:
                return SystemLanguage.Chinese;
            case ChineseTraditional:
                return SystemLanguage.ChineseTraditional;
            case EnglishUsa:
            case EnglishBritish:
                return SystemLanguage.English;
        }
        return SystemLanguage.Chinese;
    }


    /// <summary>
    /// 设置为最大化
    /// </summary>
    public static void OnSwitchMaxScreen()
    {
        var display = Screen.PrimaryScreen.WorkingArea;
        Debug.Log($"设置分辨率:{display.Width}x{display.Height}");
        SetWindowPosAndSize(display.X, display.Y, display.Width, display.Height);
        if (UnityEngine.Screen.width != display.Width)
            SetScreenSize(display.Width, display.Height);
    }

    /// <summary>
    /// 设置屏幕尺寸
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public static void SetScreenSize(int width, int height)
    {
        Debug.Log($"设置分辨率:{width}x{height}");
        UnityEngine.Screen.SetResolution(width, height, false);
        CoreEngine.Inst.StartCoroutine(CheckScreenResolution(width, height));
    }


    private static IEnumerator CheckScreenResolution(int width, int height)
    {
        yield return new WaitForFixedUpdate();
        if (UnityEngine.Screen.width != width || UnityEngine.Screen.height != height)
        {
            UnityEngine.Screen.SetResolution(width, height, false);
        }
    }
    /// <summary>
    /// 窗口一直显示在最上层
    /// </summary>
    public static void WindowShowTop()
    {
        setTopThread = new Thread(new ThreadStart(ExeWindowShowTop));
        setTopThread.Start();
    }

    /// <summary>
    /// 使外部exe窗口一直显示在最上层，需要循环调用
    /// </summary>
    static void ExeWindowShowTop()
    {
        IntPtr exeHwnd = FindWindow(null, Application.productName);

        //当前激活的进程句柄
        IntPtr activeWndHwnd = GetForegroundWindow();

        // 当前程序不是活动窗口，则设置窗口显示在上层
        if (exeHwnd != activeWndHwnd)
        {
            SetWindowPos(exeHwnd, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_SHOWWINDOW);
        }

        //递归：0.3s后调用该方法，持续检测
        Thread.Sleep(300);
        ExeWindowShowTop();
    }
}
