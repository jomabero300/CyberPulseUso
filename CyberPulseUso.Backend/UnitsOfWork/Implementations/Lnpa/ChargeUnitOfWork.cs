using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Lnpa;

public class ChargeUnitOfWork : GenericUnitOfWork<Charge>, IChargeUnitOfWork
{
    private readonly IChargeRepository _repository;

    public ChargeUnitOfWork(IGenericRepository<Charge> repository, IChargeRepository modelRepository)
        : base(repository)
    {
        _repository = modelRepository;
    }

    public override Task<ActionResponse<Charge>> GetAsync(int id) => _repository.GetAsync(id);
    public override Task<ActionResponse<IEnumerable<Charge>>> GetAsync(PaginationDTO pagination) => _repository.GetAsync(pagination);
    public override Task<ActionResponse<Charge>> DeleteAsync(int id) => _repository.DeleteAsync(id);
    public Task<ActionResponse<Charge>> AddAsync(ChargeDTO entity) => _repository.AddAsync(entity);
    public Task<IEnumerable<Charge>> GetComboAsync() => _repository.GetComboAsync();
    public Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => _repository.GetTotalRecordsAsync(pagination);
    public Task<ActionResponse<Charge>> UpdateAsync(ChargeDTO entity) => _repository.UpdateAsync(entity);
}
