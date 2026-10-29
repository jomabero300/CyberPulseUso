using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Lnpa;

public class PayrollConceptUnitOfWork : GenericUnitOfWork<PayrollConcept>, IPayrollConceptUnitOfWork
{
    private readonly IPayrollConceptRepository _repository;

    public PayrollConceptUnitOfWork(IGenericRepository<PayrollConcept> repository, IPayrollConceptRepository modelRepository)
        : base(repository)
    {
        _repository = modelRepository;
    }

    public override Task<ActionResponse<PayrollConcept>> GetAsync(int id) => _repository.GetAsync(id);
    public override Task<ActionResponse<IEnumerable<PayrollConcept>>> GetAsync(PaginationDTO pagination) => _repository.GetAsync(pagination);
    public override Task<ActionResponse<PayrollConcept>> DeleteAsync(int id) => _repository.DeleteAsync(id);
    public Task<ActionResponse<PayrollConcept>> AddAsync(PayrollConceptDTO entity) => _repository.AddAsync(entity);
    public Task<IEnumerable<PayrollConcept>> GetComboAsync() => _repository.GetComboAsync();
    public Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => _repository.GetTotalRecordsAsync(pagination);
    public Task<ActionResponse<PayrollConcept>> UpdateAsync(PayrollConceptDTO entity) => _repository.UpdateAsync(entity);
}
