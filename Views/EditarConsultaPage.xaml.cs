
using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class EditarConsultaPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    private Consulta? _consulta;

    private bool _acessoNegado;
    private bool _verificandoAcesso;
    private bool _dadosCarregados;

    public EditarConsultaPage()
    {
        InitializeComponent();
        _databaseService = new DatabaseService();
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        if (query.TryGetValue("ConsultaSelecionada", out var valor))
        {
            _consulta = valor as Consulta;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_verificandoAcesso || _acessoNegado || _dadosCarregados)
            return;

        _verificandoAcesso = true;

        try
        {
            string tipoUsuario = Preferences.Get("UsuarioTipo", "");

            if (tipoUsuario != "Administrador" &&
                tipoUsuario != "Enfermeiro")
            {
                await NegarAcessoAsync(
                    "Somente Administrador e Enfermeiro podem editar consultas.");
                return;
            }

            if (_consulta == null)
            {
                await NegarAcessoAsync(
                    "Nenhuma consulta foi selecionada para edição.");
                return;
            }

            await _databaseService.InicializarBancoAsync();

            var consultasDoIdoso =
                await _databaseService.ListarConsultasAsync(
                    _consulta.IdosoId);

            var consultaBanco = consultasDoIdoso.FirstOrDefault(
                c => c.Id == _consulta.Id);

            if (consultaBanco == null)
            {
                await NegarAcessoAsync(
                    "A consulta não foi encontrada ou não está mais disponível.");
                return;
            }

            _consulta = consultaBanco;

            PreencherCampos();
            _dadosCarregados = true;
        }
        catch
        {
            _acessoNegado = true;

            await DisplayAlert(
                "Erro",
                "Não foi possível validar a consulta. Tente novamente.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            _verificandoAcesso = false;
        }
    }

    private void PreencherCampos()
    {
        if (_consulta == null)
            return;

        txtEspecialidade.Text = _consulta.Especialidade;
        txtMedico.Text = _consulta.Medico;
        dtpData.Date = _consulta.Data;

        if (TimeSpan.TryParse(_consulta.Horario, out var horario))
        {
            tmpHorario.Time = horario;
        }

        txtLocal.Text = _consulta.Local;
        txtObservacao.Text = _consulta.Observacao;
        txtDocumentos.Text = _consulta.Documentos;
    }

    private async Task NegarAcessoAsync(string mensagem)
    {
        _acessoNegado = true;

        await DisplayAlert(
            "Acesso negado",
            mensagem,
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    private async void BtnSalvar_Clicked(
        object sender,
        EventArgs e)
    {
        string tipoUsuario = Preferences.Get("UsuarioTipo", "");

        if (tipoUsuario != "Administrador" &&
            tipoUsuario != "Enfermeiro")
        {
            await DisplayAlert(
                "Acesso negado",
                "Você não tem permissão para editar consultas.",
                "OK");

            return;
        }

        if (_acessoNegado || !_dadosCarregados || _consulta == null)
        {
            await DisplayAlert(
                "Atenção",
                "Não foi possível validar a consulta selecionada.",
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

        try
        {
            await _databaseService.InicializarBancoAsync();

            // Confere novamente se a consulta ainda existe.
            var consultasDoIdoso =
                await _databaseService.ListarConsultasAsync(
                    _consulta.IdosoId);

            var consultaBanco = consultasDoIdoso.FirstOrDefault(
                c => c.Id == _consulta.Id);

            if (consultaBanco == null)
            {
                await DisplayAlert(
                    "Atenção",
                    "Esta consulta não está mais disponível.",
                    "OK");

                await Shell.Current.GoToAsync("..");
                return;
            }

            consultaBanco.Especialidade = txtEspecialidade.Text.Trim();
            consultaBanco.Medico = txtMedico.Text.Trim();
            consultaBanco.Data = dtpData.Date ?? DateTime.Today;
            consultaBanco.Horario = tmpHorario.Time.ToString();
            consultaBanco.Local = txtLocal.Text?.Trim() ?? string.Empty;
            consultaBanco.Observacao = txtObservacao.Text?.Trim() ?? string.Empty;
            consultaBanco.Documentos = txtDocumentos.Text?.Trim() ?? string.Empty;

            await _databaseService.AtualizarConsultaAsync(consultaBanco);

            await DisplayAlert(
                "Sucesso",
                "Consulta atualizada com sucesso.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch
        {
            await DisplayAlert(
                "Erro",
                "Não foi possível atualizar a consulta. Tente novamente.",
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
