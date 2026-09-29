using MauiAppVivaSenior.Models;

namespace MauiAppVivaSenior.Views;

public partial class DetalhesIdosoPage : ContentPage, IQueryAttributable
{
    private Idoso? _idoso;

    public Idoso? IdosoSelecionado { get; set; }

    public DetalhesIdosoPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("IdosoSelecionado", out var valor))
        {
            _idoso = valor as Idoso;

            IdosoSelecionado = _idoso;

            MostrarDados();
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        MostrarDados();
    }

    private void MostrarDados()
    {
        if (_idoso == null)
            return;

        // Nome
        lblNome.Text = _idoso.Nome;

        // Idade calculada a partir da data de nascimento
        lblIdade.Text = $"{CalcularIdade(_idoso.DataNascimento)} anos";

        // Data de nascimento
        lblDataNascimento.Text =
            _idoso.DataNascimento.ToString("dd/MM/yyyy");

        // CPF
        lblCPF.Text = string.IsNullOrWhiteSpace(_idoso.CPF)
            ? "Não informado"
            : _idoso.CPF;

        // Telefone
        lblTelefone.Text = string.IsNullOrWhiteSpace(_idoso.Telefone)
            ? "Não informado"
            : _idoso.Telefone;

        // Endereço
        lblEndereco.Text = string.IsNullOrWhiteSpace(_idoso.Endereco)
            ? "Não informado"
            : _idoso.Endereco;

        // Observações
        lblObservacoes.Text = string.IsNullOrWhiteSpace(_idoso.Observacoes)
            ? "Nenhuma observação"
            : _idoso.Observacoes;
    }

    private int CalcularIdade(DateTime dataNascimento)
    {
        var hoje = DateTime.Today;

        int idade = hoje.Year - dataNascimento.Year;

        if (dataNascimento.Date > hoje.AddYears(-idade))
            idade--;

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
                {
                    "IdosoSelecionado",
                    _idoso
                }
            });
    }

    private async void BtnEditar_Clicked(
        object sender,
        EventArgs e)
    {
        if (_idoso == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(CadastroIdosoPage),
            new Dictionary<string, object>
            {
                {
                    "IdosoSelecionado",
                    _idoso
                }
            });
    }

    private async void BtnExcluir_Clicked(
        object sender,
        EventArgs e)
    {
        if (_idoso == null)
            return;

        bool confirmar = await DisplayAlert(
            "Excluir idoso",
            $"Deseja realmente excluir {_idoso.Nome}?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        var database =
            new MauiAppVivaSenior.Services.DatabaseService();

        await database.ExcluirIdosoAsync(_idoso);

        await DisplayAlert(
            "Sucesso",
            "Idoso excluído com sucesso.",
            "OK");

        await Shell.Current.GoToAsync("..");
    }
}