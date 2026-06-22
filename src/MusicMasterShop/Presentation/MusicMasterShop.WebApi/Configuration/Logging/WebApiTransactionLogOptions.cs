namespace MusicMasterShop.WebApi.Configuration.Logging;

public sealed class WebApiTransactionLogOptions
{
    public const string SectionName = "WebApiTransactionLogging";

    public bool Enabled { get; set; } = true;
    public int MaxBodyLength { get; set; } = 32_768;
    public string PathPrefix { get; set; } = "/mmshop/v";
    public string RedactionValue { get; set; } = "[REDACTED]";
    public string[] SensitiveFields { get; set; } =
    [
        "authorization",
        "senha",
        "password",
        "token",
        "accessToken",
        "refreshToken",
        "documentoCliente"
    ];
}
