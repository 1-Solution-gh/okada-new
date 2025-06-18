using OG.Services;
using Microsoft.Maui.Controls;
using OG;

namespace OG;

public partial class PhoneLoginPage : ContentPage
{
    private readonly FirebaseAuthService _authService = new();

    public PhoneLoginPage()
    {
        InitializeComponent();
    }

    async void OnSendCodeClicked(object sender, EventArgs e)
    {
        try
        {
            sendCodeButton.IsEnabled = false;

            if (string.IsNullOrWhiteSpace(phoneEntry.Text))
                throw new Exception("Please enter your phone number.");

#if ANDROID
        MainActivity.TriggerPlayIntegrity(Android.App.Application.Context);
        var token = await MainActivity.IntegrityTokenSource.Task;
#else
            var token = "unsupported-platform";
#endif
            var sessionInfo = await _authService.SendPhoneVerificationCodeAsync(phoneEntry.Text, token);

            await Navigation.PushAsync(new PhoneVerifyPage(phoneEntry.Text, sessionInfo));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            sendCodeButton.IsEnabled = true;
        }
    }

}

