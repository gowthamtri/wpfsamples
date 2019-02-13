using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace ControlLibrary
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ControlLibrary"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:ControlLibrary;assembly=ControlLibrary"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    
    public class ButtonBehavior : Behavior<Button>
    {
        public static DependencyProperty BehaviorProperty = DependencyProperty.Register(nameof(Behavior), typeof(ButtonBehaviorEnum), typeof(ButtonBehavior), new PropertyMetadata(ButtonBehaviorEnum.Close, OnPropertyChanged));

        public ButtonBehaviorEnum Behavior
        {
            get => (ButtonBehaviorEnum)this.GetValue(ButtonBehavior.BehaviorProperty);
            set => SetValue(ButtonBehavior.BehaviorProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.Click += OnClicked;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.Click -= OnClicked;
        }

        private void OnClicked(object sender, RoutedEventArgs e)
        {
            var window = GetParent(AssociatedObject);
            switch (Behavior)
            {
                case ButtonBehaviorEnum.Close:
                    window.Close();
                    break;
                case ButtonBehaviorEnum.Maximize:
                    window.WindowState = WindowState.Maximized;
                    break;
                case ButtonBehaviorEnum.Minimize:
                    window.WindowState = WindowState.Minimized;
                    break;
            }
        }

        public Window GetParent(DependencyObject depObj)
        {
            var parent = VisualTreeHelper.GetParent(depObj);

            while (parent != null && !(parent is Window))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parent as Window;
        }

        private static void OnPropertyChanged(DependencyObject dObj, DependencyPropertyChangedEventArgs args)
        {

        }
    }
}