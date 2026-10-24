namespace CyberPulseUso.Backend.Helpers;

public interface IWhatsAppService
{
    Task<string> SendTextAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default);

    Task<string> SendTemplateAsync(
        string phoneNumber,
        string templateName,
        string languageCode,
        string variable1,
        string variable2,
        CancellationToken cancellationToken = default);
}
