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
        private VirtualController? virtualController;
        private Window? controllerWindow;
        private Window owningWindow;

        public MainPage(Window owningWindow)
        {
            InitializeComponent();
            this.owningWindow = owningWindow;
            this.DataContext = this.ViewModel;
        }

        public MainPageViewModel ViewModel = new MainPageViewModel();

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

                this.virtualController = new VirtualController(result.Path);
                this.controllerWindow = new Window()
                {
                    Content = this.virtualController.Canvas,
                    Title = "Virtual Controller"
                };

                // WinUI doesn't have ResizeToFit so we need to resize the window manually accounting for DPI
                var displayInfo = DisplayInformationInterop.GetForWindow((nint)this.controllerWindow.AppWindow.Id.Value);
                var windowSize = new Windows.Graphics.SizeInt32(
                    (int)(this.virtualController.Canvas.Width * displayInfo.RawPixelsPerViewPixel),
                    (int)(this.virtualController.Canvas.Height * displayInfo.RawPixelsPerViewPixel));
                this.controllerWindow.AppWindow.ResizeClient(windowSize);

                this.controllerWindow.Activate();

                this.ViewModel.CanRecord = true;
            }
            catch (Exception ex)
            {
                this.ViewModel.LastErrorText = $"An error occurred creating the virtual controller: {ex.Message}";
            }
        }

        private void ToggleRecording_Click(object sender, RoutedEventArgs e)
        {
            this.ViewModel.IsRecording = !this.ViewModel.IsRecording;
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
}
