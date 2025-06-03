using System;
using Microsoft.Maui.Controls;

namespace OkadaGoApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        RequestPermissions();
        LoadLocalMap();
    }

    private void RequestPermissions()
    {
#if ANDROID
        _ = Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        _ = Permissions.RequestAsync<Permissions.LocationAlways>();
#endif
    }

    private async void LoadLocalMap()
    {
        var filePath = Path.Combine(FileSystem.AppDataDirectory, "map.html");
        var html = await File.ReadAllTextAsync(filePath);
        MapWebView.Source = new HtmlWebViewSource { Html = html };
    }
}
