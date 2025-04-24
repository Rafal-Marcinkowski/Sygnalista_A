namespace Sygnalista_A.LocalLibrary.Services;

public class CompanyInfoManager : BindableBase
{
    public string CompanyCode { get; set; } = string.Empty;

    private string companyName;
    public string CompanyName
    {
        get => companyName;
        set => SetProperty(ref companyName, value);
    }

    private string medianaText;
    public string MedianaText
    {
        get => medianaText;
        set => SetProperty(ref medianaText, value);
    }

    private string capitalization;
    public string Capitalization
    {
        get => capitalization;
        set => SetProperty(ref capitalization, value);
    }

    public async Task CreateMediana() => MedianaText = await NameTranslation.GetTurnoverMedianForCompany(CompanyCode);

    public async Task CreateCompanyCode(string companyName) => CompanyCode = await NameTranslation.ConvertHeaderToTranslation(companyName);
}
