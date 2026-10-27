using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Gene;

public class StateUnitOfWork : GenericUnitOfWork<State>, IStateUnitOfWork
{
    private readonly IStateRepository _stateRepository;
    public StateUnitOfWork(IGenericRepository<State> repository, IStateRepository stateRepository) : base(repository)
    {
        _stateRepository = stateRepository;
    }
    public override async Task<ActionResponse<State>> GetAsync(int id)=>await _stateRepository.GetAsync(id);
    public override async Task<ActionResponse<IEnumerable<State>>> GetAsync(PaginationDTO pagination)=>await _stateRepository.GetAsync(pagination);
    public override async Task<ActionResponse<State>> DeleteAsync(int id)=>await _stateRepository.DeleteAsync(id);
    public async Task<ActionResponse<State>> AddAsync(StateDTO entity) => await _stateRepository.AddAsync(entity);
    public async Task<IEnumerable<State>> GetComboAsync(int id) => await _stateRepository.GetComboAsync(id);
    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _stateRepository.GetTotalRecordsAsync(pagination);
    public async Task<ActionResponse<State>> UpdateAsync(StateDTO entity) => await _stateRepository.UpdateAsync(entity);
}
