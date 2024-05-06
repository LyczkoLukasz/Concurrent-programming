using Data_Layer;
using Logic_Layer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrent_Programming
{
    public class BallViewModel : INotifyPropertyChanged
    {
        private BallService ballService;
        private ObservableCollection<Ball> balls;

        public ObservableCollection<Ball> Balls
        {             
            get { return balls; }
            set
            {
                balls = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Balls)));
            }
        }

        public BallViewModel()
        {
            ballService = new BallService();
            balls = new ObservableCollection<Ball>(ballService.GetBalls());
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
