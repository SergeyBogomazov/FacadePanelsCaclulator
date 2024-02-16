using System.Drawing;

namespace Models
{
    public class LineSegment
    {
        public readonly Point a;
        public readonly Point b;

        public readonly float minX;
        public readonly float minY;

        public readonly float maxX;
        public readonly float maxY;

        public LineSegment(Point a, Point b) 
        { 
            this.a = a; 
            this.b = b;

            minX = a.X <= b.X ? a.X : b.X;
            maxX = a.X >= b.X ? a.X : b.X;

            minY = a.Y <= b.Y ? a.Y : b.Y;
            maxY = a.Y >= b.Y ? a.Y : b.Y;
        }

        public bool ContainsX(float x)
        {
            return x >= minX && x <= maxX;
        }

        /// <summary>
        /// For argument x finds points on line segment which intersects by vertical line with X = x.
        /// Return only extremum by Y points.
        /// </summary>
        public IEnumerable<Point> GetIntersectionExtremumsByX(float x)
        {
            if (!ContainsX(x))
            {
                return new Point[] { };
            }

            if (minX == maxX) // Vertical line case
            {
                return new List<Point>() { a, b };
            }

            if (minY == maxY) // Horizontal line case
            {
                return new List<Point>() { new Point(x, minY) };
            }

            return new List<Point>() { GetIntersectionBetweenXEdgesPoints(x) };
        }

        private Point GetIntersectionBetweenXEdgesPoints(float x)
        {
            if (!ContainsX(x)) {
                throw new ArgumentException($"Wrong X = {x}. Its not between x edges");
            }

            float dx = (x - minX) / (maxX - minX);

            var leftEnd = a.X <= b.X ? a : b;
            var rightEnd = a.X >= b.X ? a : b;

            float dy = 0;
            float lenY = maxY - minY;

            if (leftEnd.Y <= rightEnd.Y)
            {
                dy = minY + lenY * dx;
            }
            else
            {
                dy = maxY - lenY * dx;
            }

            return new Point(x, dy);
        }

        public override string ToString()
        {
            return $"[{a},{b}]";
        }
    }
}
