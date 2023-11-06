using Models;

namespace ApiModels
{
    public class FacadeData
    {
        public Size PanelSize { get; set; } = Constants.defaultPanelSize;
        public Point[] Profile { get; set; }
    }
}

