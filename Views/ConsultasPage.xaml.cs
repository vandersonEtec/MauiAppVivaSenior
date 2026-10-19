
using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiAppVivaSenior.Views;

public partial class ConsultasPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
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

    public ConsultasPage()
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
                lblNomeIdoso.Text = "Consultas de: " + _idoso.Nome;
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
                "Nenhum idoso foi selecionado para consultar as consultas médicas.",
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

            await CarregarConsultasAsync();
        }
        catch
        {
            _acessoNegado = true;

            await DisplayAlert(
                "Erro",
                "Não foi possível verificar a permissão para acessar as consultas.",
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
            "Você não tem permissão para visualizar as consultas deste idoso.",
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    private async Task CarregarConsultasAsync()
    {
        if (_idoso == null)
            return;

        var consultas =
            await _databaseService.ListarConsultasAsync(_idoso.Id);

        listaConsultas.ItemsSource = consultas;
    }

    private async void BtnCadastrarConsulta_Clicked(
        object sender,
        EventArgs e)
    {
        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante pode apenas visualizar as consultas.",
                "OK");

            return;
        }

        if (_idoso == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(CadastroConsultaPage),
            new Dictionary<string, object>
            {
                { "IdosoSelecionado", _idoso }
            });
    }

    private async void BtnEditarConsulta_Clicked(
        object sender,
        EventArgs e)
    {
        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante não pode alterar consultas.",
                "OK");

            return;
        }

        if (_idoso == null)
            return;

        if (sender is not Button botao ||
            botao.BindingContext is not Consulta consulta)
            return;

        // Confirma que a consulta pertence ao idoso atualmente selecionado.
        if (consulta.IdosoId != _idoso.Id)
        {
            await DisplayAlert(
                "Acesso negado",
                "Esta consulta não pertence ao idoso selecionado.",
                "OK");

            return;
        }

        await Shell.Current.GoToAsync(
            nameof(EditarConsultaPage),
            new Dictionary<string, object>
            {
                { "ConsultaSelecionada", consulta }
            });
    }

    private async void BtnExcluirConsulta_Clicked(
        object sender,
        EventArgs e)
    {
        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante não pode excluir consultas.",
                "OK");

            return;
        }

        if (_idoso == null)
            return;

        if (sender is not Button botao ||
            botao.BindingContext is not Consulta consulta)
            return;

        if (consulta.IdosoId != _idoso.Id)
        {
            await DisplayAlert(
                "Acesso negado",
                "Esta consulta não pertence ao idoso selecionado.",
                "OK");

            return;
        }

        bool confirmar = await DisplayAlert(
            "Excluir consulta",
            "Deseja realmente excluir esta consulta?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        await _databaseService.InicializarBancoAsync();

        await _databaseService.ExcluirConsultaAsync(consulta);

        await CarregarConsultasAsync();
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
