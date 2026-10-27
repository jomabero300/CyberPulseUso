using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Lnpa;

public interface ICategoryUnitOfWork
{
    Task<ActionResponse<Category>> AddAsync(CategoryDTO entity);
    Task<ActionResponse<Category>> DeleteAsync(int id);
    Task<ActionResponse<IEnumerable<Category>>> GetAsync();
    Task<ActionResponse<Category>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<Category>>> GetAsync(PaginationDTO pagination);
    Task<IEnumerable<Category>> GetComboAsync();
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<ActionResponse<Category>> UpdateAsync(CategoryDTO entity);
}
