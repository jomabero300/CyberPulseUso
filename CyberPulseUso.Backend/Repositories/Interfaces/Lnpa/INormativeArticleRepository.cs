using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public interface INormativeArticleRepository : IGenericRepository<NormativeArticle>
{
    Task<ActionResponse<IEnumerable<NormativeArticle>>> GetByChapterAsync(int chapterId);
}