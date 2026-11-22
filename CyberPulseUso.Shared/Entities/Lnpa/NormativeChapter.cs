namespace CyberPulseUso.Shared.Entities.Lnpa;

using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("NormativeChapters", Schema = "Lnpa")]
public class NormativeChapter
{
    public int Id { get; set; }

    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int NormativeId { get; set; }

    [Column(TypeName = "varchar(20)")]
    [Display(Name = "Name", ResourceType = typeof(Literals))]
    [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Column(TypeName = "varchar(70)")]
    [Display(Name = "Description", ResourceType = typeof(Literals))]
    [MaxLength(70, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Description { get; set; } = null!;

    [Display(Name = "SortOrder", ResourceType = typeof(Literals))]
    public int SortOrder { get; set; }

    public Normative? Normative { get; set; }

    public ICollection<NormativeArticle> Articles { get; set; }
        = new List<NormativeArticle>();
}
