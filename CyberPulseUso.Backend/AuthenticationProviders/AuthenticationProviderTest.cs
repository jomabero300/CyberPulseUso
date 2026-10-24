using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace CyberPulseUso.Backend.AuthenticationProviders;

public class AuthenticationProviderTest : AuthenticationStateProvider
{
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await Task.Delay(4000);

        var anonimous = new ClaimsIdentity();
        var user = new ClaimsIdentity(authenticationType: "test");
        var admin = new ClaimsIdentity(
        [
            new Claim("FirstName", "Juan"),
            new Claim("LastName", "Zulu"),
            new Claim(ClaimTypes.Name, "zulu@yopmail.com"),
            new Claim(ClaimTypes.Role, "Admin")
        ],
        authenticationType: "test");

        return await Task.FromResult(new AuthenticationState(new ClaimsPrincipal(admin)));
    }
}
