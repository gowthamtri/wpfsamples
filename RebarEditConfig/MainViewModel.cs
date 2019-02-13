using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Mvvm;
using Mvvm.Commands;

namespace RebarEditConfig
{
    public class MainViewModel : BindableBase
    {
        public MainViewModel()
        {
            CreateTypeCommand = new DelegateCommand(OnCreateReinforcementType);
            ReinforcementTypes = new ObservableCollection<ReinforcementType>();
        }

        public ICommand CreateTypeCommand { get; set; }
        public ObservableCollection<ReinforcementType> ReinforcementTypes { get; set; }

        private void OnCreateReinforcementType()
        {
            var id = ReinforcementTypes.Count + 65;
            ReinforcementTypes.Add(new ReinforcementType
            {
                Type = $"Type{(char)id}",
                Shape = "Rectangle",
                CornerBarDiameter = 8,
                HorizontalPhaseBarCount = 0,
                VerticalPhaseBarCount = 2,
            });
        }
    }


    public class ReinforcementType : BindableBase
    {
        public string Shape
        {
            get => _shape;
            set
            {
                _shape = value;
                OnPropertyChanged(() => Shape);
                OnPropertyChanged(() => DrawingVisuals);
            }
        }

        public string Type
        {
            get => _type;
            set
            {
                _type = value;
                OnPropertyChanged(() => Type);
                OnPropertyChanged(() => DrawingVisuals);
            }
        }

        public int CornerBarDiameter
        {
            get => _cornerBarDiameter;
            set
            {
                _cornerBarDiameter = value;
                OnPropertyChanged(() => CornerBarDiameter);
                OnPropertyChanged(() => DrawingVisuals);
            }
        }

        public int HorizontalPhaseBarDiameter
        {
            get => _horizontalPhaseBarDiameter;
            set
            {
                _horizontalPhaseBarDiameter = value;
                OnPropertyChanged(() => HorizontalPhaseBarDiameter);
                OnPropertyChanged(() => DrawingVisuals);
            }
        }

        public int VerticalPhaseBarDiameter
        {
            get => _verticalPhaseBarDiameter;
            set
            {
                _verticalPhaseBarDiameter = value;
                OnPropertyChanged(() => VerticalPhaseBarDiameter);
                OnPropertyChanged(() => DrawingVisuals);
            }
        }

        public int HorizontalPhaseBarCount
        {
            get => _horizontalPhaseBarCount;
            set
            {
                _horizontalPhaseBarCount = value;
                OnPropertyChanged(() => HorizontalPhaseBarCount);
                OnPropertyChanged(() => DrawingVisuals);
            }
        }

        public int VerticalPhaseBarCount
        {
            get => _verticalPhaseBarCount;
            set
            {
                _verticalPhaseBarCount = value;
                OnPropertyChanged(() => VerticalPhaseBarCount);
                OnPropertyChanged(() => DrawingVisuals);
            }
        }

        public IEnumerable<Shape> DrawingVisuals => GenerateVisuals();

        public double SectionHeight { get; } = 180;
        public double SectionWidth { get; } = 230;

        private string _shape;
        private string _type;
        private int _cornerBarDiameter;
        private int _horizontalPhaseBarDiameter;
        private int _verticalPhaseBarDiameter;
        private int _horizontalPhaseBarCount;
        private int _verticalPhaseBarCount;

        private IEnumerable<Shape> GenerateVisuals()
        {
            var horizontalBars = 2 + HorizontalPhaseBarCount;
            var verticalBars = 2 + VerticalPhaseBarCount;

            var offset = 10;

            var horizontalSpacing = (SectionWidth - (offset * 2)) / (horizontalBars - 1);
            var verticalSpacing = (SectionHeight - (offset * 2)) / (verticalBars - 1);

            var visuals = new List<Shape>
            {
                new RectangleSection(SectionWidth - offset, SectionHeight - offset, new Point(0,0))
            };

            for (var i = 0; i < horizontalBars; i++)
            {
                var off = i == 0 ? offset : 0;
                visuals.Add(new LongitudinalBar(new Point((i * horizontalSpacing) + off, offset), 8));
                visuals.Add(new LongitudinalBar(new Point((i * horizontalSpacing) + off, SectionHeight - (offset * 2)), 8));
            }

            for (var i = 1; i < verticalBars - 1; i++)
            {
                visuals.Add(new LongitudinalBar(new Point(offset, (i * verticalSpacing) + offset), 8));
                visuals.Add(new LongitudinalBar(new Point(SectionWidth - (offset * 2), (i * verticalSpacing) + offset), 8));
            }

            return visuals;
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
            Rect = new Rect(StartLocation, new Point(Width, Height))
        };

        public Point StartLocation { get; }

        public RectangleSection(double width, double height, Point? location = null)
        {
            StartLocation = location ?? new Point(0, 0);
            Height = height;
            Width = width;
            Fill = Brushes.Transparent;
            Stroke = Brushes.DarkSlateGray;
            StrokeThickness = 5;
        }
    }
}
