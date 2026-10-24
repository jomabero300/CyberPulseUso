using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;

public interface ICityUnitOfWork
{
    Task<IEnumerable<City>> GetComboAsync(int id);
    Task<ActionResponse<City>> AddAsync(CityDTO entity);
    Task<ActionResponse<City>> UpdateAsync(CityDTO entity);
}
