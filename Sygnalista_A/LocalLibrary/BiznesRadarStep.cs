using Library;
using Library.BiznesRadar;
using Sygnalista_A.LocalLibrary.Services;

namespace Sygnalista_A.LocalLibrary;

public class BiznesRadarStep(CompanyInfoManager companyInfoManager)
{
    public async Task Execute(string companyCode)
    {
        if (!string.IsNullOrEmpty(companyCode))
        {
            string biznesRadarHtml = await DownloadHtml.DownloadHtmlAsync($"https://www.biznesradar.pl/raporty-finansowe-rachunek-zyskow-i-strat/{companyCode}");
            _ = SaveTextToFile.SaveAsync("biznesRadarHtml", biznesRadarHtml);

            if (biznesRadarHtml is not null)
            {
                companyInfoManager.Capitalization = await Capitalization.Get(biznesRadarHtml);
                companyInfoManager.CompanyName = await CompanyName.Get(biznesRadarHtml);

                if (companyInfoManager.CompanyName.Contains(companyCode))
                {
                    companyInfoManager.CompanyName = companyCode;
                }
            }
        }

        else
        {
            companyInfoManager.Capitalization = "";
            companyInfoManager.CompanyName = "";
        }
    }
}
