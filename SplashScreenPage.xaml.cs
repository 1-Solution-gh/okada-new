using Microsoft.Maui.Controls;

namespace OkadaGoApp
{
    public partial class SplashScreenPage : ContentPage
    {
        public SplashScreenPage()
        {
            InitializeComponent();
            AnimateSplash();
        }

        private async void AnimateSplash()
        {
            // Fade in logo
            await LogoImage.FadeTo(1, 1000, Easing.CubicInOut);
            // Fade in text
            await LoadingLabel.FadeTo(1, 800);
            // Hold for 3 seconds
            await Task.Delay(3000);
            // Fade out before navigating 
            await this.FadeTo(0, 500);
            // Navigate to AppShell (correct way)
            if (Application.Current?.Windows?.Count > 0 && Application.Current.Windows[0] != null)
            {
                Application.Current.Windows[0].Page = new AppShell();
            }
        }
    }
}