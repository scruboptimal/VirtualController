using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VirtualControllerWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VirtualController? virtualController;
        private Window? controllerWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void OpenVirtualController_Click(object sender, RoutedEventArgs e)
        {
            if (this.controllerWindow != null)
            {
                this.controllerWindow.Close();
            }

            try
            {
                OpenFileDialog picker = new OpenFileDialog()
                {
                    Filter = "SVG files (*.svg)|*.svg|All files (*.*)|*.*",
                };

                bool? result = picker.ShowDialog();
                if (result != true)
                {
                    throw new Exception("No svg file selected");
                }

                this.virtualController = new VirtualController(picker.FileName);
                this.controllerWindow = new Window()
                {
                    Content = this.virtualController.Canvas,
                    Title = "Virtual Controller",
                    SizeToContent = SizeToContent.WidthAndHeight,
                };

                this.controllerWindow.Show();
                this.controllerWindow.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred creating the virtual controller: {ex.Message}");
            }
        }
    }
}