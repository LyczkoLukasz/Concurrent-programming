using Xunit;
using Concurrent_Programming;
using Concurrent_Programming.View;
using Data_Layer;
using Logic_Layer;


namespace Test_Concurrent
{

    
    public class UnitTest1
    {
        [StaFact]
        //[STAThread]
        //Test okno
        public void Test1()
        {
            // Arrange
            var window = new MainWindow();

            // Act
            window.SetTitle("Test Title");

            // Assert
            Assert.Equal("Test Title", window.Title);
            Assert.Equal(20 + 20, 40);
            Assert.Equal(1 * 2, 2);

        }
        //test kulka
        public void Test2()
        {
            Ball kulka = new Ball();
            kulka.Diameter = 10;
            Assert.Equal(10, kulka.Diameter);
        }


        //test logika kulka
        public void Test3()
        {
            BallService ballService = new BallService();
            ballService.GenerateBalls(10);
            Assert.Equal(10, ballService.GetBallsCount());
        }
    }

}