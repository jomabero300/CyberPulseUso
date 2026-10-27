using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Lnpa;

public class ConventionUnitOfWork : GenericUnitOfWork<Convention>, IConventionUnitOfWork
{
    private readonly IConventionRepository _repository;

    public ConventionUnitOfWork(IGenericRepository<Convention> repository, IConventionRepository modelRepository)
        : base(repository)
    {
        _repository = modelRepository;
    }

    public override Task<ActionResponse<Convention>> GetAsync(int id) => _repository.GetAsync(id);
    public override Task<ActionResponse<IEnumerable<Convention>>> GetAsync(PaginationDTO pagination) => _repository.GetAsync(pagination);
    public override Task<ActionResponse<Convention>> DeleteAsync(int id) => _repository.DeleteAsync(id);
    public Task<ActionResponse<Convention>> AddAsync(ConventionDTO entity) => _repository.AddAsync(entity);
    public Task<IEnumerable<Convention>> GetComboAsync() => _repository.GetComboAsync();
    public Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => _repository.GetTotalRecordsAsync(pagination);
    public Task<ActionResponse<Convention>> UpdateAsync(ConventionDTO entity) => _repository.UpdateAsync(entity);
}
