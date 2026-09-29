using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class CadastroIdosoPage : ContentPage, IQueryAttributable
{
    private Idoso? _idosoEmEdicao;

    private readonly DatabaseService _database;

    public CadastroIdosoPage()
    {
        InitializeComponent();

        _database = new DatabaseService();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("IdosoSelecionado", out var valor))
        {
            var idoso = valor as Idoso;

            if (idoso != null)
            {
                _idosoEmEdicao = idoso;

                txtNome.Text = idoso.Nome;
                dtpDataNascimento.Date = idoso.DataNascimento;
                txtCPF.Text = idoso.CPF;
                txtTelefone.Text = idoso.Telefone;
                txtEndereco.Text = idoso.Endereco;
                txtObservacoes.Text = idoso.Observacoes;
            }
        }
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNome.Text))
        {
            await DisplayAlert(
                "Atenção",
                "Digite o nome do idoso.",
                "OK");

            return;
        }

        if (_idosoEmEdicao == null)
        {
            var novoIdoso = new Idoso
            {
                Nome = txtNome.Text.Trim(),
                DataNascimento = dtpDataNascimento.Date ?? DateTime.Today,
                CPF = txtCPF.Text?.Trim(),
                Telefone = txtTelefone.Text?.Trim(),
                Endereco = txtEndereco.Text?.Trim(),
                Observacoes = txtObservacoes.Text?.Trim()
            };

            await _database.SalvarIdosoAsync(novoIdoso);

            await DisplayAlert(
                "Sucesso",
                "Idoso cadastrado com sucesso.",
                "OK");
        }
        else
        {
            _idosoEmEdicao.Nome = txtNome.Text.Trim();
            _idosoEmEdicao.DataNascimento =
                dtpDataNascimento.Date ?? DateTime.Today;
            _idosoEmEdicao.CPF = txtCPF.Text?.Trim();
            _idosoEmEdicao.Telefone = txtTelefone.Text?.Trim();
            _idosoEmEdicao.Endereco = txtEndereco.Text?.Trim();
            _idosoEmEdicao.Observacoes = txtObservacoes.Text?.Trim();

            await _database.AtualizarIdosoAsync(_idosoEmEdicao);

            await DisplayAlert(
                "Sucesso",
                "Dados do idoso atualizados com sucesso.",
                "OK");
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}