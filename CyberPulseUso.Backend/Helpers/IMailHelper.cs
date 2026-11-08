using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Helpers;

public interface IMailHelper
{
    Task<ActionResponse<string>> SendMail(string toName, string toEmail, string subject, string body, string language);
    Task<ActionResponse<string>> SendMail(string toName, string toEmail, string subject, string pathToHtmlTemplate, string language, string tokenLink, string pathToImage);
}
