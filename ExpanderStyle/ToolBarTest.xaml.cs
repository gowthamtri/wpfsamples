using System.Windows.Controls;
using System.Windows.Input;

namespace ExpanderStyle
{
    /// <summary>
    /// Interaction logic for ToolBarTest.xaml
    /// </summary>
    public partial class ToolBarTest : UserControl
    {
        public ToolBarTest()
        {
            InitializeComponent();
        }

        private void OnMEnter(object sender, MouseEventArgs e)
        {
            Wheel.IsOpen = true;
        }
    }
}
