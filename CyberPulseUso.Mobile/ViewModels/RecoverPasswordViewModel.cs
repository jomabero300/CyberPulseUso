using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CyberPulseUso.Mobile.Services.Interfaces;
using CyberPulseUso.Shared.EntitiesDTO.Gene;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;

namespace CyberPulseUso.Mobile.ViewModels;

public partial class RecoverPasswordViewModel : ObservableObject
{
    private readonly IApiRepository _repository;

    [ObservableProperty]    public partial EmailDTO EmailData { get; set; } = new();

    [ObservableProperty]    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]    public partial bool IsBusy { get; set; }


    public RecoverPasswordViewModel(IApiRepository repository)
    {
        _repository = repository;
    }

    [RelayCommand]
    private async Task RecoverPasswordAsync()
    {
        if (IsBusy)
            return;

        ErrorMessage = string.Empty;

        EmailData.Email = EmailData.Email?.Trim()!;
        EmailData.Language = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(EmailData);

        if (!Validator.TryValidateObject(
            EmailData,
            validationContext,
            validationResults,
            validateAllProperties: true))
        {
            ErrorMessage = validationResults.FirstOrDefault()?.ErrorMessage
                ?? "Verifique el correo electrónico.";

            return;
        }

        try
        {
            IsBusy = true;

            await _repository.PostAsync<EmailDTO, object?>(
                "/api/accounts/RecoverPassword",
                EmailData);

            await Shell.Current.DisplayAlertAsync(
                "Recuperar contraseña",
                "Si la cuenta existe, recibirás un correo con las instrucciones para recuperar tu contraseña.",
                "Aceptar");

            await Shell.Current.GoToAsync("..");
        }
        catch (HttpRequestException ex)
        {
            ErrorMessage = ex.StatusCode switch
            {
                HttpStatusCode.NotFound =>
                    "No se encontró una cuenta con ese correo electrónico.",

                HttpStatusCode.BadRequest =>
                    "No fue posible enviar el correo de recuperación.",

                _ => "No fue posible conectar con el servidor."
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(ex);
            ErrorMessage = "Ocurrió un error al solicitar la recuperación.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ReturnAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}