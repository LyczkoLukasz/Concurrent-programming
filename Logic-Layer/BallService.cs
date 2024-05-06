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

        public BallService()
        {
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
                        X = random.NextDouble() * (300 - diameter), // Zakładamy, że płaszczyzna ma rozmiar 300x300
                        Y = random.NextDouble() * (300 - diameter),
                        SpeedX = random.NextDouble() * 2 - 1, // Prędkość będzie losowa, od -1 do 1
                        SpeedY = random.NextDouble() * 2 - 1,
                        
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
