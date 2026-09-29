namespace MauiAppVivaSenior.Views;

// Esta classe controla as ações realizadas no painel principal

public partial class DashboardPage : ContentPage
{
    public DashboardPage()
    {
        InitializeComponent();
    }

    private async void BtnIdosos_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(IdososPage));
    }

    private async void BtnMedicamentos_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(CatalogoMedicamentosPage));
    }

    private async void BtnConsultas_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(IdososPage),
            new Dictionary<string, object>
            {
            {
                "AbrirConsultas",
                true
            }
            });
    }
    private async void BtnObservacoes_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ObservacoesPage));
    }
}