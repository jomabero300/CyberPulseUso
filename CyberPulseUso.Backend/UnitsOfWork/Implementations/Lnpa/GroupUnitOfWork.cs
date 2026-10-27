using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Lnpa;

public class GroupUnitOfWork : GenericUnitOfWork<Group>, IGroupUnitOfWork
{
    private readonly IGroupRepository _repository;

    public GroupUnitOfWork(IGenericRepository<Group> repository, IGroupRepository modelRepository)
        : base(repository)
    {
        _repository = modelRepository;
    }

    public override Task<ActionResponse<Group>> GetAsync(int id) => _repository.GetAsync(id);
    public override Task<ActionResponse<IEnumerable<Group>>> GetAsync(PaginationDTO pagination) => _repository.GetAsync(pagination);
    public override Task<ActionResponse<Group>> DeleteAsync(int id) => _repository.DeleteAsync(id);
    public Task<ActionResponse<Group>> AddAsync(GroupDTO entity) => _repository.AddAsync(entity);
    public Task<IEnumerable<Group>> GetComboAsync() => _repository.GetComboAsync();
    public Task<IEnumerable<Group>> GetComboAsync(int id) => _repository.GetComboAsync(id);
    public Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => _repository.GetTotalRecordsAsync(pagination);
    public Task<ActionResponse<Group>> UpdateAsync(GroupDTO entity) => _repository.UpdateAsync(entity);
}
