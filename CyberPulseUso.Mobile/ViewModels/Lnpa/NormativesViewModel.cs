using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;
using CyberPulseUso.Mobile.Services.Interfaces;

namespace CyberPulseUso.Mobile.ViewModels.Lnpa;

public class NormativesViewModel : INotifyPropertyChanged
{
    private readonly IApiRepository _apiRepository;

    private const string _baseUrl = "api/Normatives";

    private bool _isBusy;
    private string _errorMessage = string.Empty;
    private NormativeDTO? _selectedNormative;
    private NormativeTreeDTO? _normativeTree;

    public ObservableCollection<NormativeDTO> Normatives { get; } = [];

    public ObservableCollection<NormativeChapterDTO> Chapters { get; } = [];

    public NormativeDTO? SelectedNormative
    {
        get => _selectedNormative;
        set
        {
            if (_selectedNormative == value) return;
            _selectedNormative = value;
            OnPropertyChanged();
        }
    }

    public NormativeTreeDTO? NormativeTree
    {
        get => _normativeTree;
        set
        {
            if (_normativeTree == value) return;
            _normativeTree = value;
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

    public NormativesViewModel(IApiRepository apiRepository)
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

            var normatives = await _apiRepository.GetAsync<List<NormativeDTO>>(
                _baseUrl);

            Normatives.Clear();

            if (normatives != null)
            {
                foreach (var normative in normatives)
                    Normatives.Add(normative);
            }

            SelectedNormative = null;
            NormativeTree = null;
            Chapters.Clear();
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

    public async Task LoadTreeAsync(NormativeDTO? normative)
    {
        if (normative == null || IsBusy)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            SelectedNormative = normative;
            NormativeTree = null;
            Chapters.Clear();

            var tree = await _apiRepository.GetAsync<NormativeTreeDTO>(
                $"{_baseUrl}/tree/{normative.Id}");

            if (tree == null)
                throw new InvalidOperationException(
                    "No se pudo cargar el documento normativo.");

            NormativeTree = tree;

            foreach (var chapter in tree.Chapters.OrderBy(x => x.SortOrder))
                Chapters.Add(chapter);
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            SelectedNormative = null;
            NormativeTree = null;
            Chapters.Clear();
        }
        finally
        {
            IsBusy = false;
        }
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