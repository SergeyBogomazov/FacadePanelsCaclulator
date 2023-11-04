namespace ApiModels
{
    public class Point
    {
        public float X { get; set; }
        public float Y { get; set; }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }
}
