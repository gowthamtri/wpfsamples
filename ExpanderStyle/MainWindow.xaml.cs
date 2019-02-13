using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ExpanderStyle.Annotations;
using Brush = System.Windows.Media.Brush;
using Color = System.Drawing.Color;

namespace ExpanderStyle
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : INotifyPropertyChanged
    {
        private Color _selectedColor;
        private List<Brush> _selectedColorDark;

        public MainWindow()
        {
            InitializeComponent();
            this.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/ExpanderStyle;component/themes/generic.xaml", UriKind.Relative)
            });
            DataContext = this;
            LoadData();
            LoadChartData();

            NestedDict = new Dictionary<string, Dictionary<string, List<ExtendedString>>>
            {
                {"T1", new Dictionary<string, List<ExtendedString>>{{"U1", new List<ExtendedString> { "1", "2"} }}},
                {"T2", new Dictionary<string, List<ExtendedString>>{{"U2", new List<ExtendedString> { "1", "2"} }}},
                {"T3", new Dictionary<string, List<ExtendedString>>{{"U3", new List<ExtendedString> { "1", "2"} }}},
                {"T4", new Dictionary<string, List<ExtendedString>>{{"U4", new List<ExtendedString> { "1", "2"} }}},
            };

            Loaded += OnLoaded;
        }

        public Dictionary<string, Dictionary<string, List<ExtendedString>>> NestedDict { get; set; }

        public List<ChartValue> ChartValues { get; set; }

        public List<Color> Colors { get; set; }

        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                _selectedColor = value;
                OnPropertyChanged(nameof(SelectedColor));
                SelectedColorDark = GetGradients(_selectedColor);
            }
        }

        public List<List<Brush>> AllBrushes { get; set; }

        public List<Brush> SelectedColorDark
        {
            get => _selectedColorDark;
            set
            {
                _selectedColorDark = value;
                OnPropertyChanged(nameof(SelectedColorDark));
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

        }

        public ObservableCollection<LayerOption> Data { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static Color ChangeColorBrightness(Color color, double correctionFactor)
        {
            var red = (double)color.R;
            var green = (double)color.G;
            var blue = (double)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        private void LoadChartData()
        {
            ChartValues = new List<ChartValue>
            {
                new ChartValue{X=0, Y1=10, Y2=112, Y3=130, Y4=15},
                new ChartValue{X=1, Y1=20, Y2=212, Y3=230, Y4=25},
                new ChartValue{X=2, Y1=30, Y2=312, Y3=330, Y4=35},
                new ChartValue{X=3, Y1=10, Y2=112, Y3=130, Y4=15},
                new ChartValue{X=4, Y1=20, Y2=212, Y3=230, Y4=25},
                new ChartValue{X=5, Y1=30, Y2=312, Y3=330, Y4=35},
            };
        }

        private void LoadData()
        {
            Data = new ObservableCollection<LayerOption>
            {
                new LayerOption{Layer = "T1"},
                new LayerOption{Layer = "T2"},
                new LayerOption{Layer = "T3"},
                new LayerOption{Layer = "T4"},
            };

            Colors = new List<Color>
            {
                Color.FromArgb(162, 48, 6),
                Color.DarkRed,
                Color.DarkTurquoise,
                Color.YellowGreen,
                Color.Blue,
                Color.BlueViolet,
                Color.LightSeaGreen,
                Color.Brown,
                Color.Green,
                Color.SlateBlue,
                Color.OrangeRed,
                Color.SaddleBrown,
                Color.MediumVioletRed,
                Color.CadetBlue,
                Color.DarkOrange,
                Color.Crimson,
                Color.DarkKhaki
            };
            SelectedColor = Colors.First();
            AllBrushes = Colors.Select(GetGradients).ToList();
        }

        private List<Brush> GetGradients(Color color)
        {
            var list = new List<Brush>();
            for (var i = -0.1; i < 1; i += 0.05)
            {
                var c1 = ChangeColorBrightness(color, i);
                list.Add(new SolidColorBrush(System.Windows.Media.Color.FromRgb(c1.R, c1.G, c1.B)));
            }

            return list;
        }

        private void UIElement_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void PART_CLOSE_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PART_MINIMIZE_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void PART_MAXIMIZE_RESTORE_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
            }
            else
            {
                WindowState = WindowState.Normal;
            }
        }

        private void PART_TITLEBAR_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }

    public class ChartValue
    {
        public double X { get; set; }
        public double Y1 { get; set; }
        public double Y2 { get; set; }
        public double Y3 { get; set; }
        public double Y4 { get; set; }
    }

    public class LayerOption
    {
        public string Layer { get; set; }
        public bool IsEnabled { get; set; }
        public double Spacing { get; set; }
        public string SelectedBarType { get; set; }

        public List<string> Bars { get; set; }

        public LayerOption()
        {
            Bars = Enumerable.Range(0, 10).Select(x => $"x{x}").ToList();
            SelectedBarType = Bars.FirstOrDefault();
        }
    }

    public class ExtendedString
    {
        public string Content { get; set; }

        public ExtendedString(string cnt)
        {
            Content = cnt;
        }

        public static implicit operator ExtendedString(string str) => new ExtendedString(str);
    }
}
