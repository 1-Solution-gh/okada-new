using Okada.Views;
using Okada.Services;
using Okada.Models;

namespace Okada.Views;

public partial class RegisterPage : ContentPage
{
    private readonly AuthService _authService; 
    private readonly SQLiteService _sqliteService;

    public RegisterPage()
    {
        InitializeComponent();
        _authService = new AuthService();
        _sqliteService = new SQLiteService();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        string phoneNumber = PhoneEntry.Text;
        var success = await _authService.SendVerificationCodeAsync(phoneNumber);

        if (success)
        {
            await _sqliteService.InitAsync();
            await _sqliteService.AddUserAsync(new User { PhoneNumber = phoneNumber });

            await Navigation.PushAsync(new VerificationPage { BindingContext = new { PhoneNumber = phoneNumber } });
        }
        else
        {
            await DisplayAlert("Error", "Failed to send code", "OK");
        }
    }

}
