using HtmlAgilityPack;

namespace Library.BiznesRadar;

public class RefreshDate
{
    public async static Task<string> Get(string html)
    {
        HtmlDocument doc = new();
        doc.LoadHtml(html);

        var dateNode = doc.DocumentNode.SelectSingleNode("//i[contains(text(), 'Dane pobrano:')]");

        string dateText = null;
        if (dateNode != null)
        {
            dateText = dateNode.InnerText.Replace("Dane pobrano: ", "").Trim()[11..];
        }

        return dateText ?? "brak";
    }
}
