using Microsoft.UI.Xaml;
using Microsoft.Windows.Storage.Pickers;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace VirtualController
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private Window? controllerWindow;

        public MainWindow()
        {
            InitializeComponent();
        }

        public string LastErrorText { get; set; } = string.Empty;

        private async void OpenVirtualController_Click(object sender, RoutedEventArgs e)
        {
            if (this.controllerWindow != null)
            {
                this.controllerWindow.Close();
            }

            try
            {
                FileOpenPicker picker = new FileOpenPicker(this.AppWindow.Id)
                {
                    FileTypeFilter = { ".svg" },
                };

                var result = await picker.PickSingleFileAsync();
                if (result is null)
                {
                    throw new Exception("No svg file selected");
                }

                VirtualController vc = new VirtualController(result.Path);
                this.controllerWindow = new Window()
                {
                    Content = vc.Canvas,
                };
                this.controllerWindow.AppWindow.Resize(new Windows.Graphics.SizeInt32((int)vc.Canvas.Width, (int)vc.Canvas.Height));
                this.controllerWindow.Activate();
            }
            catch (Exception ex)
            {
                this.LastErrorText = $"An error occurred creating the virtual controller: {ex.Message}";
            }
        }
    }
}
