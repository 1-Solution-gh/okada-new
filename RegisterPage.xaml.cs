using OG.Services;

namespace OG;

public partial class RegisterPage : ContentPage
{
    private readonly FirebaseAuthService _authService = new();
    public RegisterPage()
	{
		InitializeComponent();
	}
    async void OnRegisterClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await _authService.RegisterWithEmailPasswordAsync(emailEntry.Text, passwordEntry.Text);

            Preferences.Default.Set("idToken", result.IdToken);
            Preferences.Default.Set("userId", result.LocalId);
            Preferences.Default.Set("userEmail", result.Email);

            await Navigation.PushAsync(new LoginPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Registration Failed", ex.Message, "OK");
        }
    }
}