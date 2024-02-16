using Models;

namespace FacadeCalculator
{
    public interface ICalculator
    {
        public Task<IEnumerable<Panel>> GetPanelsToCoverProfile(Point[] facadePoints, Size profileSize);
    }
}
