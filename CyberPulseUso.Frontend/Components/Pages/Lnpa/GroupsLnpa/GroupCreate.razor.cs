using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.GroupsLnpa;

public partial class GroupCreate
{
    private GroupForm? GroupFormRef;
    private GroupDTO GroupDTO = new();

    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ISqlInjValRepository _sqlValidator { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;

    private async Task CreateAsync()
    {
        var responseHttp = await Repository.PostAsync("/api/groups/full", GroupDTO);
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            return;
        }

        MudDialog.Close(DialogResult.Ok(true));
        Snackbar.Add(Localizer["RecordCreateOk"], Severity.Success);
    }

    private void Return()
    {
        GroupFormRef!.FormPostedSuccessfully = true;
        MudDialog.Cancel();
    }
}
