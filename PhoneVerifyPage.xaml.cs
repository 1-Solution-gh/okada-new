using OG.Services;
using Microsoft.Maui.Controls;

namespace OG;

public partial class PhoneVerifyPage : ContentPage
{
    private readonly FirebaseAuthService _authService = new();
    private readonly string _sessionInfo;
    private readonly string _phoneNumber;

    public PhoneVerifyPage(string phoneNumber, string sessionInfo)
    {
        InitializeComponent();
        _phoneNumber = phoneNumber;
        _sessionInfo = sessionInfo;
        phoneLabel.Text = $"Code successfully sent to {_phoneNumber}";
    }

    async void OnVerifyCodeClicked(object sender, EventArgs e)
    {
        try
        {
            var result = await _authService.SignInWithPhoneAsync(_phoneNumber, codeEntry.Text, _sessionInfo);

            Preferences.Default.Set("idToken", result.IdToken);
            Preferences.Default.Set("userId", result.LocalId);
            Preferences.Default.Set("userPhone", result.PhoneNumber);

            await Navigation.PushAsync(new WhereToPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Verification Failed", ex.Message, "OK");
        }
    }
}
