using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class CadastroMedicamentoCatalogoPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public CadastroMedicamentoCatalogoPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
    }

    private async void BtnSalvar_Clicked(
        object sender,
        EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNome.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Digite o nome do medicamento.",
                "OK");

            return;
        }

        await _databaseService.InicializarBancoAsync();

        var medicamentoExistente =
            await _databaseService.BuscarMedicamentoPorNomeAsync(
                txtNome.Text.Trim());

        if (medicamentoExistente != null)
        {
            await DisplayAlert(
                "Atenção",
                "Esse medicamento já está cadastrado.",
                "OK");

            return;
        }

        Medicamento medicamento = new Medicamento
        {
            Nome = txtNome.Text.Trim()
        };

        await _databaseService.SalvarMedicamentoAsync(
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