using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;

public interface IConventionUnitOfWork
{
    Task<ActionResponse<Convention>> AddAsync(ConventionDTO entity);
    Task<ActionResponse<Convention>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<Convention>>> GetAsync();
    Task<ActionResponse<Convention>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<Convention>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<Convention>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<Convention>> UpdateAsync(ConventionDTO entity);
}
