using HtmlAgilityPack;

namespace Library.BiznesRadar;

public class Capitalization
{
    public async static Task<string> Get(string html)
    {
        HtmlDocument doc = new();
        doc.LoadHtml(html);
        HtmlNode thNode = doc.DocumentNode.SelectSingleNode("//th[text()='Kapitalizacja:']");

        if (thNode != null)
        {
            HtmlNode tdNode = thNode.SelectSingleNode("following-sibling::td");
            if (tdNode != null)
            {
                string innerText = tdNode.InnerText.Trim();
                return innerText;
            }
        }

        return string.Empty;
    }
}
