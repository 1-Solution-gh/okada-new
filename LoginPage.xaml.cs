using OG.Services;
using Microsoft.Maui.Controls;

namespace OG;

public partial class LoginPage : ContentPage
{
    private readonly FirebaseAuthService _authService = new();

    public LoginPage()
    {
        InitializeComponent();
    }

    async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await _authService.SignInWithEmailPasswordAsync(emailEntry.Text, passwordEntry.Text);

            Preferences.Default.Set("idToken", result.IdToken);
            Preferences.Default.Set("userId", result.LocalId);
            Preferences.Default.Set("userEmail", result.Email);

            await Navigation.PushAsync(new MainPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Login Failed", ex.Message, "OK");
        }
    }

}

