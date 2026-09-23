using MauiAppVivaSenior.Models;

namespace MauiAppVivaSenior.Views;

public partial class DetalhesIdosoPage : ContentPage, IQueryAttributable
{
    private Idoso _idoso;

    public DetalhesIdosoPage()
    {
        InitializeComponent();
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        if (query.ContainsKey("IdosoSelecionado"))
        {
            _idoso = query["IdosoSelecionado"] as Idoso;

            if (_idoso != null)
            {
                lblNome.Text = _idoso.Nome;

                lblDataNascimento.Text =
                    _idoso.DataNascimento.ToString("dd/MM/yyyy");

                lblTelefone.Text = _idoso.Telefone;

                lblEndereco.Text = _idoso.Endereco;
            }
        }
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
                { "IdosoSelecionado", _idoso }
            });
    }
}