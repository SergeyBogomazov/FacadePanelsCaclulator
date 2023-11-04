namespace Models
{
    public class Panel
    {
        private Size _size;
        public Size size => _size;

        public Panel(Size size)
        {
            _size = size;
        }

        public bool CanCut(float height)
        { 
            return height > 0 && height <= size.Height;
        }

        public Panel Cut(float height)
        {
            _size.Height -= height;

            return new Panel(new Size(_size.Width, height));
        }
    }
}
