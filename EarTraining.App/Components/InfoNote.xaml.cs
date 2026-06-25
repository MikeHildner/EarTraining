namespace EarTraining.App.Components;

/// <summary>
/// A small "ⓘ Book reference" toggle that reveals/hides a short note — used to surface the
/// printed-book page reference for each drill (kept collapsed so it doesn't clutter the page).
/// Pages set <see cref="Text"/> to their reference string.
/// </summary>
public partial class InfoNote : ContentView
{
    public InfoNote()
    {
        InitializeComponent();
    }

    /// <summary>The reference note shown when the ⓘ is tapped.</summary>
    public string? Text
    {
        get => NoteLabel.Text;
        set => NoteLabel.Text = value;
    }

    private void OnToggle(object? sender, EventArgs e) => NoteLabel.IsVisible = !NoteLabel.IsVisible;
}
