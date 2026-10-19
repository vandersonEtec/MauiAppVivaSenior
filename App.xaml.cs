using MauiAppVivaSenior.Services;

namespace MauiAppVivaSenior
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            InicializarBanco();
        }

        private async void InicializarBanco()
        {
            var database = new DatabaseService();

            await database.InicializarBancoAsync();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}