using CyberPulseUso.Backend.Data;
using CyberPulseUso.Backend.Helpers;
using CyberPulseUso.Backend.Repositories.Interfaces.Gene;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Enums;
using CyberPulseUso.Shared.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace CyberPulseUso.Backend.Repositories.Implementations.Gene;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context; private readonly UserManager<User> _userManager; private readonly RoleManager<IdentityRole> _roleManager; private readonly SignInManager<User> _signInManager; private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
    public UserRepository(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IWebHostEnvironment env, IConfiguration configuration) { _context = context; _userManager = userManager; _roleManager = roleManager; _signInManager = signInManager; _env = env; _configuration = configuration; }
    public async Task<IdentityResult> AddUserAsync(User user, string password) { if (!string.IsNullOrWhiteSpace(user.Photo)) user.Photo = await HtmlUtilities.UploadImageAsync(user.Photo, $"{_env.WebRootPath}{_configuration["FolderUsers"]}"); return await _userManager.CreateAsync(user, password); }
    public async Task AddUserToRoleAsync(User user, string roleName) { await _userManager.AddToRoleAsync(user, roleName); }
    public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword) => await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    public async Task CheckRoleAsync(string roleName) { if (!await _roleManager.RoleExistsAsync(roleName)) await _roleManager.CreateAsync(new IdentityRole { Name = roleName }); }
    public async Task<IdentityResult> ConfirmEmailAsync(User user, string token) => await _userManager.ConfirmEmailAsync(user, token);
    public async Task<string> GenerateEmailConfirmationTokenAsync(User user) => await _userManager.GenerateEmailConfirmationTokenAsync(user);
    public async Task<string> GeneratePasswordResetTokenAsync(User user) => await _userManager.GeneratePasswordResetTokenAsync(user);
    public async Task<IEnumerable<User>> GetAsync(UserType userType) => await _context.Users.AsNoTracking().Where(x => x.UserType == userType).OrderBy(x => x.FirstName).ToListAsync();
    public async Task<User> GetUserAsync(Guid userId) { var user = await _context.Users.Include(x => x.Country).FirstOrDefaultAsync(x => x.Id == userId.ToString()); if (user != null && !string.IsNullOrWhiteSpace(user.Photo)) user.Photo = $"{_configuration["UrlBackend"]}{_configuration["FolderUsers"]}{user.Photo}"; return user!; }
    public async Task<User> GetUserAsync(string email) { var user = await _context.Users.AsNoTracking().Include(x => x.Country).FirstOrDefaultAsync(x => x.Email == email); if (user != null && !string.IsNullOrWhiteSpace(user.Photo)) user.Photo = $"{_configuration["UrlBackend"]}{_configuration["FolderUsers"]}{user.Photo}"; return user!; }
    public async Task<bool> IsUserInRoleAsync(User user, string roleName) => await _userManager.IsInRoleAsync(user, roleName);
    public async Task<SignInResult> LoginAsync(LoginDTO model) => await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, true);
    public async Task LogoutAsync() => await _signInManager.SignOutAsync();
    public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string password) => await _userManager.ResetPasswordAsync(user, token, password);
    public async Task<IdentityResult> UpdateUserAsync(User user)
    {
        var oldUser = await _context.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
        if (oldUser == null) return IdentityResult.Failed(new IdentityError { Description = "Usuario no encontrado" });
        oldUser.FirstName = user.FirstName; oldUser.LastName = user.LastName; oldUser.PhoneNumber = user.PhoneNumber;
        if (!string.IsNullOrWhiteSpace(user.Photo) && !user.Photo.Contains(_configuration["FolderUsers"]!)) { var oldImage = !string.IsNullOrWhiteSpace(oldUser.Photo) ? $"{_env.WebRootPath}{_configuration["FolderUsers"]!}{oldUser.Photo}" : ""; oldUser.Photo = await HtmlUtilities.UploadImageAsync(user.Photo, $"{_env.WebRootPath}{_configuration["FolderUsers"]!}", oldImage); }
        return await _userManager.UpdateAsync(oldUser);
    }
    public async Task<ActionResponse<User>> GetUserAsync(string userDocument, UserType userType)
    {
        var entity = userDocument.Length < 15 ? await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.DocumentId == userDocument && x.UserType == userType) : await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userDocument && x.UserType == userType);
        return entity == null ? new ActionResponse<User> { WasSuccess = false, Message = "ERR001" } : new ActionResponse<User> { WasSuccess = true, Result = entity };
    }
    public async Task<IEnumerable<User>> GetAsync(string id) => await _context.Users.AsNoTracking().Where(x => x.Id == id).OrderBy(x => x.FirstName).ToListAsync();
    public async Task<ActionResponse<IEnumerable<User>>> GetAsync(PaginationDTO pagination)
    {
        var query = _context.Users.AsNoTracking().Include(x => x.Country).AsQueryable(); if (pagination.UserType != null) query = query.Where(x => x.UserType == pagination.UserType);
        if (!string.IsNullOrWhiteSpace(pagination.Filter)) query = query.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower()) || x.LastName.ToLower().Contains(pagination.Filter.ToLower()));
        var list = await query.OrderBy(x => x.FirstName).ThenBy(x => x.LastName).Select(x => new User { Id = x.Id, FirstName = x.FirstName, LastName = x.LastName, Photo = !string.IsNullOrWhiteSpace(x.Photo) ? $"{_configuration["UrlBackend"]}{_configuration["FolderUsers"]}{x.Photo}" : $"{_configuration["Url Frontend"]}/Images/NoImage.png", UserType = x.UserType, NormalizedUserName = x.NormalizedUserName, Email = x.Email, NormalizedEmail = x.NormalizedEmail, EmailConfirmed = x.EmailConfirmed, PasswordHash = x.PasswordHash, SecurityStamp = x.SecurityStamp, ConcurrencyStamp = x.ConcurrencyStamp, PhoneNumber = x.PhoneNumber, PhoneNumberConfirmed = x.PhoneNumberConfirmed, TwoFactorEnabled = x.TwoFactorEnabled, LockoutEnd = x.LockoutEnd, LockoutEnabled = x.LockoutEnabled, AccessFailedCount = x.AccessFailedCount, Country = x.Country, CountryId = x.CountryId, DocumentId = x.DocumentId }).Paginate(pagination).ToListAsync();
        return new ActionResponse<IEnumerable<User>> { WasSuccess = true, Result = list };
    }
    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) { var query = _context.Users.AsNoTracking().AsQueryable(); if (!string.IsNullOrWhiteSpace(pagination.Filter)) query = query.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower()) || x.LastName.ToLower().Contains(pagination.Filter.ToLower())); return new ActionResponse<int> { WasSuccess = true, Result = await query.CountAsync() }; }
    public async Task UpdateUserAsync(string userId, UserType userType) { var entity = await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId); if (entity != null) await _context.Database.ExecuteSqlInterpolatedAsync($"UPDATE Admi.AspNetUsers SET UserType={(int)userType} WHERE Id={userId}"); }
    public async Task ResetAccessFailedCountAsync(User user) => await _userManager.ResetAccessFailedCountAsync(user);
}
