using CyberPulseUso.Backend.UnitsOfWork.Implementations;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public class NormativeArticleUnitOfWork : GenericUnitOfWork<NormativeArticle>, INormativeArticleUnitOfWork
{
    private readonly INormativeArticleRepository _repository;

    public NormativeArticleUnitOfWork(INormativeArticleRepository repository) : base(repository)
    {
        _repository = repository;
    }

    public async Task<ActionResponse<IEnumerable<NormativeArticle>>> GetByChapterAsync(int chapterId)
        => await _repository.GetByChapterAsync(chapterId);
}