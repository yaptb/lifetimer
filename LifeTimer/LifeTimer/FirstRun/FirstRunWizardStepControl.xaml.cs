using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LifeTimer.FirstRun
{
    public sealed partial class FirstRunWizardStepControl : UserControl
    {
        public FirstRunWizardStepControl()
        {
            InitializeComponent();
        }

        public WizardStep Step
        {
            get => (WizardStep)GetValue(StepProperty);
            set => SetValue(StepProperty, value);
        }

        public static readonly DependencyProperty StepProperty =
            DependencyProperty.Register(nameof(Step), typeof(WizardStep), typeof(FirstRunWizardStepControl), new PropertyMetadata(null));


    }
}
