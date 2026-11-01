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
    public Task<ActionResponse<IEnumerable<PayrollSettlement>>> AddAsync(IEnumerable<PayrollSettlementDTO> entity) => _repository.AddAsync(entity);
    public Task<IEnumerable<PayrollSettlement>> GetComboAsync() => _repository.GetComboAsync();
    public Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => _repository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<IEnumerable<PayrollSettlementDTO>>> GetLiqAsync(int id,string Id) => await _repository.GetLiqAsync(id,Id);

    public async Task<ActionResponse<IEnumerable<PayrollSettlementDTO>>> GetAsync(DateTime fecha, int Id, string id) => await _repository.GetAsync(fecha, Id, id);

    public async Task<ActionResponse<List<PayrollSettlement>>> UpdateAsync(List<PayrollSettlementDTO> entity) => await _repository.UpdateAsync(entity);
}
