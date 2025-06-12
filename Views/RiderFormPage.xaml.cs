using Okada.Services;
using Twilio.Types;

namespace Okada.Views;

public partial class RiderFormPage : ContentPage
{
    private readonly string _phone;
    private readonly SQLiteService _sqliteService;
    public RiderFormPage(string phoneNumber)
	{
		InitializeComponent();
        _phone = phoneNumber;
        _sqliteService = new SQLiteService();
    }
    private async void OnUploadClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Upload", "Upload Image placeholder.", "OK");
    }
    private async void OnSubmitClicked(object sender, EventArgs e)
    {
        string fullName = FullNameEntry.Text;
        string dob = DOBEntry.Text;
        string address = AddressEntry.Text;
        string licenseNumber = LicenseNumberEntry.Text;
        string licenseExpiry = LicenseExpiryEntry.Text;

        // TODO: Validate Fields

        // Storing riders info in the database
        //await _sqliteService.InitAsync();
        //var user = await _sqliteService.GetUserAsync(_phone);
        //if (user != null)
        //{
        //	user.RiderFullName = fullName;
        //	user.RiderDOB = dob;
        //	user.RiderAddress = address;
        //	user.RiderLicenseNumber = licenseNumber;
        //	user.RiderLicenseExpiry = licenseExpiry;
        //	await _sqliteService.AddUserAsync(user);
        //}
        await Navigation.PushAsync(new RiderDashboardPage());

    }
}