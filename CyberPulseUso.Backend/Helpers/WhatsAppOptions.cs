namespace CyberPulseUso.Backend.Helpers;

public class WhatsAppOptions
{
    public const string SectionName = "WhatsApp";
    public string PhoneNumberId { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string ApiVersion { get; set; } = "v26.0";
}
