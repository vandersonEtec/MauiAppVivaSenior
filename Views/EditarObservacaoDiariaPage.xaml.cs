using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class EditarObservacaoDiariaPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;

    private ObservacaoDiaria? _observacao;

    private string _tipoUsuario = "";

    public EditarObservacaoDiariaPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();

        _tipoUsuario =
            Preferences.Get("UsuarioTipo", "");
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        _tipoUsuario =
            Preferences.Get("UsuarioTipo", "");

        if (_tipoUsuario == "Acompanhante")
            return;

        if (query.TryGetValue(
            "ObservacaoSelecionada",
            out var valor))
        {
            _observacao =
                valor as ObservacaoDiaria;

            if (_observacao != null)
            {
                dtpData.Date =
                    _observacao.Data;

                txtAlimentacao.Text =
                    _observacao.Alimentacao;

                txtHidratacao.Text =
                    _observacao.Hidratacao;

                txtSono.Text =
                    _observacao.Sono;

                txtMedicacaoDiaria.Text =
                    _observacao.MedicacaoDiaria;

                txtObservacoes.Text =
                    _observacao.Observacoes;
            }
        }
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
                "O perfil Acompanhante pode apenas visualizar as observações.",
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
                "O perfil Acompanhante não pode alterar observações.",
                "OK");

            return;
        }

        if (_observacao == null)
            return;

        _observacao.Data = dtpData.Date ?? DateTime.Today;

        _observacao.Alimentacao =
            txtAlimentacao.Text?.Trim();

        _observacao.Hidratacao =
            txtHidratacao.Text?.Trim();

        _observacao.Sono =
            txtSono.Text?.Trim();

        _observacao.MedicacaoDiaria =
            txtMedicacaoDiaria.Text?.Trim();

        _observacao.Observacoes =
            txtObservacoes.Text?.Trim();

        await _databaseService
            .InicializarBancoAsync();

        await _databaseService
            .AtualizarObservacaoDiariaAsync(
                _observacao);

        await DisplayAlert(
            "Sucesso",
            "Observação diária atualizada com sucesso.",
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