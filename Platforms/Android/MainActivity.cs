using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Tasks;
using Android.OS;
using Android.Runtime;
using Google.Android.Gms.Tasks;
using Google.Android.Play.Core.Integrity;
using Microsoft.Maui;
using Microsoft.Maui.Hosting;
using System;
using System.Threading.Tasks;

namespace OG;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
public class MainActivity : MauiAppCompatActivity
{
    public static TaskCompletionSource IntegrityTokenSource;

    public static void TriggerPlayIntegrity(Context context)
    {
        IntegrityTokenSource = new TaskCompletionSource<string>();

        var integrityManager = IntegrityManagerFactory.Create(context);

        var request = new IntegrityTokenRequest.Builder()
            .SetCloudProjectNumber("YOUR_GOOGLE_CLOUD_PROJECT_NUMBER")
            .Build();

        integrityManager.RequestIntegrityToken(request)
            .AddOnSuccessListener(new SuccessListener())
            .AddOnFailureListener(new FailureListener());
    }

    class SuccessListener : Java.Lang.Object, IOnSuccessListener
    {
        public void OnSuccess(Java.Lang.Object result)
        {
            if (result is IntegrityTokenResponse response && !string.IsNullOrWhiteSpace(response.Token))
            {
                IntegrityTokenSource?.TrySetResult(response.Token);
            }
            else
            {
                IntegrityTokenSource?.TrySetException(new Exception("Integrity token was empty."));
            }
        }
    }

    class FailureListener : Java.Lang.Object, IOnFailureListener
    {
        public void OnFailure(Java.Lang.Exception e)
        {
            IntegrityTokenSource?.TrySetException(new Exception($"Play Integrity API failed: {e.Message}"));
        }
    }
}