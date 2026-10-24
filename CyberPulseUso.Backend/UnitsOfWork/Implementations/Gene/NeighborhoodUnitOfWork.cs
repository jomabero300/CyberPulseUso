using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;

namespace CyberPulseUso.Backend.UnitsOfWork.Implementations.Gene;

public class NeighborhoodUnitOfWork : GenericUnitOfWork<Neighborhood>, INeighborhoodUnitOfWork
{
    private readonly INeighborhoodRepository _neighborhood;

    public NeighborhoodUnitOfWork(IGenericRepository<Neighborhood> repository, INeighborhoodRepository neighborhood) : base(repository)
    {
        _neighborhood = neighborhood;
    }

    public async Task<IEnumerable<Neighborhood>> GetComboAsync(int id) => await _neighborhood.GetComboAsync(id);
}
