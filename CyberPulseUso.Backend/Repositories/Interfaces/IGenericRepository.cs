using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<ActionResponse<T>> AddAsync(T entity);
    Task<ActionResponse<T>> DeleteAsync(int id);
    Task<ActionResponse<T>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<T>>> GetAsync();
    Task<ActionResponse<IEnumerable<T>>> GetAsync(PaginationDTO pagination);
    Task<ActionResponse<int>> GetTotalRecordsAsync();
    Task<ActionResponse<T>> UpdateAsync(T entity);
}
