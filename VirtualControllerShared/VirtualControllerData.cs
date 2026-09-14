using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Xml;
using VirtualControllerNative.Interop;

namespace VirtualControllerShared
{
    public record struct VirtualControllerButton(GamepadButton Button, double X, double Y, double R, Color Color);

    public record class VirtualControllerData(double Width, double Height, Color Background, IReadOnlyList<VirtualControllerButton> Buttons)
    {
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

            var buttons = new List<VirtualControllerButton>();
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

                    buttons.Add(new VirtualControllerButton(button, x, y, r, color));
                }
                else
                {
                    Debug.Fail($"{buttonId} not recognized");
                }
            }

            return new VirtualControllerData(width, height, background, buttons);
        }
    }
}
