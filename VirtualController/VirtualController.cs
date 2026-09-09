using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using VirtualControllerNative.Interop;
using VirtualControllerShared;
using Windows.UI;

namespace VirtualController
{
    public class VirtualController : IDisposable
    {
        static Dictionary<string, GamepadButton> buttonMappings = new Dictionary<string, GamepadButton>()
        {
            { "buttonA", GamepadButton.A },
            { "buttonB", GamepadButton.B },
            { "buttonX", GamepadButton.X },
            { "buttonY", GamepadButton.Y },
            { "buttonLeft", GamepadButton.DPadLeft },
            { "buttonRight", GamepadButton.DPadRight },
            { "buttonDown", GamepadButton.DPadDown },
            { "buttonUp", GamepadButton.DPadUp },
            { "buttonLS", GamepadButton.LeftThumb },
            { "buttonRS", GamepadButton.RightThumb },
            { "buttonLB", GamepadButton.LeftShoulder },
            { "buttonRB", GamepadButton.RightShoulder },
            { "buttonLT", GamepadButton.LeftTrigger },
            { "buttonRT", GamepadButton.RightTrigger },
        };

        static SolidColorBrush pressedBrush = new SolidColorBrush(Color.FromArgb(0xff, 0xff, 0xff, 0xff)); // white
        static List<GamepadButton> buttons = Enum.GetValues<GamepadButton>().OfType<GamepadButton>().Where(x => x != GamepadButton.None).ToList();

        Dictionary<GamepadButton, Ellipse> controllerButtons = new();

        public Canvas Canvas { get; private set; }

        private bool disposedValue;
        private GamepadListener listener;
        private DispatcherQueue dispatcher;

        public event EventHandler<(GamepadButton, int)> ButtonsChanged;

        public VirtualController(string svgPath)
        {
            listener = new GamepadListener(OnGamepadEvent, 0);

            XmlDocument xmlDoc = new();
            xmlDoc.Load(svgPath);

            this.dispatcher = DispatcherQueue.GetForCurrentThread();

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
            this.Canvas = new Canvas()
            {
                Width = width,
                Height = height,
                Background = new SolidColorBrush(Color.FromArgb(canvasFill[0], canvasFill[1], canvasFill[2], canvasFill[3])),
            };


            XmlNode? buttonLayer = SVGHelpers.FindChild(root, "layerButtons");
            if (buttonLayer == null) { throw new KeyNotFoundException(); }
            for (int i = 0; i < buttonLayer.ChildNodes.Count; i++)
            {
                XmlNode? buttonNode = buttonLayer.ChildNodes[i];
                string? buttonId = buttonNode?.Attributes?.GetNamedItem("id")?.Value;
                if (buttonId == null) { throw new KeyNotFoundException(); }

                if (buttonMappings.TryGetValue(buttonId, out GamepadButton button))
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
                    this.Canvas.Children.Add(e);

                    this.controllerButtons.Add(button, e);
                }
                else
                {
                    Debug.Fail($"{buttonId} not recognized");
                }
            }

            this.listener.StartListening();
        }

        void OnGamepadEvent(GamepadButton state, int frameIndex)
        {
            foreach (var button in buttons)
            {
                if (!this.controllerButtons.TryGetValue(button, out var controllerButton))
                {
                    continue;
                }

                this.dispatcher.TryEnqueue(() =>
                {
                    controllerButton.Fill = state.HasFlag(button) ? pressedBrush : controllerButton.Stroke;
                });
            }

            ButtonsChanged?.Invoke(this, (state, frameIndex));
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                this.listener.StopListening();
                disposedValue = true;
            }
        }

        ~VirtualController()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
