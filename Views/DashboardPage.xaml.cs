
namespace MauiAppVivaSenior.Views;

public partial class DashboardPage : ContentPage
{
    private string _tipoUsuario = "";

    public DashboardPage()
    {
        InitializeComponent();

        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        btnAssociarAcompanhantes.IsVisible =
            _tipoUsuario == "Administrador";
    }

    private async void BtnIdosos_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(IdososPage));
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
                { "AbrirConsultas", true }
            });
    }

    private async void BtnObservacoes_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(IdososPage),
            new Dictionary<string, object>
            {
                { "AbrirObservacoes", true }
            });
    }

    private async void BtnAssociarAcompanhantes_Clicked(
        object sender,
        EventArgs e)
    {
        if (Preferences.Get("UsuarioTipo", "") != "Administrador")
        {
            await DisplayAlert(
                "Acesso negado",
                "Somente o administrador pode associar acompanhantes aos idosos.",
                "OK");

            return;
        }

        await Shell.Current.GoToAsync(
            nameof(AssociarAcompanhantePage));
    }
}
