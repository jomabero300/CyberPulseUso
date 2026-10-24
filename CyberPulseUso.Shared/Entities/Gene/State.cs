using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPulseUso.Shared.Entities.Gene;

[Table("States", Schema = "Gene")]
public class State
{
    public int Id { get; set; }

    [Column(TypeName = "varchar(100)")]
    [Display(Name = "Name", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Display(Name = "Country", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int CountryId { get; set; }

    public Country Country { get; set; } = null!;

    public ICollection<City>? Cities { get; set; }

    public int CitiesNumber =>
        Cities == null ? 0 : Cities.Count;
}
