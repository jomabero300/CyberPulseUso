using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;

public interface ILaborRemunerationParametersValueUnitOfWork
{
    Task<ActionResponse<LaborRemunerationParametersValue>> AddAsync(LaborRemunerationParametersValueDTO entity);
    Task<ActionResponse<LaborRemunerationParametersValue>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<LaborRemunerationParametersValue>>> GetAsync();
    Task<ActionResponse<LaborRemunerationParametersValue>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<LaborRemunerationParametersValue>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<LaborRemunerationParametersValue>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<LaborRemunerationParametersValue>> UpdateAsync(LaborRemunerationParametersValueDTO entity);
}
