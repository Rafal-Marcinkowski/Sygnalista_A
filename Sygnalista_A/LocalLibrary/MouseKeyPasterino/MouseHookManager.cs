using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Sygnalista_A.LocalLibrary.MouseKeyPasterino;

public class MouseHookManager
{
    private const int WH_MOUSE_LL = 14;
    private const int WM_MOUSEMOVE = 0x0200;

    private static nint _hookID = nint.Zero;

    private static nint SetHook()
    {
        using var process = System.Diagnostics.Process.GetCurrentProcess();
        using var module = process.MainModule;
        return SetWindowsHookEx(WH_MOUSE_LL, HookCallback, GetModuleHandle(module.ModuleName), 0);
    }

    public static async Task SetClipboard(string companyCode)
    {
        if (!string.IsNullOrEmpty(companyCode))
        {
            Clipboard.SetDataObject(companyCode);
        }
    }

    private static void Unhook()
    {
        if (_hookID != nint.Zero)
        {
            if (!UnhookWindowsHookEx(_hookID))
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            _hookID = nint.Zero;
        }
    }

    private static nint HookCallback(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= 0 && wParam == WM_MOUSEMOVE)
        {
            return 1;
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    public static void DisableInput()
    {
        if (_hookID != nint.Zero)
        {
            throw new InvalidOperationException("Hook is already set.");
        }
        _hookID = SetHook();
        if (_hookID == nint.Zero)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }

    public static void EnableInput()
    {
        Unhook();
    }

    #region Win32 API

    private delegate nint LowLevelMouseProc(int nCode, nint wParam, nint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, nint hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern nint GetModuleHandle(string lpModuleName);

    #endregion
}
