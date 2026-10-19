
using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiAppVivaSenior.Views;

public partial class ObservacoesPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;
    private Idoso? _idoso;

    private string _tipoUsuario = "";

    private bool _exibirCadastrar;
    private bool _exibirEditar;
    private bool _exibirExcluir;

    private bool _verificandoAcesso;
    private bool _acessoNegado;

    public bool ExibirCadastrar
    {
        get => _exibirCadastrar;
        set
        {
            _exibirCadastrar = value;
            OnPropertyChanged();
        }
    }

    public bool ExibirEditar
    {
        get => _exibirEditar;
        set
        {
            _exibirEditar = value;
            OnPropertyChanged();
        }
    }

    public bool ExibirExcluir
    {
        get => _exibirExcluir;
        set
        {
            _exibirExcluir = value;
            OnPropertyChanged();
        }
    }

    public ObservacoesPage()
    {
        InitializeComponent();

        BindingContext = this;
        _databaseService = new DatabaseService();

        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        ConfigurarPermissoes();
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        if (query.TryGetValue("IdosoSelecionado", out var valor))
        {
            _idoso = valor as Idoso;

            if (_idoso != null)
                lblNomeIdoso.Text = "Observações de: " + _idoso.Nome;
        }

        ConfigurarPermissoes();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_verificandoAcesso || _acessoNegado)
            return;

        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        ConfigurarPermissoes();

        await VerificarAcessoAsync();
    }

    private void ConfigurarPermissoes()
    {
        bool podeAlterar =
            _tipoUsuario == "Administrador" ||
            _tipoUsuario == "Enfermeiro";

        ExibirCadastrar = podeAlterar;
        ExibirEditar = podeAlterar;
        ExibirExcluir = podeAlterar;
    }

    private async Task VerificarAcessoAsync()
    {
        if (_idoso == null)
        {
            _acessoNegado = true;

            await DisplayAlert(
                "Atenção",
                "Nenhum idoso foi selecionado para visualizar as observações.",
                "OK");

            await Shell.Current.GoToAsync("..");
            return;
        }

        _verificandoAcesso = true;

        try
        {
            await _databaseService.InicializarBancoAsync();

            if (_tipoUsuario == "Acompanhante")
            {
                int usuarioId = Preferences.Get("UsuarioId", 0);

                if (usuarioId <= 0)
                {
                    await NegarAcessoAsync();
                    return;
                }

                var idososPermitidos =
                    await _databaseService.ListarIdososPorUsuarioAsync(
                        usuarioId);

                bool temAcesso = idososPermitidos.Any(
                    i => i.Id == _idoso.Id);

                if (!temAcesso)
                {
                    await NegarAcessoAsync();
                    return;
                }
            }

            await CarregarObservacoesAsync();
        }
        catch
        {
            _acessoNegado = true;

            await DisplayAlert(
                "Erro",
                "Não foi possível verificar a permissão para acessar as observações.",
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
            "Você não tem permissão para visualizar as observações deste idoso.",
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    private async Task CarregarObservacoesAsync()
    {
        if (_idoso == null)
            return;

        var observacoes =
            await _databaseService.ListarObservacoesDiariasAsync(
                _idoso.Id);

        listaObservacoes.ItemsSource = observacoes;
    }

    private async void BtnCadastrarObservacao_Clicked(
        object sender,
        EventArgs e)
    {
        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante pode apenas visualizar as observações.",
                "OK");

            return;
        }

        if (_idoso == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(CadastroObservacaoDiariaPage),
            new Dictionary<string, object>
            {
                { "IdosoSelecionado", _idoso }
            });
    }

    private async void BtnEditarObservacao_Clicked(
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

        if (_idoso == null)
            return;

        if (sender is not Button botao ||
            botao.BindingContext is not ObservacaoDiaria observacao)
            return;

        if (observacao.IdosoId != _idoso.Id)
        {
            await DisplayAlert(
                "Acesso negado",
                "Esta observação não pertence ao idoso selecionado.",
                "OK");

            return;
        }

        await Shell.Current.GoToAsync(
            nameof(EditarObservacaoDiariaPage),
            new Dictionary<string, object>
            {
                { "ObservacaoSelecionada", observacao }
            });
    }

    private async void BtnExcluirObservacao_Clicked(
        object sender,
        EventArgs e)
    {
        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante não pode excluir observações.",
                "OK");

            return;
        }

        if (_idoso == null)
            return;

        if (sender is not Button botao ||
            botao.BindingContext is not ObservacaoDiaria observacao)
            return;

        if (observacao.IdosoId != _idoso.Id)
        {
            await DisplayAlert(
                "Acesso negado",
                "Esta observação não pertence ao idoso selecionado.",
                "OK");

            return;
        }

        bool confirmar = await DisplayAlert(
            "Excluir observação",
            "Deseja realmente excluir esta observação?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        await _databaseService.InicializarBancoAsync();

        await _databaseService.ExcluirObservacaoDiariaAsync(
            observacao);

        await CarregarObservacoesAsync();
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected new void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
