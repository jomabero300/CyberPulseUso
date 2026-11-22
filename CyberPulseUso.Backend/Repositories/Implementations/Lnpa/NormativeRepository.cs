using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace CyberPulseUso.Backend.Repositories.Implementations.Lnpa;

public class NormativeRepository : GenericRepository<Normative>, INormativeRepository
{
    private readonly ApplicationDbContext _context;

    public NormativeRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ActionResponse<NormativeTreeDTO?>> GetTreeAsync(int id)
    {
        var normative = await _context.Normatives
            .AsNoTracking()
            .Where(x => x.Id == id)
            .Select(x => new NormativeTreeDTO
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,

                Chapters = x.Chapters
                    .OrderBy(c => c.SortOrder)
                    .Select(c => new NormativeChapterDTO
                    {
                        Id = c.Id,
                        NormativeId = c.NormativeId,
                        Name = c.Name,
                        Description = c.Description,
                        SortOrder = c.SortOrder,

                        Articles = c.Articles
                            .OrderBy(a => a.SortOrder)
                            .Select(a => new NormativeArticleDTO
                            {
                                Id = a.Id,
                                NormativeChapterId = a.NormativeChapterId,
                                Name = a.Name,
                                Description = a.Description,
                                Content = a.Content,
                                SortOrder = a.SortOrder
                            }).ToList()

                    }).ToList()
            })
            .FirstOrDefaultAsync();

        if (normative == null)
        {
            return new ActionResponse<NormativeTreeDTO?>
            {
                WasSuccess = false,
                Message = "ERR001"
            };
        }

        return new ActionResponse<NormativeTreeDTO?>
        {
            WasSuccess = true,
            Result = normative
        };
    }

}