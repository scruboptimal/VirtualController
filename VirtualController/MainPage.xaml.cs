using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.Windows.Storage.Pickers;
using SkiaSharp;
using SkiaSharp.Views.Windows;
using System;
using VirtualControllerNative.Interop;
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
    }
}