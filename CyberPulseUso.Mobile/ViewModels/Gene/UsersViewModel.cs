using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using CyberPulseUso.Shared.Enums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CyberPulseUso.Mobile.ViewModels.Gene;

public class UsersViewModel : INotifyPropertyChanged
{
    private readonly IApiRepository _apiRepository;
    private const string _baseUrl = "api/accounts/mobile";
    private const string _reportUrl = "api/accounts/Report";
    private int _page = 1;
    private int _recordsNumber = 10;
    private int _totalRecords;
    private string _filter = string.Empty;
    private bool _isBusy;
    private string _errorMessage = string.Empty;

    public ObservableCollection<UserMobileDTO> Users { get; } = [];

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

    public UsersViewModel(IApiRepository apiRepository)
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

            var query = BuildQuery();


            System.Diagnostics.Debug.WriteLine(
                $"URL USUARIOS: {_baseUrl}/paginated?{query}");

            var users = await _apiRepository.GetAsync<List<UserMobileDTO>>(
                $"{_baseUrl}/paginated?{query}");

            var total = await _apiRepository.GetAsync<int>(
                $"{_baseUrl}/totalrecords?{query}");

            Users.Clear();

            if (users != null)
            {
                foreach (var user in users)
                    Users.Add(user);
            }

            TotalRecords = total;
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

    public async Task UpdateUserTypeAsync(string userId,UserType userType)
    {
        if (string.IsNullOrWhiteSpace(userId))
            return;

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var url = $"{_baseUrl}/usertype" +
                      $"?id={Uri.EscapeDataString(userId)}" +
                      $"&userType={Uri.EscapeDataString(userType.ToString())}";

            await _apiRepository.PutAsync<object, object>(
                url,
                new { });

            await LoadAfterUpdateAsync();
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
    public async Task<byte[]?> ExportPdfAsync()
    {
        return await ExportReportAsync("pdf");
    }

    public async Task<byte[]?> ExportExcelAsync()
    {
        return await ExportReportAsync("excel");
    }

    private async Task<byte[]?> ExportReportAsync(string fileType)
    {
        if (IsBusy)
            return null;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var query =
                $"Filter={Uri.EscapeDataString(Filter ?? string.Empty)}" +
                $"&FileType={Uri.EscapeDataString(fileType)}";

            return await _apiRepository.GetFileAsync(
                $"{_reportUrl}?{query}");
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
    private async Task LoadAfterUpdateAsync()
    {
        var query = BuildQuery();

        var users = await _apiRepository.GetAsync<List<UserMobileDTO>>(
            $"{_baseUrl}/paginated?{query}");

        Users.Clear();

        if (users != null)
        {
            foreach (var user in users)
                Users.Add(user);
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged( [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}