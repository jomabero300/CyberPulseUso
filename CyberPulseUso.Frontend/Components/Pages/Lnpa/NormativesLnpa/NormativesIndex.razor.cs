using CyberPulseUso.Frontend.Repositories;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.NormativesLnpa;

public partial class NormativesIndex
{
    private List<NormativeDTO> normatives = [];
    private NormativeTreeDTO? selectedNormative;
    private bool loading = true;

    [Inject] private IRepository repository { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IStringLocalizer<Literals> Localizer { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;

    private int? selectedNormativeId;

    protected override async Task OnInitializedAsync()
    {
        await LoadNormativesAsync();
    }

    private async Task LoadNormativesAsync()
    {
        loading = true;

        var responseHttp = await repository.GetAsync<List<NormativeDTO>>("api/Normatives");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            loading = false;
            return;
        }

        normatives = responseHttp.Response ?? [];
        loading = false;
    }

    private async Task LoadTreeAsync(int normativeId)
    {
        loading = true;
        selectedNormative = null;

        var responseHttp = await repository.GetAsync<NormativeTreeDTO>(
            $"api/Normatives/tree/{normativeId}");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(Localizer[message!], Severity.Error);
            loading = false;
            return;
        }

        selectedNormative = responseHttp.Response;
        loading = false;
    }
    private async Task OnNormativeChanged(int? normativeId)
    {
        selectedNormativeId = normativeId;

        if (normativeId.HasValue)
        {
            await LoadTreeAsync(normativeId.Value);
        }
        else
        {
            selectedNormative = null;
        }
    }

    private async Task ShowArticleAsync(NormativeChapterDTO chapter, NormativeArticleDTO article)
    {
        var parameters = new DialogParameters
    {
        { "NormativeName", selectedNormative!.Name },
        { "ChapterName", chapter.Name },
        { "Article", article }
    };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.Medium,
            FullWidth = true,
            BackdropClick = false
        };


        await DialogService.ShowAsync<NormativeArticleDialog>(article.Name, parameters, options);
    }
}