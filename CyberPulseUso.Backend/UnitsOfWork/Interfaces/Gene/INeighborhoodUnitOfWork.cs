using CyberPulseUso.Shared.Entities.Gene;

namespace CyberPulseUso.Backend.UnitsOfWork.Interfaces.Gene;

public interface INeighborhoodUnitOfWork
{
    Task<IEnumerable<Neighborhood>> GetComboAsync(int id);
}
