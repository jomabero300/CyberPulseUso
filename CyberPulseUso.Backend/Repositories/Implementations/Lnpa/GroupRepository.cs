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

public class GroupRepository : GenericRepository<Group>, IGroupRepository
{
    private readonly ApplicationDbContext _context;

    public GroupRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<Group>> GetAsync(int id)
    {
        var entity = await _context.Set<Group>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<Group> { WasSuccess = false, Message = "ERR001" };
        return new ActionResponse<Group> { WasSuccess = true, Result = entity };
    }

    public override async Task<ActionResponse<Group>> DeleteAsync(int id)
    {
        var entity = await _context.Set<Group>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<Group> { WasSuccess = false, Message = "ERR001" };

        _context.Set<Group>().Remove(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Group> { WasSuccess = true, Message = "Success" };
        }
        catch (Exception)
        {
            return new ActionResponse<Group> { WasSuccess = false, Message = "ERR002" };
        }
    }

    public override async Task<ActionResponse<IEnumerable<Group>>> GetAsync(PaginationDTO pagination)
    {
        IQueryable<Group> query = _context.Set<Group>().Include(x => x.Statu).Include(x => x.Category).AsNoTracking();
        //query = query.Include(x => x.Statu).Include(x => x.Category);
        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            var filter = pagination.Filter.Trim().ToLower();
            query = query.Where(x =>
                x.Name!.ToLower().Contains(filter));
        }
        query = query.OrderBy(x => x.Name);
        return new ActionResponse<IEnumerable<Group>>
        {
            WasSuccess = true,
            Result = await query.Paginate(pagination).ToListAsync()
        };
    }

    public async Task<ActionResponse<Group>> AddAsync(GroupDTO entity)
    {
        var model = new Group();
        CopyMatchingProperties(entity, model, isUpdate: false);
        _context.Set<Group>().Add(model);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Group> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Group> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<Group> { WasSuccess = false, Message = ex.Message };
        }
    }

    public async Task<IEnumerable<Group>> GetComboAsync()
    {
        return await _context.Set<Group>().AsNoTracking().Include(x => x.Category).Include(x => x.Statu).OrderBy(x => x.Id).ToListAsync();
    }
    public async Task<IEnumerable<Group>> GetComboAsync(int id)
    {
        return await _context.Set<Group>().AsNoTracking()
            .Where(x => x.CategoryId == id)
            .OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        IQueryable<Group> query = _context.Set<Group>().Include(x=>x.Statu).AsNoTracking();
        //query = query.Include(x => x.Statu);
        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            var filter = pagination.Filter.Trim().ToLower();
            query = query.Where(x =>
                x.Name!.ToLower().Contains(filter));
        }

        return new ActionResponse<int> 
        { 
            WasSuccess = true, 
            Result = await query.CountAsync()
        };
    }

    public async Task<ActionResponse<Group>> UpdateAsync(GroupDTO entity)
    {
        var model = await _context.Set<Group>().FindAsync(entity.Id);
        if (model == null)
            return new ActionResponse<Group> { WasSuccess = false, Message = "ERR005" };

        CopyMatchingProperties(entity, model, isUpdate: true);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<Group> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<Group> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<Group> { WasSuccess = false, Message = ex.Message };
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
