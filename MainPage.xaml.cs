using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Microsoft.Maui.ApplicationModel;
using System.Net.Http.Json;
using Newtonsoft.Json.Linq;

namespace OG;

public partial class MainPage : ContentPage
{
    Location userLocation; 
    const string googlePlacesApiKey = "AIzaSyD4B61j7QSSVpHLHdJMPjddOp9dwRB8QaU"; 
    bool isTracking = false;

    public MainPage()
    {
        InitializeComponent();
        InitializeMapAsync();
    }

    async void OnWhereToClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new WhereToPage());

    }

    async Task InitializeMapAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
            return;

        var location = await Geolocation.GetLastKnownLocationAsync() ?? await Geolocation.GetLocationAsync();
        if (location == null)
            return;

        userLocation = location;
        map.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(location.Latitude, location.Longitude), Distance.FromKilometers(1)));

        map.Pins.Clear();
        map.Pins.Add(new Pin
        {
            Label = "You are here",
            Location = new Location(location.Latitude, location.Longitude)
        });

        await AddNearbyPlacesPinsAsync(location);
        StartTrackingUserLocation();
    }

    async Task AddNearbyPlacesPinsAsync(Location location)
    {
        var url = $"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={location.Latitude},{location.Longitude}&radius=1500&type=restaurant&key={googlePlacesApiKey}";

        using var http = new HttpClient();
        var response = await http.GetStringAsync(url);
        var places = JObject.Parse(response)["results"];

        foreach (var place in places)
        {
            var name = place["name"].ToString();
            var lat = (double)place["geometry"]["location"]["lat"];
            var lng = (double)place["geometry"]["location"]["lng"];

            map.Pins.Add(new Pin
            {
                Label = name,
                Location = new Location(lat, lng)
            });
        }
    }

    void StartTrackingUserLocation()
    {
        if (isTracking) return;
        isTracking = true;

        Task.Run(async () =>
        {
            while (isTracking)
            {
                await CheckUserLocationAsync();
                await Task.Delay(TimeSpan.FromSeconds(10));
            }
        });
    }

    async Task CheckUserLocationAsync()
    {
        try
        {
            var newLocation = await Geolocation.GetLocationAsync();
            if (newLocation == null || userLocation == null) return;

            var distance = Location.CalculateDistance(userLocation, newLocation, DistanceUnits.Kilometers);
            if (distance > 0.01) // ~10 meters
            {
                userLocation = newLocation;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Location(userLocation.Latitude, userLocation.Longitude), Distance.FromKilometers(1)));
                });
            }
        }
        catch { }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        isTracking = false;
    }

}