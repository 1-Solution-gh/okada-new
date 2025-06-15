using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors; 
using Microsoft.Maui.Maps; 
using Okada.Services; 
using System.Net.Http; 
using System.Net.Http.Headers; 
using System.Text.Json;
using Location = Microsoft.Maui.Devices.Sensors.Location;

namespace Okada.Views;

public partial class RiderDashboardPage : ContentPage { 
    private readonly FirebaseAuthService _authService = new();
    private Location _driverLocation; 
    private Location _userLocation; 
    private string _firebaseToken = string.Empty; 
    private string _phoneNumber = string.Empty;

public RiderDashboardPage(string firebaseToken, string phoneNumber)
{
    InitializeComponent();
    _firebaseToken = firebaseToken;
    _phoneNumber = phoneNumber;
    _ = InitDriverMapAsync();
}

protected override async void OnAppearing()
{
    base.OnAppearing();
    await LoadTripsAsync();
}

private async Task InitDriverMapAsync()
{
    try
    {
        if (!await CheckPermissionAsync())
        {
            await DisplayAlert("Permission Denied", "Location Permission is required to use the map.", "OK");
            return;
        }
        _driverLocation = await Geolocation.GetLastKnownLocationAsync() ?? await Geolocation.GetLocationAsync();
        if (_driverLocation == null)
        {
            await DisplayAlert("Location Error", "Unable to get current location.", "OK");
            return;
        }

        _userLocation = new Location(_driverLocation.Latitude + 0.01, _driverLocation.Longitude + 0.01);

        var driverPin = new Pin
        {
            Label = "Rider",
            Location = new Microsoft.Maui.Devices.Sensors.Location(_driverLocation.Latitude, _driverLocation.Longitude),
            Address = "You",
            Type = PinType.SavedPin
        };

        var userPin = new Pin
        {
            Label = "Passenger",
            Location = new Microsoft.Maui.Devices.Sensors.Location(_userLocation.Latitude, _userLocation.Longitude),
            Address = "Customer",
            Type = PinType.Place
        };

        DriverMap.Pins.Clear();
        DriverMap.Pins.Add(driverPin);
        DriverMap.Pins.Add(userPin);

        var center = new Location(
            (_driverLocation.Latitude + _userLocation.Latitude) / 2,
            (_driverLocation.Longitude + _userLocation.Longitude) / 2);

        DriverMap.MoveToRegion(MapSpan.FromCenterAndRadius(center, Distance.FromKilometers(2)));
    }
    catch (Exception e)
    {
        await DisplayAlert("Map Error", $"Google play services may be missing or unavailable: {e.Message}", "OK");
    }
}

private async Task<bool> CheckPermissionAsync()
{
    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
    if (status != PermissionStatus.Granted)
    {
        status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
    }
    return status == PermissionStatus.Granted;
}

private async void OnTripsClicked(object sender, EventArgs e)
{
    TripsView.IsVisible = true;
    EarningsView.IsVisible = false;
    AccountView.IsVisible = false;
    await LoadTripsAsync();
}

private async void OnEarningsClicked(object sender, EventArgs e)
{
    TripsView.IsVisible = false;
    EarningsView.IsVisible = true;
    AccountView.IsVisible = false;
    await LoadEarningsAsync();
}

private void OnAccountClicked(object sender, EventArgs e)
{
    TripsView.IsVisible = false;
    EarningsView.IsVisible = false;
    AccountView.IsVisible = true;
}

private async Task LoadTripsAsync()
{
    try
    {
        using var httpClient = new HttpClient();
        httpClient.BaseAddress = new Uri(Constants.ApiBaseUrl);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _firebaseToken);

        var response = await httpClient.GetAsync($"index.php?endpoint=get_trips&user_id={_phoneNumber}");
        if (!response.IsSuccessStatusCode)
        {
            await DisplayAlert("Error", "Failed to fetch trips", "OK");
            return;
        }

        var json = await response.Content.ReadAsStringAsync();
        var trips = JsonSerializer.Deserialize<List<Trip>>(json);

        TripsStack.Children.Clear();
        foreach (var trip in trips)
        {
            TripsStack.Children.Add(new Label { Text = $"{trip.created_at:dd MMM} - {trip.destination_address} ({trip.status})" });
        }
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", ex.Message, "OK");
    }
}

private async Task LoadEarningsAsync()
{
    
    EarningsStack.Children.Clear();
    EarningsStack.Children.Add(new Label { Text = "Earnings view coming soon..." });
}

public class Trip
{
    public string destination_address { get; set; }
    public string status { get; set; }
    public DateTime created_at { get; set; }
}

}
