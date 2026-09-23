using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class MedicamentosPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    private Idoso _idoso;

    public MedicamentosPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        if (query.ContainsKey("IdosoSelecionado"))
        {
            _idoso = query["IdosoSelecionado"] as Idoso;

            if (_idoso != null)
            {
                lblNomeIdoso.Text =
                    "Medicamentos de: " + _idoso.Nome;
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_idoso != null)
        {
            await CarregarMedicamentosAsync();
        }
    }

    private async Task CarregarMedicamentosAsync()
    {
        await _databaseService.InicializarBancoAsync();

        var medicamentos =
            await _databaseService.ListarMedicamentosAsync(_idoso.Id);

        listaMedicamentos.ItemsSource = medicamentos;
    }

    private async void BtnCadastrarMedicamento_Clicked(
    object sender,
    EventArgs e)
    {
        if (_idoso == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(CadastroMedicamentoPage),
            new Dictionary<string, object>
            {
            { "IdosoSelecionado", _idoso }
            });
    }
}