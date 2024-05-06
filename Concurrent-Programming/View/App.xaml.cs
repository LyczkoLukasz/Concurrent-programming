using System.Configuration;
using System.Data;
using System.Windows;

namespace Concurrent_Programming.View
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        [STAThread]
        public static void Main()
        {
            App app = new App();
            MainWindow mainWindow = new MainWindow();
            app.Run(mainWindow);
        }
    }

}
