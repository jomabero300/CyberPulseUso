using CyberPulseUso.Shared.Resources;
using System.ComponentModel.DataAnnotations;

namespace CyberPulseUso.Shared.Enums;

public enum UserType
{
    [Display(Name = "SAdmin", ResourceType = typeof(Literals))]
    SAdm,
    [Display(Name = "Admi", ResourceType = typeof(Literals))]
    Admi,
    [Display(Name = "work", ResourceType = typeof(Literals))]
    work,
    [Display(Name = "User", ResourceType = typeof(Literals))]
    User
}
