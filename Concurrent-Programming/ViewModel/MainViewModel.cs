using Data_Layer;
using Logic_Layer;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

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

            BallCount = 10; // Default value
        }

        public async void StartSimulation()
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

            await UpdateBallsAsync();
        }

        private async Task UpdateBallsAsync()
        {
            while (isRunning)
            {
                await ballService.UpdateBallsAsync();
                OnPropertyChanged(nameof(Balls)); // Powiadamianie o zmianach w kulkach
                await Task.Delay(16); // Aktualizuj co 16 ms
            }
        }

        public void StopSimulation()
        {
            isRunning = false;
            ((RelayCommand)StopCommand).OnCanExecuteChanged();
            ((RelayCommand)StartCommand).OnCanExecuteChanged();

            ballService.ClearBalls();
            Balls.Clear();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
