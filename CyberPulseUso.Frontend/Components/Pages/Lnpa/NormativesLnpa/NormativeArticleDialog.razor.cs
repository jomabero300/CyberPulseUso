using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.NormativesLnpa;

public partial class NormativeArticleDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = null!;

    [Parameter]
    public string NormativeName { get; set; } = string.Empty;

    [Parameter]
    public string ChapterName { get; set; } = string.Empty;

    [Parameter]
    public NormativeArticleDTO Article { get; set; } = null!;

    private void Cancel()
    {
        MudDialog.Cancel();
    }
}