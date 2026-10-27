using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Lnpa;

public class ConventionValueUnitOfWork : GenericUnitOfWork<ConventionValue>, IConventionValueUnitOfWork
{
    private readonly IConventionValueRepository _repository;

    public ConventionValueUnitOfWork(IGenericRepository<ConventionValue> repository, IConventionValueRepository modelRepository)
        : base(repository)
    {
        _repository = modelRepository;
    }

    public override Task<ActionResponse<ConventionValue>> GetAsync(int id) => _repository.GetAsync(id);
    public override Task<ActionResponse<IEnumerable<ConventionValue>>> GetAsync(PaginationDTO pagination) => _repository.GetAsync(pagination);
    public override Task<ActionResponse<ConventionValue>> DeleteAsync(int id) => _repository.DeleteAsync(id);
    public Task<ActionResponse<ConventionValue>> AddAsync(ConventionValueDTO entity) => _repository.AddAsync(entity);
    public Task<IEnumerable<ConventionValue>> GetComboAsync() => _repository.GetComboAsync();
    public Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => _repository.GetTotalRecordsAsync(pagination);
    public Task<ActionResponse<ConventionValue>> UpdateAsync(ConventionValueDTO entity) => _repository.UpdateAsync(entity);
}
