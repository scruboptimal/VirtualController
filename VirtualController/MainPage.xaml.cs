using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.Windows.Storage.Pickers;
using System;
using Windows.Graphics.Display;

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
            catch (Exception ex)
            {
                this.ViewModel.LastErrorText = $"An error occurred creating the virtual controller: {ex.Message}";
            }
        }

        private static Window OpenControllerInNewWindow(VirtualController? controller)
        {
            if (controller == null)
            {
                throw new ArgumentNullException(nameof(controller));
            }

            var controllerWindow = new Window()
            {
                Content = controller.Canvas,
                Title = "Virtual Controller"
            };

            // WinUI doesn't have ResizeToFit so we need to resize the window manually accounting for DPI
            var displayInfo = DisplayInformationInterop.GetForWindow((nint)controllerWindow.AppWindow.Id.Value);
            var windowSize = new Windows.Graphics.SizeInt32(
                (int)(controller.Canvas.Width * displayInfo.RawPixelsPerViewPixel),
                (int)(controller.Canvas.Height * displayInfo.RawPixelsPerViewPixel));
            controllerWindow.AppWindow.ResizeClient(windowSize);

            controllerWindow.Activate();

            return controllerWindow;
        }

        private void ToggleRecording_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.ToggleRecording();
        }
    }

    public partial class BooleanToObjectConverter : ObservableObject, IValueConverter
    {
        [ObservableProperty] private object? trueValue;
        [ObservableProperty] private object? falseValue;

        public object? Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is not bool bVal)
            {
                return null;
            }

            return bVal ? this.trueValue : this.falseValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ObjectNotNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
