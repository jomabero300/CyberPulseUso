using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;

public interface ILogAuditoriaUnitOfWork
{
    Task<ActionResponse<LogAuditoria>> AddAsync(LogAuditoriaDTO entity);
    Task<ActionResponse<LogAuditoria>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<LogAuditoria>>> GetAsync();
    Task<ActionResponse<LogAuditoria>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<LogAuditoria>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<LogAuditoria>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<LogAuditoria>> UpdateAsync(LogAuditoriaDTO entity);
}
