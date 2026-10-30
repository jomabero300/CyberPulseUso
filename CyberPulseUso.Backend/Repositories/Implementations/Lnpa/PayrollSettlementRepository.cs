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
using System.Runtime.Intrinsics.X86;

namespace CyberPulseUso.Backend.Repositories.Implementations.Lnpa;

public class PayrollSettlementRepository : GenericRepository<PayrollSettlement>, IPayrollSettlementRepository
{
    private readonly ApplicationDbContext _context;

    public PayrollSettlementRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<PayrollSettlement>> GetAsync(int id)
    {
        var entity = await _context.Set<PayrollSettlement>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<PayrollSettlement> { WasSuccess = false, Message = "ERR001" };
        return new ActionResponse<PayrollSettlement> { WasSuccess = true, Result = entity };
    }

    public override async Task<ActionResponse<PayrollSettlement>> DeleteAsync(int id)
    {
        var entity = await _context.Set<PayrollSettlement>().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
        if (entity == null)
            return new ActionResponse<PayrollSettlement> { WasSuccess = false, Message = "ERR001" };

        _context.Set<PayrollSettlement>().Remove(entity);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<PayrollSettlement> { WasSuccess = true, Message = "Success" };
        }
        catch (Exception)
        {
            return new ActionResponse<PayrollSettlement> { WasSuccess = false, Message = "ERR002" };
        }
    }

    public override async Task<ActionResponse<IEnumerable<PayrollSettlement>>> GetAsync(PaginationDTO pagination)
    {
        IQueryable<PayrollSettlement> query = _context.Set<PayrollSettlement>().AsNoTracking();
        query = query.Include(x => x.Statu);
        // El modelo no tiene una propiedad string 'Name'; se conserva el filtro recibido
        // en PaginationDTO, pero no se aplica un filtro específico del modelo.
        query = query.OrderBy(x => x.Id);
        return new ActionResponse<IEnumerable<PayrollSettlement>>
        {
            WasSuccess = true,
            Result = await query.Paginate(pagination).ToListAsync()
        };
    }

    public async Task<ActionResponse<PayrollSettlement>> AddAsync(PayrollSettlementDTO entity)
    {
        var model = new PayrollSettlement();
        CopyMatchingProperties(entity, model, isUpdate: false);
        _context.Set<PayrollSettlement>().Add(model);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<PayrollSettlement> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<PayrollSettlement> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<PayrollSettlement> { WasSuccess = false, Message = ex.Message };
        }
    }

    public async Task<IEnumerable<PayrollSettlement>> GetComboAsync()
    {
        return await _context.Set<PayrollSettlement>().AsNoTracking()
            .OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        IQueryable<PayrollSettlement> query = _context.Set<PayrollSettlement>().AsNoTracking();
        query = query.Include(x => x.Statu);
        // El modelo no tiene una propiedad string 'Name'; se conserva el filtro recibido
        // en PaginationDTO, pero no se aplica un filtro específico del modelo.
        var totalRecords = await query.CountAsync();
        return new ActionResponse<int> { WasSuccess = true, Result = totalRecords };
    }

    public async Task<ActionResponse<PayrollSettlement>> UpdateAsync(PayrollSettlementDTO entity)
    {
        var model = await _context.Set<PayrollSettlement>().FindAsync(entity.Id);
        if (model == null)
            return new ActionResponse<PayrollSettlement> { WasSuccess = false, Message = "ERR005" };

        CopyMatchingProperties(entity, model, isUpdate: true);
        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<PayrollSettlement> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<PayrollSettlement> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<PayrollSettlement> { WasSuccess = false, Message = ex.Message };
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

    public async Task<ActionResponse<IEnumerable<PayrollSettlementDTO>>> GetLiqAsync(int id, string Id)
    {
        var list1 = await _context.Charges
            .AsNoTracking()
            .Where(c => c.Id == id)
            .SelectMany(c => c.Group!.LaborRemunerationParametersValues!)
            .SelectMany(v => v.LRP!.PayrollConcepts!
                .Select(p => new
                {
                    ChargeId = id,
                    PayrollConceptId = p.Id,
                    PayrollConceptName = p.Name,
                    UnitValue = v.Value,
                    Statu = new
                     {
                         Id = v.Statu!.Id,
                         Name = v.Statu.Name
                     }
                }))
            .ToListAsync();


        // 2. Ejecutamos la segunda consulta de forma independiente
        var list2 = await _context.PayrollConcepts
            .AsNoTracking()
            .Include(x => x.Statu)
            .Where(x => x.LRPId == null)
            .ToListAsync();

        // 3. Mapeamos y combinamos ambos resultados en memoria de C# usando LINQ
        var combinedResult = list1.Select(x => new PayrollSettlementDTO
        {
            Id = 0,
            UserId =Id,
            ChargeId = id,
            PayrollConceptId = x.PayrollConceptId,
            Name = x.PayrollConceptName,
            Amount = 0,
            UnitValue = x.UnitValue,
            AccruedValue = 0,
            DeductedValue = 0,
            StatuId = x.Statu.Id,
            Statu = x.Statu != null ? new StatuDTO
            {
                Id = x.Statu.Id,
                Name = x.Statu.Name ?? string.Empty
            } : null
        })
        .Concat(list2.Select(x => new PayrollSettlementDTO
        {
            Id = 0,
            UserId = Id,
            ChargeId = id,
            PayrollConceptId = x.Id,
            Name = x.Name,
            Amount = 0,
            UnitValue = 0,
            AccruedValue = 0,
            DeductedValue = 0,
            StatuId = x.StatuId,
            Statu = x.Statu != null ? new StatuDTO
            {
                Id = x.Statu.Id,
                Name = x.Statu.Name ?? string.Empty
            } : null
        }))
        .OrderBy(x=>x.PayrollConceptId)
        .ToList();


        var unitValue = combinedResult.Where(x => x.PayrollConceptId == 1 || x.PayrollConceptId == 2).Sum(x => x.UnitValue);

        var result = combinedResult.Select(x =>
        {
            if (x.PayrollConceptId == 5)
            {
                x.UnitValue = unitValue/7*0.75;
            }

            if (x.PayrollConceptId == 6)
            {
                x.UnitValue = unitValue/7*2.1;
            }

            if (x.PayrollConceptId == 11)
            {
                x.UnitValue = unitValue*0.9;
            }

            if (x.PayrollConceptId == 12)
            {
                x.UnitValue = unitValue;
            }
            if (x.PayrollConceptId == 13)
            {
                x.UnitValue = unitValue*2;
            }

            return x;
        }).OrderBy(x => x.PayrollConceptId)
        .ToList();

        return new ActionResponse<IEnumerable<PayrollSettlementDTO>>
        {
            WasSuccess = true,
            Result = combinedResult
        };
    }
}