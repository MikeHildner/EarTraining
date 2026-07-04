namespace EarTraining.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }

    // Android hardware/gesture back. Every drill page is its own Shell root (navigation uses
    // absolute "//route"), so there is never a nav stack to pop and the system would otherwise
    // drop straight out of the app. Hub-and-spoke instead: any page but Home returns to Home;
    // on Home (or with the flyout open) keep the platform default. iOS never calls this.
    protected override bool OnBackButtonPressed()
    {
        if (FlyoutIsPresented) return base.OnBackButtonPressed();
        if (CurrentState?.Location?.OriginalString is not { Length: > 0 } here || here == "//home")
            return base.OnBackButtonPressed();
        Dispatcher.Dispatch(async () => await GoToAsync("//home"));
        return true;
    }
}
