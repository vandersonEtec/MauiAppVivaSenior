using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class CatalogoMedicamentosPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;

    private Idoso? _idoso;

    private string _tipoUsuario = "";

    public CatalogoMedicamentosPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();

        _tipoUsuario =
            Preferences.Get("UsuarioTipo", "");
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        _tipoUsuario =
            Preferences.Get("UsuarioTipo", "");

        if (query.TryGetValue(
            "IdosoSelecionado",
            out var valor))
        {
            _idoso = valor as Idoso;
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _tipoUsuario =
            Preferences.Get("UsuarioTipo", "");

        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante pode apenas visualizar os medicamentos.",
                "OK");

            await Shell.Current.GoToAsync("..");

            return;
        }

        await CarregarMedicamentosAsync();
    }

    private async Task CarregarMedicamentosAsync()
    {
        await _databaseService.InicializarBancoAsync();

        var medicamentos =
            await _databaseService
                .ListarTodosMedicamentosAsync();

        listaMedicamentos.ItemsSource =
            medicamentos;
    }

    private async void BtnCadastrarMedicamento_Clicked(
        object sender,
        EventArgs e)
    {
        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante não pode cadastrar medicamentos.",
                "OK");

            return;
        }

        await Shell.Current.GoToAsync(
            nameof(CadastroMedicamentoCatalogoPage));
    }

    private async void MedicamentoSelecionado(
        object sender,
        SelectionChangedEventArgs e)
    {
        if (_tipoUsuario == "Acompanhante")
        {
            listaMedicamentos.SelectedItem = null;

            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante não pode vincular medicamentos.",
                "OK");

            return;
        }

        if (e.CurrentSelection.FirstOrDefault()
            is not Medicamento medicamento)
        {
            return;
        }

        listaMedicamentos.SelectedItem = null;

        if (_idoso == null)
        {
            await DisplayAlert(
                "Atenção",
                "Nenhum idoso foi selecionado.",
                "OK");

            return;
        }

        await _databaseService
            .InicializarBancoAsync();

        var existente =
            await _databaseService
                .BuscarIdosoMedicamentoAsync(
                    _idoso.Id,
                    medicamento.Id);

        if (existente != null)
        {
            await DisplayAlert(
                "Atenção",
                "Este medicamento já está vinculado a este idoso.",
                "OK");

            return;
        }

        var associacao =
            new IdosoMedicamento
            {
                IdosoId = _idoso.Id,
                MedicamentoId = medicamento.Id
            };

        await _databaseService
            .SalvarIdosoMedicamentoAsync(
                associacao);

        await DisplayAlert(
            "Sucesso",
            "Medicamento vinculado ao idoso.",
            "OK");

        await Shell.Current.GoToAsync("..");
    }
}