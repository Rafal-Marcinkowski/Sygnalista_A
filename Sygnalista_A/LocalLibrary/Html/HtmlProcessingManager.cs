using CefSharp;
using CefSharp.Wpf;
using Library;
using Sygnalista_A.LocalLibrary.Services;
using System.Windows.Input;

namespace Sygnalista_A.LocalLibrary.Html;

public class HtmlProcessingManager(ChromiumWebBrowser webBrowser, ProcessHtml processHtml)
{
    public string HtmlText = string.Empty;
    public string PreviousHeader = string.Empty;
    public string TextReference = string.Empty;
    public string NewHeader = string.Empty;
    public bool FirstCycle = true;

    private int _failedAttempts = 0;

    public async Task WebsiteStutterWatchdog()
    {
        while (MainLoopManager.IsSearching)
        {
            await Task.Delay(2000);
            if (!HtmlText.Contains("Dane pobrano:"))
            {
                _failedAttempts++;

                if (_failedAttempts >= 3)
                {
                    webBrowser.Reload(true);
                    _failedAttempts = 0;
                }
            }
            else
            {
                _failedAttempts = 0;
            }
        }
    }

    public async Task DownloadSygnalista_ASourceAsync()
    {
        await ReFocusSygnalista_A();
        HtmlText = await webBrowser.GetSourceAsync();
        SaveTextToFile.SaveAsync("html", HtmlText);
    }

    public async Task<string> ProcessHtmlText()
    {
        return await processHtml.PrepareInformation(HtmlText);
    }

    public async Task<bool> EvaluateHeader(string companyCode)
    {
        return !await processHtml.IsForbiddenTags(companyCode);
    }

    private async Task ReFocusSygnalista_A()
    {
        Keyboard.ClearFocus();
        await Task.Delay(100);
        webBrowser.Focus();
        await Task.Delay(3000);
    }

    public async Task IsFirstCycle()
    {
        if (FirstCycle)
        {
            TextReference = NewHeader;
            FirstCycle = false;
        }
    }

    public async Task ResetProperties()
    {
        HtmlText = string.Empty;
        TextReference = string.Empty;
        FirstCycle = true;
    }

    public async Task<bool> IsNewInformation()
    {
        if (TextReference != NewHeader && NewHeader != PreviousHeader)
        {
            PreviousHeader = TextReference;
            return true;
        }

        return false;
    }
}
