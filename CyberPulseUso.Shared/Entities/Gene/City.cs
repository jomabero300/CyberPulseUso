using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPulseUso.Shared.Entities.Gene;

[Table("Cities", Schema = "Gene")]
public class City
{
    public int Id { get; set; }

    [Column(TypeName = "varchar(100)")]
    [Display(Name = "Name", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int StateId { get; set; }

    public State? State { get; set; }

    public ICollection<Neighborhood>? Neighborhoods { get; set; }

    public int NeighborhoodsCount =>
        Neighborhoods == null ? 0 : Neighborhoods.Count;
}
