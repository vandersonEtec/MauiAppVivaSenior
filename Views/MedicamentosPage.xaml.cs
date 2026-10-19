
using MauiAppVivaSenior.Models;
using MauiAppVivaSenior.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiAppVivaSenior.Views;

public partial class MedicamentosPage : ContentPage, IQueryAttributable, INotifyPropertyChanged
{
    private readonly DatabaseService _databaseService;

    private Idoso? _idoso;
    private string _tipoUsuario = "";
    private bool _exibirEditar;
    private bool _exibirExcluir;
    private bool _verificandoAcesso;
    private bool _acessoNegado;

    public bool ExibirEditar
    {
        get => _exibirEditar;
        set
        {
            _exibirEditar = value;
            OnPropertyChanged();
        }
    }

    public bool ExibirExcluir
    {
        get => _exibirExcluir;
        set
        {
            _exibirExcluir = value;
            OnPropertyChanged();
        }
    }

    public MedicamentosPage()
    {
        InitializeComponent();

        BindingContext = this;
        _databaseService = new DatabaseService();

        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        ConfigurarPermissoes();
    }

    public void ApplyQueryAttributes(
        IDictionary<string, object> query)
    {
        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        if (query.TryGetValue("IdosoSelecionado", out var valor))
        {
            _idoso = valor as Idoso;

            if (_idoso != null)
            {
                lblNomeIdoso.Text =
                    "Medicamentos de: " + _idoso.Nome;
            }
        }

        ConfigurarPermissoes();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (_verificandoAcesso || _acessoNegado)
            return;

        _tipoUsuario = Preferences.Get("UsuarioTipo", "");

        ConfigurarPermissoes();

        await VerificarAcessoAsync();
    }

    private void ConfigurarPermissoes()
    {
        bool podeAlterar =
            _tipoUsuario == "Administrador" ||
            _tipoUsuario == "Enfermeiro";

        BtnCadastrarMedicamento.IsVisible = podeAlterar;
        ExibirEditar = podeAlterar;
        ExibirExcluir = podeAlterar;
    }

    private async Task VerificarAcessoAsync()
    {
        if (_idoso == null)
        {
            _acessoNegado = true;

            await DisplayAlert(
                "Atenção",
                "Nenhum idoso foi selecionado.",
                "OK");

            await Shell.Current.GoToAsync("..");
            return;
        }

        _verificandoAcesso = true;

        try
        {
            await _databaseService.InicializarBancoAsync();

            if (_tipoUsuario == "Acompanhante")
            {
                int usuarioId = Preferences.Get("UsuarioId", 0);

                if (usuarioId <= 0)
                {
                    await NegarAcessoAsync();
                    return;
                }

                var idososPermitidos =
                    await _databaseService.ListarIdososPorUsuarioAsync(
                        usuarioId);

                bool temAcesso = idososPermitidos.Any(
                    i => i.Id == _idoso.Id);

                if (!temAcesso)
                {
                    await NegarAcessoAsync();
                    return;
                }
            }

            await CarregarMedicamentosAsync();
        }
        catch
        {
            _acessoNegado = true;

            await DisplayAlert(
                "Erro",
                "Não foi possível verificar o acesso aos medicamentos.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            _verificandoAcesso = false;
        }
    }

    private async Task NegarAcessoAsync()
    {
        _acessoNegado = true;

        await DisplayAlert(
            "Acesso negado",
            "Você não tem permissão para visualizar os medicamentos deste idoso.",
            "OK");

        await Shell.Current.GoToAsync("..");
    }

    private async Task CarregarMedicamentosAsync()
    {
        if (_idoso == null)
            return;

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
                medicamentos.Add(
                    new MedicamentoExibicao
                    {
                        Id = associacao.Id,
                        MedicamentoId = associacao.MedicamentoId,
                        Nome = medicamento.Nome,
                        ViaAdministracao = medicamento.ViaAdministracao,
                        Status = medicamento.Status,
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
        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante pode apenas visualizar os medicamentos.",
                "OK");

            return;
        }

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
        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante não pode alterar medicamentos.",
                "OK");

            return;
        }

        if (_idoso == null)
            return;

        if (sender is not Button botao ||
            botao.BindingContext is not MedicamentoExibicao medicamento)
            return;

        await _databaseService.InicializarBancoAsync();

        var associacoes =
            await _databaseService.ListarIdosoMedicamentosAsync(
                _idoso.Id);

        var idosoMedicamento = associacoes.FirstOrDefault(
            im => im.Id == medicamento.Id);

        if (idosoMedicamento == null)
            return;

        await Shell.Current.GoToAsync(
            nameof(EditarMedicamentoPage),
            new Dictionary<string, object>
            {
                { "IdosoMedicamento", idosoMedicamento },
                { "NomeMedicamento", medicamento.Nome }
            });
    }

    private async void BtnExcluirMedicamento_Clicked(
        object sender,
        EventArgs e)
    {
        if (_tipoUsuario == "Acompanhante")
        {
            await DisplayAlert(
                "Acesso",
                "O perfil Acompanhante pode apenas visualizar os medicamentos.",
                "OK");

            return;
        }

        if (_idoso == null)
            return;

        if (sender is not Button botao ||
            botao.BindingContext is not MedicamentoExibicao medicamento)
            return;

        bool confirmar = await DisplayAlert(
            "Desvincular medicamento",
            "Deseja desvincular este medicamento deste idoso?",
            "Sim",
            "Não");

        if (!confirmar)
            return;

        await _databaseService.InicializarBancoAsync();

        var associacoes =
            await _databaseService.ListarIdosoMedicamentosAsync(
                _idoso.Id);

        var associacao = associacoes.FirstOrDefault(
            im => im.Id == medicamento.Id);

        if (associacao == null)
            return;

        await _databaseService.ExcluirIdosoMedicamentoAsync(
            associacao);

        await CarregarMedicamentosAsync();
    }

    public class MedicamentoExibicao
    {
        public int Id { get; set; }

        public int MedicamentoId { get; set; }

        public string Nome { get; set; }

        public string ViaAdministracao { get; set; }

        public string Status { get; set; }

        public string Dosagem { get; set; }

        public string Horario { get; set; }

        public string Observacao { get; set; }
    }

    public new event PropertyChangedEventHandler? PropertyChanged;

    protected new void OnPropertyChanged(
        [CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(
            this,
            new PropertyChangedEventArgs(propertyName));
    }
}
