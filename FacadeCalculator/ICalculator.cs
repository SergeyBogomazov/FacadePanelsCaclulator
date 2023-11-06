using Models;

namespace FacadeCalculator
{
    internal interface ICalculator
    {
        public Task<IEnumerable<Point>> GetPanelsToCoverProfile(Point[] facadePoints, Size profileSize);
    }
}
