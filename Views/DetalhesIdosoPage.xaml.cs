
using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class DetalhesIdosoPage : ContentPage, IQueryAttributable
{
    private Idoso? _idoso;
    private string _tipoUsuario = "";
    private bool _acessoNegado;
    private bool _verificandoAcesso;

    public Idoso? IdosoSelecionado { get; set; }

    public DetalhesIdosoPage()
    {
        InitializeComponent();

        _tipoUsuario = Preferences.Get("UsuarioTipo", "");
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        if (query.TryGetValue("IdosoSelecionado", out var valor))
        {
            _idoso = valor as Idoso;
            IdosoSelecionado = _idoso;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_verificandoAcesso || _acessoNegado)
            return;

        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        await VerificarAcessoAsync();
    }

    private async Task VerificarAcessoAsync()
    {
        if (_idoso == null)
            return;

        _verificandoAcesso = true;

        try
        {
            if (_tipoUsuario == "Acompanhante")
            {
                int usuarioId = Preferences.Get("UsuarioId", 0);

                if (usuarioId <= 0)
                {
                    await NegarAcessoAsync();
                    return;
                }

                var database = new DatabaseService();

                await database.InicializarBancoAsync();

                var idososPermitidos =
                    await database.ListarIdososPorUsuarioAsync(usuarioId);

                bool temAcesso = idososPermitidos.Any(
                    i => i.Id == _idoso.Id);

                if (!temAcesso)
                {
                    await NegarAcessoAsync();
                    return;
                }
            }

            MostrarDados();
        }
        catch
        {
            await DisplayAlert(
                "Erro",
                "Não foi possível verificar a permissão de acesso. Tente novamente.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            _verificandoAcesso = false;
        }
    }

    private async Task NegarAcessoAsync()
    {
        _acessoNegado = true;

        await DisplayAlert(
            "Acesso negado",
            "Você não tem permissão para visualizar as informações deste idoso.",
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    private void MostrarDados()
    {
        if (_idoso == null)
            return;

        lblNome.Text = _idoso.Nome;

        lblIdade.Text =
            $"{CalcularIdade(_idoso.DataNascimento)} anos";

        lblDataNascimento.Text =
            _idoso.DataNascimento.ToString("dd/MM/yyyy");

        lblCPF.Text =
            string.IsNullOrWhiteSpace(_idoso.CPF)
                ? "Não informado"
                : _idoso.CPF;

        lblTelefone.Text =
            string.IsNullOrWhiteSpace(_idoso.Telefone)
                ? "Não informado"
                : _idoso.Telefone;

        lblEndereco.Text =
            string.IsNullOrWhiteSpace(_idoso.Endereco)
                ? "Não informado"
                : _idoso.Endereco;

        lblGenero.Text =
            string.IsNullOrWhiteSpace(_idoso.Genero)
                ? "Não informado"
                : _idoso.Genero;

        lblContatoEmergencia.Text =
            string.IsNullOrWhiteSpace(_idoso.ContatoEmergencia)
                ? "Não informado"
                : _idoso.ContatoEmergencia;

        lblObservacoes.Text =
            string.IsNullOrWhiteSpace(_idoso.Observacoes)
                ? "Nenhuma observação"
                : _idoso.Observacoes;

        ConfigurarPermissoes();
    }

    private void ConfigurarPermissoes()
    {
        BtnEditar.IsVisible = _tipoUsuario == "Administrador"
            || _tipoUsuario == "Enfermeiro";

        BtnExcluir.IsVisible = _tipoUsuario == "Administrador";
    }

    private int CalcularIdade(DateTime dataNascimento)
    {
        var hoje = DateTime.Today;

        int idade = hoje.Year - dataNascimento.Year;

        if (dataNascimento.Date > hoje.AddYears(-idade))
        {
            idade--;
        }

        return idade;
    }

    private async void BtnMedicamentos_Clicked(
        object sender,
        EventArgs e)
    {
        if (_idoso == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(MedicamentosPage),
            new Dictionary<string, object>
            {
                { "IdosoSelecionado", _idoso }
            });
    }

    private async void BtnEditar_Clicked(
        object sender,
        EventArgs e)
    {
        if (_idoso == null)
            return;

        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante pode apenas visualizar as informações.",
                "OK");

            return;
        }

        await Shell.Current.GoToAsync(
            nameof(CadastroIdosoPage),
            new Dictionary<string, object>
            {
                { "IdosoSelecionado", _idoso }
            });
    }

    private async void BtnExcluir_Clicked(
        object sender,
        EventArgs e)
    {
        if (_idoso == null)
            return;

        if (_tipoUsuario != "Administrador")
        {
            await DisplayAlert(
                "Acesso",
                "Somente o administrador pode excluir idosos.",
                "OK");

            return;
        }

        bool confirmar = await DisplayAlert(
            "Excluir idoso",
            $"Deseja realmente excluir {_idoso.Nome}?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        var database = new DatabaseService();

        await database.InicializarBancoAsync();
        await database.ExcluirIdosoAsync(_idoso);

        await DisplayAlert(
            "Sucesso",
            "Idoso excluído com sucesso.",
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    private async void BtnObservacoes_Clicked(
        object sender,
        EventArgs e)
    {
        if (_idoso == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(ObservacoesPage),
            new Dictionary<string, object>
            {
                { "IdosoSelecionado", _idoso }
            });
    }
}
