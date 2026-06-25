namespace EarTraining.App.Pages;

/// <summary>
/// Launch welcome: the ear + the gold gauge sweeping to 100% "Quite Good" (a little
/// reward on the way in), then it auto-fades into the main app after a couple of seconds.
/// </summary>
public partial class WelcomePage : ContentPage
{
    private bool _started;

    public WelcomePage()
    {
        InitializeComponent();
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
