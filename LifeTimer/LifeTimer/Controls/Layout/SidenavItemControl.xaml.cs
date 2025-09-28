using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;

namespace LifeTimer.Controls.Layout;

public sealed partial class SidenavItemControl : UserControl
{
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register(
            nameof(Title),
            typeof(string),
            typeof(SidenavItemControl),
            new PropertyMetadata(string.Empty));

    public event EventHandler OnSelected;

    private bool _isSelected = false;

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        private set => _isSelected = value;
    }

    public SidenavItemControl()
    {
        this.InitializeComponent();

        NavigationButton.AddHandler(PointerPressedEvent, new PointerEventHandler(NavigationButton_PointerPressed), true);

        UpdateVisualState();

    }

    private void SidenavItemControl_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        throw new NotImplementedException();
    }

    public void Select()
    {
        if (!_isSelected)
        {
            _isSelected = true;
            UpdateVisualState();
        }
    }

    public void Deselect()
    {
        if (_isSelected)
        {
            _isSelected = false;
            UpdateVisualState();
        }
    }

    private void UpdateVisualState()
    {
        if (_isSelected)
        {
            NavigationButton.Resources["ButtonBorderBrushPointerOver"] = (Brush)App.Current.Resources["AccentFillColorDefaultBrush"];
            NavigationButton.BorderBrush = (Brush)App.Current.Resources["AccentFillColorDefaultBrush"];
          //  TitleTextBlock.Foreground = (Brush)App.Current.Resources["AccentFillColorDefaultBrush"];
          //  TitleTextBlock.FontWeight = Microsoft.UI.Text.FontWeights.SemiBold;
        }
        else
        {
            NavigationButton.ClearValue(Button.BorderBrushProperty);
         //   TitleTextBlock.FontWeight = Microsoft.UI.Text.FontWeights.Normal;
          //  TitleTextBlock.ClearValue(TextBlock.ForegroundProperty);

            NavigationButton.Resources["ButtonForegroundPressed"] = App.Current.Resources["AccentFillColorDefaultBrush"];
            NavigationButton.Resources["ButtonBorderBrushPointerOver"] = App.Current.Resources["ButtonBorderBrushPointerOver"];

        }
    }

    private void NavigationButton_Tapped(object sender, TappedRoutedEventArgs e)
    {

        /*
                NavigationButton.Resources["ButtonBorderBrushPressed"] = App.Current.Resources["AccentFillColorDefaultBrush"];
                NavigationButton.Resources["ButtonForegroundPressed"] = App.Current.Resources["AccentFillColorDefaultBrush"];

                NavigationButton.Resources["ButtonBorderBrushPointerOver"] = App.Current.Resources["AccentFillColorDefaultBrush"];
                NavigationButton.Resources["ButtonForegroundPointerOver"] = App.Current.Resources["AccentFillColorDefaultBrush"];
        */
        //   NavigationButton.BorderBrush = (Brush)App.Current.Resources["AccentFillColorDefaultBrush"];
  

    }

    private void NavigationButton_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        OnSelected?.Invoke(this, EventArgs.Empty);
    }


    private void NavigationButton_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
                  
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            OnSelected?.Invoke(this, EventArgs.Empty);
            e.Handled = true;
        }
    }

}