using Library;
using Library.Events;
using Sygnalista_A.LocalLibrary.Html;
using Sygnalista_A.LocalLibrary.MouseKeyPasterino;

namespace Sygnalista_A.LocalLibrary.Services;

public class MainLoopManager : BindableBase
{
    private readonly HtmlProcessingManager htmlProcessingManager;
    private readonly CompanyInfoManager companyInfoManager;
    private readonly UIManager uIManager;
    private readonly CommunicationManager communicationManager;
    private readonly IEventAggregator eventAggregator;
    private readonly Background background;

    public MainLoopManager(HtmlProcessingManager htmlProcessingManager, CompanyInfoManager companyInfoManager,
        UIManager uIManager, CommunicationManager communicationManager, IEventAggregator eventAggregator, Background background)
    {
        this.htmlProcessingManager = htmlProcessingManager;
        this.companyInfoManager = companyInfoManager;
        this.uIManager = uIManager;
        this.communicationManager = communicationManager;
        this.eventAggregator = eventAggregator;
        this.background = background;

        _ = this.communicationManager.CreateTurnoverMedianTable();
        _ = this.communicationManager.StartListeningAsync();
        this.eventAggregator.GetEvent<CommunicationEvent>().Subscribe((payload) => this.communicationManager.OnMessageReceived(payload));
    }

    public static bool IsSearching { get; set; } = false;
    public bool ForceAutoScript { get; internal set; }

    public async Task StartLoop()
    {
        try
        {
            await SetStartingFields();
            _ = htmlProcessingManager.WebsiteStutterWatchdog();

            while (IsSearching)
            {
                await htmlProcessingManager.DownloadSygnalista_ASourceAsync();
                await CoreMechanics();
            }

            _ = new BiznesRadarStep(companyInfoManager).Execute(companyInfoManager.CompanyCode);
        }

        catch (Exception ex)
        {
            _ = SaveTextToFile.SaveAsync("Sygnalista_AErrorMessage", ex.Message + " " + ex.Data + " " + ex.Source + " " + ex.InnerException);
        }
    }

    private async Task CoreMechanics()
    {
        if (!String.IsNullOrEmpty(htmlProcessingManager.HtmlText))
        {
            htmlProcessingManager.NewHeader = await htmlProcessingManager.ProcessHtmlText();
            await htmlProcessingManager.IsFirstCycle();
            _ = SaveTextToFile.SaveAsync("newHeader", htmlProcessingManager.NewHeader);
            await companyInfoManager.CreateCompanyCode(htmlProcessingManager.NewHeader);
            _ = SaveTextToFile.SaveAsync("CompanyCode", companyInfoManager.CompanyCode);
            if (!string.IsNullOrEmpty(companyInfoManager.CompanyCode))
            {
                if (await htmlProcessingManager.EvaluateHeader(companyInfoManager.CompanyCode))
                {
                    if (!String.IsNullOrEmpty(companyInfoManager.CompanyCode))
                    {
                        await companyInfoManager.CreateMediana();
                        await EvaluateInformation();
                    }
                }
            }
            _ = uIManager.ShowRefreshDate(htmlProcessingManager.HtmlText);
        }
    }

    private async Task EvaluateInformation()
    {
        if (await htmlProcessingManager.IsNewInformation())
        {
            if (!await communicationManager.IsEspiAlreadySeen(companyInfoManager.CompanyCode))
            {
                await ReactToNewInformation();
            }

            else
            {
                if (communicationManager.EspiToAvoid.Count > 0)
                {
                    communicationManager.EspiToAvoid.Remove(companyInfoManager.CompanyCode);
                }

                companyInfoManager.CompanyName = string.Empty;
                companyInfoManager.Capitalization = string.Empty;
                htmlProcessingManager.FirstCycle = true;
                _ = new BiznesRadarStep(companyInfoManager).Execute(companyInfoManager.CompanyCode);
            }

            _ = SaveTextToFile.AddAsync("ESPI_Godzina", DateTime.Now.ToString() + ": " + companyInfoManager.CompanyCode + Environment.NewLine);
        }
    }

    private void TurnMainLoopOff()
    {
        IsSearching = false;
    }

    public async Task StopSygnalista_A()
    {
        uIManager.BackgroundColor = 1;
        IsSearching = false;
    }

    public async Task ResetStartSygnalista_A()
    {
        uIManager.IsBlinkerWorking = false;
        IsSearching = true;

        _ = communicationManager.SendMessage(["finishedreadinginfo"]);
        _ = StartLoop();
    }

    private async Task ReactToNewInformation()
    {
        companyInfoManager.CompanyName = string.Empty;
        companyInfoManager.Capitalization = string.Empty;

        _ = communicationManager.SendMessage([companyInfoManager.CompanyCode, "readinginfo"]);
        _ = Audio.PlaySoundAsync();

        await MouseHookManager.SetClipboard(companyInfoManager.CompanyCode);
        await Features.RunScripts(communicationManager.ShouldIAutoScript && ForceAutoScript, htmlProcessingManager.NewHeader.Length);

        background.ChangeBackgroundColor(companyInfoManager.MedianaText);

        TurnMainLoopOff();
    }

    private async Task SetStartingFields()
    {
        companyInfoManager.CompanyCode = string.Empty;
        await htmlProcessingManager.ResetProperties();
        uIManager.BackgroundColor = 0;
    }
}
