using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public interface IChargeRepository
{
    Task<ActionResponse<Charge>> AddAsync(ChargeDTO entity);
    Task<ActionResponse<Charge>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<Charge>>> GetAsync();
    Task<ActionResponse<Charge>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<Charge>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<Charge>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<Charge>> UpdateAsync(ChargeDTO entity);
}
