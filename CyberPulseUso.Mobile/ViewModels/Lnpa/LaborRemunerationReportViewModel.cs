using CyberPulseUso.Mobile.Services.Interfaces;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberPulseUso.Mobile.ViewModels.Lnpa;

public class LaborRemunerationReportViewModel : INotifyPropertyChanged
{
    private readonly IApiRepository _apiRepository;

    private const string _reportUrl =
        "api/laborremunerationparametersvalues/pdf/pivot";
    public bool CanGenerate => !IsBusy;
    private bool _isBusy;
    private string _errorMessage = string.Empty;

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value) return;

            _isBusy = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanGenerate));
        }
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage == value) return;
            _errorMessage = value;
            OnPropertyChanged();
        }
    }

    public LaborRemunerationReportViewModel(IApiRepository apiRepository)
    {
        _apiRepository = apiRepository;
    }

    public async Task<byte[]?> GenerateReportAsync()
    {
        if (IsBusy)
            return null;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            return await _apiRepository.GetFileAsync(_reportUrl);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}