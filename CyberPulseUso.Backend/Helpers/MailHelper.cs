using CyberPulseUso.Shared.Responses;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Utils;

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
                    Message = "La configuraciòn SMTP está¡ incompleta o no es vólida."
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


    public async Task<ActionResponse<string>> SendMail(string toName,string toEmail,string subject,string pathToHtmlTemplate,string language,string tokenLink,string pathToImage)
    {
        try
        {
            // Cargar credenciales desde el appsettings.json
            var from = _configuration["Mail:From"];
            var name = language.Equals("es", StringComparison.OrdinalIgnoreCase) ? _configuration["Mail:NameEs"] : _configuration["Mail:NameEn"];
            var smtp = _configuration["Mail:Smtp"];
            var portValue = _configuration["Mail:Port"];
            var password = _configuration["Mail:Password"];

            if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(smtp) || !int.TryParse(portValue, out var port) || string.IsNullOrWhiteSpace(password))
            {
                return new ActionResponse<string> { WasSuccess = false, Message = "La configuración SMTP está incompleta." };
            }

            // 1. Validar y leer la plantilla HTML desde el archivo físico
            if (!File.Exists(pathToHtmlTemplate))
            {
                return new ActionResponse<string> { WasSuccess = false, Message = $"No se encontró la plantilla en: {pathToHtmlTemplate}" };
            }
            string htmlRaw = await File.ReadAllTextAsync(pathToHtmlTemplate);

            // 2. Extraer automáticamente el Asunto (Subject) de la etiqueta <title> del HTML
            if (string.IsNullOrWhiteSpace(subject))
            {
                int inicioTitle = htmlRaw.IndexOf("<title>", StringComparison.OrdinalIgnoreCase);
                int finTitle = htmlRaw.IndexOf("</title>", StringComparison.OrdinalIgnoreCase);

                if (inicioTitle != -1 && finTitle != -1)
                {
                    inicioTitle += 7; // Longitud de la etiqueta "<title>"
                    subject = htmlRaw.Substring(inicioTitle, finTitle - inicioTitle).Trim();
                }
                else
                {
                    // Respaldo por seguridad si no existe la etiqueta <title>
                    subject = language.Equals("es", StringComparison.OrdinalIgnoreCase) ? "Confirmación de cuenta" : "Account confirmation";
                }
            }

            // Configuración básica del mensaje MailKit
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(name ?? from, from));
            message.To.Add(new MailboxAddress(toName ?? string.Empty, toEmail));
            message.Subject = subject;
            message.Headers.Add("X-Mailer", "CyberPulse");

            var bodyBuilder = new BodyBuilder();
            string logoContentId = "logo_fallback";

            // 3. Incrustar físicamente el logo en el paquete del correo
            if (!string.IsNullOrWhiteSpace(pathToImage) && File.Exists(pathToImage))
            {
                var imageResource = bodyBuilder.LinkedResources.Add(pathToImage);
                imageResource.ContentId = MimeUtils.GenerateMessageId(); // ID dinámico único
                logoContentId = imageResource.ContentId;
            }

            // 4. Inyectar los parámetros dinámicos reemplazando las etiquetas del HTML
            string finalHtml = htmlRaw
                .Replace("{ConfirmationLink}", tokenLink)
                .Replace("{LogoCid}", logoContentId);

            bodyBuilder.HtmlBody = finalHtml;
            bodyBuilder.TextBody = HtmlUtilities.StripTags(finalHtml);
            message.Body = bodyBuilder.ToMessageBody();

            // 5. Conexión SMTP y envío con MailKit
            using var client = new SmtpClient { Timeout = 30000 };
            await client.ConnectAsync(smtp, port, SecureSocketOptions.StartTlsWhenAvailable);
            await client.AuthenticateAsync(from, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            return new ActionResponse<string> { WasSuccess = true };
        }
        catch (Exception ex)
        {
            return new ActionResponse<string> { WasSuccess = false, Message = ex.Message };
        }
    }

}
