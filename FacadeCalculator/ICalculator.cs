using Models;

namespace FacadeCalculator
{
    internal interface ICalculator
    {
        public Task<Panel[]> GetPanelsToCoverProfile(Point[] facadePoints, Size profileSize);
    }
}
