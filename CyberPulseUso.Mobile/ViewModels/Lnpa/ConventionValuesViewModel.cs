using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberPulseUso.Mobile.ViewModels.Lnpa;

public class ConventionValuesViewModel : INotifyPropertyChanged
{
    private readonly IApiRepository _apiRepository;

    private const string _baseUrl = "api/conventionvalues";
    private const string _statusUrl = "api/status";
    private const string _conventionUrl = "api/conventions";

    private int _page = 1;
    private int _recordsNumber = 10;
    private int _totalRecords;
    private string _filter = string.Empty;
    private bool _isBusy;
    private string? _errorMessage;

    public ObservableCollection<ConventionValue> ConventionValues { get; set; } = new();
    public ObservableCollection<Statu> Statuses { get; set; } = new();
    public ObservableCollection<Convention> Conventions { get; set; } = new();

    public int Page
    {
        get => _page;
        set
        {
            if (_page != value)
            {
                _page = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CanGoPrevious));
                OnPropertyChanged(nameof(CanGoNext));
            }
        }
    }

    public int RecordsNumber
    {
        get => _recordsNumber;
        set
        {
            if (_recordsNumber != value)
            {
                _recordsNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPages));
            }
        }
    }

    public int TotalRecords
    {
        get => _totalRecords;
        set
        {
            if (_totalRecords != value)
            {
                _totalRecords = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalPages));
                OnPropertyChanged(nameof(CanGoPrevious));
                OnPropertyChanged(nameof(CanGoNext));
            }
        }
    }

    public int TotalPages =>
        TotalRecords == 0
            ? 1
            : (int)Math.Ceiling((double)TotalRecords / RecordsNumber);

    public bool CanGoPrevious => Page > 1;
    public bool CanGoNext => Page < TotalPages;

    public string Filter
    {
        get => _filter;
        set
        {
            if (_filter != value)
            {
                _filter = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsBusy
    {
        get => _isBusy;
        set
        {
            if (_isBusy != value)
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }
    }

    public string? ErrorMessage
    {
        get => _errorMessage;
        set
        {
            if (_errorMessage != value)
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public ConventionValuesViewModel(IApiRepository apiRepository)
    {
        _apiRepository = apiRepository;
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await LoadStatusesAsync();
            await LoadConventionsAsync();
            await LoadConventionValuesAsync();
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

    private async Task LoadStatusesAsync()
    {
        var response = await _apiRepository.GetAsync<List<Statu>>(
            $"{_statusUrl}/Combo/0");

        Statuses.Clear();

        if (response != null)
        {
            foreach (var item in response)
                Statuses.Add(item);
        }
    }

    private async Task LoadConventionsAsync()
    {
        var response = await _apiRepository.GetAsync<List<Convention>>(
            $"{_conventionUrl}/Combo");

        Conventions.Clear();

        if (response != null)
        {
            foreach (var item in response)
                Conventions.Add(item);
        }
    }

    private async Task LoadConventionValuesAsync()
    {
        var query = $"?Page={Page}&RecordsNumber={RecordsNumber}&Filter={Uri.EscapeDataString(Filter ?? string.Empty)}";

        var total = await _apiRepository.GetAsync<int>(
            $"{_baseUrl}/TotalRecordsPaginated{query}");

        TotalRecords = total;

        if (Page > TotalPages)
            Page = TotalPages;

        var response = await _apiRepository.GetAsync<List<ConventionValue>>(
            $"{_baseUrl}/paginated?Page={Page}&RecordsNumber={RecordsNumber}&Filter={Uri.EscapeDataString(Filter ?? string.Empty)}");

        ConventionValues.Clear();

        if (response != null)
        {
            foreach (var item in response)
                ConventionValues.Add(item);
        }

        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
    }

    public async Task SearchAsync()
    {
        if (IsBusy)
            return;

        Page = 1;
        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await LoadConventionValuesAsync();
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

    public async Task NextPageAsync()
    {
        if (IsBusy || !CanGoNext)
            return;

        Page++;
        IsBusy = true;

        try
        {
            await LoadConventionValuesAsync();
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

    public async Task PreviousPageAsync()
    {
        if (IsBusy || !CanGoPrevious)
            return;

        Page--;
        IsBusy = true;

        try
        {
            await LoadConventionValuesAsync();
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

    private int GetActiveStatusId()
    {
        var activeStatus = Statuses.FirstOrDefault(
            x => string.Equals(
                x.Name,
                "Activo",
                StringComparison.OrdinalIgnoreCase));

        return activeStatus?.Id ?? 0;
    }

    public async Task CreateAsync(
        int conventionId,
        DateTime date,
        double prince)
    {
        if (IsBusy)
            return;

        ErrorMessage = null;

        var statusId = GetActiveStatusId();

        if (statusId == 0)
        {
            ErrorMessage = "No se encontró el estado Activo.";
            return;
        }

        var convention = Conventions.FirstOrDefault(
            x => x.Id == conventionId);

        if (convention == null)
        {
            ErrorMessage = "Seleccione un convenio válido.";
            return;
        }

        if (prince <= 0)
        {
            ErrorMessage = "El valor debe ser mayor que cero.";
            return;
        }

        var dto = new ConventionValueDTO
        {
            ConventionId = convention.Id,
            Date = date,
            Prince = prince,
            StatuId = statusId,
            Convention = new ConventionDTO
            {
                Id = convention.Id,
                Name = convention.Name,
                StatuId = convention.StatuId
            }
        };

        IsBusy = true;

        try
        {
            await _apiRepository.PostAsync<ConventionValueDTO, object>(
                $"{_baseUrl}/full", dto);

            await LoadConventionValuesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = GetErrorMessage(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task UpdateAsync(
        int id,
        int conventionId,
        DateTime date,
        double prince,
        int statusId)
    {
        if (IsBusy)
            return;

        ErrorMessage = null;

        var convention = Conventions.FirstOrDefault(
            x => x.Id == conventionId);

        if (convention == null)
        {
            ErrorMessage = "Seleccione un convenio válido.";
            return;
        }

        if (statusId <= 0)
        {
            ErrorMessage = "Seleccione un estado válido.";
            return;
        }

        if (prince <= 0)
        {
            ErrorMessage = "El valor debe ser mayor que cero.";
            return;
        }

        var dto = new ConventionValueDTO
        {
            Id = id,
            ConventionId = convention.Id,
            Date = date,
            Prince = prince,
            StatuId = statusId,
            Convention = new ConventionDTO
            {
                Id = convention.Id,
                Name = convention.Name,
                StatuId = convention.StatuId
            }
        };

        IsBusy = true;

        try
        {
            await _apiRepository.PutAsync<ConventionValueDTO, object>(
                $"{_baseUrl}/full", dto);

            await LoadConventionValuesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = GetErrorMessage(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task DeleteAsync(int id)
    {
        if (IsBusy)
            return;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            await _apiRepository.DeleteAsync($"{_baseUrl}/full/{id}");

            if (ConventionValues.Count == 1 && Page > 1)
                Page--;

            await LoadConventionValuesAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = GetErrorMessage(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task<byte[]?> ExportPdfAsync()
    {
        return await ExportReportAsync("pdf");
    }

    public async Task<byte[]?> ExportExcelAsync()
    {
        return await ExportReportAsync("excel");
    }

    private async Task<byte[]?> ExportReportAsync(string type)
    {
        if (IsBusy)
            return null;

        IsBusy = true;
        ErrorMessage = null;

        try
        {
            return await _apiRepository.GetFileAsync(
                $"{_baseUrl}/Report/{type}");
        }
        catch (Exception ex)
        {
            ErrorMessage = GetErrorMessage(ex.Message);
            return null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private static string GetErrorMessage(string message)
    {
        return message switch
        {
            "ERR001" => "El registro solicitado no existe.",
            "ERR002" => Literals.ERR002,
            "ERR003" => Literals.ERR003,
            "ERR010" => Literals.ERR010,
            "ERR017" => Literals.ERR017,
            _ => message
        };
    }

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}