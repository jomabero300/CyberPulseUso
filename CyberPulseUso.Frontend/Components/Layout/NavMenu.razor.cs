using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using static System.Net.WebRequestMethods;

namespace CyberPulseUso.Frontend.Components.Layout;

public partial class NavMenu
{
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

}