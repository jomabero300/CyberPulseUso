using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPulseUso.Shared.Entities.Lnpa;

[Table("LaborRemunerationParametersValues", Schema = "Lnpa")]
public class LaborRemunerationParametersValue
{
    public int Id { get; set; }
    public int LRPId { get; set; }
    public int GroupId { get; set; }
    
    public DateTime Date { get; set; }

    [Display(Name = "Value", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public double Value { get; set; }

    [Display(Name = "Statu", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int StatuId { get; set; }
    public Statu? Statu { get; set; }
    public LaborRemunerationParameters? LRP { get; set; }

    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public Group? Group { get; set; }
}