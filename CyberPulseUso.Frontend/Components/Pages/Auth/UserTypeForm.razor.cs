using CurrieTechnologies.Razor.SweetAlert2;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Enums;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.Localization;

namespace CyberPulseUso.Frontend.Components.Pages.Auth;

public partial class UserTypeForm
{
    private EditContext editContext = null!;
    private bool esSAdm = false;
    [Inject] private SweetAlertService SweetAlertService { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [EditorRequired, Parameter] public User user { get; set; } = null!;
    [EditorRequired, Parameter] public EventCallback OnValidSubmit { get; set; }
    [EditorRequired, Parameter] public EventCallback ReturnAction { get; set; }
    [CascadingParameter] private Task<AuthenticationState>? AuthState { get; set; }
    public bool FormPostedSuccessfully { get; set; } = false;
    private int selectedValue { get; set; }
    protected override void OnInitialized()
    {
        editContext = new(user);

        selectedValue = (int)user.UserType;
    }
    protected override async Task OnInitializedAsync()
    {
        if (AuthState != null)
        {
            var authState = await AuthState;
            esSAdm = authState.User.IsInRole("SAdm");
        }
    }
    private async Task OnBeforeInternalNavigation(LocationChangingContext context)
    {
        var formwasEditad = editContext.IsModified();

        if (!formwasEditad || FormPostedSuccessfully)
        {
            return;
        }

        var result = await SweetAlertService.FireAsync(new SweetAlertOptions
        {
            Title = Localizer["Confirmation"],
            Text = Localizer["LeaveAndLoseChanges"],
            Icon = SweetAlertIcon.Warning,
            ShowCancelButton = true,
        });

        var confirm = !string.IsNullOrEmpty(result.Value);

        if (confirm)
        {
            return;
        }

        context.PreventNavigation();
    }

    private IEnumerable<UserType> TiposVisibles()
    {
        var todos = Enum.GetValues<UserType>();

        // Si es SAdm, ve todos. Si no, todos menos SAdm.
        return esSAdm
            ? todos
            : todos.Where(t => t != UserType.SAdm);
    }
}