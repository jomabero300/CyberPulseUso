using System.ComponentModel.DataAnnotations;

namespace CyberPulseUso.Shared.EntitiesDTO.Lnpa;

public class NormativeArticleDTO
{
    public int Id { get; set; }

    [Required]
    public int NormativeChapterId { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "El contenido es obligatorio.")]
    public string Content { get; set; } = null!;

    public int SortOrder { get; set; }
}