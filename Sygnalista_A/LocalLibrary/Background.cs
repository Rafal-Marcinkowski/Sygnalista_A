using Library;
using Sygnalista_A.LocalLibrary.Services;

namespace Sygnalista_A.LocalLibrary;

public class Background(UIManager uIManager)
{
    public void ChangeBackgroundColor(string value)
    {
        try
        {
            string medValueString = value;
            medValueString = RemoveWhitespacesUsingLinq(medValueString);
            int medValue = int.Parse(medValueString);
            SetColor(1);

            switch (medValue)
            {
                case < 10000:
                    uIManager.IsBlinkerWorking = true;
                    _ = BackgroundColorBlinker(2);
                    break;
                case <= 25000:
                    SetColor(2);
                    break;
                case <= 100000:
                    SetColor(3);
                    break;
                default:
                    SetColor(4);
                    break;
            }
        }

        catch (Exception ex)
        {
            _ = SaveTextToFile.SaveAsync("ErrorDuringBackgroundChanging", ex.Message);
            SetColor(1);
        }
    }

    private void SetColor(int colorId)
    {
        uIManager.BackgroundColor = colorId;
    }

    private async Task BackgroundColorBlinker(int colorId)
    {
        while (uIManager.IsBlinkerWorking)
        {
            uIManager.BackgroundColor = uIManager.BackgroundColor == colorId ? 0 : colorId;
            await Task.Delay(600);
        }
    }

    private static string RemoveWhitespacesUsingLinq(string source)
    {
        return new string([.. source.Where(c => !char.IsWhiteSpace(c))]);
    }
}
