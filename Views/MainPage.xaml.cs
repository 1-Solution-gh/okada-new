using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace Okada.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

	private async void OnPageAppearing(object sender, EventArgs e)
	{
		var location = await Geolocation.GetLastKnownLocationAsync() ?? await Geolocation.GetLocationAsync();
		if(location != null )
		{
			var userPosition = new Location(location.Latitude, location.Longitude);
			MainMap.MoveToRegion(MapSpan.FromCenterAndRadius(userPosition, Distance.FromKilometers(1)));
			MainMap.Pins.Add(new Pin { Label = "You are here", Location = userPosition });
		}
	}
	private async void WhereToEntry_Focused(object sender, FocusEventArgs e)
	{
		await Navigation.PushAsync(new LocationSearchPage());
	}

	private void SetActiveTab(Button activeButton)
	{
		var buttons = new[] { HomeTabButton, RidesTabButton, AccountTabButton };
		foreach(var btn in buttons)
		{
			btn.Style = (Style)Resources[btn == activeButton ? "TabButtonActiveStyle" : "TabButtonStyle"];
		}
	} 

	private async void OnHomeClicked(object sender, EventArgs e)
	{
		SetActiveTab(HomeTabButton);
		await DisplayAlert("Nav", "Home clicked", "OK");
	}

	private async void OnRidesClicked(object sender, EventArgs e)
	{
		SetActiveTab(RidesTabButton);
		await DisplayAlert("Nav", "Rides clicked", "OK");
	}

	private async void OnAccountClicked(object sender, EventArgs e)
	{
		SetActiveTab(AccountTabButton);
		await DisplayAlert("Nav", "Account clicked", "OK");
	}
}