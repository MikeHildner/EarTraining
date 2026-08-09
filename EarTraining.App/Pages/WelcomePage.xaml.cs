namespace EarTraining.App.Pages;

/// <summary>
/// Launch welcome: the ear + the gold gauge sweeping to 100% "Quite Good" (a little
/// reward on the way in), then it auto-fades into the main app after a couple of seconds.
/// The version rides under the tagline so it's readable without opening About — handy
/// when a tester reports something and you need to know which build they're on.
/// </summary>
public partial class WelcomePage : ContentPage
{
    private bool _started;

    public WelcomePage()
    {
        InitializeComponent();
        VersionLabel.Text = $"Version {AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_started) return;   // OnAppearing can fire more than once; run the sequence just the once
        _started = true;

        await Task.Delay(250);          // let the page lay out
        Gauge.Record(true);             // 1/1 = 100% -> gold ring sweeps up to "Quite Good"
        await Task.Delay(2000);         // let the moment land

        var window = Window;
        await this.FadeToAsync(0, 350, Easing.CubicIn);
        if (window is not null)
            window.Page = new AppShell();
    }
}
