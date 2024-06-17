using System;
using System.ComponentModel;

namespace Data_Layer
{
    public class Ball : INotifyPropertyChanged
    {
        private double x;
        public double X
        {
            get { return x; }
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
            get { return y; }
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
