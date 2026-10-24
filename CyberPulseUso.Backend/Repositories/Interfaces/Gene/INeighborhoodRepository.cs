using CyberPulseUso.Shared.Entities.Gene;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Gene;

public interface INeighborhoodRepository
{
    Task<IEnumerable<Neighborhood>> GetComboAsync(int id);
}
