
using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class AssociarAcompanhantePage : ContentPage
{
    private readonly DatabaseService _database;

    public AssociarAcompanhantePage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        string tipoUsuario = Preferences.Get("UsuarioTipo", "");

        if (tipoUsuario != "Administrador")
        {
            await DisplayAlert(
                "Acesso negado",
                "Somente o administrador pode associar acompanhantes aos idosos.",
                "OK");

            await Shell.Current.GoToAsync("..");
            return;
        }

        await CarregarDadosAsync();
    }

    private async Task CarregarDadosAsync()
    {
        await _database.InicializarBancoAsync();

        var usuarios = await _database.ListarUsuariosAsync();

        var acompanhantes = usuarios
            .Where(u => u.Tipo == "Acompanhante")
            .ToList();

        var idosos = await _database.ListarIdososAsync();

        pckAcompanhante.ItemsSource = acompanhantes;
        pckIdoso.ItemsSource = idosos;

        pckAcompanhante.SelectedItem = null;
        pckIdoso.SelectedItem = null;

        if (acompanhantes.Count == 0 || idosos.Count == 0)
        {
            string mensagem;

            if (acompanhantes.Count == 0 && idosos.Count == 0)
            {
                mensagem = "Cadastre pelo menos um acompanhante e um idoso antes de fazer a associação.";
            }
            else if (acompanhantes.Count == 0)
            {
                mensagem = "Não há acompanhantes cadastrados. Cadastre um usuário com o perfil Acompanhante.";
            }
            else
            {
                mensagem = "Não há idosos cadastrados. Cadastre um idoso antes de fazer a associação.";
            }

            await DisplayAlert("Atenção", mensagem, "OK");
        }
    }

    private async void BtnAssociar_Clicked(
        object sender,
        EventArgs e)
    {
        if (Preferences.Get("UsuarioTipo", "") != "Administrador")
        {
            await DisplayAlert(
                "Acesso negado",
                "Somente o administrador pode realizar associações.",
                "OK");

            return;
        }

        var acompanhante = pckAcompanhante.SelectedItem as Usuario;
        var idoso = pckIdoso.SelectedItem as Idoso;

        if (acompanhante == null)
        {
            await DisplayAlert(
                "Atenção",
                "Selecione um acompanhante.",
                "OK");

            return;
        }

        if (idoso == null)
        {
            await DisplayAlert(
                "Atenção",
                "Selecione um idoso.",
                "OK");

            return;
        }

        var vinculo = new UsuarioIdoso
        {
            UsuarioId = acompanhante.Id,
            IdosoId = idoso.Id
        };

        int resultado = await _database.SalvarUsuarioIdosoAsync(vinculo);

        if (resultado == 0)
        {
            await DisplayAlert(
                "Atenção",
                "Este acompanhante já está associado a esse idoso.",
                "OK");

            return;
        }

        await DisplayAlert(
            "Sucesso",
            $"O acompanhante {acompanhante.Nome} foi associado ao idoso {idoso.Nome}.",
            "OK");

        pckAcompanhante.SelectedItem = null;
        pckIdoso.SelectedItem = null;
    }

    private async void BtnVoltar_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
