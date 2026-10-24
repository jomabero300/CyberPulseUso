using CyberPulseUso.Shared.Entities.Gene;
using Microsoft.AspNetCore.Identity;

namespace CyberPulseUso.Backend.Helpers;

public class CustomEmailValidator : IUserValidator<User>
{
    private readonly HashSet<string> _allowedEmailDomains = new(StringComparer.OrdinalIgnoreCase)
    {
        "sena.edu.co",
        "sedarauca.edu.co"
    };

    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user)
    {
        var email = user.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = "EmailDomainInvalid",
                Description = "El correo electrÃ³nico es obligatorio y debe ser de un dominio vÃ¡lido."
            }));
        }

        var separatorIndex = email.LastIndexOf('@');
        var domain = separatorIndex >= 0 && separatorIndex < email.Length - 1
            ? email[(separatorIndex + 1)..]
            : string.Empty;

        if (string.IsNullOrWhiteSpace(domain) || !_allowedEmailDomains.Contains(domain))
        {
            return Task.FromResult(IdentityResult.Failed(new IdentityError
            {
                Code = "EmailDomainInvalid",
                Description = "El correo electrÃ³nico debe ser de un dominio permitido (sena.edu.co o sedarauca.edu.co)."
            }));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}
