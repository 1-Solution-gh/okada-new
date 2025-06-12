using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.Devices.Sensors;
using Okada.Models;
using Okada.Services;

namespace Okada.Views;

public partial class RideMapPage : ContentPage
{
    private Location _startLocation;
    private Location _endLocation; 
    private string _destination; 
    private readonly SQLiteService _sqliteService = new(); 
    private string _phoneNumber = string.Empty; 
    private double _fare;

    public RideMapPage(string currentLocation, string destination)
    {
        InitializeComponent();
        _destination = destination;
        var parts = currentLocation.Split(',');
        _startLocation = new Location(double.Parse(parts[0]), double.Parse(parts[1]));
        _ = LoadRouteAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _sqliteService.InitAsync();
        var user = (await _sqliteService.GetUsersAsync()).FirstOrDefault();
        if (user != null)
        {
            _phoneNumber = user.PhoneNumber;
        }
    }

    private async Task LoadRouteAsync()
    {
        var results = await Geocoding.GetLocationsAsync(_destination);
        _endLocation = results?.FirstOrDefault();

        if (_endLocation == null)
        {
            await DisplayAlert("Error", "Destination not found", "OK");
            return;
        }

        MainMap.MoveToRegion(MapSpan.FromCenterAndRadius(_startLocation, Distance.FromKilometers(2)));

        MainMap.Pins.Add(new Pin { Label = "Start", Location = _startLocation });
        MainMap.Pins.Add(new Pin { Label = _destination, Location = _endLocation });

        _fare = CalculateFare(_startLocation, _endLocation);
        FareLabel.Text = $"Estimated Fare: GHS {_fare:F0}";
    }

    private double CalculateFare(Location from, Location to)
    {
        var distance = Location.CalculateDistance(from, to, DistanceUnits.Kilometers);
        return Math.Round(1000 + distance * 300); // base fare + per km rate
    }

    private async void OnRequestRideClicked(object sender, EventArgs e)
    {
        var trip = new Trip
        {
            PhoneNumber = _phoneNumber,
            Destination = _destination,
            Status = "Requested",
            Date = DateTime.Now
        };

        var earning = new Earning
        {
            PhoneNumber = _phoneNumber,
            Amount = (decimal)_fare,
            Date = DateTime.Now
        };

        await _sqliteService.AddTripAsync(trip);
        await _sqliteService.AddEarningAsync(earning);

        await ShowConfirmationModalAsync();
    }

    private async Task ShowConfirmationModalAsync()
    {
        string message = $"Ride successfully requested to {_destination}\n" +
                         $"Fare: GHS {_fare:F0}\n" +
                         $"Date: {DateTime.Now:dd MMM yyyy, hh:mm tt}";

        bool track = await DisplayAlert("Ride Confirmed", message, "Track Ride", "Cancel Ride");

        if (track)
        {
            await Navigation.PushAsync(new RideTrackingPage());
        }
        else
        {
            await DisplayAlert("Ride Cancelled", "Your ride has been cancelled.", "OK");
        }
    }

}

