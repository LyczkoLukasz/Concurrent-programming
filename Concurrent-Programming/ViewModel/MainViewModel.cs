using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Logic_Layer;
using Data_Layer;
using System.Windows.Threading;
using System.Threading.Tasks;

namespace Concurrent_Programming.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int initialBallCount;
        public int InitialBallCount
        {
            get { return initialBallCount; }
            set
            {
                if (initialBallCount != value)
                {
                    initialBallCount = value;
                    OnPropertyChanged("InitialBallCount");
                }
            }
        }

        public ObservableCollection<Ball> Balls { get; set; }

        public ICommand StartCommand { get; set; }
        public ICommand StopCommand { get; set; }

        private BallService ballService;
        private CancellationTokenSource cts;

        public MainViewModel()
        {
            Balls = new ObservableCollection<Ball>();
            StartCommand = new RelayCommand(Start, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);
            var rectangle = new MovementRectangle { Width = 300, Height = 300 };
            this.ballService = new BallService(rectangle);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool CanStart(object parameter)
        {
            return cts == null || cts.IsCancellationRequested;
        }

        private async void Start(object parameter)
        {
            ballService.GenerateBalls(InitialBallCount);
            cts = new CancellationTokenSource();
            await UpdateBallsAsync(cts.Token);
        }

        private bool CanStop(object parameter)
        {
            return cts != null && !cts.IsCancellationRequested;
        }

        private void Stop(object parameter)
        {
            if (cts != null && !cts.IsCancellationRequested)
            {
                cts.Cancel();
                ballService.ClearBalls();
                Balls.Clear();
            }
        }

        private async Task UpdateBallsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    ballService.UpdateBalls();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Balls.Clear();
                        foreach (var ball in ballService.GetBalls())
                        {
                            Balls.Add(ball);
                        }
                    });

                    await Task.Delay(16, cancellationToken); // Aktualizuj co 16 ms
                }
                catch (TaskCanceledException)
                {
                    // Zadanie zostało anulowane, nie rób nic
                }
            }
        }

    }
}
