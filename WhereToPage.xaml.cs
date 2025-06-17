using Newtonsoft.Json.Linq;
using System.Net.Http;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace OG;

public partial class WhereToPage : ContentPage
{
    const string GooglePlacesApiKey = "AIzaSyD4B61j7QSSVpHLHdJMPjddOp9dwRB8QaU"; 
    Location originLocation; 
    Location destinationLocation;

    public WhereToPage()
    {
        InitializeComponent();
        LoadCurrentLocationAsync();
        destinationEntry.TextChanged += async (s, e) => await UpdateSuggestionsAsync(destinationEntry, destinationList, e.NewTextValue);
    }

    async void LoadCurrentLocationAsync()
    {
        var status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted) return;

        var location = await Geolocation.GetLastKnownLocationAsync() ?? await Geolocation.GetLocationAsync();
        if (location != null)
        {
            originLocation = location;
            originEntry.Text = await ReverseGeocodeAsync(location);
        }
    }

    async Task<string> ReverseGeocodeAsync(Location location)
    {
        var url = $"https://maps.googleapis.com/maps/api/geocode/json?latlng={location.Latitude},{location.Longitude}&key={GooglePlacesApiKey}";
        using var client = new HttpClient();
        var response = await client.GetStringAsync(url);
        var results = JObject.Parse(response)["results"];
        return results?.FirstOrDefault()?["formatted_address"]?.ToString() ?? "Unknown location";
    }

    async Task UpdateSuggestionsAsync(Entry input, ListView suggestionList, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            suggestionList.ItemsSource = null;
            return;
        }

        var url = $"https://maps.googleapis.com/maps/api/place/autocomplete/json?input={query}&key={GooglePlacesApiKey}";
        using var client = new HttpClient();
        var json = await client.GetStringAsync(url);
        var predictions = JObject.Parse(json)["predictions"];

        var descriptions = predictions.Select(p => p["description"].ToString()).ToList();
        suggestionList.ItemsSource = descriptions;
        suggestionList.IsVisible = true;

        suggestionList.ItemSelected += async (sender, args) =>
        {
            if (args.SelectedItem is string selected)
            {
                input.Text = selected;
                suggestionList.IsVisible = false;

                var placeId = predictions.FirstOrDefault(p => p["description"]?.ToString() == selected)?["place_id"]?.ToString();
                if (!string.IsNullOrWhiteSpace(placeId))
                {
                    await ResolvePlaceAndShowFareAsync(placeId);
                }
            }
        };
    }

    async Task ResolvePlaceAndShowFareAsync(string placeId)
    {
        var url = $"https://maps.googleapis.com/maps/api/place/details/json?placeid={placeId}&key={GooglePlacesApiKey}";
        using var client = new HttpClient();
        var json = await client.GetStringAsync(url);
        var result = JObject.Parse(json)["result"];

        var lat = (double)result["geometry"]["location"]["lat"];
        var lng = (double)result["geometry"]["location"]["lng"];
        destinationLocation = new Location(lat, lng);

        if (originLocation != null && destinationLocation != null)
        {
            var distance = Location.CalculateDistance(originLocation, destinationLocation, DistanceUnits.Kilometers);
            var fare = CalculateFare(distance);

            fareLabel.Text = $"${fare:0.00}";
            bottomSheet.IsVisible = true;
            await bottomSheet.TranslateTo(0, 0, 300, Easing.SinIn);
        }
    }

    double CalculateFare(double distanceKm)
    {
        const double baseFare = 2.50;
        const double perKmRate = 1.75;
        return baseFare + (distanceKm * perKmRate);
    }

    //void OnContinueClicked(object sender, EventArgs e)
    //{
    //    // Placeholder: you can navigate or confirm ride here
    //}

    async void OnRequestRideClicked(object sender, EventArgs e)
    {
        try
        {
            string idToken = Preferences.Default.Get("idToken", string.Empty);
            string userId = Preferences.Default.Get("userId", string.Empty);
            if (string.IsNullOrEmpty(idToken) || string.IsNullOrEmpty(userId))
            {
                await DisplayAlert("Error", "You must be logged in to request a ride.", "OK");
                return;
            }

            var origin = originEntry.Text;
            var destination = destinationEntry.Text;
            var fare = double.Parse(fareLabel.Text.Replace("$", ""));

            var firestore = new Services.FirestoreService();
            await firestore.AddRideRequestAsync(idToken, userId, origin, destination, fare);

            await DisplayAlert("Success", "Ride request submitted to Firestore.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

}

