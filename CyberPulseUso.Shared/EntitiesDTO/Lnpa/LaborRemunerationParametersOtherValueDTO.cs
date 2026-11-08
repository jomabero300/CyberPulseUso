using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace CyberPulseUso.Shared.EntitiesDTO.Lnpa;

public class LaborRemunerationParametersOtherValueDTO
{
    public int Id { get; set; }
    [Display(Name = "LaborRemunerationParameters", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int LRPId { get; set; }
    
    [Display(Name = "Value", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public double Value { get; set; }
    
    [Display(Name = "Statu", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public int StatuId { get; set; }
    
    public StatuDTO? Statu { get; set; }
    
    public LaborRemunerationParametersDTO? LRP { get; set; }
    
}

