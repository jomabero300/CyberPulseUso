using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace CyberPulseUso.Shared.EntitiesDTO.Lnpa;

public class PayrollSettlementListDTO
{
    [Display(Name = "Date", ResourceType = typeof(Literals))]
    [Required(ErrorMessageResourceName = "RequiredField", ErrorMessageResourceType = typeof(Literals))]
    public DateTime Date { get; set; }
    public string? UserId { get; set; }
    [Display(Name = "User", ResourceType = typeof(Literals))]
    public string? UserName { get; set; }
    public int ChargeId { get; set; }

    [Display(Name = "Charge", ResourceType = typeof(Literals))]
    public string? ChargeName { get; set; }
    public double TotaltotalToPay { get; set; } = 0;

}
