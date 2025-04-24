using HtmlDocument = HtmlAgilityPack.HtmlDocument;

namespace Sygnalista_A.LocalLibrary.Html;

public class ProcessHtml
{
    private readonly List<string> tagsToIgnore =
    [
    "ALIOR", "BNPPPL", "CEZ", "CYFRPLSAT", "ENEA", "ENERGA",
    "ETF", "FIZ", "GMINA MIASTA", "HANDLOWY", "IIAAV", "INGBSK",
    "KRKA", "LEGATO ABSOLUTNEJ", "MBANK", "MILLENNIUM",
    "MOL", "ORANGEPL", "PGE", "PEKAO", "PKNORLEN", "PKOBP",
    "PZU", "SANPL", "SANTANDER", "SOPHARMA", "UNICREDIT"
    ];

    public async Task<string> PrepareInformation(string html)
    {
        HtmlDocument document = new();
        document.LoadHtml(html);

        return document.DocumentNode.SelectSingleNode("//span[@class='news-issuer-name-link']").InnerText.Trim();
    }

    public async Task<bool> IsForbiddenTags(string senderText) => tagsToIgnore.Any(q => q.Equals(senderText, StringComparison.OrdinalIgnoreCase));
}
