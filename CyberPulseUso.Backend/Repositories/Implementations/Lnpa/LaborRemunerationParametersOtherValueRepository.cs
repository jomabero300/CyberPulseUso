using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Helpers;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CyberPulseUso.Backend.Repositories.Implementations.Lnpa;

public class LaborRemunerationParametersOtherValueRepository : GenericRepository<LaborRemunerationParametersOtherValue>, ILaborRemunerationParametersOtherValueRepository
{
    private readonly ApplicationDbContext _context;

    public LaborRemunerationParametersOtherValueRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<IEnumerable<LaborRemunerationParametersOtherValue>>> GetAsync()
    {
        var excludedIds = new int[] { 1, 2, 3, 14 };
        var entity = await _context.Set<LaborRemunerationParametersOtherValue>()
                                            .AsNoTracking()
                                            .Include(x=>x.LRP)                                            
                                            .Where(x=>x.StatuId==1 && !excludedIds.Contains(x.Id)).ToListAsync();
        return new ActionResponse<IEnumerable<LaborRemunerationParametersOtherValue>> { WasSuccess = true, Result = entity.OrderBy(x => x.Id).ToList() };
    }

    public override async Task<ActionResponse<LaborRemunerationParametersOtherValue>> GetAsync(int id)
    {
        var entity = await _context.Set<LaborRemunerationParametersOtherValue>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<LaborRemunerationParametersOtherValue> { WasSuccess = false, Message = "ERR001" };
        return new ActionResponse<LaborRemunerationParametersOtherValue> { WasSuccess = true, Result = entity };
    }

    public override async Task<ActionResponse<LaborRemunerationParametersOtherValue>> DeleteAsync(int id)
    {
        var entity = await _context.Set<LaborRemunerationParametersOtherValue>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<LaborRemunerationParametersOtherValue> { WasSuccess = false, Message = "ERR001" };

        _context.Set<LaborRemunerationParametersOtherValue>().Remove(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<LaborRemunerationParametersOtherValue> { WasSuccess = true, Message = "Success" };
        }
        catch (Exception)
        {
            return new ActionResponse<LaborRemunerationParametersOtherValue> { WasSuccess = false, Message = "ERR002" };
        }
    }

    public override async Task<ActionResponse<IEnumerable<LaborRemunerationParametersOtherValue>>> GetAsync(PaginationDTO pagination)
    {
        IQueryable<LaborRemunerationParametersOtherValue> query = _context.Set<LaborRemunerationParametersOtherValue>().AsNoTracking();
        query = query.Include(x => x.Statu);
                // El modelo no tiene una propiedad string 'Name'; se conserva el filtro recibido
        // en PaginationDTO, pero no se aplica un filtro específico del modelo.
        query = query.OrderBy(x => x.Id);
        return new ActionResponse<IEnumerable<LaborRemunerationParametersOtherValue>>
        {
            WasSuccess = true,
            Result = await query.Paginate(pagination).ToListAsync()
        };
    }

    public async Task<ActionResponse<IEnumerable<LaborRemunerationParametersOtherValueCRUDDTO>>> AddAsync(IEnumerable<LaborRemunerationParametersOtherValueCRUDDTO> entity)
    {

        try
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            // 1. Obtener los registros actualmente activos
            var currentRecords = await _context
                .Set<LaborRemunerationParametersOtherValue>()
                .Where(x => x.StatuId == 1)
                .ToListAsync();


            // 2. Desactivar los registros anteriores
            foreach (var item in currentRecords)
            {
                item.StatuId = 2;
            }

            var model = entity.Select(x => new LaborRemunerationParametersOtherValue
            {
                LRPId = x.LRPId,
                Value = x.Value,
                StatuId = 1,
            }).ToList();


            _context.AddRange(model);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return new ActionResponse<IEnumerable<LaborRemunerationParametersOtherValueCRUDDTO>> { WasSuccess = true, Result = entity };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<IEnumerable<LaborRemunerationParametersOtherValueCRUDDTO>> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<IEnumerable<LaborRemunerationParametersOtherValueCRUDDTO>> { WasSuccess = false, Message = ex.Message };
        }
    }

    public async Task<IEnumerable<LaborRemunerationParametersOtherValue>> GetComboAsync()
    {
        return await _context.Set<LaborRemunerationParametersOtherValue>().AsNoTracking()
            .OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        IQueryable<LaborRemunerationParametersOtherValue> query = _context.Set<LaborRemunerationParametersOtherValue>().AsNoTracking();
        query = query.Include(x => x.Statu);
                // El modelo no tiene una propiedad string 'Name'; se conserva el filtro recibido
        // en PaginationDTO, pero no se aplica un filtro específico del modelo.
        var totalRecords = await query.CountAsync();
        return new ActionResponse<int> { WasSuccess = true, Result = totalRecords };
    }

    public async Task<ActionResponse<LaborRemunerationParametersOtherValue>> UpdateAsync(LaborRemunerationParametersOtherValueDTO entity)
    {
        var model = await _context.Set<LaborRemunerationParametersOtherValue>().FindAsync(entity.Id);
        if (model == null)
            return new ActionResponse<LaborRemunerationParametersOtherValue> { WasSuccess = false, Message = "ERR005" };

        CopyMatchingProperties(entity, model, isUpdate: true);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<LaborRemunerationParametersOtherValue> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<LaborRemunerationParametersOtherValue> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<LaborRemunerationParametersOtherValue> { WasSuccess = false, Message = ex.Message };
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
