
using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class LoginPage : ContentPage
{
    private readonly DatabaseService _database;

    public LoginPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    private async void BtnEntrar_Clicked(object sender, EventArgs e)
    {
        string email = txtEmail.Text?.Trim();
        string senha = txtSenha.Text?.Trim();

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert(
                "Atenção",
                "Digite o e-mail e a senha.",
                "OK");

            return;
        }

        Usuario usuario =
            await _database.BuscarUsuarioPorEmailAsync(email);

        if (usuario == null || usuario.Senha != senha)
        {
            await DisplayAlert(
                "Login",
                "E-mail ou senha incorretos.",
                "OK");

            return;
        }

        // Guarda os dados do usuário conectado.
        Preferences.Set("UsuarioTipo", usuario.Tipo);
        Preferences.Set("UsuarioId", usuario.Id);

        await Shell.Current.GoToAsync(nameof(DashboardPage));
    }

    private async void CadastreSe_Tapped(
        object sender,
        TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(
            nameof(CadastroUsuarioPage));
    }
}
