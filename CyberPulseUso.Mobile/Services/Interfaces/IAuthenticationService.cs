using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Gene;

namespace CyberPulseUso.Mobile.Services.Interfaces;

public interface IAuthenticationService
{
    Task<TokenDTO?> LoginAsync(LoginDTO login);

    Task LogoutAsync();

    Task<bool> IsAuthenticatedAsync();

    Task<TokenDTO?> UpdateProfileAsync(User user, bool rememberMe);
}