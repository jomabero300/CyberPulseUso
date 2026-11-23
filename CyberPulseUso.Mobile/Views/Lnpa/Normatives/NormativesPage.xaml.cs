using CyberPulseUso.Mobile.ViewModels.Lnpa;
using CyberPulseUso.Shared.EntitiesDTO.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa.Normatives;

public partial class NormativesPage : ContentPage
{
    private readonly NormativesViewModel _viewModel;    

    public NormativesPage(NormativesViewModel viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        BindingContext = _viewModel;
        NormativesPicker.SelectedIndexChanged += NormativesPicker_SelectedIndexChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_viewModel.Normatives.Count == 0)
        {
            await _viewModel.LoadAsync();
        }
    }

    private async void NormativesPicker_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (NormativesPicker.SelectedItem is NormativeDTO normative)
        {
            await _viewModel.LoadTreeAsync(normative);
            LoadChapters();
        }
    }
    private void LoadChapters()
    {
        ChaptersContainer.Children.Clear();

        foreach (var chapter in _viewModel.Chapters.OrderBy(x => x.SortOrder))
        {
            var articlesLayout = new VerticalStackLayout
            {
                Spacing = 6,
                Padding = new Thickness(20, 5, 5, 5),
                IsVisible = false
            };

            foreach (var article in chapter.Articles.OrderBy(x => x.SortOrder))
            {
                var articleLabel = new Label
                {
                    Text = $"📄 {article.Name}",
                    FontSize = 14,
                    Padding = new Thickness(8),
                    TextColor = Colors.Blue
                };

                var tapGesture = new TapGestureRecognizer();

                tapGesture.Tapped += async (sender, e) =>
                {
                    await Navigation.PushAsync(
                        new NormativeArticlePage(
                            article,
                            _viewModel.NormativeTree?.Name ?? string.Empty,
                            chapter.Name));
                };

                articleLabel.GestureRecognizers.Add(tapGesture);

                articlesLayout.Children.Add(articleLabel);
            }
            var chapterButton = new Button
            {
                Text = $"＋ {chapter.Name}",
                HorizontalOptions = LayoutOptions.Fill,
                BackgroundColor = Colors.LightGray,
                TextColor = Colors.Black
            };

            chapterButton.Clicked += (sender, e) =>
            {
                articlesLayout.IsVisible = !articlesLayout.IsVisible;
                chapterButton.Text = $"{(articlesLayout.IsVisible ? "－" : "＋")} {chapter.Name}";
            };

            ChaptersContainer.Children.Add(chapterButton);
            ChaptersContainer.Children.Add(articlesLayout);
        }
    }
}