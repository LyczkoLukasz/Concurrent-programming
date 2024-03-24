using Xunit;
using Concurrent_Programming;


namespace Test_Concurrent
{
    public class UnitTest1
    {
        [StaFact]
        //[STAThread]
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
    }
}