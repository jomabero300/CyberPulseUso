using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace CyberPulseUso.Backend.Repositories.Implementations.Lnpa;

public class NormativeChapterRepository    : GenericRepository<NormativeChapter>, INormativeChapterRepository
{
    private readonly ApplicationDbContext _context;

    public NormativeChapterRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<NormativeChapter>>> GetByNormativeAsync(int normativeId)
    {
        var chapters = await _context.NormativeChapters
            .AsNoTracking()
            .Where(x => x.NormativeId == normativeId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        return new ActionResponse<IEnumerable<NormativeChapter>>
        {
            WasSuccess = true,
            Result = chapters
        };
    }
}