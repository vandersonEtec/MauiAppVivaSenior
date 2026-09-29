using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class EditarMedicamentoPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;

    private IdosoMedicamento? _idosoMedicamento;

    public EditarMedicamentoPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        if (query.TryGetValue(
            "IdosoMedicamento",
            out var valor))
        {
            _idosoMedicamento = valor as IdosoMedicamento;

            if (_idosoMedicamento != null)
            {
                txtDosagem.Text =
                    _idosoMedicamento.Dosagem;

                txtHorario.Text =
                    _idosoMedicamento.Horario;

                txtObservacao.Text =
                    _idosoMedicamento.Observacao;
            }

            if (query.TryGetValue(
                "NomeMedicamento",
                out var nome))
            {
                lblNomeMedicamento.Text =
                    nome?.ToString();
            }
        }
    }

    private async void BtnSalvar_Clicked(
        object sender,
        EventArgs e)
    {
        if (_idosoMedicamento == null)
            return;

        _idosoMedicamento.Dosagem =
            txtDosagem.Text?.Trim() ?? string.Empty;

        _idosoMedicamento.Horario =
            txtHorario.Text?.Trim() ?? string.Empty;

        _idosoMedicamento.Observacao =
            txtObservacao.Text?.Trim() ?? string.Empty;

        await _databaseService.InicializarBancoAsync();

        await _databaseService.AtualizarIdosoMedicamentoAsync(
            _idosoMedicamento);

        await DisplayAlert(
            "Sucesso",
            "Medicação atualizada com sucesso.",
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