using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ExpanderStyle
{
    /// <summary>
    /// Interaction logic for StackedDonut.xaml
    /// </summary>
    public partial class StackedDonut
    {
        public StackedDonut()
        {
            InitializeComponent();
            DataContext = this;
        }

        public ObservableCollection<UIElement> DrawingVisuals { get; set; }

        private void OnControlLoaded(object sender, RoutedEventArgs e)
        {
            DrawingVisuals = new ObservableCollection<UIElement>();
            var brushes = new List<Brush>
            {
                Brushes.LightGreen,
                Brushes.CadetBlue,
                Brushes.SlateBlue,
                Brushes.Orange,
                Brushes.Green,
                Brushes.Tomato,
                Brushes.Yellow,
                Brushes.Aqua,
                Brushes.Red,
                Brushes.PaleVioletRed,
                Brushes.Indigo,
            };
            var anglePerSegment = (Math.PI * 2) / 10;
            for (var i = 0; i < 10; i++)
            {
                var angle = i * anglePerSegment;
                var angle2 = (i + 1) * anglePerSegment;
                DrawingVisuals.Add(new DonutControl(new Point(200, 200), 200, angle, angle2, brushes[i], $"Test {i}").CreateVisual());
            }
        }
    }

    public class DonutControl
    {
        public Point Center { get; }
        public Point Point1 { get; }
        public Point Point2 { get; }
        public double Radius { get; }
        public double StartAngle { get; }
        public double EndAngle { get; }
        public double MarginX { get; }
        public double MarginY { get; }
        public Brush Fill { get; }

        public DonutControl(Point center, double radius, double startAngle, double endAngle, Brush brush, string description)
        {
            Center = center;
            Radius = radius;
            StartAngle = startAngle;
            EndAngle = endAngle;
            Point1 = new Point(center.X + radius * Math.Cos(startAngle), center.Y + radius * Math.Sin(startAngle));
            Point2 = new Point(center.X + radius * Math.Cos(endAngle), center.Y + radius * Math.Sin(endAngle));

            Description = description;
            Fill = brush;

            MarginX = center.X + (radius / 1.5) * Math.Cos(startAngle + (endAngle - startAngle) / 2);
            MarginY = center.Y + (radius / 1.5) * Math.Sin(startAngle + (endAngle - startAngle) / 2);
        }

        public UIElement CreateVisual()
        {
            _uiElement = new Path
            {
                Data = CreateGeometry(),
                Fill = Fill,
                Stroke = Brushes.White,
                StrokeThickness = 0.5
            };
            _grid = new Grid
            {
                Children =
                {
                    _uiElement,
                    new TextBlock
                    {
                        Text = Description,
                        Foreground = Brushes.Black,
                        FontSize = 11,
                        Margin = new Thickness(MarginX, MarginY, 0, 0),
                    }
                },
                ToolTip = Description,
            };
            _grid.MouseDown += OnMouseDown;
            _grid.MouseEnter += OnMouseEnter;
            _grid.MouseLeave += OnMouseLeave;
            return _grid;
        }

        private Grid _grid;
        private Path _uiElement;
        private string Description { get; }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            if (_uiElement != null)
            {
                _uiElement.StrokeThickness = 0.5;
                _uiElement.Stroke = Brushes.White;
            }
        }

        private void OnMouseEnter(object sender, MouseEventArgs mouseEventArgs)
        {
            if (_uiElement != null)
            {
                _uiElement.StrokeThickness = 2.5;
                _uiElement.Stroke = Brushes.Yellow;
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            MessageBox.Show(Description);
        }

        private Geometry CreateGeometry()
        {
            var myPathFigure = new PathFigure { StartPoint = Center };

            var myLineSegment = new LineSegment { Point = Point1, IsStroked = true };
            var arcSegment = new ArcSegment(Point2, new Size(Radius, Radius), 0, false, SweepDirection.Clockwise, true);
            var lineSegment2 = new LineSegment { Point = Center, IsStroked = true };

            myPathFigure.Segments = new PathSegmentCollection { myLineSegment, arcSegment, lineSegment2 };

            var myPathFigureCollection = new PathFigureCollection { myPathFigure };
            var myPathGeometry = new PathGeometry { Figures = myPathFigureCollection };

            return myPathGeometry;
        }
    }
}

