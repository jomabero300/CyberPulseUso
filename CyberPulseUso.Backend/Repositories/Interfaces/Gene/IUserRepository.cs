using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.EntitiesDTO;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Enums;
using CyberPulseUso.Shared.Responses;
using Microsoft.AspNetCore.Identity;

namespace CyberPulseUso.Backend.Repositories.Interfaces.Gene;

public interface IUserRepository
{
    Task<ActionResponse<IEnumerable<User>>> GetAsync(PaginationDTO pagination);
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);
    Task<string> GeneratePasswordResetTokenAsync(User user);
    Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
    Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
    Task<IdentityResult> UpdateUserAsync(User user);
    Task UpdateUserAsync(string userId, UserType userType);
    Task<User> GetUserAsync(Guid userId);
    Task<ActionResponse<User>> GetUserAsync(string userDocument, UserType userType);
    Task<IEnumerable<User>> GetAsync(UserType userType);
    Task<IEnumerable<User>> GetAsync(string id);
    Task<string> GenerateEmailConfirmationTokenAsync(User user);
    Task<IdentityResult> ConfirmEmailAsync(User user, string token);
    Task<SignInResult> LoginAsync(LoginDTO model);
    Task ResetAccessFailedCountAsync(User user);
    Task LogoutAsync();
    Task<User> GetUserAsync(string email);
    Task<IdentityResult> AddUserAsync(User user, string password);
    Task CheckRoleAsync(string roleName);
    Task AddUserToRoleAsync(User user, string roleName);
    Task<bool> IsUserInRoleAsync(User user, string roleName);
}
