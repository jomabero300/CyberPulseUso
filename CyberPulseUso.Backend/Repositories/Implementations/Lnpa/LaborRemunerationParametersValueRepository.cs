using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Helpers;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CyberPulseUso.Backend.Repositories.Implementations.Lnpa;

public class LaborRemunerationParametersValueRepository : GenericRepository<LaborRemunerationParametersValue>, ILaborRemunerationParametersValueRepository
{
    private readonly ApplicationDbContext _context;

    public LaborRemunerationParametersValueRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<LaborRemunerationParametersValue>> GetAsync(int id)
    {
        var entity = await _context.Set<LaborRemunerationParametersValue>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<LaborRemunerationParametersValue> { WasSuccess = false, Message = "ERR001" };
        return new ActionResponse<LaborRemunerationParametersValue> { WasSuccess = true, Result = entity };
    }
    public override async Task<ActionResponse<LaborRemunerationParametersValue>> DeleteAsync(int id)
    {
        var entity = await _context.Set<LaborRemunerationParametersValue>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<LaborRemunerationParametersValue> { WasSuccess = false, Message = "ERR001" };

        _context.Set<LaborRemunerationParametersValue>().Remove(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<LaborRemunerationParametersValue> { WasSuccess = true, Message = "Success" };
        }
        catch (Exception)
        {
            return new ActionResponse<LaborRemunerationParametersValue> { WasSuccess = false, Message = "ERR002" };
        }
    }
    public override async Task<ActionResponse<IEnumerable<LaborRemunerationParametersValue>>> GetAsync(PaginationDTO pagination)
    {
        IQueryable<LaborRemunerationParametersValue> query = _context.Set<LaborRemunerationParametersValue>().AsNoTracking();
        query = query.Include(x => x.Statu).Include(x => x.Group).Include(x => x.LRP);

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            query = query.Where(x =>
                x.Value.ToString().ToLower().Contains(pagination.Filter.ToLower()) ||
                x.Group!.Name!.ToLower().Contains(pagination.Filter.ToLower()) ||
                x.LRP!.Name!.ToLower().Contains(pagination.Filter.ToLower()) ||
                x.Statu!.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        query = query.OrderBy(x => x.Id);
        return new ActionResponse<IEnumerable<LaborRemunerationParametersValue>>
        {
            WasSuccess = true,
            Result = await query.Paginate(pagination).ToListAsync()
        };
    }
    public async Task<ActionResponse<LaborRemunerationParametersValue>> AddAsync(LaborRemunerationParametersValueDTO entity)
    {
        var model = new LaborRemunerationParametersValue();
        CopyMatchingProperties(entity, model, isUpdate: false);
        _context.Set<LaborRemunerationParametersValue>().Add(model);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<LaborRemunerationParametersValue> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<LaborRemunerationParametersValue> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<LaborRemunerationParametersValue> { WasSuccess = false, Message = ex.Message };
        }
    }
    public async Task<IEnumerable<LaborRemunerationParametersValue>> GetComboAsync()
    {
        return await _context.Set<LaborRemunerationParametersValue>().AsNoTracking()
            .OrderBy(x => x.Id).ToListAsync();
    }
    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        //IQueryable<LaborRemunerationParametersValue> query = _context.Set<LaborRemunerationParametersValue>().AsNoTracking();

        //query = query.Include(x => x.LaborRemunerationParameters).Include(x => x.Statu).Include(x => x.Group);

        var query=  _context.LaborRemunerationParametersValues.AsNoTracking()
            .Include(x => x.LRP)
            .Include(x => x.Statu)
            .Include(x => x.Group)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(pagination.Filter))
        {
            query = query.Where(x =>
                x.Value.ToString().ToLower().Contains(pagination.Filter.ToLower()) ||
                x.Group!.Name!.ToLower().Contains(pagination.Filter.ToLower()) ||
                x.LRP!.Name!.ToLower().Contains(pagination.Filter.ToLower()) ||
                x.Statu!.Name.ToLower().Contains(pagination.Filter.ToLower()));
        }

        var totalRecords = await query.CountAsync();

        return new ActionResponse<int> { WasSuccess = true, Result = totalRecords };
    }
    public async Task<ActionResponse<LaborRemunerationParametersValue>> UpdateAsync(LaborRemunerationParametersValueDTO entity)
    {
        var model = await _context.Set<LaborRemunerationParametersValue>().FindAsync(entity.Id);
        if (model == null)
            return new ActionResponse<LaborRemunerationParametersValue> { WasSuccess = false, Message = "ERR005" };

        CopyMatchingProperties(entity, model, isUpdate: true);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<LaborRemunerationParametersValue> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<LaborRemunerationParametersValue> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<LaborRemunerationParametersValue> { WasSuccess = false, Message = ex.Message };
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