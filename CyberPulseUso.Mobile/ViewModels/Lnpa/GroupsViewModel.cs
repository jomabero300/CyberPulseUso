using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.Entities.Gene;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Resources;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberPulseUso.Mobile.ViewModels.Lnpa;

public class GroupsViewModel : INotifyPropertyChanged
{
    private readonly IApiRepository _apiRepository;

    private const string _baseUrl = "api/groups";
    private const string _categoryUrl = "api/categories";
    private const string _statusUrl = "api/status";

    private int _page = 1;
    private int _recordsNumber = 10;
    private int _totalRecords;
    private string _filter = string.Empty;
    private bool _isBusy;
    private string _errorMessage = string.Empty;

    private List<Category> _allCategories = [];

    public ObservableCollection<Group> Groups { get; } = [];
    public ObservableCollection<Category> Categories { get; } = [];
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

    public GroupsViewModel(IApiRepository apiRepository)
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
            await LoadCategoriesAsync();
            await LoadGroupsAsync();
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

    private async Task LoadCategoriesAsync()
    {
        var categories = await _apiRepository.GetAsync<List<Category>>(
            $"{_categoryUrl}/Combo");

        _allCategories = categories ?? [];

        Categories.Clear();

        var activeStatusId = GetActiveStatusId();

        foreach (var category in _allCategories.Where(
            x => x.StatuId == activeStatusId))
        {
            Categories.Add(category);
        }
    }
    public List<Category> GetCategoriesForEdit(int selectedCategoryId)
    {
        var availableCategories = Categories.ToList();

        var selectedCategory = _allCategories.FirstOrDefault(
            x => x.Id == selectedCategoryId);

        if (selectedCategory != null &&
            !availableCategories.Any(x => x.Id == selectedCategoryId))
        {
            availableCategories.Add(selectedCategory);
        }

        return availableCategories
            .OrderBy(x => x.Name)
            .ToList();
    }

    private async Task LoadGroupsAsync()
    {
        var query = BuildQuery();

        var groups = await _apiRepository.GetAsync<List<Group>>(
            $"{_baseUrl}/paginated?{query}");

        var total = await _apiRepository.GetAsync<int>(
            $"{_baseUrl}/TotalRecordsPaginated?{query}");

        Groups.Clear();

        if (groups != null)
        {
            foreach (var group in groups)
                Groups.Add(group);
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

    public async Task CreateAsync(string name, int categoryId)
    {
        if (string.IsNullOrWhiteSpace(name) || IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var category = Categories.FirstOrDefault(
                x => x.Id == categoryId);

            if (category == null)
                throw new InvalidOperationException(
                    "Seleccione una categoría activa.");

            var group = new GroupDTO
            {
                Name = name.Trim(),
                CategoryId = category.Id,
                StatuId = GetActiveStatusId(),

                Category = new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    StatuId = category.StatuId
                }
            };

            await _apiRepository.PostAsync<GroupDTO, object>(
                $"{_baseUrl}/full",
                group);

            await LoadGroupsAsync();
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
    public async Task UpdateAsync(int id, string name, int categoryId, int originalCategoryId, int statusId)
    {
        if (string.IsNullOrWhiteSpace(name) || IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            // Solo permitir categorías activas,
            // excepto la categoría que el grupo ya tenía asignada.
            var isActiveCategory = Categories.Any(
                x => x.Id == categoryId);

            var isOriginalCategory = categoryId == originalCategoryId;

            if (!isActiveCategory && !isOriginalCategory)
                throw new InvalidOperationException(
                    "Seleccione una categoría activa.");

            var category = _allCategories.FirstOrDefault(
                x => x.Id == categoryId);

            if (category == null)
                throw new InvalidOperationException(
                    "No se encontró la categoría seleccionada.");

            if (!Statuses.Any(x => x.Id == statusId))
                throw new InvalidOperationException(
                    "Seleccione un estado válido.");

            var group = new GroupDTO
            {
                Id = id,
                Name = name.Trim(),
                CategoryId = category.Id,
                StatuId = statusId,

                Category = new CategoryDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    StatuId = category.StatuId
                }
            };

            await _apiRepository.PutAsync<GroupDTO, object>(
                $"{_baseUrl}/full",
                group);

            await LoadGroupsAsync();
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

            await LoadGroupsAsync();
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

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}