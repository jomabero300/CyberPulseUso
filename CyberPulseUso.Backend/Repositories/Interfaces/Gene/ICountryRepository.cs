using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Gene;

public interface ICountryRepository
{
    Task<ActionResponse<Country>> GetAsync(int id);
    Task<ActionResponse<IEnumerable<Country>>> GetAsync();
    Task<ActionResponse<Country>> AddAsync(CountryDTO country);
    Task<ActionResponse<Country>> UpdateAsync(CountryDTO country);
    Task<ActionResponse<Country>> GetAsync(int id, bool lb);
    Task<ActionResponse<Country>> DeleteAsync(int id);
    Task<IEnumerable<Country>> GetComboAsync();
    Task<ActionResponse<IEnumerable<Country>>> GetAsync(PaginationDTO pagination);
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
}
