using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;

namespace EarTraining.App.Pages;

/// <summary>
/// About / credits: app + book attribution and the license notices for the
/// open-source components actually bundled in the app (MIT/OFL require the
/// notice to travel with distributions). Links open in the system browser.
/// </summary>
public partial class AboutPage : ContentPage
{
    public ICommand OpenUrl { get; }

    public AboutPage()
    {
        InitializeComponent();
        OpenUrl = new Command<string>(async url =>
        {
            if (!string.IsNullOrWhiteSpace(url))
            {
                try { await Launcher.Default.OpenAsync(url); }
                catch { /* no handler for the scheme — ignore */ }
            }
        });
        LicenseText.Text = LicenseNotice;
        VersionLabel.Text = $"Version {AppInfo.Current.VersionString}";
        BindingContext = this;
    }

    private void OnToggleLicenses(object? sender, EventArgs e)
    {
        LicenseText.IsVisible = !LicenseText.IsVisible;
        LicenseToggle.Text = LicenseText.IsVisible ? "Hide licenses" : "Open-source licenses";
    }

    private const string LicenseNotice =
@"The following are used under the MIT License:
  • Plugin.Maui.Audio — Copyright (c) Gerald Versluis
  • VexFlow — Copyright (c) Mohit Muthanna Cheppudira
  • .NET MAUI — Copyright (c) .NET Foundation and Contributors

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

Font Awesome 4.7 — icon font by Dave Gandy, licensed under the SIL Open Font License 1.1. https://fontawesome.com/v4/";
}
