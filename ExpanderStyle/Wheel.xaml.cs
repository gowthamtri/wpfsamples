using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExpanderStyle
{
    /// <summary>
    /// Interaction logic for Wheel.xaml
    /// </summary>
    public partial class Wheel : UserControl
    {
        public Wheel()
        {
            InitializeComponent();
        }

        private void OnMEnter(object sender, MouseButtonEventArgs e)
        {
            WheelPopup.IsOpen = !WheelPopup.IsOpen;
        }

        private void OnBorderClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show((sender as Border)?.Name ?? "");
        }

        private void OnOptionBorderMouseEnter(object sender, MouseEventArgs e)
        {
            OptionsPopup.IsOpen = true;
        }
    }
}
