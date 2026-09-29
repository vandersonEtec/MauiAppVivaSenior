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

        var associacoes =
            await _databaseService.ListarIdosoMedicamentosAsync(
                _idoso.Id);

        var medicamentos = new List<MedicamentoExibicao>();

        foreach (var associacao in associacoes)
        {
            var medicamento =
                await _databaseService.BuscarMedicamentoPorIdAsync(
                    associacao.MedicamentoId);

            if (medicamento != null)
            {
                medicamentos.Add(new MedicamentoExibicao
                {
                    Id = associacao.Id,
                    MedicamentoId = associacao.MedicamentoId,
                    Nome = medicamento.Nome,
                    Dosagem = associacao.Dosagem,
                    Horario = associacao.Horario,
                    Observacao = associacao.Observacao
                });
            }
        }

        listaMedicamentos.ItemsSource = medicamentos;
    }

    private async void BtnCadastrarMedicamento_Clicked(
    object sender,
    EventArgs e)
    {
        if (_idoso == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(CatalogoMedicamentosPage),
            new Dictionary<string, object>
            {
            { "IdosoSelecionado", _idoso }
            });
    }

    private async void BtnEditarMedicamento_Clicked(
    object sender,
    EventArgs e)
    {
        var botao = sender as Button;

        if (botao == null)
            return;

        var medicamento =
            botao.BindingContext as MedicamentoExibicao;

        if (medicamento == null)
            return;

        await _databaseService.InicializarBancoAsync();

        var associacao =
            await _databaseService
                .ListarIdosoMedicamentosAsync(_idoso.Id);

        var idosoMedicamento =
            associacao.FirstOrDefault(
                im => im.Id == medicamento.Id);

        if (idosoMedicamento == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(EditarMedicamentoPage),
            new Dictionary<string, object>
            {
            {
                "IdosoMedicamento",
                idosoMedicamento
            },
            {
                "NomeMedicamento",
                medicamento.Nome
            }
            });
    }
    public class MedicamentoExibicao
    {
        public int Id { get; set; }

        public int MedicamentoId { get; set; }

        public string Nome { get; set; }

        public string Dosagem { get; set; }

        public string Horario { get; set; }

        public string Observacao { get; set; }
    }
    private async void BtnExcluirMedicamento_Clicked(
    object sender,
    EventArgs e)
    {
        var botao = sender as Button;

        if (botao == null)
            return;

        var medicamento =
            botao.BindingContext as MedicamentoExibicao;

        if (medicamento == null)
            return;

        bool confirmar = await DisplayAlert(
            "Excluir medicamento",
            "Deseja excluir este medicamento deste idoso?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        await _databaseService.InicializarBancoAsync();

        var medicamentoBanco =
            await _databaseService.BuscarMedicamentoPorNomeAsync(
                medicamento.Nome);

        if (medicamentoBanco == null)
            return;

        var associacao =
            await _databaseService.BuscarIdosoMedicamentoAsync(
                _idoso.Id,
                medicamentoBanco.Id);

        if (associacao == null)
            return;

        await _databaseService.ExcluirIdosoMedicamentoAsync(
            associacao);

        await CarregarMedicamentosAsync();
    }
}