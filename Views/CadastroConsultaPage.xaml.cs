
using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class CadastroConsultaPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    private Idoso? _idoso;

    private string _tipoUsuario = "";
    private bool _acessoNegado;
    private bool _verificandoAcesso;

    public CadastroConsultaPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        if (query.TryGetValue("IdosoSelecionado", out var valor))
        {
            _idoso = valor as Idoso;

            if (_idoso != null)
            {
                lblNomeIdoso.Text = "Consulta de: " + _idoso.Nome;
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_verificandoAcesso || _acessoNegado)
            return;

        _verificandoAcesso = true;

        try
        {
            _tipoUsuario = Preferences.Get("UsuarioTipo", "");

            if (_tipoUsuario != "Administrador" &&
                _tipoUsuario != "Enfermeiro")
            {
                _acessoNegado = true;

                await DisplayAlert(
                    "Acesso negado",
                    "Somente Administrador e Enfermeiro podem cadastrar consultas.",
                    "OK");

                await Shell.Current.GoToAsync("..");
                return;
            }

            if (_idoso == null)
            {
                _acessoNegado = true;

                await DisplayAlert(
                    "Atenção",
                    "Nenhum idoso foi selecionado para esta consulta.",
                    "OK");

                await Shell.Current.GoToAsync("..");
                return;
            }

            await _databaseService.InicializarBancoAsync();
        }
        catch
        {
            _acessoNegado = true;

            await DisplayAlert(
                "Erro",
                "Não foi possível validar o acesso ao cadastro de consultas.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            _verificandoAcesso = false;
        }
    }

    private async void BtnSalvar_Clicked(
        object sender,
        EventArgs e)
    {
        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        if (_tipoUsuario != "Administrador" &&
            _tipoUsuario != "Enfermeiro")
        {
            await DisplayAlert(
                "Acesso negado",
                "Você não tem permissão para cadastrar consultas.",
                "OK");

            return;
        }

        if (_acessoNegado || _idoso == null)
        {
            await DisplayAlert(
                "Atenção",
                "Não foi possível identificar o idoso selecionado.",
                "OK");

            return;
        }

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
            Especialidade = txtEspecialidade.Text.Trim(),
            Medico = txtMedico.Text.Trim(),
            Data = dtpData.Date ?? DateTime.Today,
            Horario = tmpHorario.Time.ToString(),
            Local = txtLocal.Text?.Trim() ?? string.Empty,
            Observacao = txtObservacao.Text?.Trim() ?? string.Empty,
            Documentos = txtDocumentos.Text?.Trim() ?? string.Empty
        };

        try
        {
            await _databaseService.InicializarBancoAsync();

            await _databaseService.SalvarConsultaAsync(consulta);

            await DisplayAlert(
                "Sucesso",
                "Consulta cadastrada com sucesso!",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch
        {
            await DisplayAlert(
                "Erro",
                "Não foi possível salvar a consulta. Tente novamente.",
                "OK");
        }
    }

    private async void BtnCancelar_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
