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

        public bool ContainsPointByX(Point point)
        {
            return point.X >= minX && point.X <= maxX;
        }

        public List<Point> GetIntersectionExtremumsByX(Point point)
        {
            var result = new List<Point>();

            if (!ContainsPointByX(point))
            {
                return result;
            }

            if (minX == maxX)
            {
                result.Add(a);
                result.Add(b);
            }
            else if (minY == maxY)
            {
                result.Add(new Point(point.X, minY));
            }
            else
            {
                float dx = (point.X - minX) / (maxX - minX);

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

                result.Add(new Point(point.X, dy));
            }

            return result;
        }

        public override string ToString()
        {
            return $"[{a},{b}]";
        }
    }
}
