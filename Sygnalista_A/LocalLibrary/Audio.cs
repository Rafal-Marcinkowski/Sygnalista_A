using System.Media;

namespace Sygnalista_A.LocalLibrary;

public class Audio
{
    public async static Task PlayCaptchaSoundAsync()
    {
        using SoundPlayer player = new();
        player.SoundLocation = "D:\\Microsoft Visual Studio 2022\\ImportantProjects\\Sygnalista_A\\Sygnalista_A\\Miscellaneous\\captcha.wav";
        player.Play();
    }

    public async static Task PlaySoundAsync()
    {
        using SoundPlayer player = new();
        player.SoundLocation = "D:\\Microsoft Visual Studio 2022\\ImportantProjects\\Sygnalista_A\\Sygnalista_A\\Miscellaneous\\waterdrop.wav";
        player.Play();
    }
}