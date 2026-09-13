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

        private static readonly int frameWidth = 8;
        private static readonly int buttonHeight = 16;

        private static SKPaint framePaint = new()
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke,
        };

        private static SKPaint pressedFramePaint = new()
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Fill,
        };

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
            this.TimelineCanvas.Invalidate();
        }

        private void OnTimelinePaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var canvas = e.Surface.Canvas;
            int numButtons = Enum.GetValues(typeof(GamepadButton)).Length;
            int numFrames = e.Info.Width / frameWidth;

            canvas.Clear(SKColors.DarkGray);

            var recording = this.ViewModel.DisplayRecording;
            if (recording != null)
            {
                for (int i = 0; i < recording.Frames.Count; i++)
                {
                    var frame = recording.Frames[i];
                    var nextFrame = i + 1 < recording.Frames.Count ? recording.Frames[i + 1] : null;
                    if (frame.State == GamepadButton.None)
                    {
                        // Nothing to do
                        continue;
                    }

                    float curFrameX = frame.FrameIdx * frameWidth;
                    float nextFrameX = nextFrame != null ? nextFrame.FrameIdx * frameWidth : e.Info.Width;

                    for (int buttonIdx = 1; buttonIdx < numButtons; buttonIdx++)
                    {
                        var button = (GamepadButton)(1 << (buttonIdx - 1));
                        if (frame.State.HasFlag(button))
                        {
                            canvas.DrawRect(new SKRect(curFrameX, buttonIdx * buttonHeight, nextFrameX, (buttonIdx + 1) * buttonHeight), pressedFramePaint);
                        }
                    }
                }
            }

            for (int frameIdx = 0; frameIdx < numFrames; frameIdx++)
            {
                for (int buttonIdx = 0; buttonIdx < numButtons; buttonIdx++)
                {
                    canvas.DrawRect(GetRect(frameIdx, buttonIdx), framePaint);
                }
            }
        }

        static SKRect GetRect(int frameIdx, int buttonIdx)
        {
            float x = frameIdx * frameWidth;
            float y = buttonIdx * buttonHeight;
            return new SKRect(x, y, x + frameWidth, y + buttonHeight);
        }
    }
}