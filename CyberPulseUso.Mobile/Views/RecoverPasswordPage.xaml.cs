using CyberPulseUso.Mobile.ViewModels;

namespace CyberPulseUso.Mobile.Views;

public partial class RecoverPasswordPage : ContentPage
{
    public RecoverPasswordPage(RecoverPasswordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}