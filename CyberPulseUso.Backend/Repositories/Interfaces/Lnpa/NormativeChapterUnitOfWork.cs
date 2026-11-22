using CyberPulseUso.Backend.UnitsOfWork.Implementations;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public class NormativeChapterUnitOfWork : GenericUnitOfWork<NormativeChapter>, INormativeChapterUnitOfWork
{
    private readonly INormativeChapterRepository _repository;

    public NormativeChapterUnitOfWork(INormativeChapterRepository repository) : base(repository)
    {
        _repository = repository;
    }

    public async Task<ActionResponse<IEnumerable<NormativeChapter>>> GetByNormativeAsync(int normativeId)
        => await _repository.GetByNormativeAsync(normativeId);
}