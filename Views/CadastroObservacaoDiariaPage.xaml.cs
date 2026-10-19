using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class CadastroObservacaoDiariaPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;

    private Idoso? _idoso;
    private string _tipoUsuario = "";
    private bool _acessoNegado;
    private bool _verificandoAcesso;

    public CadastroObservacaoDiariaPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
        _tipoUsuario = Preferences.Get("UsuarioTipo", "");
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        if (query.TryGetValue("IdosoSelecionado", out var valor))
        {
            _idoso = valor as Idoso;

            if (_idoso != null)
            {
                lblNomeIdoso.Text = "Observação de: " + _idoso.Nome;
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_verificandoAcesso)
            return;

        _verificandoAcesso = true;
        _acessoNegado = false;

        try
        {
            _tipoUsuario = Preferences.Get("UsuarioTipo", "");

            if (_tipoUsuario != "Administrador" &&
                _tipoUsuario != "Enfermeiro")
            {
                await NegarAcessoAsync(
                    "Somente os perfis Administrador e Enfermeiro podem cadastrar observações diárias.");
                return;
            }

            if (_idoso == null)
            {
                await NegarAcessoAsync(
                    "Nenhum idoso foi selecionado.");
                return;
            }

            await _databaseService.InicializarBancoAsync();

            var idososBanco = await _databaseService.ListarIdososAsync();

            bool idosoExiste = idososBanco.Any(i => i.Id == _idoso.Id);

            if (!idosoExiste)
            {
                await NegarAcessoAsync(
                    "O idoso selecionado não foi encontrado no banco de dados.");
            }
        }
        catch (Exception ex)
        {
            _acessoNegado = true;

            await DisplayAlert(
                "Erro",
                $"Não foi possível verificar o acesso: {ex.Message}",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            _verificandoAcesso = false;
        }
    }

    private async Task NegarAcessoAsync(string mensagem)
    {
        _acessoNegado = true;

        await DisplayAlert("Acesso", mensagem, "OK");

        await Shell.Current.GoToAsync("..");
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        if (_acessoNegado ||
            (_tipoUsuario != "Administrador" &&
             _tipoUsuario != "Enfermeiro"))
        {
            await DisplayAlert(
                "Acesso",
                "Você não tem permissão para cadastrar observações diárias.",
                "OK");

            return;
        }

        if (_idoso == null)
        {
            await DisplayAlert(
                "Atenção",
                "Nenhum idoso foi selecionado.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(txtAlimentacao.Text))
        {
            await DisplayAlert(
                "Campo obrigatório",
                "Preencha o campo Alimentação.",
                "OK");

            txtAlimentacao.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtHidratacao.Text))
        {
            await DisplayAlert(
                "Campo obrigatório",
                "Preencha o campo Hidratação.",
                "OK");

            txtHidratacao.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtSono.Text))
        {
            await DisplayAlert(
                "Campo obrigatório",
                "Preencha o campo Sono.",
                "OK");

            txtSono.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(txtMedicacaoDiaria.Text))
        {
            await DisplayAlert(
                "Campo obrigatório",
                "Preencha o campo Medicação diária.",
                "OK");

            txtMedicacaoDiaria.Focus();
            return;
        }

        bool confirmar = await DisplayAlert(
            "Confirmar gravação",
            "Deseja realmente salvar esta observação diária?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        try
        {
            await _databaseService.InicializarBancoAsync();

            var idosos = await _databaseService.ListarIdososAsync();

            bool idosoExiste = idosos.Any(i => i.Id == _idoso.Id);

            if (!idosoExiste)
            {
                await DisplayAlert(
                    "Atenção",
                    "O idoso selecionado não foi encontrado. Selecione-o novamente.",
                    "OK");

                return;
            }

            var observacao = new ObservacaoDiaria
            {
                IdosoId = _idoso.Id,
                Data = dtpData.Date ?? DateTime.Today,
                Alimentacao = txtAlimentacao.Text.Trim(),
                Hidratacao = txtHidratacao.Text.Trim(),
                Sono = txtSono.Text.Trim(),
                MedicacaoDiaria = txtMedicacaoDiaria.Text.Trim(),
                Observacoes = txtObservacoes.Text?.Trim()
            };

            await _databaseService.SalvarObservacaoDiariaAsync(observacao);

            await DisplayAlert(
                "Sucesso",
                "Observação diária cadastrada com sucesso.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert(
                "Erro",
                $"Não foi possível cadastrar a observação: {ex.Message}",
                "OK");
        }
    }

    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}