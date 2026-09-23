using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

// Esta classe carrega e apresentar os idosos cadastrados.

public partial class IdososPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public IdososPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
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

    private async void BtnCadastrarIdoso_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CadastroIdosoPage));
    }

    private async void ListaIdosos_SelectionChanged(
    object sender,
    SelectionChangedEventArgs e)
    {
        var idosoSelecionado = e.CurrentSelection.FirstOrDefault()
            as MauiAppVivaSenior.Models.Idoso;

        if (idosoSelecionado == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(DetalhesIdosoPage),
            new Dictionary<string, object>
            {
            { "IdosoSelecionado", idosoSelecionado }
            });

        ((CollectionView)sender).SelectedItem = null;
    }
}