using MauiAppVivaSenior.Views;

namespace MauiAppVivaSenior;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(DashboardPage), typeof(DashboardPage));
        Routing.RegisterRoute(nameof(IdososPage), typeof(IdososPage));
        Routing.RegisterRoute(nameof(MedicamentosPage), typeof(MedicamentosPage));
        Routing.RegisterRoute(nameof(ConsultasPage), typeof(ConsultasPage));
        Routing.RegisterRoute(nameof(ObservacoesPage), typeof(ObservacoesPage));
        Routing.RegisterRoute(nameof(CadastroIdosoPage), typeof(CadastroIdosoPage));
        Routing.RegisterRoute(nameof(DetalhesIdosoPage), typeof(DetalhesIdosoPage));
        Routing.RegisterRoute(nameof(CadastroMedicamentoPage),typeof(CadastroMedicamentoPage));
    }
}