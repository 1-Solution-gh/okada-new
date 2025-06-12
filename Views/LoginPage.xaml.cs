using Okada.Services;
using Okada.Views;

namespace Okada.Views;

public partial class LoginPage : ContentPage
{
    private readonly SQLiteService _sqliteService;

    public LoginPage()
    {
        InitializeComponent();
        _sqliteService = new SQLiteService();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        var phoneNumber = PhoneEntry.Text;
        await Navigation.PushAsync(new MainPage());
        //await _sqliteService.InitAsync();
        //var user = await _sqliteService.GetUserAsync(phoneNumber);

        //if (user != null && !string.IsNullOrEmpty(user.JwtToken))
        //{
        //    if (!string.IsNullOrEmpty(user.RiderLicenseNumber))
        //    {
        //        await Navigation.PushAsync(new RiderDashboardPage());
        //    }
        //    else
        //    {
        //        await Navigation.PushAsync(new MainPage());
        //    }
        //}
        //else
        //{
        //    await DisplayAlert("Login Failed", "User not verified or token missing.", "OK");
        //}
    }

}