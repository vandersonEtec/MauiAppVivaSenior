using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class EditarConsultaPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    private Consulta? _consulta;

    public EditarConsultaPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("ConsultaSelecionada", out var valor))
        {
            _consulta = valor as Consulta;

            if (_consulta != null)
            {
                txtEspecialidade.Text = _consulta.Especialidade;
                txtMedico.Text = _consulta.Medico;
                dtpData.Date = _consulta.Data;

                if (TimeSpan.TryParse(_consulta.Horario, out var horario))
                    tmpHorario.Time = horario;

                txtObservacao.Text = _consulta.Observacao;
            }
        }
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        if (_consulta == null)
            return;

        if (string.IsNullOrWhiteSpace(txtEspecialidade.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Digite a especialidade.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(txtMedico.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Digite o nome do médico.",
                "OK");

            return;
        }

        _consulta.Especialidade = txtEspecialidade.Text.Trim();
        _consulta.Medico = txtMedico.Text.Trim();
        _consulta.Data = dtpData.Date ?? DateTime.Today;
        _consulta.Horario = tmpHorario.Time.ToString();
        _consulta.Observacao =
            txtObservacao.Text?.Trim() ?? string.Empty;

        await _databaseService.InicializarBancoAsync();

        await _databaseService.AtualizarConsultaAsync(_consulta);

        await DisplayAlert(
            "Sucesso",
            "Consulta atualizada com sucesso.",
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}