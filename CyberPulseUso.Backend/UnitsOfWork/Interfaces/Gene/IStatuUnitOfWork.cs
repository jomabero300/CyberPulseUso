using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;

public interface IStatuUnitOfWork
{
    Task<ActionResponse<Statu>> GetAsync(int id);
    Task<ActionResponse<Statu>> GetAsync(string name, int nivel);
    Task<ActionResponse<IEnumerable<Statu>>> GetAsync();
    Task<ActionResponse<Statu>> AddAsync(StatuDTO entity);
    Task<ActionResponse<Statu>> DeleteAsync(int id);
    Task<IEnumerable<Statu>> GetComboAsync(int id);
    Task<ActionResponse<IEnumerable<Statu>>> GetAsync(PaginationDTO pagination);
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<Statu>> UpdateAsync(StatuDTO entity);
}
