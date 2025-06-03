namespace OkadaGoApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Set initial page to Splash
        MainPage = new SplashScreenPage();
    }
}
