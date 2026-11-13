namespace CyberPulseUso.Mobile.Services.Interfaces;

public interface IApiRepository
{
    Task<T?> GetAsync<T>(string url,CancellationToken cancellationToken = default);

    Task<TResponse?> PostAsync<TRequest, TResponse>(string url,TRequest data,CancellationToken cancellationToken = default);

    Task<TResponse?> PutAsync<TRequest, TResponse>(string url,TRequest data,CancellationToken cancellationToken = default);

    Task DeleteAsync(string url,CancellationToken cancellationToken = default);
    Task<byte[]> GetFileAsync(string url,CancellationToken cancellationToken = default);
}
