using Okada.Services;
using Okada.Models;
using Okada.Views;

namespace Okada.Views;

public partial class VerificationPage : ContentPage
{
    private readonly AuthService _authService; 
    private readonly SQLiteService _sqliteService;

    public VerificationPage()
    {
        InitializeComponent();
        _authService = new AuthService();
        _sqliteService = new SQLiteService();
    }

    private async void OnVerifyClicked(object sender, EventArgs e)
    {
        var phoneNumber = (BindingContext as dynamic)?.PhoneNumber;
        await Navigation.PushAsync(new RiderCheckModal(phoneNumber));
        //var inputCode = CodeEntry.Text;
        //var token = await _authService.ValidateCodeAsync(phoneNumber, inputCode);

        //if (token != null)
        //{
        //    await _sqliteService.InitAsync();
        //    var user = await _sqliteService.GetUserAsync(phoneNumber);
        //    if (user != null)
        //    {
        //        user.JwtToken = token;
        //        await _sqliteService.AddUserAsync(user);
        //    }

        //    await Navigation.PushAsync(new RiderCheckModal(phoneNumber));
        //}
        //else
        //{
        //    await DisplayAlert("Invalid", "Code incorrect", "OK");
        //}
    }

}
