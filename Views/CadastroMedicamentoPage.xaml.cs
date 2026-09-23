using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

// Esta classe controla o cadastro de medicamentos.

public partial class CadastroMedicamentoPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    private Idoso _idoso;

    public CadastroMedicamentoPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        if (query.ContainsKey("IdosoSelecionado"))
        {
            _idoso = query["IdosoSelecionado"] as Idoso;
        }
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        if (_idoso == null)
            return;

        if (string.IsNullOrWhiteSpace(txtNome.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Digite o nome do medicamento.",
                "OK");

            return;
        }

        Medicamento medicamento = new Medicamento
        {
            IdosoId = _idoso.Id,
            Nome = txtNome.Text,
            Dosagem = txtDosagem.Text,
            Horario = tmpHorario.Time.ToString(),
            Observacao = txtObservacao.Text
        };

        await _databaseService.InicializarBancoAsync();

        await _databaseService.SalvarMedicamentoAsync(medicamento);

        await DisplayAlert(
            "Sucesso",
            "Medicamento cadastrado com sucesso!",
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}