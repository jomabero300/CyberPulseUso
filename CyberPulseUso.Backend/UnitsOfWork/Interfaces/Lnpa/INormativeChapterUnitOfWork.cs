using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;

public interface INormativeChapterUnitOfWork : IGenericUnitOfWork<NormativeChapter>
{
    Task<ActionResponse<IEnumerable<NormativeChapter>>> GetByNormativeAsync(int normativeId);
}