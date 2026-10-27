using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CyberPulseUso.Shared.Entities.Lnpa;

[Table("ConventionValues", Schema = "Lnpa")]
public class ConventionValue
{
    public int Id { get; set; }

    public int ConventionId { get; set; }
    public DateTime Date { get; set; }

    [Display(Name = "Prince", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public double Prince { get; set; }

    [Display(Name = "Statu", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int StatuId { get; set; }
    public Statu? Statu { get; set; }
    public Convention? Convention { get; set; }
}