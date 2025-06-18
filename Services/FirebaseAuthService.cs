using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace OG.Services;

public class FirebaseAuthResult 
{ 
    public string IdToken { get; set; } 
    public string LocalId { get; set; } 
    public string PhoneNumber { get; set; } 
}

public class FirebaseAuthService
{
    private const string ApiKey = "AIzaSyCevj-9BnTPWwHPOF1mn9W077gYQNUUofo"; 
    private readonly HttpClient _http = new();

    public async Task<FirebaseAuthResult> SignInWithPhoneAsync(string phoneNumber, string verificationCode, string sessionInfo)
    {
        var endpoint = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPhoneNumber?key={ApiKey}";

        var payload = new
        {
            code = verificationCode,
            sessionInfo = sessionInfo
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
            PhoneNumber = doc.GetProperty("phoneNumber").GetString()
        };
    }

    public async Task<string> SendPhoneVerificationCodeAsync(string phoneNumber, string recaptchaToken)
    {
        var endpoint = $"https://identitytoolkit.googleapis.com/v1/accounts:sendVerificationCode?key={ApiKey}";

        var payload = new
        {
            phoneNumber = phoneNumber,
            recaptchaToken = recaptchaToken
        };

        var response = await _http.PostAsJsonAsync(endpoint, payload);
        var content = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception(JsonDocument.Parse(content).RootElement.GetProperty("error").GetProperty("message").GetString());

        return JsonDocument.Parse(content).RootElement.GetProperty("sessionInfo").GetString();
    }

}

