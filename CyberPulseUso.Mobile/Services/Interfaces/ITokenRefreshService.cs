namespace CyberPulseUso.Mobile.Services.Interfaces;

public interface ITokenRefreshService
{
    Task<bool> RefreshAsync();
}