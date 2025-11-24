using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace LifeTimer.Controls.Settings.Components
{


public sealed partial class SettingsToggleControl : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(SettingsToggleControl),
            new PropertyMetadata("(no title)", OnTitleChanged));

    public static readonly DependencyProperty DescriptionProperty =
        DependencyProperty.Register(
            nameof(Description),
            typeof(string),
            typeof(SettingsToggleControl),
            new PropertyMetadata("(no description)", OnDescriptionChanged));

    public static readonly DependencyProperty IsToggledProperty =
        DependencyProperty.Register(
            nameof(IsToggled),
            typeof(bool),
            typeof(SettingsToggleControl),
            new PropertyMetadata(false, OnIsToggledChanged));

    public event EventHandler<bool> ToggledChanged;

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Description
    {
        get => (string)GetValue(DescriptionProperty);
        set => SetValue(DescriptionProperty, value);
    }

    public bool IsToggled
    {
        get => (bool)GetValue(IsToggledProperty);
        set => SetValue(IsToggledProperty, value);
    }

    public SettingsToggleControl()
    {
        this.InitializeComponent();
    }

    private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SettingsToggleControl control)
            control.TitleTextBlock.Text = e.NewValue?.ToString();
    }

    private static void OnDescriptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SettingsToggleControl control)
            control.DescriptionTextBlock.Text = e.NewValue?.ToString();
    }

    private static void OnIsToggledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SettingsToggleControl control)
            control.ToggleSwitch.IsOn = (bool)e.NewValue;
    }

    private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
    {
        IsToggled = ToggleSwitch.IsOn;
        ToggledChanged?.Invoke(this, IsToggled);
    }
}

}
