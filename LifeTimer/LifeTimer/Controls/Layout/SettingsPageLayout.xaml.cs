using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LifeTimer.Controls.Layout
{
    public sealed partial class SettingsPageLayout : UserControl
    {
        public SettingsPageLayout()
        {
            InitializeComponent();
        }

        public object CustomContent
        {
            get { return (object)GetValue(CustomContentProperty); }
            set { SetValue(CustomContentProperty, value); }
        }

        public static readonly DependencyProperty CustomContentProperty =
        DependencyProperty.Register("CustomContent", typeof(object), typeof(SettingsPageLayout), new PropertyMetadata(null, PropertyChangedCallback));

        public static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                (d as SettingsPageLayout).Content.Content = e.NewValue;
            }
        }
    }
}
