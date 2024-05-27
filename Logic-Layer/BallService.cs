using Data_Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic_Layer
{
    public class BallService
    {
        private List<Ball> balls;
        private readonly object lockObject = new object();
        private MovementRectangle rectangle;

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
                    double diameter = 10; // srednica kulku, dyszka


                    Ball ball = new Ball
                    {
                        Diameter = diameter,
                        X = random.NextDouble() * (rectangle.Width - diameter), // Zakładamy, że płaszczyzna ma rozmiar 300x300
                        Y = random.NextDouble() * (rectangle.Height - diameter),
                        SpeedX = random.NextDouble() * 2 - 1, // Prędkość będzie losowa, od -1 do 1
                        SpeedY = random.NextDouble() * 2 - 1,
                        Weight = 2 // Waga kuli
                        
                    };

                    balls.Add(ball);
                }
            }
        }


        public void UpdateBalls()
        {
            lock (lockObject)
            {
                foreach (var ball in balls)
                {
                    ball.X += ball.SpeedX;
                    ball.Y += ball.SpeedY;

                    // Sprawdzamy czy kula nie wyszła poza płaszczyznę. Jeśli tak, odwracamy jej kierunek
                    if (ball.X < 0 || ball.X > (300 - ball.Diameter)) ball.SpeedX = -ball.SpeedX;
                    if (ball.Y < 0 || ball.Y > (300 - ball.Diameter)) ball.SpeedY = -ball.SpeedY;

                    // Sprawdzamy kolizje między kulami
                    foreach (var otherBall in balls)
                    {
                        if (otherBall == ball) continue;

                        double dx = ball.X - otherBall.X;
                        double dy = ball.Y - otherBall.Y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        if (distance < ball.Diameter / 2 + otherBall.Diameter / 2)
                        {
                            // Kule kolidują, obliczamy nowe prędkości
                            double angle = Math.Atan2(dy, dx);
                            double sin = Math.Sin(angle);
                            double cos = Math.Cos(angle);

                            // Prędkość kuli po kolizji
                            double speedX1 = ball.SpeedX * cos + ball.SpeedY * sin;
                            double speedY1 = ball.SpeedY * cos - ball.SpeedX * sin;

                            // Prędkość drugiej kuli po kolizji
                            double speedX2 = otherBall.SpeedX * cos + otherBall.SpeedY * sin;
                            double speedY2 = otherBall.SpeedY * cos - otherBall.SpeedX * sin;

                            // Finalna prędkość po kolizji
                            double finalSpeedX1 = ((ball.Weight - otherBall.Weight) * speedX1 + (otherBall.Weight + otherBall.Weight) * speedX2) / (ball.Weight + otherBall.Weight);
                            double finalSpeedY1 = speedY1;

                            // Finalna prędkość drugiej kuli po kolizji
                            double finalSpeedX2 = ((ball.Weight + ball.Weight) * speedX1 + (otherBall.Weight - ball.Weight) * speedX2) / (ball.Weight + otherBall.Weight);
                            double finalSpeedY2 = speedY2;

                            // Przypisujemy nowe prędkości kulom
                            ball.SpeedX = finalSpeedX1 * cos - finalSpeedY1 * sin;
                            ball.SpeedY = finalSpeedY1 * cos + finalSpeedX1 * sin;

                            otherBall.SpeedX = finalSpeedX2 * cos - finalSpeedY2 * sin;
                            otherBall.SpeedY = finalSpeedY2 * cos + finalSpeedX2 * sin;
                        }
                    }
                }
            }
        }



        public List<Ball> GetBalls()
        {
            lock (lockObject)
            {
                return balls; // Zwracamy kopię listy aby nie można było zmienić zawartości listy z zewnątrz
                //zapewniam w ten sposob spojnosc danych w wielowatkowym srodowisku
            }
            
        }

        public void ClearBalls()
        {
            lock (lockObject)
            {
                balls.Clear();
            }
        }

        public int GetBallsCount()
        {
            lock (lockObject)
            {
                return balls.Count;
            }
        }
    }

}
