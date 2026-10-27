using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Helpers;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CyberPulseUso.Backend.Repositories.Implementations.Gene;

public class LogAuditoriaRepository : GenericRepository<LogAuditoria>, ILogAuditoriaRepository
{
    private readonly ApplicationDbContext _context;

    public LogAuditoriaRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<LogAuditoria>> GetAsync(int id)
    {
        var entity = await _context.Set<LogAuditoria>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<LogAuditoria> { WasSuccess = false, Message = "ERR001" };
        return new ActionResponse<LogAuditoria> { WasSuccess = true, Result = entity };
    }

    public override async Task<ActionResponse<LogAuditoria>> DeleteAsync(int id)
    {
        var entity = await _context.Set<LogAuditoria>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<LogAuditoria> { WasSuccess = false, Message = "ERR001" };

        _context.Set<LogAuditoria>().Remove(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<LogAuditoria> { WasSuccess = true, Message = "Success" };
        }
        catch (Exception)
        {
            return new ActionResponse<LogAuditoria> { WasSuccess = false, Message = "ERR002" };
        }
    }

    public override async Task<ActionResponse<IEnumerable<LogAuditoria>>> GetAsync(PaginationDTO pagination)
    {
        IQueryable<LogAuditoria> query = _context.Set<LogAuditoria>().AsNoTracking();
                // El modelo no tiene una propiedad string 'Name'; se conserva el filtro recibido
        // en PaginationDTO, pero no se aplica un filtro específico del modelo.
        query = query.OrderBy(x => x.Id);
        return new ActionResponse<IEnumerable<LogAuditoria>>
        {
            WasSuccess = true,
            Result = await query.Paginate(pagination).ToListAsync()
        };
    }

    public async Task<ActionResponse<LogAuditoria>> AddAsync(LogAuditoriaDTO entity)
    {
        var model = new LogAuditoria();
        CopyMatchingProperties(entity, model, isUpdate: false);
        _context.Set<LogAuditoria>().Add(model);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<LogAuditoria> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<LogAuditoria> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<LogAuditoria> { WasSuccess = false, Message = ex.Message };
        }
    }

    public async Task<IEnumerable<LogAuditoria>> GetComboAsync()
    {
        return await _context.Set<LogAuditoria>().AsNoTracking()
            .OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        IQueryable<LogAuditoria> query = _context.Set<LogAuditoria>().AsNoTracking();
                // El modelo no tiene una propiedad string 'Name'; se conserva el filtro recibido
        // en PaginationDTO, pero no se aplica un filtro específico del modelo.
        var totalRecords = await query.CountAsync();
        return new ActionResponse<int> { WasSuccess = true, Result = totalRecords };
    }

    public async Task<ActionResponse<LogAuditoria>> UpdateAsync(LogAuditoriaDTO entity)
    {
        var model = await _context.Set<LogAuditoria>().FindAsync(entity.Id);
        if (model == null)
            return new ActionResponse<LogAuditoria> { WasSuccess = false, Message = "ERR005" };

        CopyMatchingProperties(entity, model, isUpdate: true);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<LogAuditoria> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<LogAuditoria> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<LogAuditoria> { WasSuccess = false, Message = ex.Message };
        }
    }

    private static void CopyMatchingProperties(object source, object target, bool isUpdate)
    {
        var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var targetProperties = target.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var sourceProperty in sourceProperties)
        {
            if (!sourceProperty.CanRead || sourceProperty.Name == "Id")
                continue;

            var targetProperty = targetProperties.FirstOrDefault(p =>
                p.Name == sourceProperty.Name && p.CanWrite &&
                p.PropertyType.IsAssignableFrom(sourceProperty.PropertyType));

            if (targetProperty == null)
                continue;

            var value = sourceProperty.GetValue(source);
            if (isUpdate && value == null && Nullable.GetUnderlyingType(sourceProperty.PropertyType) != null)
                continue;

            targetProperty.SetValue(target, value);
        }
    }
}
