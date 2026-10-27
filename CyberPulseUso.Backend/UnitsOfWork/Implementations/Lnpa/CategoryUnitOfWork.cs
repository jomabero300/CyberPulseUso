using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Lnpa;

public class CategoryUnitOfWork : GenericUnitOfWork<Category>, ICategoryUnitOfWork
{
    private readonly ICategoryRepository _repository;

    public CategoryUnitOfWork(IGenericRepository<Category> repository, ICategoryRepository modelRepository)
        : base(repository)
    {
        _repository = modelRepository;
    }

    public override Task<ActionResponse<Category>> GetAsync(int id) => _repository.GetAsync(id);
    public override Task<ActionResponse<IEnumerable<Category>>> GetAsync(PaginationDTO pagination) => _repository.GetAsync(pagination);
    public override Task<ActionResponse<Category>> DeleteAsync(int id) => _repository.DeleteAsync(id);
    public Task<ActionResponse<Category>> AddAsync(CategoryDTO entity) => _repository.AddAsync(entity);
    public Task<IEnumerable<Category>> GetComboAsync() => _repository.GetComboAsync();
    public Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => _repository.GetTotalRecordsAsync(pagination);
    public Task<ActionResponse<Category>> UpdateAsync(CategoryDTO entity) => _repository.UpdateAsync(entity);
}
