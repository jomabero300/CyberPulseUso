using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPulseUso.Shared.Entities.Lnpa;

[Table("NormativeArticles", Schema = "Lnpa")]
public class NormativeArticle
{
    public int Id { get; set; }

    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int NormativeChapterId { get; set; }

    [Column(TypeName = "varchar(20)")]
    [Display(Name = "Name", ResourceType = typeof(Literals))]
    [MaxLength(20, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Name { get; set; } = null!;

    [Column(TypeName = "varchar(150)")]
    [Display(Name = "Description", ResourceType = typeof(Literals))]
    [MaxLength(150, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Description { get; set; } = null!;

    [Column(TypeName = "varchar(2700)")]
    [Display(Name = "Content", ResourceType = typeof(Literals))]
    [MaxLength(2700, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string Content { get; set; } = null!;

    [Display(Name = "SortOrder", ResourceType = typeof(Literals))]
    public int SortOrder { get; set; }

    public NormativeChapter? NormativeChapter { get; set; }
}