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

public class ChargeRepository : GenericRepository<Charge>, IChargeRepository
{
    private readonly ApplicationDbContext _context;

    public ChargeRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<Charge>> GetAsync(int id)
    {
        var entity = await _context.Set<Charge>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<Charge> { WasSuccess = false, Message = "ERR001" };
        return new ActionResponse<Charge> { WasSuccess = true, Result = entity };
    }

    public override async Task<ActionResponse<Charge>> DeleteAsync(int id)
    {
        var entity = await _context.Set<Charge>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<Charge> { WasSuccess = false, Message = "ERR001" };

        _context.Set<Charge>().Remove(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Charge> { WasSuccess = true, Message = "Success" };
        }
        catch (Exception)
        {
            return new ActionResponse<Charge> { WasSuccess = false, Message = "ERR002" };
        }
    }

    public override async Task<ActionResponse<IEnumerable<Charge>>> GetAsync(PaginationDTO pagination)
    {
        IQueryable<Charge> query = _context.Set<Charge>().AsNoTracking();
        query = query.Include(x => x.Group).Include(x => x.Statu);
                if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            var filter = pagination.Filter.Trim().ToLower();
            query = query.Where(x =>
                (x.Name != null && x.Name.ToLower().Contains(filter)) ||
                (x.Statu != null && x.Statu.Name.ToLower().Contains(filter)));
        }
        query = query.OrderBy(x => x.Name);
        return new ActionResponse<IEnumerable<Charge>>
        {
            WasSuccess = true,
            Result = await query.Paginate(pagination).ToListAsync()
        };
    }

    public async Task<ActionResponse<Charge>> AddAsync(ChargeDTO entity)
    {
        var model = new Charge();
        CopyMatchingProperties(entity, model, isUpdate: false);
        _context.Set<Charge>().Add(model);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Charge> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Charge> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<Charge> { WasSuccess = false, Message = ex.Message };
        }
    }

    public async Task<IEnumerable<Charge>> GetComboAsync()
    {
        return await _context.Set<Charge>().AsNoTracking()
            .OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        IQueryable<Charge> query = _context.Set<Charge>().AsNoTracking();
        query = query.Include(x => x.Statu);
        
        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            var filter = pagination.Filter.Trim().ToLower();
            query = query.Where(x =>
                (x.Name != null && x.Name.ToLower().Contains(filter)) ||
                (x.Statu != null && x.Statu.Name.ToLower().Contains(filter)));
        }

        var totalRecords = await query.CountAsync();
        return new ActionResponse<int> { WasSuccess = true, Result = totalRecords };
    }

    public async Task<ActionResponse<Charge>> UpdateAsync(ChargeDTO entity)
    {
        var model = await _context.Set<Charge>().FindAsync(entity.Id);
        if (model == null)
            return new ActionResponse<Charge> { WasSuccess = false, Message = "ERR005" };

        CopyMatchingProperties(entity, model, isUpdate: true);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Charge> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Charge> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<Charge> { WasSuccess = false, Message = ex.Message };
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
