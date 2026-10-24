using CyberPulseUso.Shared.Responses;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace CyberPulseUso.Backend.Helpers;

public class MailHelper : IMailHelper
{
    private readonly IConfiguration _configuration;

    public MailHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<ActionResponse<string>> SendMail(string toName, string toEmail, string subject, string body, string language)
    {
        try
        {
            var from = _configuration["Mail:From"];
            var name = language.Equals("es", StringComparison.OrdinalIgnoreCase)
                ? _configuration["Mail:NameEs"]
                : _configuration["Mail:NameEn"];
            var smtp = _configuration["Mail:Smtp"];
            var portValue = _configuration["Mail:Port"];
            var password = _configuration["Mail:Password"];

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(smtp) ||
                !int.TryParse(portValue, out var port) || string.IsNullOrWhiteSpace(password))
            {
                return new ActionResponse<string>
                {
                    WasSuccess = false,
                    Message = "La configuraciÃ³n SMTP estÃ¡ incompleta o no es vÃ¡lida."
                };
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(name ?? from, from));
            message.To.Add(new MailboxAddress(toName ?? string.Empty, toEmail));
            message.Subject = subject;
            message.Headers.Add("X-Mailer", "CyberPulse");
            message.Headers.Add("X-Priority", "3");

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body,
                TextBody = HtmlUtilities.StripTags(body)
            };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient
            {
                Timeout = 30000
            };

            // Se mantiene la validaciÃ³n TLS predeterminada de MailKit; no aceptar certificados invÃ¡lidos.
            await client.ConnectAsync(smtp, port, SecureSocketOptions.StartTlsWhenAvailable);
            await client.AuthenticateAsync(from, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return new ActionResponse<string> { WasSuccess = true };
        }
        catch (Exception ex)
        {
            return new ActionResponse<string>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }
}
