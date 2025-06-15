using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Okada.Models;
using Okada.Services;
using System.Net.Http.Headers;
using System.Text.Json;
using Location = Microsoft.Maui.Devices.Sensors.Location;

namespace Okada.Views;

public partial class RiderDashboardPage : ContentPage
{
    private readonly HttpClient _httpClient = new();
    private string _firebaseToken = string.Empty;
    private string _phoneNumber = string.Empty;
    private Location _driverLocation;
    private Location _userLocation;

    public RiderDashboardPage()
    {
        InitializeComponent();
        _ = InitDriverMapAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load token and phone from secure storage
        _firebaseToken = await SecureStorage.GetAsync("firebase_token") ?? "";
        _phoneNumber = await SecureStorage.GetAsync("phone_number") ?? "";

        if (string.IsNullOrWhiteSpace(_firebaseToken))
        {
            await DisplayAlert("Auth Error", "User not logged in", "OK");
            return;
        }

        _httpClient.BaseAddress = new Uri(Constants.ApiBaseUrl);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _firebaseToken);

        await LoadTripsAsync();
    }

    private async Task InitDriverMapAsync()
    {
        try
        {
            if (!await CheckPermissionAsync())
            {
                await DisplayAlert("Permission Denied", "Location permission is required to use the map.", "OK");
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
            await DisplayAlert("Map Error", $"Google Play Services error: {e.Message}", "OK");
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

    private void OnEarningsClicked(object sender, EventArgs e)
    {
        TripsView.IsVisible = false;
        EarningsView.IsVisible = true;
        AccountView.IsVisible = false;
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
        var response = await _httpClient.GetAsync($"api.php?endpoint=get_trips&user_id={_phoneNumber}");
        if (!response.IsSuccessStatusCode)
        {
            await DisplayAlert("Error", "Failed to load trips from server.", "OK");
            return;
        }

        var json = await response.Content.ReadAsStringAsync();
        var trips = JsonSerializer.Deserialize<List<Trip>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        TripsStack.Children.Clear();
        foreach (var trip in trips ?? new List<Trip>())
        {
            TripsStack.Children.Add(new Label
            {
                Text = $"{trip.Date:dd MMM} - {trip.Destination} ({trip.Status})",
                FontSize = 14
            });
        }
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"Could not load trips: {ex.Message}", "OK");
    }
}
}
