namespace MauiAppVivaSenior.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    // Esta classe contém as ações e comandos utilizados pela tela de login.

    private async void BtnEntrar_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(DashboardPage));
    }
}