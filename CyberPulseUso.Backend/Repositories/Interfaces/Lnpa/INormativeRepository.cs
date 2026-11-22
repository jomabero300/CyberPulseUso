using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public interface INormativeRepository : IGenericRepository<Normative>
{
    Task<ActionResponse<NormativeTreeDTO?>> GetTreeAsync(int id);
}