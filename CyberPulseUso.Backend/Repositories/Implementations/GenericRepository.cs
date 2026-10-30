using CyberPulseUso.Backend.Helpers;
using CyberPulseUso.Backend.Repositories.Interfaces;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.Responses;
using Microsoft.EntityFrameworkCore;

namespace CyberPulseUso.Backend.Repositories.Implementations;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly CyberPulseUso.Backend.Data.ApplicationDbContext _context;
    private readonly DbSet<T> _entity;
    public GenericRepository(CyberPulseUso.Backend.Data.ApplicationDbContext context) { _context = context; _entity = _context.Set<T>(); }
    public virtual async Task<ActionResponse<T>> AddAsync(T entity)
    {
        _context.Add(entity);
        try { await _context.SaveChangesAsync(); return new ActionResponse<T> { WasSuccess = true, Result = entity }; }
        catch (DbUpdateException) { return DbUpdateExceptionActionResponse(); }
        catch (Exception exception) { return ExceptionActionResponse(exception); }
    }
    public virtual async Task<ActionResponse<T>> DeleteAsync(int id)
    {
        var response = await _entity.FindAsync(id);
        if (response == null) return new ActionResponse<T> { WasSuccess = false, Message = "ERR001" };
        _entity.Remove(response);
        try { await _context.SaveChangesAsync(); return new ActionResponse<T> { WasSuccess = true }; }
        catch { return new ActionResponse<T> { WasSuccess = false, Message = "ERR002" }; }
    }
    public virtual async Task<ActionResponse<T>> GetAsync(int id)
    {
        var row = await _entity.FindAsync(id);
        if (row == null) return new ActionResponse<T> { WasSuccess = false, Message = "ERR001" };
        return new ActionResponse<T> { WasSuccess = true, Result = row };
    }
    public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync() => new() { WasSuccess = true, Result = await _entity.ToListAsync() };
    public virtual async Task<ActionResponse<IEnumerable<T>>> GetAsync(PaginationDTO pagination) => new() { WasSuccess = true, Result = await _entity.AsQueryable().Paginate(pagination).ToListAsync() };
    public virtual async Task<ActionResponse<int>> GetTotalRecordsAsync() => new() { WasSuccess = true, Result = await _entity.CountAsync() };
    public virtual async Task<ActionResponse<T>> UpdateAsync(T entity)
    {
        _context.Update(entity);
        try { await _context.SaveChangesAsync(); return new ActionResponse<T> { WasSuccess = true, Result = entity }; }
        catch (DbUpdateException) { return DbUpdateExceptionActionResponse(); }
        catch (Exception exception) { return ExceptionActionResponse(exception); }
    }
    private ActionResponse<T> ExceptionActionResponse(Exception exception) => new() { WasSuccess = false, Message = exception.Message };
    private ActionResponse<T> DbUpdateExceptionActionResponse() => new() { WasSuccess = false, Message = "ERR003" };
}
