using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace RebarTypeDrawing.View
{
    /// <summary>
    /// Interaction logic for DrawingView.xaml
    /// </summary>
    public partial class DrawingView
    {
        public DrawingView()
        {
            InitializeComponent();
        }

        private Point _currentPoint;
        private readonly List<Point> _points = new List<Point>();
        private readonly List<Line> _tempLines = new List<Line>();
        private void Canvas_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                _currentPoint = e.GetPosition(this);
                _points.Add(_currentPoint);
            }
        }

        private void Canvas_MouseMove_1(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var line = GetLine(_currentPoint.X, _currentPoint.Y, e.GetPosition(this).X, e.GetPosition(this).Y);

                _currentPoint = e.GetPosition(this);
                _points.Add(_currentPoint);
                _tempLines.Add(line);

                PaintSurface.Children.Add(line);
            }
        }

        private void Canvas_MouseUp_1(object sender, MouseButtonEventArgs e)
        {
            _tempLines.ForEach(x => PaintSurface.Children.Remove(x));

            if (_points.Count > 1)
            {
                var line = GetLine(_points.First().X, _points.First().Y, _points.Last().X, _points.Last().Y);
                PaintSurface.Children.Add(line);
            }

            _points.Clear();
            _tempLines.Clear();
        }

        private Line GetLine(double x1, double y1, double x2, double y2)
        {
            var line = new Line
            {
                Stroke = SystemColors.WindowFrameBrush,
                StrokeThickness = 4,
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2
            };
            return line;
        }
    }
}
