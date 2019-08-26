using System;
using System.Windows;
using System.Windows.Controls;

namespace ExpanderStyle
{
    /// <summary>
    /// Interaction logic for RangeSlider.xaml
    /// </summary>
    public partial class RangeSlider : UserControl
    {
        public RangeSlider()
        {
            InitializeComponent();
            this.Loaded += RangeSlider_Loaded;
        }

        void RangeSlider_Loaded(object sender, RoutedEventArgs e)
        {
            S1.ValueChanged += S1ValueChanged;
            S2.ValueChanged += S2ValueChanged;
            S3.ValueChanged += S3ValueChanged;
        }

        private void S1ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            S2.Value = Math.Max(S1.Value, S2.Value);
            S3.Value = Math.Max(S2.Value, S3.Value);
        }

        private void S2ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            S1.Value = Math.Min(S1.Value, S2.Value);
            S3.Value = Math.Max(S2.Value, S3.Value);
        }

        private void S3ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            S1.Value = Math.Min(S1.Value, S2.Value);
            S3.Value = Math.Max(S2.Value, S3.Value);
        }
    }
}
