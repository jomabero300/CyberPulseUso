using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Responses;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;

public interface INeighborhoodUnitOfWork
{
    Task<IEnumerable<Neighborhood>> GetComboAsync(int id);
}
