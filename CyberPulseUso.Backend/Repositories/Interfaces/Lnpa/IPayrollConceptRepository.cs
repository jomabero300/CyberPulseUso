using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public interface IPayrollConceptRepository
{
    Task<ActionResponse<PayrollConcept>> AddAsync(PayrollConceptDTO entity);
    Task<ActionResponse<PayrollConcept>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<PayrollConcept>>> GetAsync();
    Task<ActionResponse<PayrollConcept>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<PayrollConcept>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<PayrollConcept>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<PayrollConcept>> UpdateAsync(PayrollConceptDTO entity);
}
