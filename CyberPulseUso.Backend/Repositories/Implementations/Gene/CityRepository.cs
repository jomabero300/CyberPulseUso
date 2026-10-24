using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;
using Microsoft.EntityFrameworkCore;
namespace CyberPulseUso.Backend.Repositories.Implementations.Gene;
public class CityRepository : GenericRepository<City>, ICityRepository
{
    private readonly ApplicationDbContext _context;
    public CityRepository(ApplicationDbContext context) : base(context) { _context = context; }
    public async Task<ActionResponse<City>> AddAsync(CityDTO entity)
    {
        var city = new City { Id = entity.Id, Name = entity.Name };

        _context.Add(city);
        try { await _context.SaveChangesAsync(); return new ActionResponse<City> { WasSuccess = true, Result = city }; }
        catch (DbUpdateException) { return new ActionResponse<City> { WasSuccess = false, Message = "ERR003" }; }
        catch (Exception ex) { return new ActionResponse<City> { WasSuccess = false, Message = ex.Message }; }
    }
    public async Task<IEnumerable<City>> GetComboAsync(int id) => await _context.Cities.AsNoTracking().Where(x => x.StateId == id).OrderBy(x => x.Name).ToListAsync();
    public async Task<ActionResponse<City>> UpdateAsync(CityDTO entity)
    {
        var city = await _context.Cities.FindAsync(entity.Id);
        if (city == null) return new ActionResponse<City> { WasSuccess = false, Message = "ERR005" };
        city.Name = entity.Name;

        _context.Update(city);
        try { await _context.SaveChangesAsync(); return new ActionResponse<City> { WasSuccess = true, Result = city }; }
        catch (DbUpdateException) { return new ActionResponse<City> { WasSuccess = false, Message = "ERR003" }; }
        catch (Exception ex) { return new ActionResponse<City> { WasSuccess = false, Message = ex.Message }; }
    }
}
