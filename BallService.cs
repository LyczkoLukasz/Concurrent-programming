public class BallService
{
    private List<Ball> balls;

    public BallService()
    {
        balls = new List<Ball>();
    }

    public void GenerateBalls(int count)
    {
        // Tutaj dodajemy logik� generowania kul
    }

    public void UpdateBalls()
    {
        // Tutaj dodajemy logik� aktualizacji pozycji kul
    }

    public List<Ball> GetBalls()
    {
        return balls;
    }
}
