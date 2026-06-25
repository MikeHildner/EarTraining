namespace EarTraining.App.Pages;

/// <summary>
/// The app's landing hub: branding plus a tappable menu of the chapters and their drills.
/// Each drill button carries the target ShellContent route in CommandParameter; tapping
/// navigates there via Shell. Reached on launch (the welcome screen hands off to Home).
/// </summary>
public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnDrill(object? sender, EventArgs e)
    {
        if (sender is Button { CommandParameter: string route })
            await Shell.Current.GoToAsync($"//{route}");
    }
}
