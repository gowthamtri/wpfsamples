﻿namespace RebarEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void GridMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            var eDelta = e.Delta < 0 ? -0.1 : 0.1;
            Slider.Value = Slider.Value + eDelta;
        }
    }
}
