using CyberPulseUso.Backend.Data;

using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace CyberPulseUso.Backend.Repositories.Implementations.Gene;

public class StateRepository : GenericRepository<State>, IStateRepository
{
    private readonly ApplicationDbContext _context;
    public StateRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<State>> GetComboAsync(int id)
    {
        return await _context.States
                                    .AsNoTracking()
                                    .Where(x => x.CountryId == id)
                                    .OrderBy(x => x.Name)
                                    .ToListAsync();
    }

}
