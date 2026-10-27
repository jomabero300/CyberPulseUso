using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public interface ILaborRemunerationParametersRepository
{
    Task<ActionResponse<LaborRemunerationParameters>> AddAsync(LaborRemunerationParametersDTO entity);
    Task<ActionResponse<LaborRemunerationParameters>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<LaborRemunerationParameters>>> GetAsync();
    Task<ActionResponse<LaborRemunerationParameters>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<LaborRemunerationParameters>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<LaborRemunerationParameters>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<LaborRemunerationParameters>> UpdateAsync(LaborRemunerationParametersDTO entity);
}
