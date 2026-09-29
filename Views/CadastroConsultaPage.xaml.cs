using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class CadastroConsultaPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    private Idoso _idoso;

    public CadastroConsultaPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("IdosoSelecionado"))
        {
            _idoso = query["IdosoSelecionado"] as Idoso;

            if (_idoso != null)
            {
                lblNomeIdoso.Text =
                    "Consulta de: " + _idoso.Nome;
            }
        }
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        if (_idoso == null)
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

        Consulta consulta = new Consulta
        {
            IdosoId = _idoso.Id,
            Especialidade = txtEspecialidade.Text,
            Medico = txtMedico.Text,
            Data = dtpData.Date ?? DateTime.Today,
            Horario = tmpHorario.Time.ToString(),
            Observacao = txtObservacao.Text
        };

        await _databaseService.InicializarBancoAsync();

        await _databaseService.SalvarConsultaAsync(consulta);

        await DisplayAlert(
            "Sucesso",
            "Consulta cadastrada com sucesso!",
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}