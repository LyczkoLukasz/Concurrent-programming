using System.ComponentModel;

namespace Concurrent_Programming.ViewModel
{
    public class BallViewModel : INotifyPropertyChanged
    {
        private double x;
        public double X
        {
            get => x;
            set
            {
                if (x != value)
                {
                    x = value;
                    OnPropertyChanged(nameof(X));
                }
            }
        }

        private double y;
        public double Y
        {
            get => y;
            set
            {
                if (y != value)
                {
                    y = value;
                    OnPropertyChanged(nameof(Y));
                }
            }
        }

        public double SpeedX { get; set; }
        public double SpeedY { get; set; }
        public double Weight { get; set; }
        public double Diameter { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
