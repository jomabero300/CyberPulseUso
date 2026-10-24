namespace CyberPulseUso.Backend.Services;

public interface ILoginService
{
    Task LoginAsync(string token);
    Task LogoutAsync();
}
