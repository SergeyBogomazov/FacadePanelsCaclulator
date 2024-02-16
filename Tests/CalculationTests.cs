using Xunit;
using Models;
using FacadeCalculator;
using FacadeCalculator.Exceptions;

namespace Tests
{
    public class CalculationTests
    {
        private readonly Size defaultPanelSize = new Size(500f, 13500f);

        [Fact]
        public void CheckingWrongFacade1()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = defaultPanelSize;
            var facade = new Point[] {
            };

            //Assert
            Assert.Throws<InvalidFacadeException>(() => caclulator.GetPanelsToCoverProfile(facade, profileSize));
        }

        [Fact]
        public void CheckingWrongFacade2()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = defaultPanelSize;
            var facade = new Point[] {
                new Point(0, 0),
                new Point(99, 500),
            };

            Assert.Throws<InvalidFacadeException>(() => caclulator.GetPanelsToCoverProfile(facade, profileSize));
        }

        [Fact]
        public void CheckingWrongFacade3()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = defaultPanelSize;
            var facade = new Point[] {
                new Point(0, 0),
                new Point(0, 500),
                new Point(0, 1500),
                new Point(0, 999),
            };

            Assert.Throws<InvalidFacadeException>(() => caclulator.GetPanelsToCoverProfile(facade, profileSize));
        }

        [Fact]
        public void CheckingWrongFacade4()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = defaultPanelSize;
            var facade = new Point[] {
                new Point(123, 100),
                new Point(434, 100),
                new Point(322, 100),
                new Point(434, 100),
            };

            Assert.Throws<InvalidFacadeException>(() => caclulator.GetPanelsToCoverProfile(facade, profileSize));
        }

        [Fact]
        public void CheckingWrongFacade5()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = defaultPanelSize;
            var facade = new Point[] {
                new Point(123, 100.12f),
                new Point(434, 100.12f),
                new Point(322, 100.12f),
                new Point(434, 100.12f),
            };

            Assert.Throws<InvalidFacadeException>(() => caclulator.GetPanelsToCoverProfile(facade, profileSize));
        }

        [Fact]
        public void CalculateQuadFacade()
        { 
            //Arrange
            var caclulator = new Calculator();

            var profileSize = defaultPanelSize;
            var facade = new Point[] { 
                new Point(0, 0),
                new Point(0, 500),
                new Point(400, 0),
                new Point(400, 500),
            };

            //Act
            var panels = caclulator.GetPanelsToCoverProfile(facade, profileSize);
            var result = panels.Select(p => p.size.Height);

            //Assert
            var expected = new float[] { 500f};
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateQuadFacadeLittleOverflow()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = defaultPanelSize;
            var facade = new Point[] {
                new Point(0, 0),
                new Point(100, 510),
                new Point(510, 500),
                new Point(300, 100),
            };

            //Act
            var panels = caclulator.GetPanelsToCoverProfile(facade, profileSize);
            var result = panels.Select(p => p.size.Height);

            //Assert
            Assert.True(panels.Count() == 2 && panels.Any(p => p.size.Height == 510f) && panels.Any(p => p.size.Height < 200f));
        }

        [Fact]
        public void CalculateFacadeQuadAndTriangle()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = defaultPanelSize;
            var facade = new Point[] {
                new Point(0, 0),
                new Point(0, 100),
                new Point(500, 0),
                new Point(500, 100),
                new Point(750, 50),
            };

            //Act
            var panels = caclulator.GetPanelsToCoverProfile(facade, profileSize);
            var result = panels.Select(p => p.size.Height);

            //Assert
            var expected = new float[] { 100f, 100f };
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateFacadeQuadAndTriangle2()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = new Size(150, 2000);
            var facade = new Point[] {
                new Point(0, 0),
                new Point(0, 100),
                new Point(100, 0),
                new Point(100, 100),
                new Point(200, 50),
            };

            //Act
            var panels = caclulator.GetPanelsToCoverProfile(facade, profileSize);
            var result = panels.Select(p => p.size.Height);

            //Assert
            var expected = new float[] { 100f, 50f };
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateHeightOverflowFacade()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = new Size(150, 2000);
            var facade = new Point[] {
                new Point(0, 0),
                new Point(0, 5500f),
                new Point(100, 0),
                new Point(100, 100),
            };

            //Act
            var panels = caclulator.GetPanelsToCoverProfile(facade, profileSize);
            var result = panels.Select(p => p.size.Height);

            //Assert
            var expected = new float[] { 2000f, 2000f, 1500f };
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateHeightOverflowFacade1()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = new Size(150, 2000);
            var facade = new Point[] {
                new Point(0, 0),
                new Point(0, 2001f),
                new Point(100, 0),
                new Point(100, 100),
            };

            //Act
            var panels = caclulator.GetPanelsToCoverProfile(facade, profileSize);
            var result = panels.Select(p => p.size.Height);

            //Assert
            var expected = new float[] { 2000f, 1f };
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateFacadeTriangle()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = new Size(150, 2000);
            var facade = new Point[] {
                new Point(0, 0),
                new Point(0, 100),
                new Point(100, 0),
            };

            //Act
            var panels = caclulator.GetPanelsToCoverProfile(facade, profileSize);
            var result = panels.Select(p => p.size.Height);

            //Assert
            var expected = new float[] { 100f};
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateFacadeRomb()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = new Size(150, 2000);
            var facade = new Point[] {
                new Point(0, 100),
                new Point(100, -100),
                new Point(100, 300),
                new Point(200, 100),
            };

            //Act
            var panels = caclulator.GetPanelsToCoverProfile(facade, profileSize);
            var result = panels.Select(p => p.size.Height);

            //Assert
            var expected = new float[] { 400f, 200f };
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateFacadeDifficult1()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = new Size(500, 2000);
            var facade = new Point[] {
                new Point(-100, -100),
                new Point(100, 0),
                new Point(-75, 300),
                new Point(120, 270),
                new Point(50, 500),
                new Point(75, 550),
            };

            //Assert
            Assert.Throws<NotConvexFigure>(() => caclulator.GetPanelsToCoverProfile(facade, profileSize));
        }

        [Fact]
        public void CalculateFacadeDifficult2()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = new Size(500, 2000);
            var facade = new Point[] {
                new Point(-100, -100),
                new Point(100, 0),
                new Point(-75, 300),
                new Point(120, 270),
                new Point(50, 400),
            };

            //Act
            var panels = caclulator.GetPanelsToCoverProfile(facade, profileSize);
            var result = panels.Select(p => p.size.Height);

            //Assert
            var expected = new float[] { 500f };
            Assert.Equal(expected, result);
        }

        [Fact]
        public void CalculateDifficultFacade()
        {
            //Arrange
            var caclulator = new Calculator();

            var profileSize = defaultPanelSize;
            var facade = new Point[] {
                new Point(0, 0),
                new Point(-1703, 5838),
                new Point(5949, 9964),
                new Point(10494, 7549),
                new Point(10494, 339),
            };

            //Act
            var panels = caclulator.GetPanelsToCoverProfile(facade, profileSize);

            //Assert
            Assert.True(panels.Count() == 25 && panels.Max(p => p.size.Height) < 9800f);
        }
    }
}
