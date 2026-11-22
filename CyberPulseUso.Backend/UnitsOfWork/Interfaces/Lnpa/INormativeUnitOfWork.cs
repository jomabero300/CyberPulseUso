using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;

public interface INormativeUnitOfWork : IGenericUnitOfWork<Normative>
{
    Task<ActionResponse<NormativeTreeDTO?>> GetTreeAsync(int id);
}