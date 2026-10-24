using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Gene;

public interface ICityRepository
{
    Task<IEnumerable<City>> GetComboAsync(int id);
    Task<ActionResponse<City>> AddAsync(CityDTO entity);
    Task<ActionResponse<City>> UpdateAsync(CityDTO entity);
}
