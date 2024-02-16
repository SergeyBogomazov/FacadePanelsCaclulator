using FacadeCalculator.Exceptions;
using Models;

namespace FacadeCalculator
{
    public class Calculator : ICalculator
    {
        public IEnumerable<Panel> GetPanelsToCoverProfile(Point[] facadePoints, Size panelSize)
        {
            if (IsFacadeDenerate(facadePoints))
            {
                throw new InvalidFacadeException();
            }
            if (panelSize.IsDenerate)
            { 
                throw new InvalidPanelException();
            }

            // получаем сегменты фигуры
            LineSegment[] lines = GetSegmentsOfFigure(facadePoints);

            Console.WriteLine($"Count of segments = {lines.Length}");
            Console.WriteLine(string.Join<LineSegment>(' ', lines));

            // находим крайние точки по оси Х
            var left = facadePoints[0];
            var right = facadePoints[0];

            foreach (var point in facadePoints)
            {
                if (point.X < left.X) { left = point; }
                if (point.X > right.X) { right = point; }
            }

            Console.WriteLine($"Left point = {left}");
            Console.WriteLine($"Right point = {right}");

            // Вычисляем ширину фасада чтобы узнать сколько точек разбиения нам понадобиться
            // Разбиение будем делать слева направо
            // Чтобы найти оптимальное решение, на мой взгляд нужно поиграть с левым отспупом в диапазоне (-длинаПрофиля, 0]
            float facadeLengthX = right.X - left.X;

            int pointsCount = (int)(Math.Ceiling(facadeLengthX / panelSize.Width) + 1);

            Console.WriteLine($"Facade width: {facadeLengthX}");
            Console.WriteLine($"pointCount = {pointsCount}");

            Point[] partitionX = new Point[pointsCount];

            // заполняем точки разбиения
            for (int i = 0; i < pointsCount; ++i)
            {
                float dot = left.X + i * panelSize.Width;

                partitionX[i] = new Point(dot, 0);
            }

            partitionX[pointsCount - 1] = new Point(right.X, 0);

            // panels содержить панели для установки
            // panelsPull содержит панели для нарезки, ниже будет подробно описано как работает пулл
            // cutLengths содержит длины панелей, которые нужно нарезать
            List<Panel> panels = new List<Panel>();
            List<Panel> panelsPull = new List<Panel>();
            List<float> cutLengths = new List<float>(); 

            // пробегаемся по отрезкам разбиения 
            for (int i = 0; i < partitionX.Length - 1; ++i)
            {
                var p1 = partitionX[i];
                var p2 = partitionX[i + 1];

                // В отрезок разбиения могут входить как отдельные точки, так и сегменты
                List<Point> extremumPoints = new List<Point>();
                List<LineSegment> segments = new List<LineSegment>();

                // соберём точки фасада, которые входят в отрезок разбиения
                foreach (Point p in facadePoints)
                {
                    if (p.X >= p1.X && p.X <= p2.X)
                    { 
                        extremumPoints.Add(p);
                    }
                }

                // соберём сегменты, которые сможем пересеч
                foreach (LineSegment segment in lines)
                {
                    var leftEnd = segment.a.X <= segment.b.X ? segment.a : segment.b;
                    var rightEnd = leftEnd.Equals(segment.a) ? segment.b : segment.a;

                    if (p1.X >= leftEnd.X && p1.X <= rightEnd.X)
                    {
                        segments.Add(segment);
                    }
                    else if (p2.X >= leftEnd.X && p2.X <= rightEnd.X)
                    {
                        segments.Add(segment);
                    }
                }

                // пройдёмся по собранным сегментам и добавим точки пересечения сегмента и границ отрезка разбиения
                // добавляется не одна точка, а коллекция, потому что экстремумов может быть два.
                foreach (var segment in segments)
                {
                    extremumPoints.AddRange(segment.GetIntersectionExtremumsByX(p1.X));
                    extremumPoints.AddRange(segment.GetIntersectionExtremumsByX(p2.X));
                }

                // отбираем экстремумы из собранных точек
                float minY = Single.MaxValue;
                float maxY = Single.MinValue;

                foreach (var point in extremumPoints)
                {
                    if (minY > point.Y)
                    { 
                        minY = point.Y;
                    }
                    if (maxY < point.Y)
                    { 
                        maxY = point.Y;
                    }
                }

                // добавляем полученную длину панели
                float panelLength = maxY - minY;
                cutLengths.Add(panelLength);
            }

            // здесь небольшая попытка сделать жадный алгоритм - сначала отрезать отрезки побольше,
            // а потом пройтись по мелким и попытаться получить их отрезая от остатков
            cutLengths.Sort();
            cutLengths.Reverse();

            foreach (var cut in cutLengths)
            {
                AddPanelToResultWithLength(cut);
            }

            return panels;

            // дабавляет панель нужной длины к результату, если длина больше панели, то будет добавлено несколько панелей.
            void AddPanelToResultWithLength(float length)
            {
                Console.WriteLine($"Try cat panel for length = {length}");

                while (length > 0)
                {
                    float lengthToCut = 0f;

                    lengthToCut = length > panelSize.Height ? panelSize.Height : length;
                    length -= lengthToCut;

                    Panel panelToCut = null;

                    // ищем подходящую панель
                    foreach (var panel in panelsPull)
                    {
                        if (!panel.CanCut(lengthToCut)) { continue; }

                        if (panelToCut == null) { panelToCut = panel; }
                        else if (panel.size.Height < panelToCut.size.Height) {
                            panelToCut = panel;
                        }
                    }

                    // если такой панели нет, то нужно в пулл добавить новую
                    // иначе берём первую, то есть самую короткую из подходящих
                    if (panelToCut == null)
                    {
                        panelToCut = new Panel(panelSize);
                        panelsPull.Add(panelToCut);
                    }

                    // далее от выбранной панели отрежется нужная длина,
                    // отрезанная панель добавиться в результат
                    // а выбранная панель укоротиться и останется в пулле

                    Console.WriteLine($"Try cut {lengthToCut} from panel with height {panelToCut.size.Height}");
                    panels.Add(panelToCut.Cut(lengthToCut));
                }
            }
        }

        public LineSegment[] GetSegmentsOfFigure(Point[] points)
        {
            // список указателей на массив точек, которые в итоге будут формировать "круг"
            // то есть для каждого указателя в массиве соседи указывают на точки, с которыми можно образовать сегмент
            List<int> ordered = new List<int>() { 0 };

            // здесь отмечаем точки, которые уже заняты
            bool[] pointsTaken = new bool[points.Length];
            pointsTaken[0] = true;

            while (ordered.Count != points.Length)
            {
                var pointer = ordered[ordered.Count - 1];
                bool found = false;

                // try find neighbour for pointer
                for (int i = 0; i < points.Length; ++i)
                {
                    if (pointsTaken[i]) { continue; }

                    // это счётчики - сколько точек лежит строго по левую или правую сторону от линии, проведённой через выбранные точки
                    var left = 0;
                    var right = 0;

                    // выбранные точки для постройки линии
                    var p1 = points[pointer];
                    var p2 = points[i];

                    var seg = new LineSegment(p1, p2);

                    for (int k = 0; k < points.Length; ++k)
                    {
                        // отбираем точки и смотрим с какой стороны от линии они лежат
                        if (k == i || k == pointer) { continue; }

                        var p3 = points[k];

                        

                        var side = seg.PointSideByLine(p3);

                        if (side < 0) { ++left; }
                        if (side > 0) { ++right; }
                    }

                    // по определнию выпуклой фигуры, все точки должны лежать по одну сторону от линии,
                    // если это не так, то данная точка нам не пара, идём к следующей
                    if (left != 0 && right != 0)
                    {
                        continue;
                    }

                    pointsTaken[i] = true;
                    found = true;
                    ordered.Add(i);
                    break;
                }

                if (!found)
                {
                    throw new NotConvexFigure();
                }
            }

            LineSegment[] segments = new LineSegment[ordered.Count];

            // add zero index to make pair with last element
            ordered.Add(0);

            for (int i = 0; i < ordered.Count - 1; ++i)
            {
                segments[i] = new LineSegment(points[ordered[i]], points[ordered[i + 1]]);
            }

            return segments;
        }

        public bool IsFacadeDenerate(Point[] data)
        {
            if (data.Length < 3) { return true; }

            var X = data[0].X;
            var Y = data[0].Y;

            return data.All(d => d.X == X) || data.All(d => d.Y == Y);
        }
    }
}
