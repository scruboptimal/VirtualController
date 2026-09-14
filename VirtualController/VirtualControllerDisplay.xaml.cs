using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System.Collections.Generic;
using VirtualControllerNative.Interop;
using VirtualControllerShared;
using Windows.System;
using Windows.UI;

namespace VirtualController
{
    public sealed partial class VirtualControllerDisplay : UserControl
    {
        static SolidColorBrush pressedBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xff, 0xff)); // white

        private VirtualControllerData controllerData;
        private GamepadListener listener;
        private DispatcherQueue dispatcher;

        public double CanvasWidth { get => this.Canvas.Width; }
        public double CanvasHeight { get => this.Canvas.Height; }

        Dictionary<GamepadButton, Ellipse> ellipseButtons = new();

        public VirtualControllerDisplay(VirtualControllerData controllerData, GamepadListener listener)
        {
            InitializeComponent();

            this.listener = listener;
            this.listener.OnGamepadEvent += this.OnGamepadEvent;

            this.dispatcher = Windows.System.DispatcherQueue.GetForCurrentThread();
            this.controllerData = controllerData;

            this.Canvas.Width = this.controllerData.Width;
            this.Canvas.Height = this.controllerData.Height;
            this.Canvas.Background = Helpers.GetBrushForColor(this.controllerData.Background);
            foreach (var button in this.controllerData.Buttons)
            {
                var brush = Helpers.GetBrushForColor(button.Color);
                Ellipse e = new()
                {
                    Width = button.R * 2,
                    Height = button.R * 2,
                    Fill = brush,
                    Stroke = brush,
                };
                Canvas.SetLeft(e, button.X - button.R);
                Canvas.SetTop(e, button.Y - button.R);
                this.ellipseButtons.Add(button.Button, e);

                this.Canvas.Children.Add(e);
            }

            this.Content = this.Canvas;
        }

        void OnGamepadEvent(GamepadButton state, int frameIndex)
        {
            foreach (var button in GamepadButtons.Buttons)
            {
                if (!this.ellipseButtons.TryGetValue(button, out var controllerButton))
                {
                    continue;
                }

                this.dispatcher.TryEnqueue(() =>
                {
                    controllerButton.Fill = state.HasFlag(button) ? pressedBrush : controllerButton.Stroke;
                });
            }
        }
    }
}
