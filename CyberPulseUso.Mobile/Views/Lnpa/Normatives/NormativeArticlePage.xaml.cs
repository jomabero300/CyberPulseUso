using CyberPulseUso.Shared.EntitiesDTO.Lnpa;

namespace CyberPulseUso.Mobile.Views.Lnpa.Normatives;

public partial class NormativeArticlePage : ContentPage
{
    public NormativeArticlePage(NormativeArticleDTO article, string normativeName, string chapterName)
    {
        InitializeComponent();

        Title = article.Name;

        LblNormative.Text = normativeName;
        LblChapter.Text = chapterName;
        LblTitle.Text = article.Name;
        LblDescription.Text = article.Description;
        LblContent.Text = article.Content;
    }

    private async void OnBackClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}