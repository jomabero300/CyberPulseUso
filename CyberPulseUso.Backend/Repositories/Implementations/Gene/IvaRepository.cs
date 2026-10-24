using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Helpers;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;
using Microsoft.EntityFrameworkCore;
namespace CyberPulseUso.Backend.Repositories.Implementations.Gene;
public class IvaRepository : GenericRepository<Iva>, IIvaRepository
{
    private readonly ApplicationDbContext _context;
    public IvaRepository(ApplicationDbContext context) : base(context) { _context = context; }
    public override async Task<ActionResponse<Iva>> GetAsync(int id)
    {
        var entity = await _context.Ivas.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        return entity == null ? new ActionResponse<Iva> { WasSuccess = false, Message = "ERR001" } : new ActionResponse<Iva> { WasSuccess = true, Result = entity };
    }
    public override async Task<ActionResponse<IEnumerable<Iva>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.Ivas.AsNoTracking().Include(x => x.Statu).AsQueryable();
        if (!string.IsNullOrWhiteSpace(pagination.Filter)) queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()) || x.Statu!.Name.ToLower().Contains(pagination.Filter.ToLower()) || x.Worth.ToString().ToLower().Contains(pagination.Filter.ToLower()));
        return new ActionResponse<IEnumerable<Iva>> { WasSuccess = true, Result = await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync() };
    }
    public override async Task<ActionResponse<Iva>> DeleteAsync(int id)
    {
        var entity = await _context.Ivas.FindAsync(id);
        if (entity == null) return new ActionResponse<Iva> { WasSuccess = false, Message = "ERR001" };
        _context.Remove(entity);
        try { await _context.SaveChangesAsync(); return new ActionResponse<Iva> { WasSuccess = true }; }
        catch { return new ActionResponse<Iva> { WasSuccess = false, Message = "ERR002" }; }
    }
    public async Task<ActionResponse<Iva>> AddAsync(IvaDTO entity)
    {
        var model = new Iva { Id = entity.Id, Name = HtmlUtilities.ToTitleCase(entity.Name.Trim().ToLower()), Worth = entity.Worth, StatuId = entity.StatuId };
        _context.Add(model);
        try { await _context.SaveChangesAsync(); return new ActionResponse<Iva> { WasSuccess = true, Result = model }; }
        catch (DbUpdateException) { return new ActionResponse<Iva> { WasSuccess = false, Message = "ERR003" }; }
        catch (Exception ex) { return new ActionResponse<Iva> { WasSuccess = false, Message = ex.Message }; }
    }
    public async Task<IEnumerable<Iva>> GetComboAsync() => await _context.Ivas.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.Ivas.Include(x => x.Statu).AsQueryable();
        if (!string.IsNullOrWhiteSpace(pagination.Filter)) queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()) || x.Statu!.Name.ToLower().Contains(pagination.Filter.ToLower()) || x.Worth.ToString().ToLower().Contains(pagination.Filter.ToLower()));
        return new ActionResponse<int> { WasSuccess = true, Result = await queryable.CountAsync() };
    }
    public async Task<ActionResponse<Iva>> UpdateAsync(IvaDTO entity)
    {
        var model = await _context.Ivas.FindAsync(entity.Id);
        if (model == null) return new ActionResponse<Iva> { WasSuccess = false, Message = "ERR005" };
        model.Name = HtmlUtilities.ToTitleCase(entity.Name.Trim().ToLower()); model.Worth = entity.Worth; model.StatuId = entity.StatuId;
        _context.Update(model);
        try { await _context.SaveChangesAsync(); return new ActionResponse<Iva> { WasSuccess = true, Result = model }; }
        catch (DbUpdateException) { return new ActionResponse<Iva> { WasSuccess = false, Message = "ERR003" }; }
        catch (Exception ex) { return new ActionResponse<Iva> { WasSuccess = false, Message = ex.Message }; }
    }
}
