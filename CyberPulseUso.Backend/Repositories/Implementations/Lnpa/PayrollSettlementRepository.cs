using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Helpers;
using CyberPulseUso.Backend.Repositories.Interfaces.Lnpa;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa.Reports;
using CyberPulseUso.Shared.Responses;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

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
        if (!string.IsNullOrWhiteSpace(pagination.Email))
        {
            var queryable = _context.PayrollSettlements
                .AsNoTracking()
                .Where(x => x.UserId == pagination.Email);

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                var filter = pagination.Filter.Trim();

                if (DateTime.TryParse(filter, out DateTime date))
                {
                    // Si es una fecha, busca exclusivamente por fecha.
                    var startDate = date.Date;
                    var endDate = startDate.AddDays(1);

                    queryable = queryable.Where(x =>
                        x.Date >= startDate &&
                        x.Date < endDate);
                }
                else
                {
                    // Si no es una fecha, busca por cargo, nombre o apellido.
                    var search = filter.ToLower();

                    queryable = queryable.Where(x =>
                        (x.Charge != null &&
                         x.Charge.Name != null &&
                         x.Charge.Name.ToLower().Contains(search)) ||

                        (x.User != null &&
                         x.User.FirstName != null &&
                         x.User.FirstName.ToLower().Contains(search)) ||

                        (x.User != null &&
                         x.User.LastName != null &&
                         x.User.LastName.ToLower().Contains(search))
                    );
                }
            }

            var result = await queryable
                .GroupBy(x => new { x.Date, x.ChargeId })
                .Select(g => new PayrollSettlement
                {
                    Id = 0,
                    Date = g.Key.Date,
                    UserId = g.First().UserId,
                    ChargeId = g.Key.ChargeId,
                    PayrollConceptId = 0,
                    Amount = 0,
                    UnitValue = 0,
                    AccruedValue = g.Sum(p => p.AccruedValue),
                    DeductedValue = g.Sum(p => p.DeductedValue),
                    Charge = g.First().Charge,
                    User = g.First().User
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return new ActionResponse<IEnumerable<PayrollSettlement>>
            {
                WasSuccess = true,
                Result = result
            };

        }
        else
        {
            var queryable = _context.PayrollSettlements
                .AsNoTracking()
                .Include(x => x.User)
                .Include(x => x.Charge)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                var filter = pagination.Filter.Trim();

                if (DateTime.TryParse(filter, out DateTime date))
                {
                    // Buscar exclusivamente por fecha.
                    var startDate = date.Date;
                    var endDate = startDate.AddDays(1);

                    queryable = queryable.Where(x =>
                        x.Date >= startDate &&
                        x.Date < endDate);
                }
                else
                {
                    // Buscar exclusivamente por nombre del cargo.
                    var search = filter.ToLower();

                    queryable = queryable.Where(x =>
                        (x.Charge != null &&
                        x.Charge.Name != null &&
                        x.Charge.Name.ToLower().Contains(search)) ||

                        (x.User != null &&
                         x.User.FirstName != null &&
                         x.User.FirstName.ToLower().Contains(search)) ||

                        (x.User != null &&
                         x.User.LastName != null &&
                         x.User.LastName.ToLower().Contains(search))
                    );
                }
            }

            var result = await queryable
                .GroupBy(x => new { x.Date, x.ChargeId })
                .Select(g => new PayrollSettlement
                {
                    Id = 0,
                    Date = g.Key.Date,
                    UserId = g.First().UserId,
                    ChargeId = g.Key.ChargeId,
                    PayrollConceptId = 0,
                    Amount = 0,
                    UnitValue = 0,
                    AccruedValue = g.Sum(p => p.AccruedValue),
                    DeductedValue = g.Sum(p => p.DeductedValue),
                    Charge = g.First().Charge,
                    User = g.First().User
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            return new ActionResponse<IEnumerable<PayrollSettlement>>
            {
                WasSuccess = true,
                Result = result
            };

        }
    }

    public async Task<ActionResponse<IEnumerable<PayrollSettlement>>> AddAsync(IEnumerable<PayrollSettlementDTO> entity)
    {
        var model = new List<PayrollSettlement>();

        CopyMatchingProperties(entity, model, false);

        await _context.AddRangeAsync(model);

        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<IEnumerable<PayrollSettlement>> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<IEnumerable<PayrollSettlement>> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<IEnumerable<PayrollSettlement>> { WasSuccess = false, Message = ex.Message };
        }
    }

    public async Task<IEnumerable<PayrollSettlement>> GetComboAsync()
    {
        return await _context.Set<PayrollSettlement>().AsNoTracking()
            .OrderBy(x => x.Id).ToListAsync();
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        //DateTime Fecha = DateTime.Parse(pagination.Filter!);

        if (!string.IsNullOrWhiteSpace(pagination.Email))
        {
            var queryable = !string.IsNullOrWhiteSpace(pagination.Filter) ?
                                    _context.PayrollSettlements
                                                .AsNoTracking()
                                                .Where(x => x.UserId == pagination.Email && x.Charge!.Name!.Contains(pagination.Filter))
                                                .GroupBy(x => new { x.Date, x.ChargeId })
                                                .Select(g => g.Sum(p => (decimal?)p.AccruedValue) - g.Sum(p => (decimal?)p.DeductedValue))
                                                .AsQueryable() :
                                    _context.PayrollSettlements
                                                .AsNoTracking()
                                                .Where(x => x.UserId == pagination.Email)
                                                .GroupBy(x => new { x.Date, x.ChargeId })
                                                .Select(g => g.Sum(p => (decimal?)p.AccruedValue) - g.Sum(p => (decimal?)p.DeductedValue))
                                                .AsQueryable();
            int count = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = count
            };
        }
        else
        {
            var queryable = !string.IsNullOrWhiteSpace(pagination.Filter) ?
                                    _context.PayrollSettlements
                                                .AsNoTracking()
                                                .Where(x=>x.Charge!.Name!.Contains(pagination.Filter))
                                                .GroupBy(x => new { x.Date, x.ChargeId })
                                                .Select(g => g.Sum(p => (decimal?)p.AccruedValue) - g.Sum(p => (decimal?)p.DeductedValue))
                                                .AsQueryable() :
                                    _context.PayrollSettlements
                                                .AsNoTracking()
                                                .GroupBy(x => new { x.Date, x.ChargeId })
                                                .Select(g => g.Sum(p => (decimal?)p.AccruedValue) - g.Sum(p => (decimal?)p.DeductedValue))
                                                .AsQueryable();
            int count = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = count
            };
        }
    }

    public async Task<ActionResponse<List<PayrollSettlement>>> UpdateAsync(List<PayrollSettlementDTO> entity)
    {

        var firstEntity = entity.FirstOrDefault();
        if (firstEntity == null)
            return new ActionResponse<List<PayrollSettlement>> { WasSuccess = false, Message = "ERR005" };

        var model = await _context.Set<PayrollSettlement>().Where(
            x => x.ChargeId == firstEntity.ChargeId &&
            x.UserId == firstEntity.UserId && 
            x.Date.Year== firstEntity.Date.Year &&
            x.Date.Month== firstEntity.Date.Month &&
            x.Date.Day== firstEntity.Date.Day &&
            x.Date.Hour==firstEntity.Date.Hour &&
            x.Date.Minute== firstEntity.Date.Minute
            ).ToListAsync();

        if (model == null || model.Count()==0)
            return new ActionResponse<List<PayrollSettlement>> { WasSuccess = false, Message = "ERR005" };
        
        var entityDict = entity.ToDictionary(x => x.Id);

        foreach (var item in model)
        {
            if (entityDict.TryGetValue(item.Id, out var match))
            {
                item.Amount = match.Amount;
                item.AccruedValue = match.AccruedValue;
                item.DeductedValue = match.DeductedValue;
            }
        }

        _context.UpdateRange(model);
        //CopyMatchingProperties(entity, model, isUpdate: true);

        try
        {
            await _context.SaveChangesAsync();
            return new ActionResponse<List<PayrollSettlement>> { WasSuccess = true, Result = model };
        }
        catch (DbUpdateException)
        {
            return new ActionResponse<List<PayrollSettlement>> { WasSuccess = false, Message = "ERR003" };
        }
        catch (Exception ex)
        {
            return new ActionResponse<List<PayrollSettlement>> { WasSuccess = false, Message = ex.Message };
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
        DateTime date = DateTime.UtcNow;
        // 3. Mapeamos y combinamos ambos resultados en memoria de C# usando LINQ
        var combinedResult = list1.Select(x => new PayrollSettlementDTO
        {
            Id = 0,
            Date = date,
            UserId = Id,
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
            Date = date,
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
        .OrderBy(x => x.PayrollConceptId)
        .ToList();


        //var unitValue = combinedResult.Where(x => x.PayrollConceptId == 1 || x.PayrollConceptId == 2).Sum(x => x.UnitValue);

        //var result = combinedResult.Select(x =>
        //{
        //    if (x.PayrollConceptId == 5)
        //    {
        //        x.UnitValue = unitValue / 7 * 0.75;
        //    }

        //    if (x.PayrollConceptId == 6)
        //    {
        //        x.UnitValue = unitValue / 7 * 2.1;
        //    }

        //    if (x.PayrollConceptId == 11)
        //    {
        //        x.UnitValue = unitValue * 0.9;
        //    }

        //    if (x.PayrollConceptId == 12)
        //    {
        //        x.UnitValue = unitValue;
        //    }
        //    if (x.PayrollConceptId == 13)
        //    {
        //        x.UnitValue = unitValue * 2;
        //    }

        //    return x;
        //}).OrderBy(x => x.PayrollConceptId)
        //.ToList();

        return new ActionResponse<IEnumerable<PayrollSettlementDTO>>
        {
            WasSuccess = true,
            Result = combinedResult
        };
    }

    private static void CopyMatchingProperties<TSource, TTarget>(IEnumerable<TSource> source, ICollection<TTarget> target, bool isUpdate) where TTarget : new()
    {
        if (source == null || target == null)
            return;

        target.Clear();

        foreach (var sourceItem in source)
        {
            var targetItem = new TTarget();

            CopyMatchingProperties(sourceItem!, targetItem!, isUpdate);

            target.Add(targetItem);
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

    public async Task<ActionResponse<IEnumerable<PayrollSettlementListDTO>>> GetAsync(string Id)
    {
        var _model = await _context.PayrollSettlements.AsNoTracking().Include(x=>x.Charge).Include(x=>x.User)
            .Where(p => p.UserId == Id)
            .GroupBy(p => new { p.Date,p.UserId,p.User!.FullName, p.Charge!.Name,p.ChargeId })
            .Select(g => new
            {
                Date = g.Key.Date,
                UserId = g.Key.UserId,
                UserName = g.Key.FullName,
                ChargeId = g.Key.ChargeId,
                ChargeName = g.Key.Name,
                TotalPagar = g.Sum(p => p.AccruedValue) - g.Sum(p => p.DeductedValue)
            })
            .OrderByDescending(x => x.Date)
            .ToListAsync();

        var result = _model.Select(x => new PayrollSettlementListDTO
        {
            Date = x.Date,
            UserId = x.UserId,
            UserName = x.UserName,
            ChargeId = x.ChargeId,
            ChargeName = x.ChargeName,
            TotaltotalToPay=x.TotalPagar
        }).ToList();

        return new ActionResponse<IEnumerable<PayrollSettlementListDTO>>
        {
            WasSuccess = true,
            Result = result.ToList()
        };
    }

    public async Task<ActionResponse<IEnumerable<PayrollSettlementDTO>>> GetAsync(DateTime fecha, int Id, string id)
    {
        var _datos = await _context.PayrollSettlements.AsNoTracking().Include(x => x.PayrollConcept).Include(x => x.Statu)
                                                    .Where(x =>
                                                    x.ChargeId == Id &&
                                                    x.Date.Year == fecha.Year &&
                                                    x.Date.Month == fecha.Month &&
                                                    x.Date.Day == fecha.Day &&
                                                    x.Date.Hour == fecha.Hour &&
                                                    x.Date.Minute == fecha.Minute &&
                                                    x.UserId == id)
                                                    .ToListAsync();
        var result = _datos.Select(x => new PayrollSettlementDTO
        {
            Id = x.Id,
            Date = x.Date,
            UserId = x.UserId,
            ChargeId = x.ChargeId,
            PayrollConceptId = x.PayrollConceptId,
            Name = x.PayrollConcept!.Name,
            Amount = x.Amount,
            UnitValue = x.UnitValue,
            AccruedValue = x.AccruedValue,
            DeductedValue = x.DeductedValue,
            StatuId = x.StatuId,
            Statu = new StatuDTO { Id = x.StatuId, Name = x.Statu!.Name }
        }).ToList();

        return new ActionResponse<IEnumerable<PayrollSettlementDTO>>
        {
            WasSuccess = true,
            Result = result.OrderBy(x => x.PayrollConceptId).ToList()
        };
    }
    public async Task<ActionResponse<LiquidacionReportModel>> GetAsync(DateTime fecha, int Id, string id, bool esta)
    {
        var _result= await _context.PayrollSettlements.AsNoTracking()
                                         .Include(x => x.Charge).ThenInclude(x => x!.Group)
                                         .Include(x => x.User)
                                         .Include(x => x.PayrollConcept)
                                         .Include(x => x.Statu)
                    .Where(x =>
                    x.ChargeId == Id &&
                    x.Date.Year == fecha.Year &&
                    x.Date.Month == fecha.Month &&
                    x.Date.Day == fecha.Day &&
                    x.Date.Hour == fecha.Hour &&
                    x.Date.Minute == fecha.Minute &&
                    x.UserId == id)
                    .ToListAsync();

        var result = new LiquidacionReportModel()
        {
            Cargo=_result.First().Charge!.Name!,
            Fecha=_result.First().Date,
            Grupo=_result.First().Charge!.Group!.Name!,
            RealizadoPor= $"{_result.First().User!.FirstName} {_result.First().User!.LastName}",
            RetencionFuente=2,
            RetencionSaludPension=1,
            Items= _result.Select(x => new ItemLiquidacion {
                PayrollConceptId=x.PayrollConceptId,
                Cantidad=x.Amount,
                Concepto=x.PayrollConcept!.Name!,
                ValorUnitario=x.UnitValue,
                Deducido=x.DeductedValue,
                DevengadoTotal=x.AccruedValue
            }).ToList()
        };

        return new ActionResponse<LiquidacionReportModel>
        {
            WasSuccess = true,
            Result = result
        };
    }

    //dashboard
    public Task<ActionResponse<PayrollDashboardDTO>> GetWorkerDashboardAsync(string userId)
    {
        var query = _context.PayrollSettlements
            .AsNoTracking()
            .Where(x => x.UserId == userId);

        return BuildDashboardAsync(query);
    }

    public Task<ActionResponse<PayrollDashboardDTO>> GetAdminDashboardAsync()
    {
        var query = _context.PayrollSettlements
            .AsNoTracking()
            .AsQueryable();

        return BuildDashboardAsync(query);
    }
    private async Task<ActionResponse<PayrollDashboardDTO>> BuildDashboardAsync(IQueryable<PayrollSettlement> query)
    {
        try
        {
            var today = DateTime.Today;
            var startDate = new DateTime(today.Year, 1, 1);
            var endDate = today.AddDays(1);

            query = query.Where(x =>
                x.Date >= startDate &&
                x.Date < endDate &&
                x.Statu != null &&
                x.Statu.Name == "Activo");

            var totalAccrued = await query
                .SumAsync(x => (double?)x.AccruedValue) ?? 0;

            var totalDeducted = await query
                .SumAsync(x => (double?)x.DeductedValue) ?? 0;

            var settlementCount = await query.CountAsync();

            var workerCount = await query
                .Where(x => x.UserId != null)
                .Select(x => x.UserId)
                .Distinct()
                .CountAsync();

            var monthlyResults = await query
                .GroupBy(x => new
                {
                    x.Date.Year,
                    x.Date.Month
                })
                .Select(g => new PayrollDashboardMonthlyDTO
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    AccruedValue = g.Sum(x => x.AccruedValue),
                    DeductedValue = g.Sum(x => x.DeductedValue),
                    NetValue = g.Sum(x => x.AccruedValue - x.DeductedValue)
                })
                .ToListAsync();

            // Incluye también los meses sin liquidaciones.
            var monthlyData = Enumerable.Range(1, today.Month)
                .Select(month =>
                {
                    var data = monthlyResults.FirstOrDefault(x =>
                        x.Year == today.Year && x.Month == month);

                    return data ?? new PayrollDashboardMonthlyDTO
                    {
                        Year = today.Year,
                        Month = month,
                        AccruedValue = 0,
                        DeductedValue = 0,
                        NetValue = 0
                    };
                })
                .ToList();

            var recentSettlements = await query
                .GroupBy(x => new
                {
                    x.ChargeId,
                    Date = x.Date.Date,
                    ChargeName = x.Charge != null
                        ? x.Charge.Name
                        : string.Empty
                })
                .Select(g => new PayrollDashboardRecentDTO
                {
                    Id = g.Min(x => x.Id),
                    Date = g.Key.Date,
                    UserName = string.Empty,
                    ChargeName = g.Key.ChargeName,
                    AccruedValue = g.Sum(x => x.AccruedValue),
                    DeductedValue = g.Sum(x => x.DeductedValue),
                    NetValue = g.Sum(x => x.AccruedValue)
                              - g.Sum(x => x.DeductedValue)
                })
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Id)
                .Take(10)
                .ToListAsync();

            var dashboard = new PayrollDashboardDTO
            {
                TotalAccrued = totalAccrued,
                TotalDeducted = totalDeducted,
                TotalNet = totalAccrued - totalDeducted,
                SettlementCount = settlementCount,
                WorkerCount = workerCount,
                MonthlyData = monthlyData,
                RecentSettlements = recentSettlements
            };

            return new ActionResponse<PayrollDashboardDTO>
            {
                WasSuccess = true,
                Result = dashboard
            };
        }
        catch (Exception ex)
        {
            return new ActionResponse<PayrollDashboardDTO>
            {
                WasSuccess = false,
                Message = ex.Message
            };
        }
    }
}