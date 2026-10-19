using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

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

    private async void BtnSalvar_Clicked(
        object sender,
        EventArgs e)
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

        if (pckViaAdministracao.SelectedItem == null)
        {
            await DisplayAlert(
                "Atenção",
                "Selecione a via de administração.",
                "OK");

            return;
        }

        if (pckStatus.SelectedItem == null)
        {
            await DisplayAlert(
                "Atenção",
                "Selecione o status do medicamento.",
                "OK");

            return;
        }

        await _databaseService.InicializarBancoAsync();

        var medicamento =
            await _databaseService.BuscarMedicamentoPorNomeAsync(
                txtNome.Text.Trim());

        if (medicamento == null)
        {
            medicamento = new Medicamento
            {
                Nome = txtNome.Text.Trim(),

                ViaAdministracao =
                    pckViaAdministracao.SelectedItem.ToString(),

                Status =
                    pckStatus.SelectedItem.ToString()
            };

            await _databaseService.SalvarMedicamentoAsync(
                medicamento);
        }
        else
        {
            medicamento.ViaAdministracao =
                pckViaAdministracao.SelectedItem.ToString();

            medicamento.Status =
                pckStatus.SelectedItem.ToString();

            await _databaseService.AtualizarMedicamentoAsync(
                medicamento);
        }

        var associacao =
            await _databaseService.BuscarIdosoMedicamentoAsync(
                _idoso.Id,
                medicamento.Id);

        if (associacao != null)
        {
            await DisplayAlert(
                "Atenção",
                "Esse medicamento já está associado a este idoso.",
                "OK");

            return;
        }

        IdosoMedicamento idosoMedicamento =
            new IdosoMedicamento
            {
                IdosoId = _idoso.Id,
                MedicamentoId = medicamento.Id,

                Dosagem =
                    txtDosagem.Text?.Trim() ?? string.Empty,

                Horario =
                    tmpHorario.Time.ToString(),

                Observacao =
                    txtObservacao.Text?.Trim() ?? string.Empty
            };

        await _databaseService.SalvarIdosoMedicamentoAsync(
            idosoMedicamento);

        await DisplayAlert(
            "Sucesso",
            "Medicamento associado ao idoso com sucesso!",
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