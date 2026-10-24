using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;

public interface IStateUnitOfWork
{
    Task<ActionResponse<State>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<State>>> GetAsync();
    Task<ActionResponse<State>> DeleteAsync(int id);
    Task<IEnumerable<State>> GetComboAsync(int id);
}
