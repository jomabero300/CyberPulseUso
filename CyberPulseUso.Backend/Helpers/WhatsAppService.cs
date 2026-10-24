using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CyberPulseUso.Backend.Helpers;

public class WhatsAppService : IWhatsAppService
{
    private readonly HttpClient _httpClient;
    private readonly WhatsAppOptions _options;

    public WhatsAppService(HttpClient httpClient, IOptions<WhatsAppOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<string> SendTemplateAsync(
        string phoneNumber,
        string templateName,
        string languageCode,
        string variable1,
        string variable2,
        CancellationToken cancellationToken = default)
    {
        var url = BuildMessagesUrl();
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);
        request.Content = JsonContent.Create(new
        {
            messaging_product = "whatsapp",
            to = phoneNumber,
            type = "template",
            template = new
            {
                name = templateName,
                language = new { code = languageCode },
                components = new[]
                {
                    new
                    {
                        type = "body",
                        parameters = new[]
                        {
                            new { type = "text", text = variable1 },
                            new { type = "text", text = variable2 }
                        }
                    }
                }
            }
        });

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"WhatsApp API error ({(int)response.StatusCode}): {content}");

        return content;
    }

    public async Task<string> SendTextAsync(
        string phoneNumber,
        string message,
        CancellationToken cancellationToken = default)
    {
        var url = BuildMessagesUrl();
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.AccessToken);
        request.Content = JsonContent.Create(new
        {
            messaging_product = "whatsapp",
            recipient_type = "individual",
            to = phoneNumber,
            type = "text",
            text = new
            {
                preview_url = false,
                body = message
            }
        });

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"WhatsApp API error ({(int)response.StatusCode}): {content}");

        return content;
    }

    private string BuildMessagesUrl()
    {
        if (string.IsNullOrWhiteSpace(_options.ApiVersion) ||
            string.IsNullOrWhiteSpace(_options.PhoneNumberId) ||
            string.IsNullOrWhiteSpace(_options.AccessToken))
        {
            throw new InvalidOperationException("La configuraciÃ³n de WhatsApp estÃ¡ incompleta.");
        }

        return $"https://graph.facebook.com/{_options.ApiVersion}/{_options.PhoneNumberId}/messages";
    }
}
