using CyberPulseUso.Shared.Entities.Gene;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Gene;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByHashAsync(string tokenHash);

    Task AddAsync(RefreshToken entity);

    Task<bool> RotateAsync( string currentTokenHash, RefreshToken replacement, DateTime now);

    Task RevokeAllAsync(string userId);

    Task RevokeFamilyAsync(Guid familyId, DateTime now);
}