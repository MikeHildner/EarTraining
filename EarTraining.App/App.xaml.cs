namespace EarTraining.App;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        UserAppTheme = Services.SettingsStore.Theme;   // apply the saved theme (Unspecified = follow system)
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new Pages.WelcomePage()) { Title = "Ear Training" };
    }
}
