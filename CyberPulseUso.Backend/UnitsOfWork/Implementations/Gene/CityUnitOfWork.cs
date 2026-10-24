using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Gene;

public class CityUnitOfWork : GenericUnitOfWork<City>, ICityUnitOfWork
{
    private readonly ICityRepository _cityRepository;

    public CityUnitOfWork(IGenericRepository<City> repository, ICityRepository cityRepository) : base(repository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<ActionResponse<City>> AddAsync(CityDTO entity)=>await _cityRepository.AddAsync(entity);
    public async Task<IEnumerable<City>> GetComboAsync(int id) => await _cityRepository.GetComboAsync(id);
    public async Task<ActionResponse<City>> UpdateAsync(CityDTO entity)=>await _cityRepository.UpdateAsync(entity);
}
