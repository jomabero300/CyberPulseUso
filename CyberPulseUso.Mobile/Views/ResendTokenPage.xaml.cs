using CyberPulseUso.Mobile.ViewModels;

namespace CyberPulseUso.Mobile.Views;

public partial class ResendTokenPage : ContentPage
{
    public ResendTokenPage(ResendTokenViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}