using CyberPulseUso.Shared.Enums;

namespace CyberPulseUso.Shared.EntitiesDTO;

public class PaginationDTO
{
    public int Id { get; set; }

    public int Page { get; set; } = 1;

    public int RecordsNumber { get; set; } = 10;

    public string? Filter { get; set; }

    public string? Email { get; set; }

    public string? otro { get; set; }

    public UserType? UserType { get; set; }
}
