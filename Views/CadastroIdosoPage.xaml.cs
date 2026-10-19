
using System;
using System.Collections.Generic;
using System.Linq;
using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior.Views;

public partial class CadastroIdosoPage : ContentPage, IQueryAttributable
{
    private Idoso? _idosoEmEdicao;

    private readonly DatabaseService _database;

    private string _tipoUsuario = "";

    public CadastroIdosoPage()
    {
        InitializeComponent();

        _database = new DatabaseService();

        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        if (_tipoUsuario == "Acompanhante")
        {
            BloquearAcesso();
        }
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        if (_tipoUsuario == "Acompanhante")
        {
            BloquearAcesso();
            return;
        }

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
                pckGenero.SelectedItem = idoso.Genero;
                txtContatoEmergencia.Text = idoso.ContatoEmergencia;
                txtObservacoes.Text = idoso.Observacoes;
            }
        }
    }

    private async void BloquearAcesso()
    {
        await DisplayAlert(
            "Acesso",
            "O perfil Acompanhante pode apenas visualizar as informações.",
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    private bool CPFValido(string? cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;

        cpf = new string(cpf.Where(char.IsDigit).ToArray());

        if (cpf.Length != 11)
            return false;

        if (cpf.Distinct().Count() == 1)
            return false;

        int[] pesosPrimeiroDigito =
            { 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        int soma = 0;

        for (int i = 0; i < 9; i++)
        {
            soma += (cpf[i] - '0') * pesosPrimeiroDigito[i];
        }

        int resto = soma % 11;

        int primeiroDigito =
            resto < 2 ? 0 : 11 - resto;

        if (cpf[9] - '0' != primeiroDigito)
            return false;

        int[] pesosSegundoDigito =
            { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        soma = 0;

        for (int i = 0; i < 10; i++)
        {
            soma += (cpf[i] - '0') * pesosSegundoDigito[i];
        }

        resto = soma % 11;

        int segundoDigito =
            resto < 2 ? 0 : 11 - resto;

        return cpf[10] - '0' == segundoDigito;
    }

    private int CalcularIdade(DateTime nascimento)
    {
        DateTime hoje = DateTime.Today;

        if (nascimento.Date > hoje)
            return -1;

        int idade = hoje.Year - nascimento.Year;

        if (nascimento.Date > hoje.AddYears(-idade))
            idade--;

        return idade;
    }

    private bool TelefoneValido(string? telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return false;

        bool caracteresValidos = telefone.All(c =>
            char.IsDigit(c) ||
            char.IsWhiteSpace(c) ||
            c == '+' ||
            c == '(' ||
            c == ')' ||
            c == '-');

        if (!caracteresValidos)
            return false;

        string numeros = new string(
            telefone.Where(char.IsDigit).ToArray());

        if (numeros.Length == 10 || numeros.Length == 11)
            return true;

        if (numeros.StartsWith("55") &&
            (numeros.Length == 12 || numeros.Length == 13))
            return true;

        return false;
    }

    private async void BtnSalvar_Clicked(
        object sender,
        EventArgs e)
    {
        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante não pode cadastrar ou alterar informações.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(txtNome.Text))
        {
            await DisplayAlert(
                "Campo obrigatório",
                "Digite o nome do idoso.",
                "OK");

            txtNome.Focus();
            return;
        }

        DateTime nascimento = dtpDataNascimento.Date ?? DateTime.Today;
        int idade = CalcularIdade(nascimento);

        if (idade <= 0)
        {
            await DisplayAlert(
                "Data de nascimento inválida",
                "Informe a  data de nascimento.",
                "OK");

            return;
        }

        if (!CPFValido(txtCPF.Text))
        {
            await DisplayAlert(
                "CPF inválido",
                "Digite um CPF válido para continuar.",
                "OK");

            txtCPF.Focus();
            return;
        }

        if (!TelefoneValido(txtTelefone.Text))
        {
            await DisplayAlert(
                "Telefone inválido",
                "Informe um telefone válido.",
                "OK");

            txtTelefone.Focus();
            return;
        }

        if (pckGenero.SelectedItem == null ||
            string.IsNullOrWhiteSpace(
                pckGenero.SelectedItem.ToString()))
        {
            await DisplayAlert(
                "Campo obrigatório",
                "Selecione o gênero do idoso.",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(txtContatoEmergencia.Text))
        {
            await DisplayAlert(
                "Campo obrigatório",
                "Informe o contato de emergência.",
                "OK");

            txtContatoEmergencia.Focus();
            return;
        }

        bool confirmar = await DisplayAlert(
            "Confirmar salvamento",
            _idosoEmEdicao == null
                ? "Deseja realmente salvar o cadastro deste idoso?"
                : "Deseja realmente salvar as alterações deste idoso?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        if (_idosoEmEdicao == null)
        {
            var novoIdoso = new Idoso
            {
                Nome = txtNome.Text.Trim(),
                DataNascimento = nascimento,
                CPF = new string(
                    txtCPF.Text!.Where(char.IsDigit).ToArray()),
                Telefone = txtTelefone.Text.Trim(),
                Endereco = txtEndereco.Text?.Trim(),
                Genero = pckGenero.SelectedItem?.ToString(),
                ContatoEmergencia = txtContatoEmergencia.Text.Trim(),
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
            _idosoEmEdicao.DataNascimento = nascimento;
            _idosoEmEdicao.CPF = new string(
                txtCPF.Text!.Where(char.IsDigit).ToArray());
            _idosoEmEdicao.Telefone = txtTelefone.Text.Trim();
            _idosoEmEdicao.Endereco = txtEndereco.Text?.Trim();
            _idosoEmEdicao.Genero = pckGenero.SelectedItem?.ToString();
            _idosoEmEdicao.ContatoEmergencia =
                txtContatoEmergencia.Text.Trim();
            _idosoEmEdicao.Observacoes = txtObservacoes.Text?.Trim();

            await _database.AtualizarIdosoAsync(_idosoEmEdicao);

            await DisplayAlert(
                "Sucesso",
                "Dados do idoso atualizados com sucesso.",
                "OK");
        }

        await Shell.Current.GoToAsync("..");
    }

    private async void BtnCancelar_Clicked(
        object sender,
        EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}
