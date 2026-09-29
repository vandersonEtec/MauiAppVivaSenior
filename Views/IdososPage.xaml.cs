using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class IdososPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    private bool _abrirConsultas;

    public IdososPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        _abrirConsultas = false;

        if (query.ContainsKey("AbrirConsultas"))
        {
            _abrirConsultas = true;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await CarregarIdososAsync();
    }

    private async Task CarregarIdososAsync()
    {
        await _databaseService.InicializarBancoAsync();

        var idosos = await _databaseService.ListarIdososAsync();

        listaIdosos.ItemsSource = idosos;
    }

    private async void BtnCadastrarIdoso_Clicked(
        object sender,
        EventArgs e)
    {
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

        if (_abrirConsultas)
        {
            await Shell.Current.GoToAsync(
                nameof(ConsultasPage),
                new Dictionary<string, object>
                {
                    {
                        "IdosoSelecionado",
                        idosoSelecionado
                    }
                });

            return;
        }

        await Shell.Current.GoToAsync(
            nameof(DetalhesIdosoPage),
            new Dictionary<string, object>
            {
                {
                    "IdosoSelecionado",
                    idosoSelecionado
                }
            });
    }
}