using CyberPulseUso.Shared.Resources;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPulseUso.Shared.Entities.Gene;

[Table("RefreshTokens", Schema = "Gene")]
public class RefreshToken
{
    public int Id { get; set; }

    [Column(TypeName = "nvarchar(450)")]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string UserId { get; set; } = null!;

    // Se almacena el HASH, nunca el token original.
    [Column(TypeName = "varchar(64)")]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string TokenHash { get; set; } = null!;
    // Identifica la sesión de un dispositivo
    public Guid FamilyId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }
    // Límite absoluto de duración de la sesión.
    public DateTime SessionExpiresAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    [Column(TypeName = "varchar(64)")]
    public string? ReplacedByTokenHash { get; set; }

    public bool IsRevoked => RevokedAt.HasValue;

    public User User { get; set; } = null!;

}