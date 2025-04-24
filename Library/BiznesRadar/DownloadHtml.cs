namespace Library.BiznesRadar;

public class DownloadHtml
{
    public async static Task<string> DownloadHtmlAsync(string urlFragment, string quarterAtTheEnd = ",Q")
    {
        string url = $"{urlFragment}{quarterAtTheEnd}";
        return await DownloadHtmlWithRedirectAsync(url);
    }

    private async static Task<string> DownloadHtmlWithRedirectAsync(string url)
    {
        HttpClientHandler handler = new()
        {
            ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
        };

        using HttpClient client = new(handler);
        using HttpResponseMessage response = await client.GetAsync(url);
        string requestUri = response.RequestMessage.RequestUri.ToString();

        if (requestUri != url)
        {
            return await DownloadHtmlWithRedirectAsync($"{requestUri},Q");
        }

        using HttpContent content = response.Content;
        var html = await content.ReadAsStringAsync();

        return html;
    }
}
