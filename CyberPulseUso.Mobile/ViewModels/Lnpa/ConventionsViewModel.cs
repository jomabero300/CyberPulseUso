using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberPulseUso.Mobile.ViewModels.Lnpa;

public class ConventionsViewModel : INotifyPropertyChanged
{
    private readonly IApiRepository _apiRepository;

    private const string _baseUrl = "api/conventions";
    private const string _statusUrl = "api/status";

    private int _page = 1;
    private int _recordsNumber = 10;
    private int _totalRecords;
    private string _filter = string.Empty;
    private bool _isBusy;
    private string _errorMessage = string.Empty;

    public ObservableCollection<Convention> Conventions { get; } = [];
    public ObservableCollection<Statu> Statuses { get; } = [];

    public int Page
    {
        get => _page;
        set
        {
            if (_page == value) return;
            _page = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
        }
    }

    public int RecordsNumber
    {
        get => _recordsNumber;
        set
        {
            if (_recordsNumber == value) return;
            _recordsNumber = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(CanGoNext));
        }
    }

    public int TotalRecords
    {
        get => _totalRecords;
        set
        {
            if (_totalRecords == value) return;
            _totalRecords = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalPages));
            OnPropertyChanged(nameof(CanGoNext));
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
            if (_filter == value) return;
            _filter = value;
            OnPropertyChanged();
        }
    }

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

    public ConventionsViewModel(IApiRepository apiRepository)
    {
        _apiRepository = apiRepository;
    }

    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            await LoadStatusesAsync();
            await LoadConventionsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = GetErrorMessage(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadStatusesAsync()
    {
        if (Statuses.Count > 0)
            return;

        var statuses = await _apiRepository.GetAsync<List<Statu>>(
            $"{_statusUrl}/Combo/0");

        if (statuses != null)
        {
            foreach (var status in statuses)
                Statuses.Add(status);
        }
    }

    private async Task LoadConventionsAsync()
    {
        var query = BuildQuery();

        var conventions = await _apiRepository.GetAsync<List<Convention>>(
            $"{_baseUrl}/paginated?{query}");

        var total = await _apiRepository.GetAsync<int>(
            $"{_baseUrl}/TotalRecordsPaginated?{query}");

        Conventions.Clear();

        if (conventions != null)
        {
            foreach (var convention in conventions)
                Conventions.Add(convention);
        }

        TotalRecords = total;
    }

    private string BuildQuery()
    {
        return $"Page={Page}" +
               $"&RecordsNumber={RecordsNumber}" +
               $"&Filter={Uri.EscapeDataString(Filter ?? string.Empty)}";
    }

    public async Task SearchAsync()
    {
        Page = 1;
        await LoadAsync();
    }

    public async Task NextPageAsync()
    {
        if (!CanGoNext)
            return;

        Page++;
        await LoadAsync();
    }

    public async Task PreviousPageAsync()
    {
        if (!CanGoPrevious)
            return;

        Page--;
        await LoadAsync();
    }

    private int GetActiveStatusId()
    {
        var activeStatus = Statuses.FirstOrDefault(
            x => string.Equals(
                x.Name,
                "Activo",
                StringComparison.OrdinalIgnoreCase));

        if (activeStatus == null)
            throw new InvalidOperationException(
                "No se encontró el estado Activo.");

        return activeStatus.Id;
    }

    public async Task CreateAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var convention = new ConventionDTO
            {
                Name = name.Trim(),
                StatuId = GetActiveStatusId()
            };

            await _apiRepository.PostAsync<ConventionDTO, object>(
                $"{_baseUrl}/full",
                convention);

            await LoadConventionsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = GetErrorMessage(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async Task UpdateAsync(int id, string name, int statusId)
    {
        if (string.IsNullOrWhiteSpace(name) || IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (!Statuses.Any(x => x.Id == statusId))
                throw new InvalidOperationException(
                    "Seleccione un estado válido.");

            var convention = new ConventionDTO
            {
                Id = id,
                Name = name.Trim(),
                StatuId = statusId
            };

            await _apiRepository.PutAsync<ConventionDTO, object>(
                $"{_baseUrl}/full",
                convention);

            await LoadConventionsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = GetErrorMessage(ex);
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

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            await _apiRepository.DeleteAsync(
                $"{_baseUrl}/full/{id}");

            await LoadConventionsAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = GetErrorMessage(ex);
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

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            return await _apiRepository.GetFileAsync(
                $"{_baseUrl}/Report/{type}");
        }
        catch (Exception ex)
        {
            ErrorMessage = GetErrorMessage(ex);
            return null;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private string GetErrorMessage(Exception ex)
    {
        if (ex.Message.Contains(
            "ERR002", StringComparison.OrdinalIgnoreCase))
            return Literals.ERR002;

        if (ex.Message.Contains(
            "ERR003", StringComparison.OrdinalIgnoreCase))
            return Literals.ERR003;

        return ex.Message;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}