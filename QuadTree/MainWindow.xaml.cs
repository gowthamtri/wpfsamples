using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace QuadTreeApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ItemsControl.ItemsSource = DrawingVisuals;
        }

        public readonly ObservableCollection<Shape> DrawingVisuals = new ObservableCollection<Shape>();

        private string _file = @"E:\Python\ML\Reinforcements\Data\robot_results.csv";

        public QuadTree Tree { get; set; }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            var index = 0;
            var nodes = File.ReadAllLines(_file).Skip(1).Select(x =>
            {
                var splits = x.Split(',');
                return new Node(index++, double.Parse(splits[0]) + 50.3894, double.Parse(splits[1]) + 35.3043); // to get rid of negatives
            }).ToList();


            //var nodes = new List<Node>();

            //nodes.Add(new Node(1, 0, 0));
            //nodes.Add(new Node(1, 10, 0));
            //nodes.Add(new Node(1, 0, 10));
            //nodes.Add(new Node(1, 10, 10));
            //nodes.Add(new Node(1, 2, 2));
            //nodes.Add(new Node(1, 8, 8));
            //nodes.Add(new Node(1, 6, 2));

            var minX = nodes.Min(x => x.X);
            var minY = nodes.Min(x => x.Y);
            var maxX = nodes.Max(x => x.X);
            var maxY = nodes.Max(x => x.Y);

            Tree = new QuadTree(new Rectangle(minX, minY, maxX, maxY));
            foreach (var node in nodes) //.Where(x=>x.X.IsLessThan(16) && x.Y.IsLessThan(16)))
            {
                Tree.Insert(node);
            }

            DrawingVisuals.Clear();
            ShowNode(Tree.Root);
        }

        private void OnClip(object sender, RoutedEventArgs e)
        {
            var start = ClipStart.Text.Trim().Split(' ').Select(double.Parse).ToList();
            var end = ClipEnd.Text.Trim().Split(' ').Select(double.Parse).ToList();

            var results = Tree.Query(new Rectangle(start[0], start[1], end[0], end[1]));
            HighlightNodes(results);
        }

        private void HighlightNodes(List<Node> nodes)
        {
            var existingNodes = DrawingVisuals.OfType<HighLightNodeShape>().ToList();
            existingNodes.ForEach(x => DrawingVisuals.Remove(x));
            var xMul = ItemsControl.ActualWidth / Tree.Root.Bounds.Width;
            var yMul = ItemsControl.ActualHeight / Tree.Root.Bounds.Width;
            var color = Colors.Red;
            nodes.ForEach(x => DrawingVisuals.Add(new HighLightNodeShape(x, xMul, yMul, color)));
        }

        private void ShowNode(QuadArea root)
        {
            var xMul = ItemsControl.ActualWidth / Tree.Root.Bounds.Width;
            var yMul = ItemsControl.ActualHeight / Tree.Root.Bounds.Width;
            var color = GetColor();
            if (root.Nodes.Any())
            {
                root.Nodes.ForEach(x => DrawingVisuals.Add(new NodeShape(x, xMul, yMul, color)));
            }
            root.SubSpaces.ForEach(x => DrawingVisuals.Add(new SubSpace(x.Bounds, xMul, yMul, color)));
            root.SubSpaces.ForEach(ShowNode);
        }

        private int _index = 0;

        private Color GetColor()
        {
            _index++;
            var list = typeof(Colors).GetProperties().Select(x => (Color)x.GetValue(null)).ToList();
            return list[_index % list.Count];
        }
    }

    public class NodeShape : Shape
    {
        private readonly Node _node;
        private readonly double _xMul;
        private readonly double _yMul;

        public NodeShape(Node node, double xMultiplier, double yMultiplier, Color color, double opacity = 0.5)
        {
            _node = node;
            _xMul = xMultiplier;
            _yMul = yMultiplier;
            Fill = new SolidColorBrush(color);
            Opacity = opacity;
            ToolTip = $"{_node.X:F1}, {_node.Y:F1}";
        }

        protected override Geometry DefiningGeometry => new EllipseGeometry { Center = new Point(_node.X * _xMul, _node.Y * _yMul), RadiusX = 3, RadiusY = 3 };
    }

    public class HighLightNodeShape : NodeShape
    {
        public HighLightNodeShape(Node node, double xMultiplier, double yMultiplier, Color color, double opacity = 1.0)
            : base(node, xMultiplier, yMultiplier, color, opacity)
        {

        }
    }


    public class SubSpace : Shape
    {
        private readonly Rectangle _bounds;
        private readonly double _xMul;
        private readonly double _yMul;

        public SubSpace(Rectangle bounds, double xMultiplier, double yMultiplier, Color color)
        {
            _bounds = bounds;
            _xMul = xMultiplier;
            _yMul = yMultiplier;
            Fill = Brushes.Transparent;
            Opacity = 0.5;
            Stroke = new SolidColorBrush(Colors.LightGray);
        }

        protected override Geometry DefiningGeometry => new RectangleGeometry { Rect = new Rect(new Point(_bounds.X1 * _xMul, _bounds.Y1 * _yMul), new Point(_bounds.X2 * _xMul, _bounds.Y2 * _yMul)) };
    }

    public class QuadTree
    {
        public QuadTree(Rectangle boundingRectangle)
        {
            Bounds = boundingRectangle;
            Root = new QuadArea(Bounds);
        }

        public Rectangle Bounds { get; }

        public QuadArea Root { get; }

        public void Insert(Node node)
        {
            if (Root.Bounds.Contains(node.X, node.Y))
            {
                Root.Insert(node);
            }
        }

        public List<Node> Query(Rectangle area)
        {
            var results = new List<Node>();
            Root.Query(area, results);
            return results;
        }
    }

    public class QuadArea
    {
        public List<Node> Nodes { get; } = new List<Node>();
        public List<QuadArea> SubSpaces { get; } = new List<QuadArea>();
        public Rectangle Bounds { get; }

        public List<Node> Items
        {
            get
            {
                var results = new List<Node>();
                SubSpaces.ForEach(x => results.AddRange(x.Items));
                results.AddRange(Nodes);
                return results;
            }
        }

        public int Count
        {
            get
            {
                var count = Nodes.Count;
                foreach (var node in SubSpaces)
                    count += node.Count;
                return count;
            }
        }

        public QuadArea(Rectangle rect)
        {
            Bounds = rect;
        }

        public QuadArea(double x1, double y1, double x2, double y2)
        {
            Bounds = new Rectangle(x1, y1, x2, y2);
        }

        public void Insert(Node pNode)
        {
            if (!Bounds.Contains(pNode.X, pNode.Y))
            {
                return;
            }

            if (SubSpaces.Count == 0)
            {
                CreateSubNodes();
            }

            foreach (var node in SubSpaces)
            {
                if (node.Bounds.Contains(pNode.X, pNode.Y))
                {
                    node.Insert(pNode);
                    return;
                }
            }

            Nodes.Add(pNode);
        }

        public void Query(Rectangle queryArea, List<Node> results)
        {
            results.AddRange(Nodes.Where(x => queryArea.Contains(x.X, x.Y)));

            foreach (var node in SubSpaces)
            {
                if (node.Bounds.Contains(queryArea))
                {
                    node.Query(queryArea, results);
                    break;
                }

                if (queryArea.Contains(node.Bounds))
                {
                    results.AddRange(node.Items);
                }
                else if (node.Bounds.IntersectsWith(queryArea) || queryArea.IntersectsWith(node.Bounds))
                {
                    node.Query(queryArea, results);
                }
            }
        }

        private void CreateSubNodes()
        {
            if (Bounds.Area <= 20)
                return;

            var xNew = Bounds.Width / 2 + Bounds.X1;
            var yNew = Bounds.Height / 2 + Bounds.Y1;

            SubSpaces.Add(new QuadArea(Bounds.X1, Bounds.Y1, xNew, yNew));
            SubSpaces.Add(new QuadArea(xNew, Bounds.Y1, Bounds.X2, yNew));
            SubSpaces.Add(new QuadArea(xNew, yNew, Bounds.X2, Bounds.Y2));
            SubSpaces.Add(new QuadArea(Bounds.X1, yNew, xNew, Bounds.Y2));
        }
    }

    public class Rectangle
    {
        public double X1, Y1, X2, Y2;
        public double Width => X2 - X1;
        public double Height => Y2 - Y1;
        public double Area => Height * Width;

        public double Top => Y2;
        public double Bottom => Y1;
        public double Left => X1;
        public double Right => X2;

        public Rectangle(Rectangle r)
        {
            X1 = r.X1; X2 = r.X2; Y1 = r.Y1; Y2 = r.Y2;
        }

        public Rectangle(double x1, double y1, double x2, double y2)
        {
            this.X1 = x1; this.Y1 = y1; this.X2 = x2; this.Y2 = y2;
        }

        public bool IntersectsWith(Rectangle r)
        {
            return Contains(r.X1, r.Y1) || Contains(r.X1, r.Y2) || Contains(r.X2, r.Y1) ||
                Contains(r.X2, r.Y2);
        }

        public bool Contains(Rectangle r)
        {
            if (!Contains(r.X1, r.Y1)) return false;
            if (!Contains(r.X2, r.Y1)) return false;
            if (!Contains(r.X2, r.Y2)) return false;
            if (!Contains(r.X1, r.Y2)) return false;
            return true;
        }

        public bool Contains(double x, double y)
        {
            if (x.IsLessThan(Left)) return false;
            if (x.IsGreaterThan(Right)) return false;
            if (y.IsGreaterThan(Top)) return false;
            if (y.IsLessThan(Bottom)) return false;
            return true;
        }

        public void Translate(double dx, double dy)
        {
            X1 += dx;
            X2 += dx;
            Y1 += dy;
            Y2 += dy;
        }

        public void MoveAbs(float centerX, float centerY)
        {
            X1 = centerX - Width / 2;
            X2 = centerX + Width / 2;
            Y1 = centerY - Height / 2;
            Y2 = centerY + Height / 2;
        }

        public void Expand(ref double point, double adjustment)
        {
            point += adjustment;
        }

        public override string ToString()
        {
            return $"{X1},{Y1},{X2},{Y2}";
        }
    }

    public class Node
    {
        public Node(int id, double x, double y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public int Id { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    /// <summary>
    /// Summary description for Comparisons.
    /// </summary>
    public static class Comparisons
    {
        // 0.0001
        public const double ToleranceDoubleDefault = 0.000001;

        // Equal 
        public static bool IsEqual(this double firstValue, double secondValue)
        {
            return IsEqual(firstValue, secondValue, ToleranceDoubleDefault);
        }

        // Equal 
        public static bool IsEqual(this double firstValue, double secondValue, double tolerance)
        {
            double deference = System.Math.Abs(firstValue - secondValue);

            if (deference <= tolerance)
                return true;
            else
                return false;
        }

        public static bool IsEqualToZero(double value)
        {
            if (value.IsEqual(0, ToleranceDoubleDefault))
                return true;
            else
                return false;
        }

        public static double Round(this double val, int decimalPlaces)
        {
            return Math.Round(val, decimalPlaces);
        }

        /// <summary>
        /// Check a string whether it has value or not
        /// </summary>
        /// <param name="value"></param>
        /// <returns>true if value null or length is 0 , false if value is correct</returns>
        public static bool IsNullOrEmpty(string value)
        {
            bool returnValue = false;
            if (value == null)
                returnValue = true;
            else if (value.Length <= 0)
                returnValue = true;

            return returnValue;
        }

        public static bool IsGreaterThan(this double x, double y, double tolerance = 0.000001)
        {
            return x > y && Math.Abs(x - y) > tolerance;
        }

        public static bool IsGreaterOrEqual(this double x, double y, double tolerance = 0.000001)
        {
            return x > y || Math.Abs(x - y) <= tolerance;
        }

        public static bool IsLessThan(this double x, double y, double tolerance = 0.000001)
        {
            return x < y && Math.Abs(x - y) > tolerance;
        }

        public static bool IsLesserOrEqual(this double x, double y, double tolerance = 0.000001)
        {
            return x < y || Math.Abs(x - y) <= tolerance;
        }
    }
}
