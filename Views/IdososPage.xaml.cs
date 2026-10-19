
using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class IdososPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;

    private bool _abrirConsultas;
    private bool _abrirObservacoes;

    private string _tipoUsuario = "";

    public IdososPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();

        _tipoUsuario = Preferences.Get("UsuarioTipo", "");
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        _abrirConsultas = false;
        _abrirObservacoes = false;

        if (query.ContainsKey("AbrirConsultas"))
        {
            _abrirConsultas = true;
        }

        if (query.ContainsKey("AbrirObservacoes"))
        {
            _abrirObservacoes = true;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        await CarregarIdososAsync();
    }

    private async Task CarregarIdososAsync()
    {
        await _databaseService.InicializarBancoAsync();

        List<Idoso> idosos;

        if (_tipoUsuario == "Acompanhante")
        {
            int usuarioId = Preferences.Get("UsuarioId", 0);

            if (usuarioId <= 0)
            {
                idosos = new List<Idoso>();
            }
            else
            {
                idosos = await _databaseService
                    .ListarIdososPorUsuarioAsync(usuarioId);
            }
        }
        else
        {
            idosos = await _databaseService.ListarIdososAsync();
        }

        listaIdosos.ItemsSource = idosos;
    }

    private async void BtnCadastrarIdoso_Clicked(
        object sender,
        EventArgs e)
    {
        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante pode apenas visualizar as informações dos idosos vinculados à sua conta.",
                "OK");

            return;
        }

        await Shell.Current.GoToAsync(
            nameof(CadastroIdosoPage));
    }

    private async void ListaIdosos_SelectionChanged(
        object sender,
        SelectionChangedEventArgs e)
    {
        var idosoSelecionado =
            e.CurrentSelection.FirstOrDefault() as Idoso;

        if (idosoSelecionado == null)
            return;

        listaIdosos.SelectedItem = null;

        // Confirma o vínculo antes de abrir as informações do idoso.
        if (_tipoUsuario == "Acompanhante")
        {
            int usuarioId = Preferences.Get("UsuarioId", 0);

            if (usuarioId <= 0)
            {
                await DisplayAlert(
                    "Acesso",
                    "Não foi possível identificar o usuário conectado.",
                    "OK");

                return;
            }

            var idososPermitidos = await _databaseService
                .ListarIdososPorUsuarioAsync(usuarioId);

            bool temAcesso = idososPermitidos.Any(
                i => i.Id == idosoSelecionado.Id);

            if (!temAcesso)
            {
                await DisplayAlert(
                    "Acesso negado",
                    "Você não tem permissão para visualizar as informações deste idoso.",
                    "OK");

                await CarregarIdososAsync();

                return;
            }
        }

        if (_abrirConsultas)
        {
            await Shell.Current.GoToAsync(
                nameof(ConsultasPage),
                new Dictionary<string, object>
                {
                    { "IdosoSelecionado", idosoSelecionado }
                });

            return;
        }

        if (_abrirObservacoes)
        {
            await Shell.Current.GoToAsync(
                nameof(ObservacoesPage),
                new Dictionary<string, object>
                {
                    { "IdosoSelecionado", idosoSelecionado }
                });

            return;
        }

        await Shell.Current.GoToAsync(
            nameof(DetalhesIdosoPage),
            new Dictionary<string, object>
            {
                { "IdosoSelecionado", idosoSelecionado }
            });
    }
}
