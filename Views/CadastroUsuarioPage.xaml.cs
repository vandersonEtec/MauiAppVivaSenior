using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class CadastroUsuarioPage : ContentPage
{
    private readonly DatabaseService _database;

    public CadastroUsuarioPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    private async void BtnCadastrar_Clicked(
        object sender,
        EventArgs e)
    {
        string nome =
            txtNome.Text?.Trim();

        string email =
            txtEmail.Text?.Trim();

        string senha =
            txtSenha.Text?.Trim();

        string tipo =
            pckTipo.SelectedItem?.ToString();

        if (string.IsNullOrWhiteSpace(nome))
        {
            await DisplayAlert(
                "Atenção",
                "Digite o nome.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert(
                "Atenção",
                "Digite o e-mail.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert(
                "Atenção",
                "Digite a senha.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(tipo))
        {
            await DisplayAlert(
                "Atenção",
                "Selecione o tipo de usuário.",
                "OK");

            return;
        }

        var usuarioExistente =
            await _database
                .BuscarUsuarioPorEmailAsync(email);

        if (usuarioExistente != null)
        {
            await DisplayAlert(
                "Atenção",
                "Este e-mail já está cadastrado.",
                "OK");

            return;
        }

        await _database
            .InicializarBancoAsync();

        var usuario = new Usuario
        {
            Nome = nome,
            Email = email,
            Senha = senha,
            Tipo = tipo
        };

        await _database
            .SalvarUsuarioAsync(usuario);

        if (tipo == "Administrador")
        {
            var administrador = new Administrador
            {
                UsuarioId = usuario.Id,
                Permissao = "Gerenciar usuários"
            };

            await _database
                .SalvarAdministradorAsync(
                    administrador);
        }

        await DisplayAlert(
            "Sucesso",
            "Usuário cadastrado com sucesso.",
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    private async void BtnCancelar_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}