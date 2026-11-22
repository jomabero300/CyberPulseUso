using CyberPulseUso.Frontend.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace CyberPulseUso.Frontend.Components.Pages.Lnpa.ReportLnpa;

public partial class LaborRemunerationReport
{
    [Inject] IJSRuntime JS { get; set; } = null!;
    [Inject] private IRepository repository { get; set; } = null!;
    private bool _isProcessing = false;

    private async Task GenerarReportePivotAsync()
    {
        _isProcessing = true;

        try
        {
            var response = await repository.GetBytesAsync("api/laborremunerationparametersvalues/pdf/pivot");

            if (response.Error || response.Response == null)
            {
                return;
            }

            await JS.InvokeVoidAsync("displayPdf", response.Response);
        }
        finally
        {
            _isProcessing = false;
        }
    }

}