using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public interface INormativeChapterRepository : IGenericRepository<NormativeChapter>
{
    Task<ActionResponse<IEnumerable<NormativeChapter>>> GetByNormativeAsync(int normativeId);
}