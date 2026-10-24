using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Helpers;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;
using Microsoft.EntityFrameworkCore;
namespace CyberPulseUso.Backend.Repositories.Implementations.Gene;
public class StatuRepository : GenericRepository<Statu>, IStatuRepository
{
    private readonly ApplicationDbContext _context;
    public StatuRepository(ApplicationDbContext context) : base(context) { _context = context; }

    public async Task<ActionResponse<Statu>> AddAsync(StatuDTO entity)
    {
        var statu = new Statu { Id = entity.Id, Name = entity.Name };

        _context.Add(statu);
        try { await _context.SaveChangesAsync(); return new ActionResponse<Statu> { WasSuccess = true, Result = statu }; }
        catch (DbUpdateException) { return new ActionResponse<Statu> { WasSuccess = false, Message = "ERR003" }; }
        catch (Exception ex) { return new ActionResponse<Statu> { WasSuccess = false, Message = ex.Message }; }
    }
    public override async Task<ActionResponse<IEnumerable<Statu>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Status.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(pagination.Filter)) queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        return new ActionResponse<IEnumerable<Statu>> { WasSuccess = true, Result = await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync() };
    }
    public async Task<ActionResponse<Statu>> GetAsync(string name, int nivel)
    {
        var response = await _context.Status.AsNoTracking().FirstOrDefaultAsync(x => x.Name.ToLower() == name.ToLower() && x.Nivel == nivel);
        return response == null ? new ActionResponse<Statu> { WasSuccess = false, Message = "ERR001" } : new ActionResponse<Statu> { WasSuccess = true, Result = response };
    }
    public async Task<IEnumerable<Statu>> GetComboAsync(int id)
    {
        var query = _context.Status.AsNoTracking().AsQueryable();
        if (id >= 0) query = query.Where(x => x.Nivel == id);
        return await query.OrderBy(x => x.Name).ToListAsync();
    }
    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Status.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(pagination.Filter)) queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
        return new ActionResponse<int> { WasSuccess = true, Result = await queryable.CountAsync() };
    }
    public async Task<ActionResponse<Statu>> UpdateAsync(StatuDTO entity)
    {
        var statu = await _context.Status.FindAsync(entity.Id);
        if (statu == null) return new ActionResponse<Statu> { WasSuccess = false, Message = "ERR005" };
        statu.Name = entity.Name;

        _context.Update(statu);
        try { await _context.SaveChangesAsync(); return new ActionResponse<Statu> { WasSuccess = true, Result = statu }; }
        catch (DbUpdateException) { return new ActionResponse<Statu> { WasSuccess = false, Message = "ERR003" }; }
        catch (Exception ex) { return new ActionResponse<Statu> { WasSuccess = false, Message = ex.Message }; }
    }
}
