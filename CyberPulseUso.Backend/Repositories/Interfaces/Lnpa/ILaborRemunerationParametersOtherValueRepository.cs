using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public interface ILaborRemunerationParametersOtherValueRepository
{
    Task<ActionResponse<IEnumerable<LaborRemunerationParametersOtherValueCRUDDTO>>> AddAsync(IEnumerable<LaborRemunerationParametersOtherValueCRUDDTO> entity);
    Task<ActionResponse<LaborRemunerationParametersOtherValue>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<LaborRemunerationParametersOtherValue>>> GetAsync();
    Task<ActionResponse<LaborRemunerationParametersOtherValue>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<LaborRemunerationParametersOtherValue>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<LaborRemunerationParametersOtherValue>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<LaborRemunerationParametersOtherValue>> UpdateAsync(LaborRemunerationParametersOtherValueDTO entity);
}
