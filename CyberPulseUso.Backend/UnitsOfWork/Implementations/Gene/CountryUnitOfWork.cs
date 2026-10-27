using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Gene;

public class CountryUnitOfWork : GenericUnitOfWork<Country>, ICountryUnitOfWork
{
    private readonly ICountryRepository _countryRepository;

    public CountryUnitOfWork(IGenericRepository<Country> repository, ICountryRepository countryRepository) : base(repository)
    {
        _countryRepository = countryRepository;
    }

    public async Task<ActionResponse<Country>> AddAsync(CountryDTO country) => await _countryRepository.AddAsync(country);
    public async Task<ActionResponse<Country>> GetAsync(int id, bool lb) => await _countryRepository.GetAsync(id, lb);
    public async Task<IEnumerable<Country>> GetComboAsync() => await _countryRepository.GetComboAsync();
    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _countryRepository.GetTotalRecordsAsync(pagination);
    public async Task<ActionResponse<Country>> UpdateAsync(CountryDTO country) => await _countryRepository.UpdateAsync(country);
}
