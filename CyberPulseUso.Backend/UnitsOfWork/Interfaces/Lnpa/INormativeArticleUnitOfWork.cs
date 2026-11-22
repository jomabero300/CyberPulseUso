using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;

public interface INormativeArticleUnitOfWork : IGenericUnitOfWork<NormativeArticle>
{
    Task<ActionResponse<IEnumerable<NormativeArticle>>> GetByChapterAsync(int chapterId);
}