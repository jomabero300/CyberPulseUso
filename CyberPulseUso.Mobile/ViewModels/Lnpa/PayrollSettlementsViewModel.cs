using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.Entities.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Shared.Responses;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace CyberPulseUso.Mobile.ViewModels.Lnpa;

public class PayrollSettlementsViewModel : INotifyPropertyChanged
{
    private readonly IApiRepository _apiRepository;

    private const string _baseUrl = "api/payrollsettlements";

    private int _page = 1;
    private int _recordsNumber = 10;
    private int _totalRecords;
    private string _filter = string.Empty;
    private bool _isBusy;
    private string _errorMessage = string.Empty;
    private bool _isAdministrator;
    public bool IsAdministrator
    {
        get => _isAdministrator;
        set
        {
            _isAdministrator = value;
            OnPropertyChanged();
        }
    }

    private bool _isWorker;

    public bool IsWorker
    {
        get => _isWorker;
        set
        {
            if (_isWorker == value) return;
            _isWorker = value;
            OnPropertyChanged();
        }
    }


    public ObservableCollection<PayrollSettlement> PayrollSettlements { get; } = [];

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

    public PayrollSettlementsViewModel(IApiRepository apiRepository)
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

            await LoadPayrollSettlementsAsync();
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

    private async Task LoadPayrollSettlementsAsync()
    {
        var query = BuildQuery();

        var settlements = await _apiRepository.GetAsync<List<PayrollSettlement>>(
            $"{_baseUrl}/paginated?{query}");

        var total = await _apiRepository.GetAsync<int>(
            $"{_baseUrl}/TotalRecordsPaginated?{query}");

        PayrollSettlements.Clear();

        if (settlements != null)
        {
            foreach (var settlement in settlements)
                PayrollSettlements.Add(settlement);
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

            await LoadPayrollSettlementsAsync();
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

    public async Task<byte[]?> GeneratePdfAsync(DateTime date, int chargeId, string userId)
    {
        if (IsBusy)
            return null;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var formattedDate = Uri.EscapeDataString(
                date.ToString("yyyy-MM-ddTHH:mm:ss"));

            return await _apiRepository.GetFileAsync(
                $"{_baseUrl}/generarpdf/{formattedDate}/{chargeId}/{Uri.EscapeDataString(userId)}");
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

    public async Task<List<PayrollSettlementDTO>?> GetForEditAsync(DateTime date, int chargeId, string userId)
    {
        try
        {
            ErrorMessage = string.Empty;

            var formattedDate = Uri.EscapeDataString(
                date.ToString("yyyy-MM-ddTHH:mm:ss"));

            var url = $"{_baseUrl}/full/{formattedDate}/{chargeId}/{Uri.EscapeDataString(userId)}";

            return await _apiRepository.GetAsync<List<PayrollSettlementDTO>>(url);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return null;
        }
    }

    public async Task<List<PayrollSettlement>?> UpdateFullAsync(
        List<PayrollSettlementDTO> settlements)
    {
        return await _apiRepository.PutAsync<
            List<PayrollSettlementDTO>,
            List<PayrollSettlement>>(
                $"{_baseUrl}/full",
                settlements);
    }

    public async Task<List<PayrollSettlementDTO>?> GetForCreateAsync()
    {
        try
        {
            ErrorMessage = string.Empty;

            var response = await _apiRepository.GetAsync<
                IEnumerable<PayrollSettlementDTO>>(
                    $"{_baseUrl}/Sett");

            return response?.ToList();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return null;
        }
    }
    public async Task<List<PayrollSettlement>?> CreateFullAsync(
        List<PayrollSettlementDTO> settlements)
    {
        try
        {
            ErrorMessage = string.Empty;

            return await _apiRepository.PostAsync<
                List<PayrollSettlementDTO>,
                List<PayrollSettlement>>(
                    $"{_baseUrl}/full",
                    settlements);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            return null;
        }
    }
}
