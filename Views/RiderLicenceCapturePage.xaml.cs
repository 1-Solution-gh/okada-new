using Okada.Services;
using System.Globalization;

namespace Okada.Views;

public partial class RiderLicenceCapturePage : ContentPage
{
	private readonly string _phone;
	private readonly SQLiteService _sqliteService; 
	public RiderLicenceCapturePage(string phoneNumber)
	{
		InitializeComponent();
		_phone = phoneNumber;
		_sqliteService = new SQLiteService();
	}

	private async void OnCaptureClicked(object sender, EventArgs e)
	{
		await DisplayAlert("Camera", "Camera capture placeholder.", "OK");
	}

	private async void OnFormClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new RiderFormPage(_phone));
	}
}