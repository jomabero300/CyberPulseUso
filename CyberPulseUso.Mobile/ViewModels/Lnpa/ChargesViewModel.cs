using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberPulseUso.Mobile.ViewModels.Lnpa;

public class ChargesViewModel : INotifyPropertyChanged
{
    private readonly IApiRepository _apiRepository;

    private const string _baseUrl = "api/charges";
    private const string _groupUrl = "api/groups";
    private const string _statusUrl = "api/status";

    private int _page = 1;
    private int _recordsNumber = 10;
    private int _totalRecords;
    private string _filter = string.Empty;
    private bool _isBusy;
    private string _errorMessage = string.Empty;

    private List<Group> _allGroups = [];

    public ObservableCollection<Charge> Charges { get; } = [];
    public ObservableCollection<Group> Groups { get; } = [];
    public ObservableCollection<Statu> Statuses { get; } = [];

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

    public ChargesViewModel(IApiRepository apiRepository)
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
            await LoadGroupsAsync();
            await LoadChargesAsync();
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

    private async Task LoadGroupsAsync()
    {
        var groups = await _apiRepository.GetAsync<List<Group>>(
            $"{_groupUrl}/Combo");

        _allGroups = groups ?? [];

        Groups.Clear();

        var activeStatusId = GetActiveStatusId();

        foreach (var group in _allGroups.Where(
            x => x.StatuId == activeStatusId))
        {
            Groups.Add(group);
        }
    }

    public List<Group> GetGroupsForEdit(int selectedGroupId)
    {
        var availableGroups = Groups.ToList();

        var selectedGroup = _allGroups.FirstOrDefault(
            x => x.Id == selectedGroupId);

        if (selectedGroup != null &&
            !availableGroups.Any(x => x.Id == selectedGroupId))
        {
            availableGroups.Add(selectedGroup);
        }

        return availableGroups
            .OrderBy(x => x.Name)
            .ToList();
    }

    private async Task LoadChargesAsync()
    {
        var query = BuildQuery();

        var charges = await _apiRepository.GetAsync<List<Charge>>(
            $"{_baseUrl}/paginated?{query}");

        var total = await _apiRepository.GetAsync<int>(
            $"{_baseUrl}/TotalRecordsPaginated?{query}");

        Charges.Clear();

        if (charges != null)
        {
            foreach (var charge in charges)
                Charges.Add(charge);
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
        if (Page >= TotalPages)
            return;

        Page++;
        await LoadAsync();
    }

    public async Task PreviousPageAsync()
    {
        if (Page <= 1)
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

    public async Task CreateAsync(string name, int groupId)
    {
        if (string.IsNullOrWhiteSpace(name) || IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var group = Groups.FirstOrDefault(
                x => x.Id == groupId);

            if (group == null)
                throw new InvalidOperationException(
                    "Seleccione un grupo activo.");

            var charge = new ChargeDTO
            {
                Name = name.Trim(),
                GroupId = group.Id,
                StatuId = GetActiveStatusId(),

                Group = new GroupDTO
                {
                    Id = group.Id,
                    Name = group.Name,
                    CategoryId = group.CategoryId,
                    StatuId = group.StatuId,
                    Category = new CategoryDTO
                    {
                        Id = group.Category!.Id,
                        Name = group.Category.Name,
                        StatuId = group.Category.StatuId
                    }
                }
            };

            await _apiRepository.PostAsync<ChargeDTO, object>(
                $"{_baseUrl}/full",
                charge);

            await LoadChargesAsync();
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

    public async Task UpdateAsync(int id, string name, int groupId, int originalGroupId, int statusId)
    {
        if (string.IsNullOrWhiteSpace(name) || IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var isActiveGroup = Groups.Any(
                x => x.Id == groupId);

            var isOriginalGroup = groupId == originalGroupId;

            if (!isActiveGroup && !isOriginalGroup)
                throw new InvalidOperationException(
                    "Seleccione un grupo activo.");

            var group = _allGroups.FirstOrDefault(
                x => x.Id == groupId);

            if (group == null)
                throw new InvalidOperationException(
                    "No se encontró el grupo seleccionado.");

            if (!Statuses.Any(x => x.Id == statusId))
                throw new InvalidOperationException(
                    "Seleccione un estado válido.");

            var charge = new ChargeDTO
            {
                Id = id,
                Name = name.Trim(),
                GroupId = group.Id,
                StatuId = statusId,

                Group = new GroupDTO
                {
                    Id = group.Id,
                    Name = group.Name,
                    CategoryId = group.CategoryId,
                    StatuId = group.StatuId,
                    Category = new CategoryDTO
                    {
                        Id = group.Category!.Id,
                        Name = group.Category.Name,
                        StatuId = group.Category.StatuId
                    }
                }
            };

            await _apiRepository.PutAsync<ChargeDTO, object>(
                $"{_baseUrl}/full",
                charge);

            await LoadChargesAsync();
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

            await LoadChargesAsync();
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