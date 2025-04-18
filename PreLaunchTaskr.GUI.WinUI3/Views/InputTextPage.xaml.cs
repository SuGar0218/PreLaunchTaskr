using Microsoft.UI.Xaml.Controls;

namespace PreLaunchTaskr.GUI.WinUI3.Views;

public sealed partial class InputTextPage : Page
{
    public InputTextPage()
    {
        InitializeComponent();
    }

    public string Header { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;
}
