using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using VirtualControllerNative.Interop;
using VirtualControllerShared;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

        public VirtualControllerDisplay(string svgPath, GamepadListener listener)
        {
            InitializeComponent();

            this.listener = listener;
            this.listener.OnGamepadEvent += this.OnGamepadEvent;

            this.dispatcher = Windows.System.DispatcherQueue.GetForCurrentThread();
            this.controllerData = LoadFromSvg(svgPath);

            this.Canvas.Width = this.controllerData.Width;
            this.Canvas.Height = this.controllerData.Height;
            this.Canvas.Background = new SolidColorBrush(this.controllerData.Background);
            foreach (var button in this.controllerData.Buttons.Values)
            {
                this.Canvas.Children.Add(button);
            }

            this.Content = this.Canvas;
        }

        public static VirtualControllerData LoadFromSvg(string svgPath)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(svgPath);

            var root = xmlDoc.DocumentElement;
            if (root == null)
            {
                throw new InvalidOperationException();
            }

            XmlNode? backgroundLayerNode = SVGHelpers.FindChild(root, "layerBackground");
            XmlNode? backgroundRectNode = SVGHelpers.FindChild(backgroundLayerNode, "background");

            double width = double.Parse(backgroundRectNode?.Attributes?.GetNamedItem("width")?.Value ?? "0");
            double height = double.Parse(backgroundRectNode?.Attributes?.GetNamedItem("height")?.Value ?? "0");
            byte[] canvasFill = SVGHelpers.GetFillArgb(backgroundRectNode);
            Color background = Color.FromArgb(canvasFill[0], canvasFill[1], canvasFill[2], canvasFill[3]);

            XmlNode? buttonLayer = SVGHelpers.FindChild(root, "layerButtons");
            if (buttonLayer == null)
            {
                throw new KeyNotFoundException();
            }

            var buttons = new Dictionary<GamepadButton, Ellipse>();
            for (int i = 0; i < buttonLayer.ChildNodes.Count; i++)
            {
                XmlNode? buttonNode = buttonLayer.ChildNodes[i];
                string? buttonId = buttonNode?.Attributes?.GetNamedItem("id")?.Value;
                if (buttonId == null)
                {
                    throw new KeyNotFoundException();
                }

                if (GamepadButtons.ButtonSVGMappings.TryGetValue(buttonId, out GamepadButton button))
                {
                    double x = double.Parse(buttonNode?.Attributes?.GetNamedItem("cx")?.Value ?? "0");
                    double y = double.Parse(buttonNode?.Attributes?.GetNamedItem("cy")?.Value ?? "0");
                    double r = double.Parse(buttonNode?.Attributes?.GetNamedItem("r")?.Value ?? "0");
                    byte[] buttonFill = SVGHelpers.GetFillArgb(buttonNode);
                    Color color = Color.FromArgb(buttonFill[0], buttonFill[1], buttonFill[2], buttonFill[3]);

                    Ellipse e = new()
                    {
                        Width = r * 2,
                        Height = r * 2,
                        Fill = new SolidColorBrush(color),
                        Stroke = new SolidColorBrush(color),
                    };
                    Canvas.SetLeft(e, x - r);
                    Canvas.SetTop(e, y - r);

                    buttons.Add(button, e);
                }
                else
                {
                    Debug.Fail($"{buttonId} not recognized");
                }
            }

            return new VirtualControllerData(width, height, background, buttons);
        }

        void OnGamepadEvent(GamepadButton state, int frameIndex)
        {
            foreach (var button in GamepadButtons.Buttons)
            {
                if (!this.controllerData.Buttons.TryGetValue(button, out var controllerButton))
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

    public record class VirtualControllerData(double Width, double Height, Color Background, IReadOnlyDictionary<GamepadButton, Ellipse> Buttons);
}
