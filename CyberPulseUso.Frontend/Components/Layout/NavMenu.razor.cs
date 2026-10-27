using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace CyberPulseUso.Frontend.Components.Layout;

public partial class NavMenu
{
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

}