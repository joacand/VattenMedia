using System.Windows;
using Unity;
using VattenMedia.ViewModels;
using VattenMedia.Views;

namespace VattenMedia
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            using IUnityContainer container = new UnityContainer();
            container.AddRegistrations();

            var mainWindowViewModel = container.Resolve<MainWindowViewModel>();
            var window = new MainWindow { DataContext = mainWindowViewModel };
            window.Show();
        }
    }
}
