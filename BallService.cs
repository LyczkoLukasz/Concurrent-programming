public class BallService
{
    private List<Ball> balls;

    public BallService()
    {
        balls = new List<Ball>();
    }

    public void GenerateBalls(int count)
    {
        // Tutaj dodajemy logikê generowania kul
    }

    public void UpdateBalls()
    {
        // Tutaj dodajemy logikê aktualizacji pozycji kul
    }

    public List<Ball> GetBalls()
    {
        return balls;
    }
}
