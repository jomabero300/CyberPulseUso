using CyberPulseUso.Mobile.ViewModels;

namespace CyberPulseUso.Mobile.Views;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _viewModel;
    
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
        
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.Countries.Count == 0)
            await _viewModel.InitializeAsync();
    }
}