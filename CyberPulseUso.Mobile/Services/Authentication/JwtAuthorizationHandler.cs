using CyberPulseUso.Mobile.Services.Interfaces;
using System.Net.Http.Headers;

namespace CyberPulseUso.Mobile.Services.Authentication;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly ITokenService _tokenService;
    private readonly ITokenRefreshService _refreshService;

    public JwtAuthorizationHandler(ITokenService tokenService,ITokenRefreshService refreshService)
    {
        _tokenService = tokenService;
        _refreshService = refreshService;
    }
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,CancellationToken cancellationToken)
    {
        var path = request.RequestUri?.AbsolutePath ?? string.Empty;

        var isAuthenticationEndpoint =
            path.EndsWith(
                "/api/Accounts/Login",
                StringComparison.OrdinalIgnoreCase) ||
            path.EndsWith(
                "/api/Accounts/Refresh",
                StringComparison.OrdinalIgnoreCase);

        if (isAuthenticationEndpoint)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var expiration = await _tokenService.GetExpirationAsync();

        if (expiration.HasValue &&
            expiration.Value <= DateTime.UtcNow.AddMinutes(1))
        {
            await _refreshService.RefreshAsync();
        }

        var token = await _tokenService.GetAccessTokenAsync();

        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}