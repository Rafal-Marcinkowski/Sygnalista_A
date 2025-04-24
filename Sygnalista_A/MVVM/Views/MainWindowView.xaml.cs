using CefSharp;
using CefSharp.Wpf;
using System.IO;
using System.Windows;

namespace Sygnalista_A.MVVM.Views;

public partial class MainWindowView
{
    public MainWindowView()
    {
        InitializeCefSharp();
        InitializeComponent();

        this.Left = 1670;
        this.Top = 0;
        chrome.FrameLoadEnd += Chrome_FrameLoadEnd;
    }

    private void InitializeCefSharp()
    {
        var settings = new CefSettings
        {
            RootCachePath = Path.Combine(Environment.CurrentDirectory, "CefCache")
        };

        Cef.Initialize(settings);
    }

    private void Chrome_FrameLoadEnd(object? sender, FrameLoadEndEventArgs e)
    {

        string cssCode = @"
        body { overflow: hidden; }
        .ui.borderless.stackable.menu { display: none !important; }
        .ui.breadcrumb { display: none !important; }
        .ui.very.basic.table { display: none !important; }
        .ui.pointing.secondary.menu { display: none !important; }
        .ui.secondary.attached.segment { display: none !important; }
        #bb-that { display: none !important; }
        ";

        string script = $"var style = document.createElement('style'); style.innerHTML = `{cssCode}`; document.head.appendChild(style);";
        chrome.ExecuteScriptAsync(script);
    }

    private void Shutdown(object sender, RoutedEventArgs e)
    {
        App.Current.Shutdown();
    }

    private void ChromeGoBack(object sender, RoutedEventArgs e)
    {
        if (chrome.CanGoBack)
        {
            chrome.BackCommand.Execute(null);
        }
    }
}