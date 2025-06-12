using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Maps;
using Okada.Services;
using Location = Microsoft.Maui.Devices.Sensors.Location;

namespace Okada.Views;

public partial class RiderDashboardPage : ContentPage
{
    private readonly SQLiteService _sqliteService = new(); 
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
        await _sqliteService.InitAsync();
        var users = await _sqliteService.GetUsersAsync();
        _phoneNumber = users.FirstOrDefault()?.PhoneNumber ?? string.Empty;
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


            Location driverPinLocation = new(_driverLocation.Latitude, _driverLocation.Longitude);
            Location userPinLocation = new(_userLocation.Latitude, _userLocation.Longitude);

            var driverPin = new Pin
            {
                Label = "Rider",
                Location = driverPinLocation,
                Address = "You",
                Type = PinType.SavedPin
            };

            var userPin = new Pin
            {
                Label = "Passenger",
                Location = userPinLocation,
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
        catch (Exception e) {
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
        var trips = await _sqliteService.GetTripsAsync(_phoneNumber);
        TripsStack.Children.Clear();
        foreach (var trip in trips)
        {
            TripsStack.Children.Add(new Label { Text = $"{trip.Date:dd MMM} - {trip.Destination} ({trip.Status})" });
        }
    }

    private async Task LoadEarningsAsync()
    {
        var earnings = await _sqliteService.GetEarningsAsync(_phoneNumber);
        EarningsStack.Children.Clear();
        foreach (var earn in earnings)
        {
            EarningsStack.Children.Add(new Label { Text = $"{earn.Date:dd MMM}: GHS {earn.Amount:GHS 0}" });
        }
    }

}

