namespace Models
{
    public struct Size
    {
        private float _height;
        private float _width;

        public float Height
        {
            get => _height;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }

                _height = value;
            }
        }

        public float Width
        {
            get => _width;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException();
                }

                _width = value;
            }
        }

        public Size(float width, float height) {
            if (height <= 0 || width <= 0)
            {
                throw new ArgumentException();
            }

            _height = height;
            _width = width;
        }
    }
}
