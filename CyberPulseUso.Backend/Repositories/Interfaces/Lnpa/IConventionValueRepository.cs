using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public interface IConventionValueRepository
{
    Task<ActionResponse<ConventionValue>> AddAsync(ConventionValueDTO entity);
    Task<ActionResponse<ConventionValue>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<ConventionValue>>> GetAsync();
    Task<ActionResponse<ConventionValue>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<ConventionValue>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<ConventionValue>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<ConventionValue>> UpdateAsync(ConventionValueDTO entity);
}
