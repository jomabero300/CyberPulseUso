using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;

public interface IStateUnitOfWork
{
    Task<ActionResponse<State>> AddAsync(StateDTO entity);
    Task<ActionResponse<State>> DeleteAsync(int id);
    Task<ActionResponse<State>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<State>>> GetAsync();
    Task<IEnumerable<State>> GetComboAsync(int id);
    Task<ActionResponse<IEnumerable<State>>> GetAsync(PaginationDTO pagination);
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<State>> UpdateAsync(StateDTO entity);
}