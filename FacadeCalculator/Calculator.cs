using FacadeCalculator.Exceptions;
using Models;
using System.Collections.Concurrent;

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

            // создаём фигуру
            var figure = new ConvexFigure(facadePoints);

            // создаём разбиение
            var partitionX = new Partition(figure.GetMinX(), figure.GetMaxX(), panelSize.Width);

            // находим длины
            var lengths = GetLengths(figure, partitionX);

            // находим панели
            var panels = GetPanels(lengths, panelSize);

            return panels.OrderByDescending(x => x.size.Height);
        }

        private Stack<float> GetLengths(ConvexFigure figure, Partition partitionX)
        {
            var cutLengths = new Stack<float>();

            // пробегаемся по отрезкам разбиения 
            for (int i = 0; i < partitionX.Count - 1; ++i)
            {
                var p1 = partitionX[i];
                var p2 = partitionX[i + 1];

                // В отрезок разбиения могут входить как отдельные точки, так и сегменты
                List<Point> extremumPoints = new List<Point>();
                List<LineSegment> segments = new List<LineSegment>();

                // соберём точки фасада, которые входят в отрезок разбиения
                foreach (Point p in figure.points)
                {
                    if (p.X >= p1 && p.X <= p2)
                    {
                        extremumPoints.Add(p);
                    }
                }

                // соберём сегменты, которые сможем пересеч
                foreach (LineSegment segment in figure.segments)
                {
                    if (segment.ContainsX(p1) || segment.ContainsX(p2))
                    {
                        segments.Add(segment);
                    }
                }

                // пройдёмся по собранным сегментам и добавим точки пересечения сегмента и границ отрезка разбиения
                // добавляется не одна точка, а коллекция, потому что экстремумов может быть два.
                foreach (var segment in segments)
                {
                    extremumPoints.AddRange(segment.GetIntersectionExtremumsByX(p1));
                    extremumPoints.AddRange(segment.GetIntersectionExtremumsByX(p2));
                }

                // отбираем экстремумы из собранных точек
                float minY = Single.MaxValue;
                float maxY = Single.MinValue;

                foreach (var point in extremumPoints)
                {
                    if (minY > point.Y) { minY = point.Y; }
                    if (maxY < point.Y) { maxY = point.Y; }
                }

                // добавляем полученную длину панели
                cutLengths.Push(maxY - minY);
            }

            return cutLengths;
        }

        private IEnumerable<Panel> GetPanels(Stack<float> lengths, Size panelSize)
        {
            List<Panel> panels = new List<Panel>();
            List<Panel> panelsPull = new List<Panel>();

            while (lengths.Count > 0)
            {
                AddPanelToResultWithLength(lengths.Pop());
            }

            return panels;

            void AddPanelToResultWithLength(float length)
            {
                while (length >= panelSize.Height)
                {
                    panels.Add(new Panel(panelSize));

                    length -= panelSize.Height;
                }

                if (length == 0) { return; }

                Panel panelToCut = null;

                foreach (var panel in panelsPull)
                {
                    if (panel.CanCut(length))
                    {
                        if (panelToCut == null)
                        {
                            panelToCut = panel;
                        }
                        else if (panelToCut.size.Height > panel.size.Height)
                        {
                            panelToCut = panel;
                        }
                    }
                }

                if (panelToCut == null)
                {
                    panelToCut = new Panel(panelSize);
                    panelsPull.Add(panelToCut);
                }

                panels.Add(panelToCut.Cut(length));
            }
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
