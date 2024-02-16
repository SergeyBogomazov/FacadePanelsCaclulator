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

                // добавляем полученную длину панели для этого разбиения
                cutLengths.Push(GetLength(figure, p1, p2));
            }

            return cutLengths;
        }

        private float GetLength(ConvexFigure figure, float left, float right)
        {
            // отбираем экстремумы из собранных точек
            float minY = Single.MaxValue;
            float maxY = Single.MinValue;

            // соберём точки фасада, которые входят в отрезок разбиения
            foreach (Point p in figure.points)
            {
                if (p.X >= left && p.X <= right)
                {
                    UpdateResult(p);
                }
            }

            // соберём сегменты, которые сможем пересеч
            foreach (LineSegment segment in figure.segments)
            {
                if (segment.ContainsX(left) || segment.ContainsX(right))
                {
                    UpdateResultBySegment(segment);
                }
            }

            return maxY - minY;

            void UpdateResultBySegment(LineSegment segment)
            {
                var a = segment.GetIntersectionExtremumsByX(left);
                var b = segment.GetIntersectionExtremumsByX(right);

                foreach (var c in a) { UpdateResult(c); }
                foreach (var c in b) { UpdateResult(c); }
            }

            void UpdateResult(Point p)
            {
                if (p.Y < minY) { minY = p.Y; };
                if (p.Y > maxY) { maxY = p.Y; }
            }
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

                Panel panelToCut = GetPanelFromPull(length);

                if (panelToCut == null)
                {
                    panelToCut = new Panel(panelSize);
                    panelsPull.Add(panelToCut);
                }

                panels.Add(panelToCut.Cut(length));
            }

            Panel GetPanelFromPull(float len)
            {
                Panel res = null;

                foreach (var panel in panelsPull)
                {
                    if (panel.CanCut(len))
                    {
                        if (res == null)
                        {
                            res = panel;
                        }
                        else if (res.size.Height > panel.size.Height)
                        {
                            res = panel;
                        }
                    }
                }

                return res;
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
