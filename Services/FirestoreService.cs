using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace OG.Services;

public class FirestoreService
{
    private const string ProjectId = "okadago-82b33"; 
    private readonly HttpClient _http = new();

    public async Task AddRideRequestAsync(string idToken, string userId, string origin, string destination, double fare)
    {
        var uri = $"https://firestore.googleapis.com/v1/projects/{ProjectId}/databases/(default)/documents/rideRequests";

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", idToken);

        var payload = new
        {
            fields = new
            {
                userId = new { stringValue = userId },
                origin = new { stringValue = origin },
                destination = new { stringValue = destination },
                fare = new { doubleValue = fare },
                timestamp = new { timestampValue = DateTime.UtcNow.ToString("o") }
            }
        };

        var response = await _http.PostAsJsonAsync(uri, payload);
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception("Firestore write failed: " + error);
        }
    }

}
