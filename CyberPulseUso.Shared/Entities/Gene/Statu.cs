using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPulseUso.Shared.Entities.Gene;

[Table("Status", Schema = "Gene")]
public class Statu
{
    public int Id { get; set; }

    [Column(TypeName = "varchar(80)")]
    [Display(Name = "Name", ResourceType = typeof(Literals))]
    [MaxLength(80, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    [Range(
        0,
        int.MaxValue,
        ErrorMessageResourceName = "RequiredField",
        ErrorMessageResourceType = typeof(Literals))]
    public int Nivel { get; set; }
}
