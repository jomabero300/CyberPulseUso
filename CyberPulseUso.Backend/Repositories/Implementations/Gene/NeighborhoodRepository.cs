using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using Microsoft.EntityFrameworkCore;
namespace CyberPulseUso.Backend.Repositories.Implementations.Gene;
public class NeighborhoodRepository : GenericRepository<Neighborhood>, INeighborhoodRepository
{
    private readonly ApplicationDbContext _context;
    public NeighborhoodRepository(ApplicationDbContext context) : base(context) { _context = context; }
    public async Task<IEnumerable<Neighborhood>> GetComboAsync(int id) => await _context.Neighborhoods.AsNoTracking().Where(x => x.CityId == id).OrderBy(x => x.Name).ToListAsync();
}
