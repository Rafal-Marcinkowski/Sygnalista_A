using Library;
using Sygnalista_A.LocalLibrary.MouseKeyPasterino;
using System.Diagnostics;
using System.IO;

namespace Sygnalista_A.LocalLibrary;

public class Features
{
    public async static Task ClickStartStopButton()
    {
        await Mouse.MoveLeftClick(2515, 1385);
    }

    public async static Task RunAutoHotkeyScript()
    {
        await Task.Delay(100);
        try
        {
            Process.Start(new ProcessStartInfo("C:\\Users\\rafal\\Desktop\\Pogromcy\\wws.ahk") { Verb = "runas", UseShellExecute = true });
            await Task.Delay(100);
        }

        catch (Exception ex)
        {
            _ = SaveTextToFile.SaveAsync("AutoScriptError", ex.Message);
        }

        await Task.Delay(100);
    }

    public async static Task RunScripts(bool executeScript, int headerLength)
    {
        try
        {
            MouseHookManager.DisableInput();
            await Task.Delay(100);
            await Mouse.MouseClickNewInfo(1793, 82, headerLength);
        }

        finally
        {
            MouseHookManager.EnableInput();
        }

        if (executeScript)
        {
            await RunAutoHotkeyScript();
        }
    }

    public async static Task RestartSygnalista_B()
    {
        await Task.Delay(3000);

        Process.Start(new ProcessStartInfo("C:\\Users\\rafal\\Desktop\\Pogromcy\\restartSygnalista_B.ahk") { Verb = "runas", UseShellExecute = true });
    }

    public static void ClearCache()
    {
        var cachePath = Path.Combine(Environment.CurrentDirectory, "CefCache");

        if (Directory.Exists(cachePath))
        {
            Directory.Delete(cachePath, true);
        }
    }
}
