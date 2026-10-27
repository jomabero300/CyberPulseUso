using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Helpers;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CyberPulseUso.Backend.Repositories.Implementations.Lnpa;

public class ConventionValueRepository : GenericRepository<ConventionValue>, IConventionValueRepository
{
    private readonly ApplicationDbContext _context;

    public ConventionValueRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<ConventionValue>> GetAsync(int id)
    {
        var entity = await _context.Set<ConventionValue>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<ConventionValue> { WasSuccess = false, Message = "ERR001" };
        return new ActionResponse<ConventionValue> { WasSuccess = true, Result = entity };
    }

    public override async Task<ActionResponse<ConventionValue>> DeleteAsync(int id)
    {
        var entity = await _context.Set<ConventionValue>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<ConventionValue> { WasSuccess = false, Message = "ERR001" };

        _context.Set<ConventionValue>().Remove(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<ConventionValue> { WasSuccess = true, Message = "Success" };
        }
        catch (Exception)
        {
            return new ActionResponse<ConventionValue> { WasSuccess = false, Message = "ERR002" };
        }
    }

    public override async Task<ActionResponse<IEnumerable<ConventionValue>>> GetAsync(PaginationDTO pagination)
    {
        IQueryable<ConventionValue> query = _context.Set<ConventionValue>().AsNoTracking();
        query = query.Include(x => x.Statu).Include(x => x.Convention);

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            query = query.Where(x =>
                x.Prince.ToString().ToLower().Contains(pagination.Filter.ToLower()));

        }

        query = query.OrderBy(x => x.Id);
        return new ActionResponse<IEnumerable<ConventionValue>>
        {
            WasSuccess = true,
            Result = await query.Paginate(pagination).ToListAsync()
        };
    }

    public async Task<ActionResponse<ConventionValue>> AddAsync(ConventionValueDTO entity)
    {
        var model = new ConventionValue();
        CopyMatchingProperties(entity, model, isUpdate: false);
        _context.Set<ConventionValue>().Add(model);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<ConventionValue> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<ConventionValue> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<ConventionValue> { WasSuccess = false, Message = ex.Message };
        }
    }

    public async Task<IEnumerable<ConventionValue>> GetComboAsync()
    {
        return await _context.Set<ConventionValue>().AsNoTracking()
            .OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        IQueryable<ConventionValue> query = _context.Set<ConventionValue>().AsNoTracking();
        query = query.Include(x => x.Statu);
                // El modelo no tiene una propiedad string 'Name'; se conserva el filtro recibido
        // en PaginationDTO, pero no se aplica un filtro específico del modelo.
        var totalRecords = await query.CountAsync();
        return new ActionResponse<int> { WasSuccess = true, Result = totalRecords };
    }

    public async Task<ActionResponse<ConventionValue>> UpdateAsync(ConventionValueDTO entity)
    {
        var model = await _context.Set<ConventionValue>().FindAsync(entity.Id);
        if (model == null)
            return new ActionResponse<ConventionValue> { WasSuccess = false, Message = "ERR005" };

        CopyMatchingProperties(entity, model, isUpdate: true);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<ConventionValue> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<ConventionValue> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<ConventionValue> { WasSuccess = false, Message = ex.Message };
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
