using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;

namespace LifeTimer.Controls.Settings.Components
{
    public sealed partial class SettingsSliderControl : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(SettingsSliderControl),
                new PropertyMetadata("Setting Title", OnTitleChanged));

        public static readonly DependencyProperty SubtitleProperty =
            DependencyProperty.Register(
                nameof(Subtitle),
                typeof(string),
                typeof(SettingsSliderControl),
                new PropertyMetadata("Setting subtitle", OnSubtitleChanged));

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                nameof(Minimum),
                typeof(double),
                typeof(SettingsSliderControl),
                new PropertyMetadata(0.0, OnMinimumChanged));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                nameof(Maximum),
                typeof(double),
                typeof(SettingsSliderControl),
                new PropertyMetadata(100.0, OnMaximumChanged));

        public static readonly DependencyProperty IncrementProperty =
            DependencyProperty.Register(
                nameof(Increment),
                typeof(double),
                typeof(SettingsSliderControl),
                new PropertyMetadata(1.0, OnIncrementChanged));

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                nameof(Value),
                typeof(double),
                typeof(SettingsSliderControl),
                new PropertyMetadata(50.0, OnValuePropertyChanged));

        public event EventHandler<double> OnValueChanged;

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Subtitle
        {
            get => (string)GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }

        public double Increment
        {
            get => (double)GetValue(IncrementProperty);
            set => SetValue(IncrementProperty, value);
        }

        public double Value
        {
            get => (double)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        public SettingsSliderControl()
        {
            this.InitializeComponent();
        }

        private static void OnTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsSliderControl control)
                control.TitleTextBlock.Text = e.NewValue?.ToString();
        }

        private static void OnSubtitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsSliderControl control)
                control.SubtitleTextBlock.Text = e.NewValue?.ToString();
        }

        private static void OnMinimumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsSliderControl control)
                control.ValueSlider.Minimum = (double)e.NewValue;
        }

        private static void OnMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsSliderControl control)
                control.ValueSlider.Maximum = (double)e.NewValue;
        }

        private static void OnIncrementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsSliderControl control)
                control.ValueSlider.StepFrequency = (double)e.NewValue;
        }

        private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SettingsSliderControl control)
                control.ValueSlider.Value = (double)e.NewValue;
        }

        private void ValueSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            Value = e.NewValue;
            OnValueChanged?.Invoke(this, e.NewValue);
        }
    }
}