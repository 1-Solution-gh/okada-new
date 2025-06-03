namespace OkadaGoApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// Start with the splash screen
        return new Window(new SplashScreenPage());
	}
}