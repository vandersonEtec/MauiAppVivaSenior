using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class ConsultasPage : ContentPage, IQueryAttributable
{
    private readonly DatabaseService _databaseService;
    private Idoso? _idoso;

    public ConsultasPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        if (query.TryGetValue(
            "IdosoSelecionado",
            out var valor))
        {
            _idoso = valor as Idoso;

            if (_idoso != null)
            {
                lblNomeIdoso.Text =
                    "Consultas de: " + _idoso.Nome;
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_idoso != null)
        {
            await CarregarConsultasAsync();
        }
    }

    private async Task CarregarConsultasAsync()
    {
        if (_idoso == null)
            return;

        await _databaseService.InicializarBancoAsync();

        var consultas =
            await _databaseService.ListarConsultasAsync(
                _idoso.Id);

        listaConsultas.ItemsSource = consultas;
    }

    private async void BtnCadastrarConsulta_Clicked(
        object sender,
        EventArgs e)
    {
        if (_idoso == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(CadastroConsultaPage),
            new Dictionary<string, object>
            {
            {
                "IdosoSelecionado",
                _idoso
            }
            });
    }
    private async void BtnEditarConsulta_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button botao)
            return;

        if (botao.BindingContext is not Consulta consulta)
            return;

        await Shell.Current.GoToAsync(
            nameof(EditarConsultaPage),
            new Dictionary<string, object>
            {
                {
                    "ConsultaSelecionada",
                    consulta
                }
            });
    }

    private async void BtnExcluirConsulta_Clicked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button botao)
            return;

        if (botao.BindingContext is not Consulta consulta)
            return;

        bool confirmar = await DisplayAlert(
            "Excluir consulta",
            "Deseja realmente excluir esta consulta?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        await _databaseService.InicializarBancoAsync();

        await _databaseService.ExcluirConsultaAsync(
            consulta);

        await CarregarConsultasAsync();
    }
}