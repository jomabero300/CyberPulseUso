using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;

public interface IGroupRepository
{
    Task<ActionResponse<Group>> AddAsync(GroupDTO entity);
    Task<ActionResponse<Group>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<Group>>> GetAsync();
    Task<ActionResponse<Group>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<Group>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<Group>> GetComboAsync();
    Task<IEnumerable<Group>> GetComboAsync(int id);
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<Group>> UpdateAsync(GroupDTO entity);
}
