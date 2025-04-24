using Library;
using Library.Communication;
using Library.Interfaces;

namespace Sygnalista_A.LocalLibrary.Services;

public class CommunicationManager
{
    private readonly ICommunicationService communicationService;
    private readonly Dictionary<int, bool> isPortListening = [];
    private readonly Dictionary<int, bool> isOtherProgramReadingInfo = [];
    public List<string> EspiToAvoid { get; set; } = [];
    public bool ShouldIAutoScript => !isOtherProgramReadingInfo[12346] && !isOtherProgramReadingInfo[12348];

    public CommunicationManager(ICommunicationService communicationService)
    {
        this.communicationService = communicationService;
        isPortListening.Add(12346, false);
        isPortListening.Add(12348, false);
        isOtherProgramReadingInfo.Add(12346, false);
        isOtherProgramReadingInfo.Add(12348, false);
    }

    public async Task HandleCommunicationChanged(int port)
    {
        await Task.Run(async () =>
        {
            await Task.Delay(250);
            bool isConnected = false;
            while (!isConnected)
            {
                try
                {
                    await communicationService.SendMessageAsync("startedlistening", port);
                    isPortListening[port] = true;
                    isConnected = true;
                }
                catch
                {
                    await Task.Delay(2000);
                }
            }
        });
    }

    public async Task OnMessageReceived(CommunicationPayload payload)
    {
        Action action = payload.Message switch
        {
            "readinginfo" => () => isOtherProgramReadingInfo[payload.Port] = true
            ,
            "finishedreadinginfo" => () => isOtherProgramReadingInfo[payload.Port] = false
            ,
            "startedlistening" => () => isPortListening[payload.Port] = true,
            "stoppedlistening" => () =>
            {
                isPortListening[payload.Port] = false;
                isOtherProgramReadingInfo[payload.Port] = false;

                if (EspiToAvoid.Count > 0)
                {
                    EspiToAvoid.RemoveAt(EspiToAvoid.Count - 1);
                }

                _ = HandleCommunicationChanged(payload.Port);
            }
            ,
            "captcha" => async () => await Features.RestartSygnalista_B()
            ,
            "EXT" => () =>
            {
                string cleanMessage = payload.Text.Substring(4);
                _ = SaveTextToFile.SaveAsync("MessageFrom_Extension", cleanMessage);
            }
            ,
            _ => () =>
            {
                _ = SaveTextToFile.SaveAsync($"MessageFrom_{payload.Port}", $"{payload.Message}");
                EspiToAvoid.Add(payload.Message);
            }
        };

        action();
        _ = SaveTextToFile.SaveAsync($"MessageFrom_{payload.Port}", $"{payload.Message}");
    }

    public async Task StartListeningAsync()
    {
        _ = communicationService.ListenAsync();
        _ = HandleCommunicationChanged(12346);  // Sygnalista_B
        _ = HandleCommunicationChanged(12348);  // Bossa
    }

    public async Task SendMessage(string[] messages)
    {
        foreach (var message in messages)
        {
            foreach (var port in isPortListening)
            {
                if (port.Value)
                {
                    _ = communicationService.SendMessageAsync(message, port.Key);
                }
            }

            await Task.Delay(250);
        }
    }

    public async Task SendToBiznesRadar(string message)
    {
        _ = communicationService.SendMessageAsync(message, 12347);
    }

    public async Task CreateTurnoverMedianTable()
    {
        await NameTranslation.InitializeTurnoverMedian();
        _ = SendMessage(["copyturnovermedian"]);
    }

    public async Task<bool> IsEspiAlreadySeen(string companyCode) => EspiToAvoid.Any(q => q.Equals(companyCode));
}
