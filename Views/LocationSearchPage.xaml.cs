using Microsoft.Maui.Devices.Sensors;

namespace Okada.Views;

public partial class LocationSearchPage : ContentPage
{
    public string CurrentLocation { get; set; }
    public string Destination { get; set; }

    public LocationSearchPage()
    {
        InitializeComponent();
        _ = SetCurrentLocationAsync();
    }

    private async Task SetCurrentLocationAsync()
    {
        var location = await Geolocation.GetLastKnownLocationAsync() ?? await Geolocation.GetLocationAsync();
        if (location != null)
            CurrentLocationEntry.Text = $"{location.Latitude},{location.Longitude}";
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        var currentLocation = CurrentLocationEntry.Text;
        var destination = DestinationEntry.Text;
        await Navigation.PushAsync(new RideMapPage(currentLocation, destination));
    }

}