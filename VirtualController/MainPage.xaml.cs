using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.Storage.Pickers;
using System;
using System.IO;
using System.Text.Json;
using VirtualControllerShared;
using Windows.Graphics.Display;
using WinRT;

namespace VirtualController
{
    public sealed partial class MainPage : Page
    {
        private Window? controllerWindow;
        private Window owningWindow;

        public MainPage(Window owningWindow)
        {
            InitializeComponent();
            this.owningWindow = owningWindow;
            this.DataContext = this.ViewModel;

            this.ViewModel.OpenController(@"C:\Users\cmfra\OneDrive\Desktop\bldDesktop\hitbox.svg");
        }

        public MainPageViewModel ViewModel { get; } = new MainPageViewModel();

        private async void OpenVirtualController_Click(object sender, RoutedEventArgs e)
        {
            if (this.controllerWindow != null)
            {
                this.controllerWindow.Close();
            }

            try
            {
                FileOpenPicker picker = new FileOpenPicker(this.owningWindow.AppWindow.Id)
                {
                    FileTypeFilter = { ".svg" },
                };

                var result = await picker.PickSingleFileAsync();
                if (result is null)
                {
                    throw new Exception("No svg file selected");
                }

                this.ViewModel.OpenController(result.Path);

                if (this.ViewModel.OpenInNewWindow)
                {
                    this.controllerWindow = OpenControllerInNewWindow(this.ViewModel.Controller);
                }
            }
            catch (Exception)
            {
            }
        }

        private static Window OpenControllerInNewWindow(VirtualControllerDisplay? controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            var controllerWindow = new Window()
            {
                Content = controller,
                Title = "Virtual Controller"
            };

            // WinUI doesn't have ResizeToFit so we need to resize the window manually accounting for DPI
            var displayInfo = DisplayInformationInterop.GetForWindow((nint)controllerWindow.AppWindow.Id.Value);
            var windowSize = new Windows.Graphics.SizeInt32(
                (int)(controller.CanvasWidth * displayInfo.RawPixelsPerViewPixel),
                (int)(controller.CanvasHeight * displayInfo.RawPixelsPerViewPixel));
            controllerWindow.AppWindow.ResizeClient(windowSize);

            controllerWindow.Activate();

            return controllerWindow;
        }

        private void ToggleRecording_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.ToggleRecording();
        }

        private void PracticeRecording_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window()
            {
                Content = new ComboPlaybackPage(this.ViewModel.CreatePlaybackVM()),
                Title = "Practice"
            };

            window.Activate();
        }

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewModel.DisplayRecording == null)
            {
                return;
            }

            var picker = new FileSavePicker(this.owningWindow.AppWindow.Id)
            {
                DefaultFileExtension = ".json"
            };

            var result = await picker.PickSaveFileAsync();
            if (result is null)
            {
                throw new Exception("No file selected");
            }

            string json = JsonSerializer.Serialize(this.ViewModel.DisplayRecording);
            File.WriteAllText(result.Path, json);
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker(this.owningWindow.AppWindow.Id)
            {
                FileTypeFilter = { ".json" },
            };

            var result = await picker.PickSingleFileAsync();
            if (result is null)
            {
                throw new Exception("No file selected");
            }

            string json = File.ReadAllText(result.Path);
            this.ViewModel.DisplayRecording = JsonSerializer.Deserialize<Recording>(json);
        }
    }
}