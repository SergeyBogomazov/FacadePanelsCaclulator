using Models;

namespace FacadeCalculator
{
    public interface ICalculator
    {
        public IEnumerable<Panel> GetPanelsToCoverProfile(Point[] facadePoints, Size profileSize);
    }
}
