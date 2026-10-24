using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPulseUso.Shared.Entities.Gene;

[Table("Countries", Schema = "Gene")]
public class Country
{
    public int Id { get; set; }

    [Column(TypeName = "varchar(100)")]
    [Display(Name = "Name", ResourceType = typeof(Literals))]
    [MaxLength(100, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Column(TypeName = "varchar(200)")]
    [Display(Name = "Image", ResourceType = typeof(Literals))]
    [MaxLength(200, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    public string? Image { get; set; }

    public ICollection<State>? States { get; set; }

    public int StatesNumber =>
        States == null ? 0 : States.Count;

    public ICollection<User>? Users { get; set; }

    public int UsersCount =>
        Users == null ? 0 : Users.Count;

    public string FullImage =>
        string.IsNullOrWhiteSpace(Image)
            ? "/Images/NoImage.png"
            : Image;
}
