namespace Models
{
    public class Figure
    {
        public readonly Point[] points;

        private float? minX;
        private float? maxX;

        public Figure(Point[] points) { 
            this.points = points;
        }

        public float GetMinX()
        {
            if (minX != null) { return (float)minX; }

            float min = 0;

            foreach (Point point in points)
            {
                if (point.X < min)
                { 
                    min = point.X;
                }
            }

            minX = min;
            return min;
        }

        public float GetMaxX()
        {
            if (maxX != null) { return (float)maxX; }

            float max = 0;

            foreach (Point point in points)
            {
                if (point.X > max)
                {
                    max = point.X;
                }
            }

            maxX = max;
            return max;
        }
    }

    public class ConvexFigure : Figure {
        
        public readonly LineSegment[] segments;

        public ConvexFigure(Point[] points) : base(points)
        {
            segments = GetSegmentsOfFigure(points);
        }

        private LineSegment[] GetSegmentsOfFigure(Point[] points)
        {
            // здесь будут храниться отрезки фигуры
            LineSegment[] segments = new LineSegment[points.Length];
            int segmentsPointer = 0;

            // здесь отмечаем точки, которые уже заняты
            bool[] pointsTaken = new bool[points.Length];

            // инициализируем последнюю точку
            pointsTaken[0] = true;
            int lastPoint = 0;

            while (segmentsPointer != points.Length - 1)
            {
                bool found = false;

                // ищем соседа для последней точки
                for (int i = 0; i < points.Length; ++i)
                {
                    if (pointsTaken[i]) { continue; }

                    // этот счётчик считает сколько точек по какую сторону расположено от отрезка
                    // если точка по одну сторону, то он увеличивается, если по другую уменьшается
                    // нам нужно чтобы все точки были по одну сторону, то есть чтобы этот счётчик
                    // по модулю был равен количеству всех точек кроме пары, которая образует отрезок
                    var sidesCounter = 0;

                    // выбранные точки для постройки линии
                    var p1 = points[lastPoint];
                    var p2 = points[i];

                    var segment = new LineSegment(p1, p2);

                    // отбираем точки и смотрим с какой стороны от линии они лежат
                    for (int k = 0; k < points.Length; ++k)
                    {
                        if (k == i || k == lastPoint) { continue; }

                        var point3 = points[k];

                        sidesCounter += segment.PointSideByLine(point3);
                    }

                    // по определнию выпуклой фигуры, все точки должны лежать по одну сторону от линии,
                    // если это не так, то данная точка нам не пара, идём к следующей
                    if (Math.Abs(sidesCounter) != points.Length - 2)
                    {
                        continue;
                    }

                    found = true;

                    pointsTaken[i] = true;
                    lastPoint = i;

                    segments[segmentsPointer] = segment;
                    ++segmentsPointer;

                    break;
                }

                if (!found)
                {
                    throw new NotConvexFigure();
                }
            }

            // замыкаем фигуру, соединяя первую и последнюю точку
            segments[segmentsPointer] = new LineSegment(points[0], points[lastPoint]);

            return segments;
        }
    }
}
