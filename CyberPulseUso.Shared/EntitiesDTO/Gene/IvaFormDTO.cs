using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace CyberPulseUso.Shared.EntitiesDTO.Gene;

public class IvaFormDTO : IvaDTO
{
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public StatuDTO? Statu { get; set; }
}
