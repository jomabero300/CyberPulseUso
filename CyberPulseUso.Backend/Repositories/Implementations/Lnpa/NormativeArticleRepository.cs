using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace CyberPulseUso.Backend.Repositories.Implementations.Lnpa;

public class NormativeArticleRepository    : GenericRepository<NormativeArticle>, INormativeArticleRepository
{
    private readonly ApplicationDbContext _context;

    public NormativeArticleRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<IEnumerable<NormativeArticle>>> GetByChapterAsync(int chapterId)
    {
        var articles = await _context.NormativeArticles
            .AsNoTracking()
            .Where(x => x.NormativeChapterId == chapterId)
            .OrderBy(x => x.SortOrder)
            .ToListAsync();

        return new ActionResponse<IEnumerable<NormativeArticle>>
        {
            WasSuccess = true,
            Result = articles
        };
    }
}