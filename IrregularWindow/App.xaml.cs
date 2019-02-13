using System;
using System.Windows;

namespace IrregularWindow
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void PART_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
