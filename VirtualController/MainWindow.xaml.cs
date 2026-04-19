using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.Windows.Storage.Pickers;
using System;
using Windows.Graphics.Display;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace VirtualController
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private VirtualController? virtualController;
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

                this.virtualController = new VirtualController(result.Path);
                this.controllerWindow = new Window()
                {
                    Content = this.virtualController.Canvas,
                };

                // WinUI doesn't have ResizeToFit so we need to resize the window manually accounting for DPI
                var displayInfo = DisplayInformationInterop.GetForWindow((nint)this.controllerWindow.AppWindow.Id.Value);
                var windowSize = new Windows.Graphics.SizeInt32(
                    (int)(this.virtualController.Canvas.Width * displayInfo.RawPixelsPerViewPixel),
                    (int)(this.virtualController.Canvas.Height * displayInfo.RawPixelsPerViewPixel));
                this.controllerWindow.AppWindow.ResizeClient(windowSize);

                this.controllerWindow.Activate();
            }
            catch (Exception ex)
            {
                this.LastErrorText = $"An error occurred creating the virtual controller: {ex.Message}";
            }
        }
    }
}
