using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Gene;

public class StateUnitOfWork : GenericUnitOfWork<State>, IStateUnitOfWork
{
    private readonly IStateRepository _stateRepository;
    public StateUnitOfWork(IGenericRepository<State> repository, IStateRepository stateRepository) : base(repository)
    {
        _stateRepository = stateRepository;
    }

    public async Task<IEnumerable<State>> GetComboAsync(int id) => await _stateRepository.GetComboAsync(id);

}
