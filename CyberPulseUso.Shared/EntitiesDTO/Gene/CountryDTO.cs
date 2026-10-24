using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace CyberPulseUso.Shared.EntitiesDTO.Gene;

public class CountryDTO
{
    public int Id { get; set; }

    [Display(Name = "Name", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Display(Name = "Image", ResourceType = typeof(Literals))]
    public string? Image { get; set; }

    public ICollection<State>? States { get; set; }

    public int StatesNumber => States == null ? 0 : States.Count;

    public string FullImage =>
        string.IsNullOrWhiteSpace(Image)
            ? "/Images/NoImage.png"
            : Image;
}
