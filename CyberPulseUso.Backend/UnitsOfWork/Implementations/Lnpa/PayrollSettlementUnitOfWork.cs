using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Lnpa;

public class PayrollSettlementUnitOfWork : GenericUnitOfWork<PayrollSettlement>, IPayrollSettlementUnitOfWork
{
    private readonly IPayrollSettlementRepository _repository;

    public PayrollSettlementUnitOfWork(IGenericRepository<PayrollSettlement> repository, IPayrollSettlementRepository modelRepository)
        : base(repository)
    {
        _repository = modelRepository;
    }

    public override Task<ActionResponse<PayrollSettlement>> GetAsync(int id) => _repository.GetAsync(id);
    public override Task<ActionResponse<IEnumerable<PayrollSettlement>>> GetAsync(PaginationDTO pagination) => _repository.GetAsync(pagination);
    public override Task<ActionResponse<PayrollSettlement>> DeleteAsync(int id) => _repository.DeleteAsync(id);
    public Task<ActionResponse<PayrollSettlement>> AddAsync(PayrollSettlementDTO entity) => _repository.AddAsync(entity);
    public Task<IEnumerable<PayrollSettlement>> GetComboAsync() => _repository.GetComboAsync();
    public Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => _repository.GetTotalRecordsAsync(pagination);
    public Task<ActionResponse<PayrollSettlement>> UpdateAsync(PayrollSettlementDTO entity) => _repository.UpdateAsync(entity);
}
