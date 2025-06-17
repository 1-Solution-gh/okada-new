using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace OG.Services;

public class FirebaseAuthResult { 
    public string IdToken { get; set; }
    public string LocalId { get; set; }
    public string Email { get; set; } 
}

public class FirebaseAuthService
{
    private const string ApiKey = "AIzaSyCevj-9BnTPWwHPOF1mn9W077gYQNUUofo";
    private readonly HttpClient _http = new();

    public async Task<FirebaseAuthResult> SignInWithEmailPasswordAsync(string email, string password)
    {
        var endpoint = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}";

        var payload = new
        {
            email = email,
            password = password,
            returnSecureToken = true
        };

        var response = await _http.PostAsJsonAsync(endpoint, payload);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(JsonDocument.Parse(content).RootElement.GetProperty("error").GetProperty("message").GetString());

        var doc = JsonDocument.Parse(content).RootElement;
        return new FirebaseAuthResult
        {
            IdToken = doc.GetProperty("idToken").GetString(),
            LocalId = doc.GetProperty("localId").GetString(),
            Email = doc.GetProperty("email").GetString()
        };
    }

    public async Task<FirebaseAuthResult> RegisterWithEmailPasswordAsync(string email, string password)
    {
        var endpoint = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={ApiKey}";

        var payload = new
        {
            email = email,
            password = password,
            returnSecureToken = true
        };

        var response = await _http.PostAsJsonAsync(endpoint, payload);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(JsonDocument.Parse(content).RootElement.GetProperty("error").GetProperty("message").GetString());

        var doc = JsonDocument.Parse(content).RootElement;
        return new FirebaseAuthResult
        {
            IdToken = doc.GetProperty("idToken").GetString(),
            LocalId = doc.GetProperty("localId").GetString(),
            Email = doc.GetProperty("email").GetString()
        };
    }

}

