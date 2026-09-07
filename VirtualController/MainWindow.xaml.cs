using Microsoft.UI.Xaml;

namespace VirtualController
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Content = new MainPage(this);
        }
    }
}
