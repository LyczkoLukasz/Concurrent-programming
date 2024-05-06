using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private DispatcherTimer timer;

        public MainViewModel()
        {
            Balls = new ObservableCollection<Ball>();
            StartCommand = new RelayCommand(Start, CanStart);
            StopCommand = new RelayCommand(Stop, CanStop);
            this.ballService = new BallService();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(16); //16 ms poniewaz 1000ms/60fps = 16.(6) wiec 16ms odpowiada nieco ponad 60fps
            timer.Tick += Timer_Tick;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool CanStart(object parameter)
        {
            return !timer.IsEnabled;
        }

        private void Start(object parameter)
        {
            ballService.GenerateBalls(InitialBallCount);
            timer.Start();
        }

        private bool CanStop(object parameter)
        {
            return timer.IsEnabled;
        }

        private void Stop(object parameter)
        {
            timer.Stop();
            ballService.ClearBalls();
            Balls.Clear();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            ballService.UpdateBalls();
            Balls.Clear();
            foreach (var ball in ballService.GetBalls())
            {
                Balls.Add(ball);
            }
        }
    }
}
