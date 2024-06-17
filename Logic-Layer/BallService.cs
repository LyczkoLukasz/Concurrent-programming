using Data_Layer;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer
{
    public class BallService
    {
        private List<Ball> balls;
        private readonly object lockObject = new object();
        private MovementRectangle rectangle;
        private readonly string logFilePath = "ball_diagnostics.json";
        private static readonly object fileLock = new object();

        public BallService(MovementRectangle rectangle)
        {
            this.rectangle = rectangle;
            balls = new List<Ball>();
        }

        public void GenerateBalls(int count)
        {
            Random random = new Random();
            lock (lockObject)
            {
                for (int i = 0; i < count; i++)
                {
                    double diameter = 10;
                    Ball ball = new Ball
                    {
                        Diameter = diameter,
                        X = random.NextDouble() * (rectangle.Width - diameter),
                        Y = random.NextDouble() * (rectangle.Height - diameter),
                        SpeedX = random.NextDouble() * 2 - 1,
                        SpeedY = random.NextDouble() * 2 - 1,
                        Weight = 2
                    };
                    balls.Add(ball);
                }
            }
        }

        public async Task UpdateBallsAsync()
        {
            await Task.Run(() =>
            {
                lock (lockObject)
                {
                    foreach (var ball in balls)
                    {
                        ball.X += ball.SpeedX;
                        ball.Y += ball.SpeedY;

                        if (ball.X < 0 || ball.X > (rectangle.Width - ball.Diameter)) ball.SpeedX = -ball.SpeedX;
                        if (ball.Y < 0 || ball.Y > (rectangle.Height - ball.Diameter)) ball.SpeedY = -ball.SpeedY;

                        foreach (var otherBall in balls)
                        {
                            if (ball != otherBall && IsColliding(ball, otherBall))
                            {
                                HandleCollision(ball, otherBall);
                            }
                        }
                    }
                }
            });

            await LogDiagnosticsAsync();
        }

        private bool IsColliding(Ball ball1, Ball ball2)
        {
            double dx = ball1.X - ball2.X;
            double dy = ball1.Y - ball2.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance < (ball1.Diameter + ball2.Diameter) / 2;
        }

        private void HandleCollision(Ball ball, Ball otherBall)
        {
            double dx = otherBall.X - ball.X;
            double dy = otherBall.Y - ball.Y;

            double collisionAngle = Math.Atan2(dy, dx);

            double speed1 = Math.Sqrt(ball.SpeedX * ball.SpeedX + ball.SpeedY * ball.SpeedY);
            double speed2 = Math.Sqrt(otherBall.SpeedX * otherBall.SpeedX + otherBall.SpeedY * otherBall.SpeedY);

            double direction1 = Math.Atan2(ball.SpeedY, ball.SpeedX);
            double direction2 = Math.Atan2(otherBall.SpeedY, otherBall.SpeedX);

            double newSpeedX1 = speed1 * Math.Cos(direction1 - collisionAngle);
            double newSpeedY1 = speed1 * Math.Sin(direction1 - collisionAngle);
            double newSpeedX2 = speed2 * Math.Cos(direction2 - collisionAngle);
            double newSpeedY2 = speed2 * Math.Sin(direction2 - collisionAngle);

            double finalSpeedX1 = ((ball.Weight - otherBall.Weight) * newSpeedX1 + (otherBall.Weight + otherBall.Weight) * newSpeedX2) / (ball.Weight + otherBall.Weight);
            double finalSpeedY1 = newSpeedY1;

            double finalSpeedX2 = ((ball.Weight + ball.Weight) * newSpeedX1 + (otherBall.Weight - ball.Weight) * newSpeedX2) / (ball.Weight + otherBall.Weight);
            double finalSpeedY2 = newSpeedY2;

            ball.SpeedX = Math.Cos(collisionAngle) * finalSpeedX1 + Math.Cos(collisionAngle + Math.PI / 2) * finalSpeedY1;
            ball.SpeedY = Math.Sin(collisionAngle) * finalSpeedX1 + Math.Sin(collisionAngle + Math.PI / 2) * finalSpeedY1;

            otherBall.SpeedX = Math.Cos(collisionAngle) * finalSpeedX2 + Math.Cos(collisionAngle + Math.PI / 2) * finalSpeedY2;
            otherBall.SpeedY = Math.Sin(collisionAngle) * finalSpeedX2 + Math.Sin(collisionAngle + Math.PI / 2) * finalSpeedY2;
        }

        private async Task LogDiagnosticsAsync()
        {
            List<object> diagnosticsData;
            lock (lockObject)
            {
                diagnosticsData = new List<object>();
                foreach (var ball in balls)
                {
                    diagnosticsData.Add(new
                    {
                        ball.X,
                        ball.Y,
                        ball.SpeedX,
                        ball.SpeedY,
                        ball.Weight,
                        ball.Diameter
                    });
                }
            }

            var json = JsonConvert.SerializeObject(diagnosticsData, Newtonsoft.Json.Formatting.Indented);

            await Task.Run(() =>
            {
                lock (fileLock)
                {
                    File.WriteAllText(logFilePath, json);
                }
            });

            // Tymczasowe wyjście konsolowe, aby sprawdzić ścieżkę do pliku
            Console.WriteLine($"Log file saved to: {Path.GetFullPath(logFilePath)}");
        }

        public List<Ball> GetBalls()
        {
            lock (lockObject)
            {
                return new List<Ball>(balls);
            }
        }

        public void ClearBalls()
        {
            lock (lockObject)
            {
                balls.Clear();
            }
        }
    }
}
