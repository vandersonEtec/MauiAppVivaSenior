using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class CadastroMedicamentoCatalogoPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    private string _tipoUsuario = "";

    public CadastroMedicamentoCatalogoPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();

        _tipoUsuario =
            Preferences.Get("UsuarioTipo", "");
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _tipoUsuario =
            Preferences.Get("UsuarioTipo", "");

        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante pode apenas visualizar os medicamentos.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
    }

    private async void BtnSalvar_Clicked(
        object sender,
        EventArgs e)
    {
        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante não pode cadastrar medicamentos.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(txtNome.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Digite o nome do medicamento.",
                "OK");

            return;
        }

        await _databaseService
            .InicializarBancoAsync();

        var medicamentoExistente =
            await _databaseService
                .BuscarMedicamentoPorNomeAsync(
                    txtNome.Text.Trim());

        if (medicamentoExistente != null)
        {
            await DisplayAlert(
                "Atenção",
                "Esse medicamento já está cadastrado.",
                "OK");

            return;
        }

        Medicamento medicamento =
            new Medicamento
            {
                Nome =
                    txtNome.Text.Trim()
            };

        await _databaseService
            .SalvarMedicamentoAsync(
                medicamento);

        await DisplayAlert(
            "Sucesso",
            "Medicamento cadastrado com sucesso!",
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