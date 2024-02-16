namespace Models
{
    public class Partition
    {
        private readonly float a;
        private readonly float b; 
        private readonly float delta;
        private readonly int count;

        private readonly float[] dots;

        public int Count { get { return count; } }

        public Partition(float a, float b, float delta) {
            if (b < a) { throw new ArgumentException("a must be less then b"); }

            float length = b - a;

            count = (int)(Math.Ceiling(length / delta) + 1);

            dots = new float[count];

            for (int i = 0; i < count; ++i)
            {
                dots[i] = a + i * delta;
            }
            dots[count - 1] = b;
        }

        public float this[int index]
        {
            get { return dots[index]; }
        }
    }
}