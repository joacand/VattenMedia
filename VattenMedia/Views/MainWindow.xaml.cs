using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;

namespace VattenMedia.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string filename = "settings.txt";

        public MainWindow()
        {
            InitializeComponent();

            // Refresh restore bounds from previous window opening
            var storage = IsolatedStorageFile.GetUserStoreForAssembly();
            try
            {
                using var stream = new IsolatedStorageFileStream(filename, FileMode.Open, storage);
                using var reader = new StreamReader(stream);
                var restoreBounds = Rect.Parse(reader.ReadLine().Replace(';', ','));
                Left = restoreBounds.Left;
                Top = restoreBounds.Top;
                Width = restoreBounds.Width;
                Height = restoreBounds.Height;
            }
            catch
            {
                // Happens during first application launch
                // If other exception, ignore - window will get the default size/location
            }
        }

        void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // Save restore bounds for the next time this window is opened
            var storage = IsolatedStorageFile.GetUserStoreForAssembly();
            using var stream = new IsolatedStorageFileStream(filename, FileMode.Create, storage);
            using var writer = new StreamWriter(stream);
            writer.WriteLine(RestoreBounds.ToString(CultureInfo.InvariantCulture));
        }
    }
}
