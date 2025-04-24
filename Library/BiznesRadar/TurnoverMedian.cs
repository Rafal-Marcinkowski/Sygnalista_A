using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Library.BiznesRadar;

public class TurnoverMedian
{
    public static async Task<string> Get(string html)
    {
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var rows = doc.DocumentNode.SelectNodes("//table[@class='qTableFull']/tr");

        if (rows == null)
            return "0";

        var turnoverValues = new List<decimal>();

        foreach (var row in rows)
        {
            var cells = row.SelectNodes("td");

            if (cells?.Count >= 7)
            {
                var turnoverStr = cells[6].InnerText.Trim();
                if (decimal.TryParse(Regex.Replace(turnoverStr, @"\s+", ""), out var turnover))
                {
                    turnoverValues.Add(turnover);
                    if (turnoverValues.Count >= 20)
                        break;
                }
            }
        }

        if (turnoverValues.Count < 20)
            return "0";

        var medianTurnover = turnoverValues.Order().ElementAt(turnoverValues.Count / 2);
        return ((int)medianTurnover).ToString();
    }
}
