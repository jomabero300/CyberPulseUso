using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.Entities.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa;

public partial class GroupsPage : ContentPage
{
    private readonly GroupsViewModel _viewModel;

    public GroupsPage(GroupsViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await _viewModel.LoadAsync();
    }

    private async void OnSearchClicked(object? sender, EventArgs e)
    {
        await _viewModel.SearchAsync();
    }

    private async void OnNextPageClicked(object? sender, EventArgs e)
    {
        await _viewModel.NextPageAsync();
    }

    private async void OnPreviousPageClicked(object? sender, EventArgs e)
    {
        await _viewModel.PreviousPageAsync();
    }

    private async void OnCreateClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy)
            return;

        var name = await DisplayPromptAsync(
            "Nuevo grupo",
            "Nombre del grupo:",
            "Continuar",
            "Cancelar",
            maxLength: 100);

        if (string.IsNullOrWhiteSpace(name))
            return;

        var category = await SelectCategoryAsync();

        if (category == null)
            return;

        await _viewModel.CreateAsync(name, category.Id);

        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await DisplayAlertAsync(
                "Error",
                _viewModel.ErrorMessage,
                "Aceptar");
        }
    }

    private async void OnEditClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not Group group)
            return;

        var name = await DisplayPromptAsync(
            "Editar grupo",
            "Nombre del grupo:",
            "Continuar",
            "Cancelar",
            initialValue: group.Name,
            maxLength: 100);

        if (string.IsNullOrWhiteSpace(name))
            return;

        var categories = _viewModel.GetCategoriesForEdit(
            group.CategoryId);

        var category = await SelectCategoryAsync(categories);

        if (category == null)
            return;

        var statusOptions = _viewModel.Statuses
            .Select(x => x.Name)
            .ToArray();

        if (statusOptions.Length == 0)
        {
            await DisplayAlertAsync(
                "Estados",
                "No hay estados disponibles.",
                "Aceptar");

            return;
        }

        var selectedStatus = await DisplayActionSheetAsync(
            "Seleccione el estado",
            "Cancelar",
            null,
            statusOptions);

        if (string.IsNullOrWhiteSpace(selectedStatus) ||
            selectedStatus == "Cancelar")
            return;

        var status = _viewModel.Statuses.FirstOrDefault(
            x => x.Name == selectedStatus);

        if (status == null)
            return;

        await _viewModel.UpdateAsync(
            group.Id,
            name,
            category.Id,
            group.CategoryId,
            status.Id);

        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await DisplayAlertAsync(
                "Error",
                _viewModel.ErrorMessage,
                "Aceptar");
        }
    }

    private async Task<Category?> SelectCategoryAsync()
    {
        return await SelectCategoryAsync(_viewModel.Categories.ToList());
    }

    private async Task<Category?> SelectCategoryAsync(
        List<Category> categories)
    {
        if (categories.Count == 0)
        {
            await DisplayAlertAsync(
                "Categorías",
                "No hay categorías disponibles.",
                "Aceptar");

            return null;
        }

        var categoryOptions = categories
            .Select(x => x.Name ?? string.Empty)
            .ToArray();

        var selectedCategory = await DisplayActionSheetAsync(
            "Seleccione la categoría",
            "Cancelar",
            null,
            categoryOptions);

        if (string.IsNullOrWhiteSpace(selectedCategory) ||
            selectedCategory == "Cancelar")
            return null;

        return categories.FirstOrDefault(
            x => x.Name == selectedCategory);
    }

    private async void OnDeleteClicked(object? sender, EventArgs e)
    {
        if (_viewModel.IsBusy ||
            sender is not ImageButton button ||
            button.CommandParameter is not Group group)
            return;

        var confirm = await DisplayAlertAsync(
            "Eliminar grupo",
            $"¿Desea eliminar el grupo {group.Name}?",
            "Sí",
            "No");

        if (!confirm)
            return;

        await _viewModel.DeleteAsync(group.Id);

        if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
        {
            await DisplayAlertAsync(
                "Error",
                _viewModel.ErrorMessage,
                "Aceptar");
        }
    }

    private async void OnExportPdfClicked(object? sender, EventArgs e)
    {
        var fileBytes = await _viewModel.ExportPdfAsync();

        if (fileBytes == null)
        {
            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            {
                await DisplayAlertAsync(
                    "Error",
                    _viewModel.ErrorMessage,
                    "Aceptar");
            }

            return;
        }

        await SaveAndOpenFileAsync(
            fileBytes,
            "Grupos.pdf",
            "application/pdf");
    }

    private async void OnExportExcelClicked(object? sender, EventArgs e)
    {
        var fileBytes = await _viewModel.ExportExcelAsync();

        if (fileBytes == null)
        {
            if (!string.IsNullOrWhiteSpace(_viewModel.ErrorMessage))
            {
                await DisplayAlertAsync(
                    "Error",
                    _viewModel.ErrorMessage,
                    "Aceptar");
            }

            return;
        }

        await SaveAndOpenFileAsync(
            fileBytes,
            "Grupos.xlsx",
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    private async Task SaveAndOpenFileAsync(
        byte[] fileBytes,
        string fileName,
        string contentType)
    {
        try
        {
            var filePath = Path.Combine(
                FileSystem.CacheDirectory,
                fileName);

            await File.WriteAllBytesAsync(filePath, fileBytes);

            await Launcher.Default.OpenAsync(
                new OpenFileRequest
                {
                    File = new ReadOnlyFile(filePath, contentType)
                });
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync(
                "Archivo",
                $"No fue posible abrir el archivo: {ex.Message}",
                "Aceptar");
        }
    }
}