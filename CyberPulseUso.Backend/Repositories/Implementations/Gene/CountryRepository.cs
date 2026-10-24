using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Responses;
using Microsoft.EntityFrameworkCore;
namespace CyberPulseUso.Backend.Repositories.Implementations.Gene;
public class CountryRepository : GenericRepository<Country>, ICountryRepository
{
    private readonly ApplicationDbContext _context; private readonly IWebHostEnvironment _env;
    public CountryRepository(ApplicationDbContext context, IWebHostEnvironment env) : base(context) { _context = context; _env = env; }
    public async Task<ActionResponse<Country>> GetAsync(int id, bool lb)
    { var entity = await _context.Countries.FirstOrDefaultAsync(c => c.Id == id); return entity == null ? new ActionResponse<Country> { WasSuccess = false, Message = "ERR001" } : new ActionResponse<Country> { WasSuccess = true, Result = entity }; }
    public async Task<ActionResponse<Country>> AddAsync(CountryDTO entity)
    {
        var country = new Country { Id = entity.Id, Name = entity.Name, Image = entity.Image, States = entity.States };
        if (!string.IsNullOrWhiteSpace(entity.Image)) country.Image = await UploadImageAsync(entity.Image, 0);
        _context.Add(country);
        try { await _context.SaveChangesAsync(); return new ActionResponse<Country> { WasSuccess = true, Result = country }; }
        catch (DbUpdateException) { return new ActionResponse<Country> { WasSuccess = false, Message = "ERR003" }; }
        catch (Exception ex) { return new ActionResponse<Country> { WasSuccess = false, Message = ex.Message }; }
    }
    public async Task<IEnumerable<Country>> GetComboAsync() => await _context.Countries.AsNoTracking().OrderBy(x => x.Name).ToListAsync();
    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    { var queryable = _context.Countries.AsNoTracking().AsQueryable(); if (!string.IsNullOrWhiteSpace(pagination.Filter)) queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower())); return new ActionResponse<int> { WasSuccess = true, Result = await queryable.CountAsync() }; }
    public async Task<ActionResponse<Country>> UpdateAsync(CountryDTO entity)
    {
        var country = await _context.Countries.FindAsync(entity.Id);
        if (country == null) return new ActionResponse<Country> { WasSuccess = false, Message = "ERR005" };
        country.Name = entity.Name;
        if (!string.IsNullOrWhiteSpace(entity.Image)) country.Image = await UploadImageAsync(entity.Image, entity.Id, country.Image!);
        _context.Update(country);
        try { await _context.SaveChangesAsync(); return new ActionResponse<Country> { WasSuccess = true, Result = country }; }
        catch (DbUpdateException) { return new ActionResponse<Country> { WasSuccess = false, Message = "ERR003" }; }
        catch (Exception ex) { return new ActionResponse<Country> { WasSuccess = false, Message = ex.Message }; }
    }
    private async Task<string?> UploadImageAsync(string image, int id, string imageOld = "")
    {
        string webRootPath = _env.WebRootPath; string directoryFolder = "\\Images\\Countries\\"; string directoryPath = Path.Combine(webRootPath, "Images", "Countries");
        var imageBase64 = Convert.FromBase64String(image); string pathImage = $"{Guid.NewGuid()}.jpg";
        if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
        await File.WriteAllBytesAsync(Path.Combine(directoryPath, pathImage), imageBase64);
        if (id != 0 && !string.IsNullOrWhiteSpace(imageOld)) { var oldPath = Path.Combine(webRootPath, imageOld.TrimStart('\\', '/')); if (File.Exists(oldPath)) File.Delete(oldPath); }
        return $"{directoryFolder}{pathImage}";
    }
}
