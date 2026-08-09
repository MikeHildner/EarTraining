namespace EarTraining.App.Components;

/// <summary>
/// Wraps a <see cref="Picker"/> and draws a small ▾ at its trailing edge. iOS renders a
/// Picker as a bare rounded rectangle with no hint that it opens anything — Mark couldn't
/// tell the dropdowns were tappable on his iPhone — while native iOS pop-up buttons signal
/// with exactly this chevron. Android's Material underline is faint too, so both platforms
/// get it. Pages keep their <c>x:Name</c> on the inner Picker, so no code-behind changes:
/// <code>&lt;c:PickerField Grid.Row="1" Grid.Column="1"&gt;&lt;Picker x:Name="KeyPicker" /&gt;&lt;/c:PickerField&gt;</code>
/// </summary>
public partial class PickerField : ContentView
{
    private const double DisabledOpacity = 0.35;

    private Label? _chevron;
    private Picker? _picker;

    public PickerField()
    {
        InitializeComponent();
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        _chevron = GetTemplateChild("Chevron") as Label;
        Attach();
    }

    // The template is applied before the XAML parser assigns Content, so the Picker isn't
    // there yet at OnApplyTemplate time — re-attach whenever Content arrives or changes.
    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == nameof(Content)) Attach();
    }

    // A disabled Picker (e.g. Blank Sheet's key signature while Clef = None) shouldn't
    // advertise a dropdown, so the chevron dims with it.
    private void Attach()
    {
        if (_picker is not null) _picker.PropertyChanged -= OnPickerPropertyChanged;
        _picker = Content as Picker;
        if (_picker is not null) _picker.PropertyChanged += OnPickerPropertyChanged;
        SyncChevron();
    }

    private void OnPickerPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(IsEnabled)) SyncChevron();
    }

    private void SyncChevron()
    {
        if (_chevron is not null)
            _chevron.Opacity = _picker?.IsEnabled == false ? DisabledOpacity : 1.0;
    }
}
