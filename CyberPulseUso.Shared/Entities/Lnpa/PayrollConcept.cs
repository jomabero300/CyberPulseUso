using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPulseUso.Shared.Entities.Lnpa;

[Table("PayrollConcepts", Schema = "Lnpa")]
public class PayrollConcept
{
    public int Id { get; set; }

    [Column(TypeName = "varchar(80)")]
    [Display(Name = "Name", ResourceType = typeof(Literals))]
    [MaxLength(80, ErrorMessageResourceName = "MaxLength", ErrorMessageResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public string? Name { get; set; }

    [Display(Name = "Statu", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int StatuId { get; set; }
    public Statu? Statu { get; set; }

    public int? LRPId { get; set; }
    public LaborRemunerationParameters? LRP { get; set; }

    public ICollection<PayrollSettlement>? payrollSettlements { get; set; }
}
