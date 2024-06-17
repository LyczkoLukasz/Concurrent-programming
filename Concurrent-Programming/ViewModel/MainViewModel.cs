using Data_Layer;
using Logic_Layer;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Timers; // Dodaj przestrzeń nazw dla System.Timers

namespace Concurrent_Programming.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private ObservableCollection<Ball> balls = new ObservableCollection<Ball>();
        public ObservableCollection<Ball> Balls
        {
            get { return balls; }
            set
            {
                balls = value;
                OnPropertyChanged(nameof(Balls));
            }
        }

        private BallService ballService;
        private bool isRunning;
        private System.Timers.Timer updateTimer; // Użyj pełnej nazwy przestrzeni nazw

        private int ballCount;
        public int BallCount
        {
            get => ballCount;
            set
            {
                ballCount = value;
                OnPropertyChanged(nameof(BallCount));
            }
        }

        public ICommand StartCommand { get; }
        public ICommand StopCommand { get; }

        public MainWindowViewModel()
        {
            var rectangle = new MovementRectangle { Width = 300, Height = 300 };
            ballService = new BallService(rectangle);

            StartCommand = new RelayCommand(_ => StartSimulation(), _ => !isRunning);
            StopCommand = new RelayCommand(_ => StopSimulation(), _ => isRunning);

            BallCount = 10; // Wartość domyślna liczby kulek

            // Inicjalizacja Timer
            updateTimer = new System.Timers.Timer(16); // Aktualizuj co 16 ms (około 60 razy na sekundę)
            updateTimer.Elapsed += UpdateBalls;
        }

        public void StartSimulation()
        {
            ballService.GenerateBalls(BallCount);
            Balls.Clear();
            foreach (var ball in ballService.GetBalls())
            {
                Balls.Add(ball);
            }

            isRunning = true;
            ((RelayCommand)StopCommand).OnCanExecuteChanged();
            ((RelayCommand)StartCommand).OnCanExecuteChanged();

            updateTimer.Start(); // Uruchom Timer
        }

        public void StopSimulation()
        {
            isRunning = false;
            ((RelayCommand)StopCommand).OnCanExecuteChanged();
            ((RelayCommand)StartCommand).OnCanExecuteChanged();

            updateTimer.Stop(); // Zatrzymaj Timer

            ballService.ClearBalls();
            Balls.Clear();
        }

        // Aktualizacja kulek przy każdym wywołaniu Timera
        private async void UpdateBalls(object? sender, ElapsedEventArgs e)
        {
            await ballService.UpdateBallsAsync();
            OnPropertyChanged(nameof(Balls)); // Powiadamianie o zmianach w kulkach
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
