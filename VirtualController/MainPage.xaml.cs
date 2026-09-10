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
            this.TimelineCanvas.Invalidate();
        }

        private void OnTimelinePaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            const int frameWidth = 8;
            const int buttonHeight = 16;

            var canvas = e.Surface.Canvas;
            int numButtons = Enum.GetValues(typeof(GamepadButton)).Length;
            int numFrames = e.Info.Width / frameWidth;

            static SKRect GetRect(int frameIdx, int buttonIdx)
            {
                float x = frameIdx * frameWidth;
                float y = buttonIdx * buttonHeight;
                return new SKRect(x, y, x + frameWidth, y + buttonHeight);
            }

            canvas.Clear(SKColors.DarkGray);

            var framePaint = new SKPaint()
            {
                Color = SKColors.Black,
                Style = SKPaintStyle.Stroke,
            };

            var pressedFramePaint = new SKPaint()
            {
                Color = SKColors.Red,
                Style = SKPaintStyle.Fill,
            };

            for (int frameIdx = 0; frameIdx < numFrames; frameIdx++)
            {
                for (int buttonIdx = 0; buttonIdx < numButtons; buttonIdx++)
                {
                    canvas.DrawRect(GetRect(frameIdx, buttonIdx), framePaint);
                }
            }

            var recording = this.ViewModel.DisplayRecording;
            if (recording != null)
            {
                foreach (var frame in recording.Frames)
                {
                    int frameIdx = frame.frameIdx;
                    var nextFrame = frameIdx + 1 < recording.Frames.Count ? recording.Frames[frameIdx + 1] : null;

                    float curFrameX = frameIdx * frameWidth;
                    float nextFrameX = nextFrame != null ? nextFrame.frameIdx * frameWidth : e.Info.Width;

                    for (int buttonIdx = 0; buttonIdx < numButtons; buttonIdx++)
                    {
                        var button = (GamepadButton)(buttonIdx << (buttonIdx - 1));
                        if (frame.state.HasFlag(button))
                        {
                            canvas.DrawRect(new SKRect(curFrameX, nextFrameX, buttonIdx * buttonHeight, (buttonIdx + 1) * buttonHeight), pressedFramePaint);
                        }
                    }
                }
            }
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