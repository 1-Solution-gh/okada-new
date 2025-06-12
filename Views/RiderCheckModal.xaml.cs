

namespace Okada.Views;

public partial class RiderCheckModal : ContentPage
{
	private readonly string _phone;
	public RiderCheckModal(string phoneNumber)
	{
		InitializeComponent();
		_phone = phoneNumber;
	}

	private async void OnProceedClicked(object sender, EventArgs e)
	{
		if (YesRadio.IsChecked)
		{
			await Navigation.PushAsync(new RiderLicenceCapturePage(_phone));
		}
		else if (NoRadio.IsChecked)
		{
			await Navigation.PushAsync(new LoginPage());
		}
	}
}