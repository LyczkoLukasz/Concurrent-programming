using Concurrent_Programming.ViewModel;
using Data_Layer;
using Logic_Layer;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Test_Concurrent
{
    public class BallServiceTests
    {
        [Fact]
        public void GenerateBalls_ShouldCreateSpecifiedNumberOfBalls()
        {
            var rectangle = new MovementRectangle { Width = 300, Height = 300 };
            var ballService = new BallService(rectangle);
            ballService.GenerateBalls(10);

            var balls = ballService.GetBalls();
            Assert.Equal(10, balls.Count);
        }

        [Fact]
        public async Task UpdateBallsAsync_ShouldUpdateBallsPositions()
        {
            var rectangle = new MovementRectangle { Width = 300, Height = 300 };
            var ballService = new BallService(rectangle);
            ballService.GenerateBalls(1);
            var initialPosition = ballService.GetBalls().First().X;

            await ballService.UpdateBallsAsync();
            var newPosition = ballService.GetBalls().First().X;

            Assert.NotEqual(initialPosition, newPosition);
        }

        [Fact]
        public async Task LogDiagnosticsAsync_ShouldCreateLogFile()
        {
            var rectangle = new MovementRectangle { Width = 300, Height = 300 };
            var ballService = new BallService(rectangle);
            ballService.GenerateBalls(1);

            await ballService.UpdateBallsAsync();

            var logFilePath = "ball_diagnostics.json";
            Assert.True(File.Exists(logFilePath));

            var logContent = await File.ReadAllTextAsync(logFilePath);
            var diagnosticsData = JsonConvert.DeserializeObject<List<object>>(logContent);
            Assert.NotNull(diagnosticsData);
            Assert.Single(diagnosticsData);
        }

        [Fact]
        public async Task DiagnosticLogging_ShouldNotAffectBallBehavior()
        {
            var rectangle = new MovementRectangle { Width = 300, Height = 300 };
            var ballService = new BallService(rectangle);
            ballService.GenerateBalls(1);
            var initialPositions = ballService.GetBalls().Select(ball => (ball.X, ball.Y)).ToList();

            await ballService.UpdateBallsAsync();
            var positionsAfterFirstUpdate = ballService.GetBalls().Select(ball => (ball.X, ball.Y)).ToList();

            await ballService.UpdateBallsAsync();
            var positionsAfterSecondUpdate = ballService.GetBalls().Select(ball => (ball.X, ball.Y)).ToList();

            Assert.NotEqual(initialPositions, positionsAfterFirstUpdate);
            Assert.NotEqual(positionsAfterFirstUpdate, positionsAfterSecondUpdate);
        }

        [Fact]
        public async Task CollisionDetection_ShouldProtectDataIntegrity()
        {
            var rectangle = new MovementRectangle { Width = 300, Height = 300 };
            var ballService = new BallService(rectangle);
            ballService.GenerateBalls(2);

            var balls = ballService.GetBalls();
            balls[0].X = 50;
            balls[0].Y = 50;
            balls[0].SpeedX = 1;
            balls[0].SpeedY = 0;

            balls[1].X = 60;
            balls[1].Y = 50;
            balls[1].SpeedX = -1;
            balls[1].SpeedY = 0;

            await ballService.UpdateBallsAsync();
            var positionsAfterUpdate = ballService.GetBalls().Select(ball => (ball.X, ball.Y)).ToList();

            Assert.True(positionsAfterUpdate[0] != (50, 50));
            Assert.True(positionsAfterUpdate[1] != (60, 50));
        }
    }

    public class MainWindowViewModelTests
    {
        [Fact]
        public async Task BallsPositionOnScreen_ShouldBeSynchronized()
        {
            var viewModel = new MainWindowViewModel();
            viewModel.BallCount = 2;
            viewModel.StartSimulation();

            var initialPositions = viewModel.Balls.Select(ball => (ball.X, ball.Y)).ToList();
            await Task.Delay(100); // Poczekaj, aby kule mia³y czas na aktualizacjê

            var updatedPositions = viewModel.Balls.Select(ball => (ball.X, ball.Y)).ToList();
            viewModel.StopSimulation();

            Assert.NotEqual(initialPositions, updatedPositions);
        }
    }
}
