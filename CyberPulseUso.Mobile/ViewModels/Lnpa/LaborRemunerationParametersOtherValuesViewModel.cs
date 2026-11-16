using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberPulseUso.Mobile.ViewModels.Lnpa;

public class LaborRemunerationParametersOtherValuesViewModel : INotifyPropertyChanged
{
    private readonly IApiRepository _apiRepository;

    private const string _baseUrl = "api/LaborRemunerationParametersOtherValues";

    private bool _isBusy;
    private string _errorMessage = string.Empty;

    public ObservableCollection<LaborRemunerationParametersOtherValueDTO> OtherValues { get; } = [];

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy == value) return;
            _isBusy = value;
            OnPropertyChanged();
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

    public LaborRemunerationParametersOtherValuesViewModel(IApiRepository apiRepository)
    {
        _apiRepository = apiRepository;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
 

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var response = await _apiRepository.GetAsync<
                List<LaborRemunerationParametersOtherValueDTO>>(
                    _baseUrl);

            OtherValues.Clear();

            if (response != null)
            {
                foreach (var item in response)
                    OtherValues.Add(item);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }
    public async Task<bool> SaveAsync()
    {
        if (IsBusy)
            return false;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var valuesToSave = OtherValues.Select(x =>
                new LaborRemunerationParametersOtherValueCRUDDTO
                {
                    Id = x.Id,
                    LRPId = x.LRPId,
                    Value = x.Value,
                    StatuId = x.StatuId
                }).ToList();

            var response = await _apiRepository.PostAsync<
                List<LaborRemunerationParametersOtherValueCRUDDTO>,
                List<LaborRemunerationParametersOtherValueDTO>>(
                    $"{_baseUrl}/full",
                    valuesToSave);

            if (response == null)
            {
                ErrorMessage = "No fue posible guardar los valores.";
                return false;
            }

            await LoadAfterSaveAsync();

            return true;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return false;
        }
        finally
        {
            IsBusy = false;
        }
    }
    private async Task LoadAfterSaveAsync()
    {
        var response = await _apiRepository.GetAsync<
            List<LaborRemunerationParametersOtherValueDTO>>(
                _baseUrl);

        OtherValues.Clear();

        if (response != null)
        {
            foreach (var item in response)
                OtherValues.Add(item);
        }
    }
}