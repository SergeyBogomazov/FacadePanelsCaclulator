namespace Models
{
    public struct Point : IEquatable<Point>
    {
        public readonly float X { get; }
        public readonly float Y { get; }

        public Point(float x, float y)
        { 
            X = x; 
            Y = y;
        }

        public bool Equals(Point other)
        {
            return other.X == X && other.Y == Y;
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
