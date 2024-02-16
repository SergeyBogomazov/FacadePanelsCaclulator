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

        public bool IsVertical => minX == maxX;
        public bool IsHorizontal => minY == maxY;

        public bool ContainsX(float x)
        {
            return x >= minX && x <= maxX;
        }

        /// <summary>
        /// Говорит с какой стороны относительно линии расположена точка.
        /// Если результат равен нулю, то точка находиться на линии.
        /// Иначе выше = 1 или ниже = -1.
        /// </summary>
        /// <param name="a">First point of line</param>
        /// <param name="b">Second point of line</param>
        /// <param name="c">Point</param>
        public int PointSideByLine(Point c)
        {
            var d = (c.X - a.X) * (b.Y - a.Y) - (c.Y - a.Y) * (b.X - a.X);

            if (d > 0) { return 1; }
            if (d < 0) { return -1; }
            return 0;
        }

        /// <summary>
        /// For argument x finds points on line segment which intersects by vertical line with X = x.
        /// Return only extremum by Y points.
        /// </summary>
        public IEnumerable<Point> GetIntersectionExtremumsByX(float x)
        {
            if (!ContainsX(x))
            {
                return new Point[0];
            }

            if (IsVertical) // Vertical line case
            {
                return new Point[2] { a, b };
            }

            if (IsHorizontal) // Horizontal line case
            {
                return new Point[1] { new Point(x, minY) };
            }

            return new Point[1] { GetIntersectionBetweenXEdgesPoints(x) };
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
