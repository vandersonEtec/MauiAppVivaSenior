using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

// Esta classe controla o cadastro de um novo idoso.

public partial class CadastroIdosoPage : ContentPage
{
    private readonly DatabaseService _databaseService;

    public CadastroIdosoPage()
    {
        InitializeComponent();

        _databaseService = new DatabaseService();
    }

    private async void BtnSalvar_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtNome.Text))
        {
            await DisplayAlert("Atenção", "Digite o nome do idoso.", "OK");
            return;
        }

        Idoso idoso = new Idoso
        {
            Nome = txtNome.Text,
            DataNascimento = dtpNascimento.Date ?? DateTime.Today,
            Telefone = txtTelefone.Text,
            Endereco = txtEndereco.Text
        };

        await _databaseService.InicializarBancoAsync();

        await _databaseService.SalvarIdosoAsync(idoso);

        await DisplayAlert("Sucesso", "Idoso cadastrado com sucesso!","OK");

        await Shell.Current.GoToAsync("..");
    }

    private async void BtnCancelar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}