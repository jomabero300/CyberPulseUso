using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa.Reports;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public interface IPayrollSettlementRepository
{
    Task<ActionResponse<IEnumerable<PayrollSettlement>>> AddAsync(IEnumerable<PayrollSettlementDTO> entity);
    Task<ActionResponse<PayrollSettlement>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<PayrollSettlement>>> GetAsync();
    Task<ActionResponse<PayrollSettlement>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<PayrollSettlement>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<PayrollSettlement>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<List<PayrollSettlement>>> UpdateAsync(List<PayrollSettlementDTO> entity);
    Task<ActionResponse<IEnumerable<PayrollSettlementDTO>>> GetLiqAsync(int id,string Id);
    Task<ActionResponse<IEnumerable<PayrollSettlementListDTO>>> GetAsync(string Id);
    Task<ActionResponse<IEnumerable<PayrollSettlementDTO>>> GetAsync(DateTime fecha, int Id,string id);
    Task<ActionResponse<LiquidacionReportModel>> GetAsync(DateTime fecha, int Id, string id, bool esta);
}
