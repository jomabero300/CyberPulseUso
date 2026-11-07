using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using Microsoft.EntityFrameworkCore;

namespace CyberPulseUso.Backend.Repositories.Implementations.Gene;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken entity)
    {
        await _context.RefreshTokens.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
    {
        return await _context.RefreshTokens.AsNoTracking()
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
    }

    public async Task RevokeAllAsync(string userId)
    {
        var now = DateTime.UtcNow;

        await _context.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                x.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.RevokedAt, now));
    }

    public async Task RevokeFamilyAsync(Guid familyId, DateTime now)
    {
        await _context.RefreshTokens
            .Where(x =>
                x.FamilyId == familyId &&
                x.RevokedAt == null)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.RevokedAt, now));
    }

    public async Task<bool> RotateAsync(string currentTokenHash, RefreshToken replacement, DateTime now)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync();

        try
        {
            // Actualización condicional: solamente una solicitud
            // puede consumir el Refresh Token vigente.
            var affected = await _context.RefreshTokens
                .Where(x =>
                    x.TokenHash == currentTokenHash &&
                    x.RevokedAt == null &&
                    x.ExpiresAt > now &&
                    x.SessionExpiresAt > now)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(x => x.RevokedAt, now)
                    .SetProperty(
                        x => x.ReplacedByTokenHash,
                        replacement.TokenHash));

            if (affected != 1)
            {
                await transaction.RollbackAsync();
                return false;
            }

            await _context.RefreshTokens.AddAsync(replacement);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
