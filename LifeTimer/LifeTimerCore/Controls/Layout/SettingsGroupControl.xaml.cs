using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LifeTimer.Controls.Layout;

public sealed partial class SettingsGroupControl : UserControl
{
    public static readonly DependencyProperty CustomContentProperty =
        DependencyProperty.Register(
            nameof(Content),
            typeof(object),
            typeof(SettingsGroupControl),
            new PropertyMetadata(null, PropertyChangedCallback));

    public object CustomContent
    {
        get => GetValue(CustomContentProperty);
        set => SetValue(CustomContentProperty, value);
    }

    public static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue != e.OldValue)
        {
            (d as SettingsGroupControl).Content.Content = e.NewValue;
        }
    }

    public SettingsGroupControl()
    {
        this.InitializeComponent();
    }
}