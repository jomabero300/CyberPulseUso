using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberPulseUso.Mobile.ViewModels.Lnpa;

public class LaborRemunerationParametersValuesViewModel : INotifyPropertyChanged
{
    private readonly IApiRepository _apiRepository;

    private const string _baseUrl = "api/laborremunerationparametersvalues";
    private const string _statusUrl = "api/status";
    private const string _groupUrl = "api/groups";
    private const string _parameterUrl = "api/LaborRemunerationParameterss";

    private int _page = 1;
    private int _recordsNumber = 10;
    private int _totalRecords;
    private string _filter = string.Empty;
    private bool _isBusy;
    private string _errorMessage = string.Empty;

    private List<GroupDTO> _allGroups = [];
    private List<LaborRemunerationParametersDTO> _allParameters = [];

    public ObservableCollection<LaborRemunerationParametersValueDTO>
        LaborRemunerationParametersValues
    { get; } = [];

    public ObservableCollection<StatuDTO> Statuses { get; } = [];

    public ObservableCollection<GroupDTO> Groups { get; } = [];

    public ObservableCollection<LaborRemunerationParametersDTO>
        Parameters
    { get; } = [];

    public int Page
    {
        get => _page;
        set
        {
            if (_page == value) return;
            _page = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(TotalPages));
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

    public LaborRemunerationParametersValuesViewModel(
        IApiRepository apiRepository)
    {
        _apiRepository = apiRepository;
    }

    public async Task LoadAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            await LoadStatusesAsync();
            await LoadGroupsAsync();
            await LoadParametersAsync();
            await LoadValuesAsync();
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
        if (Statuses.Count > 0) return;

        var response = await _apiRepository.GetAsync<List<StatuDTO>>(
            $"{_statusUrl}/combo/0");

        if (response != null)
        {
            foreach (var status in response)
                Statuses.Add(status);
        }
    }

    private async Task LoadGroupsAsync()
    {
        var response = await _apiRepository.GetAsync<List<GroupDTO>>(
            $"{_groupUrl}/combo");

        _allGroups = response ?? [];

        Groups.Clear();

        int activeStatusId = GetActiveStatusId();

        foreach (var group in _allGroups.Where(
            x => x.StatuId == activeStatusId).OrderBy(x => x.Name))
        {
            Groups.Add(group);
        }
    }

    private async Task LoadParametersAsync()
    {
        var response =
            await _apiRepository.GetAsync<List<LaborRemunerationParametersDTO>>(
                $"{_parameterUrl}/combo");

        _allParameters = response ?? [];

        Parameters.Clear();

        int activeStatusId = GetActiveStatusId();

        foreach (var parameter in _allParameters.Where(
            x => x.StatuId == activeStatusId).OrderBy(x => x.Name))
        {
            Parameters.Add(parameter);
        }
    }

    private async Task LoadValuesAsync()
    {
        var query = BuildQuery();

        var values =
            await _apiRepository.GetAsync<List<LaborRemunerationParametersValueDTO>>(
                $"{_baseUrl}/paginated?{query}");

        var total = await _apiRepository.GetAsync<int>(
            $"{_baseUrl}/TotalRecordsPaginated?{query}");

        LaborRemunerationParametersValues.Clear();

        if (values != null)
        {
            foreach (var value in values)
                LaborRemunerationParametersValues.Add(value);
        }

        TotalRecords = total;

        OnPropertyChanged(nameof(CanGoPrevious));
        OnPropertyChanged(nameof(CanGoNext));
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
        if (!CanGoNext) return;

        Page++;
        await LoadAsync();
    }

    public async Task PreviousPageAsync()
    {
        if (!CanGoPrevious) return;

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

    public List<GroupDTO> GetGroupsForEdit(int selectedGroupId)
    {
        var available = Groups.ToList();

        var selected = _allGroups.FirstOrDefault(
            x => x.Id == selectedGroupId);

        if (selected != null &&
            !available.Any(x => x.Id == selectedGroupId))
        {
            available.Add(selected);
        }

        return available.OrderBy(x => x.Name).ToList();
    }

    public List<LaborRemunerationParametersDTO>
        GetParametersForEdit(int selectedParameterId)
    {
        var available = Parameters.ToList();

        var selected = _allParameters.FirstOrDefault(
            x => x.Id == selectedParameterId);

        if (selected != null &&
            !available.Any(x => x.Id == selectedParameterId))
        {
            available.Add(selected);
        }

        return available.OrderBy(x => x.Name).ToList();
    }

    public async Task<LaborRemunerationParametersValueDTO?> GetByIdAsync(
        int id)
    {
        if (IsBusy) return null;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            return await _apiRepository.GetAsync<
                LaborRemunerationParametersValueDTO>(
                $"{_baseUrl}/{id}");
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

    public async Task CreateAsync(
        LaborRemunerationParametersValueDTO value)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            ValidateSelections(value);

            value.StatuId = GetActiveStatusId();

            await _apiRepository.PostAsync<
                LaborRemunerationParametersValueDTO, object>(
                $"{_baseUrl}/full", value);

            await LoadValuesAsync();
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

    public async Task UpdateAsync(
        LaborRemunerationParametersValueDTO value)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (!Statuses.Any(x => x.Id == value.StatuId))
                throw new InvalidOperationException(
                    "Seleccione un estado válido.");

            ValidateSelections(value);

            await _apiRepository.PutAsync<
                LaborRemunerationParametersValueDTO, object>(
                $"{_baseUrl}/full", value);

            await LoadValuesAsync();
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

    private void ValidateSelections(
        LaborRemunerationParametersValueDTO value)
    {
        if (!Groups.Any(x => x.Id == value.GroupId))
            throw new InvalidOperationException(
                "Seleccione un grupo activo.");

        if (!Parameters.Any(x => x.Id == value.LRPId))
            throw new InvalidOperationException(
                "Seleccione un parámetro de remuneración activo.");
    }

    public async Task DeleteAsync(int id)
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            await _apiRepository.DeleteAsync(
                $"{_baseUrl}/full/{id}");

            await LoadValuesAsync();
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

    public Task<byte[]?> ExportPdfAsync()
    {
        return ExportReportAsync("pdf");
    }

    public Task<byte[]?> ExportExcelAsync()
    {
        return ExportReportAsync("excel");
    }

    private async Task<byte[]?> ExportReportAsync(string type)
    {
        if (IsBusy) return null;

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
            "ERR001", StringComparison.OrdinalIgnoreCase))
            return Literals.ERR001;

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
            this, new PropertyChangedEventArgs(propertyName));
    }
}