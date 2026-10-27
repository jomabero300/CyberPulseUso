using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Gene;

public interface INeighborhoodRepository
{
    Task<IEnumerable<Neighborhood>> GetComboAsync(int id);
}
