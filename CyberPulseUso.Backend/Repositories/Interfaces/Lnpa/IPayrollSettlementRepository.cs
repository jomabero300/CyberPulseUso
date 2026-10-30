using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public interface IPayrollSettlementRepository
{
    Task<ActionResponse<PayrollSettlement>> AddAsync(PayrollSettlementDTO entity);
    Task<ActionResponse<PayrollSettlement>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<PayrollSettlement>>> GetAsync();
    Task<ActionResponse<PayrollSettlement>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<PayrollSettlement>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<PayrollSettlement>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<PayrollSettlement>> UpdateAsync(PayrollSettlementDTO entity);

    Task<ActionResponse<IEnumerable<PayrollSettlementDTO>>> GetLiqAsync(int id,string Id);
}
