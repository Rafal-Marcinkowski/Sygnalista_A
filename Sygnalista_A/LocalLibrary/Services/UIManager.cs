using Library.BiznesRadar;

namespace Sygnalista_A.LocalLibrary.Services;

public class UIManager : BindableBase
{
    public async Task ShowRefreshDate(string htmlText)
    {
        DanePobrano = await RefreshDate.Get(htmlText);
    }

    private string _danePobrano;
    public string DanePobrano
    {
        get => _danePobrano;
        set => SetProperty(ref _danePobrano, value);
    }

    public bool IsBlinkerWorking { get; set; }

    private int backgroundColor = 1;
    public int BackgroundColor
    {
        get => backgroundColor;
        set => SetProperty(ref backgroundColor, value);
    }
}
