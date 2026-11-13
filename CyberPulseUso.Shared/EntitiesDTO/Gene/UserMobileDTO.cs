using CyberPulseUso.Shared.Enums;

namespace CyberPulseUso.Shared.EntitiesDTO.Gene;

public class UserMobileDTO
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Photo { get; set; } = string.Empty;
    public UserType UserType { get; set; }
}