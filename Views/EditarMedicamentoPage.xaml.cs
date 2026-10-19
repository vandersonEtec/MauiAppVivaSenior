
using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class EditarMedicamentoPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;

    private IdosoMedicamento? _idosoMedicamento;
    private Medicamento? _medicamento;

    public EditarMedicamentoPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    public async void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        if (query.TryGetValue("IdosoMedicamento", out var valor))
        {
            _idosoMedicamento = valor as IdosoMedicamento;

            if (_idosoMedicamento != null)
            {
                txtDosagem.Text = _idosoMedicamento.Dosagem;
                txtHorario.Text = _idosoMedicamento.Horario;
                txtObservacao.Text = _idosoMedicamento.Observacao;

                await CarregarMedicamentoAsync();
            }
        }

        if (query.TryGetValue("NomeMedicamento", out var nome))
        {
            lblNomeMedicamento.Text = nome?.ToString();
        }
    }

    private async Task CarregarMedicamentoAsync()
    {
        if (_idosoMedicamento == null)
            return;

        await _databaseService.InicializarBancoAsync();

        _medicamento = await _databaseService
            .BuscarMedicamentoPorIdAsync(
                _idosoMedicamento.MedicamentoId);

        if (_medicamento == null)
            return;

        pckViaAdministracao.SelectedItem =
            _medicamento.ViaAdministracao;

        pckStatus.SelectedItem =
            _medicamento.Status;
    }

    private async void BtnSalvar_Clicked(
        object sender,
        EventArgs e)
    {
        if (_idosoMedicamento == null)
            return;

        if (_medicamento == null)
        {
            await DisplayAlert(
                "Atenção",
                "Não foi possível localizar o medicamento.",
                "OK");
            return;
        }

        // Validar dosagem
        if (string.IsNullOrWhiteSpace(txtDosagem.Text))
        {
            await DisplayAlert(
                "Campo obrigatório",
                "Informe a dosagem do medicamento.",
                "OK");

            txtDosagem.Focus();
            return;
        }

        // Validar horário
        if (string.IsNullOrWhiteSpace(txtHorario.Text))
        {
            await DisplayAlert(
                "Campo obrigatório",
                "Informe o horário do medicamento.",
                "OK");

            txtHorario.Focus();
            return;
        }

        // Validar via de administração
        if (pckViaAdministracao.SelectedItem == null ||
            string.IsNullOrWhiteSpace(
                pckViaAdministracao.SelectedItem.ToString()))
        {
            await DisplayAlert(
                "Campo obrigatório",
                "Selecione a via de administração.",
                "OK");

            return;
        }

        // Validar status
        if (pckStatus.SelectedItem == null ||
            string.IsNullOrWhiteSpace(
                pckStatus.SelectedItem.ToString()))
        {
            await DisplayAlert(
                "Campo obrigatório",
                "Selecione o status do medicamento.",
                "OK");

            return;
        }

        // Atualizar dados do medicamento
        _medicamento.ViaAdministracao =
            pckViaAdministracao.SelectedItem.ToString();

        _medicamento.Status =
            pckStatus.SelectedItem.ToString();

        // Atualizar dados do vínculo com o idoso
        _idosoMedicamento.Dosagem =
            txtDosagem.Text.Trim();

        _idosoMedicamento.Horario =
            txtHorario.Text.Trim();

        // Observação continua opcional
        _idosoMedicamento.Observacao =
            txtObservacao.Text?.Trim() ?? string.Empty;

        await _databaseService.InicializarBancoAsync();

        await _databaseService
            .AtualizarMedicamentoAsync(_medicamento);

        await _databaseService
            .AtualizarIdosoMedicamentoAsync(_idosoMedicamento);

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
