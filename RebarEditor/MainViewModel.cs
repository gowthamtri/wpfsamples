using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Mvvm;
using Mvvm.Commands;

namespace RebarEditor
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {
            CanvasWidth = 550;
            CanvasHeight = 350;
            DrawingVisuals = new ObservableCollection<Shape>
            {
                new RectangleSection(500, 300)
            };


            LongitudinalBarConfiguration.DataChanged += delegate { OnGenerateBars(); };
            StirrupConfiguration.DataChanged += delegate { OnGenerateBars(); };

            LongitudinalBarConfigurations = new ObservableCollection<LongitudinalBarConfiguration>();
            StirrupConfigurations = new ObservableCollection<StirrupConfiguration>();
            GenerateBarsCommand = new DelegateCommand(OnGenerateBars);
        }

        public double CanvasHeight { get; set; }
        public double CanvasWidth { get; set; }

        public ObservableCollection<Shape> DrawingVisuals { get; set; }
        public ObservableCollection<LongitudinalBarConfiguration> LongitudinalBarConfigurations { get; set; }
        public ObservableCollection<StirrupConfiguration> StirrupConfigurations { get; set; }

        public ICommand GenerateBarsCommand { get; set; }

        private void OnGenerateBars()
        {
            GenerateLongitudinalBars();
            GenerateStirrups();
        }

        private void GenerateStirrups()
        {
            var stirrups = DrawingVisuals.OfType<Stirrup>().ToList();
            stirrups.ForEach(x => DrawingVisuals.Remove(x));
            foreach (var config in StirrupConfigurations)
            {
                DrawingVisuals.Add(new Stirrup(new Point(config.X, config.Y), config.Width, config.Height));
            }
        }

        private void GenerateLongitudinalBars()
        {
            var bars = DrawingVisuals.OfType<LongitudinalBar>().ToList();
            bars.ForEach(x => DrawingVisuals.Remove(x));
            foreach (var reinforcementType in LongitudinalBarConfigurations)
            {
                for (var j = 0; j < reinforcementType.Count; j++)
                {
                    var xInc = reinforcementType.Orientation == 0 ? (j * reinforcementType.Spacing) : 0;
                    var yInc = reinforcementType.Orientation != 0 ? (j * reinforcementType.Spacing) : 0;
                    DrawingVisuals.Add(new LongitudinalBar(new Point(reinforcementType.X + xInc, reinforcementType.Y + yInc), reinforcementType.Size));
                }
            }
        }
    }

    public class StirrupConfiguration : BindableBase
    {
        public static event EventHandler DataChanged = (sender, args) => { };

        public double X
        {
            get => _x;
            set
            {
                _x = value;
                PropertyChangedLocal(() => X);
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                PropertyChangedLocal(() => Y);
            }
        }

        public double Size
        {
            get => _size;
            set
            {
                _size = value;
                PropertyChangedLocal(() => Size);
            }
        }

        public double Width
        {
            get => _width;
            set
            {
                _width = value;
                PropertyChangedLocal(() => Width);
            }
        }

        public double Height
        {
            get => _height;
            set
            {
                _height = value;
                PropertyChangedLocal(() => Height);
            }
        }

        private double _x;
        private double _y;
        private double _size;
        private double _width;
        private double _height;

        private void PropertyChangedLocal<T>(Expression<Func<T>> propertyExpression)
        {
            DataChanged?.Invoke(this, null);
            OnPropertyChanged(PropertySupport.ExtractPropertyName<T>(propertyExpression));
        }
    }

    public class LongitudinalBarConfiguration : BindableBase
    {
        public static event EventHandler DataChanged = (sender, args) => { };

        public double X
        {
            get => _x;
            set
            {
                _x = value;
                PropertyChangedLocal(() => X);
            }
        }

        public double Y
        {
            get => _y;
            set
            {
                _y = value;
                PropertyChangedLocal(() => Y);
            }
        }

        public double Size
        {
            get => _size;
            set
            {
                _size = value;
                PropertyChangedLocal(() => Size);
            }
        }

        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                PropertyChangedLocal(() => Count);
            }
        }

        public double Spacing
        {
            get => _spacing;
            set
            {
                _spacing = value;
                PropertyChangedLocal(() => Spacing);
            }
        }

        public int Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                PropertyChangedLocal(() => Orientation);
            }
        }

        private double _x;
        private double _y;
        private double _size;
        private int _count;
        private double _spacing;
        private int _orientation;

        private void PropertyChangedLocal<T>(Expression<Func<T>> propertyExpression)
        {
            DataChanged?.Invoke(this, null);
            OnPropertyChanged(PropertySupport.ExtractPropertyName<T>(propertyExpression));
        }
    }

    public class LongitudinalBar : Shape
    {
        protected override Geometry DefiningGeometry => new EllipseGeometry
        {
            Center = Point,
            RadiusX = Size / 2,
            RadiusY = Size / 2
        };

        public Point Point { get; }
        public double Size { get; }

        public LongitudinalBar(Point location, double size)
        {
            Point = location;
            Size = size;
            Fill = Brushes.Brown;
        }
    }

    public class Stirrup : Shape
    {
        protected override Geometry DefiningGeometry => new RectangleGeometry
        {
            Rect = new Rect(Point, new Point(Point.X + Width, Point.Y + Height))
        };

        public Point Point { get; set; }

        public Stirrup(Point location, double width, double height)
        {
            Point = location;
            Height = height;
            Width = width;
            Fill = Brushes.Transparent;
            Stroke = Brushes.Yellow;
            StrokeThickness = 2;
        }
    }

    public class RectangleSection : Shape
    {
        protected override Geometry DefiningGeometry => new RectangleGeometry
        {
            Rect = new Rect(new Point(0, 0), new Point(Width, Height))
        };

        public RectangleSection(double width, double height)
        {
            Height = height;
            Width = width;
            Fill = Brushes.Transparent;
            Stroke = Brushes.DarkSlateGray;
            StrokeThickness = 5;
        }
    }
}
