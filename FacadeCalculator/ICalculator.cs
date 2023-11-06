using Models;

namespace FacadeCalculator
{
    internal interface ICalculator
    {
        public Task<IEnumerable<Panel>> GetPanelsToCoverProfile(Point[] facadePoints, Size profileSize);
    }
}
