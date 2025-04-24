using HtmlAgilityPack;

namespace Library.BiznesRadar;

public class CompanyName
{
    public async static Task<string> Get(string html)
    {
        HtmlDocument doc = new();
        doc.LoadHtml(html);
        HtmlNode h1Node = doc.DocumentNode.SelectSingleNode("//div[@id='profile-header']//h1");
        string extractedText = h1Node.InnerText;
        string[] textParts = extractedText.Split("strat", StringSplitOptions.RemoveEmptyEntries);

        return textParts.Length > 1 ? textParts[1].Trim() : string.Empty;
    }
}
